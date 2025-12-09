using System.Diagnostics;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.States;
using Hangfire.Storage;
using OpenTelemetry.Trace;

namespace Diquis.Infrastructure.BackgroundJobs.Telemetry;

/// <summary>
/// Hangfire filter that creates OpenTelemetry traces for background job execution.
/// </summary>
public class HangfireActivityFilter : IClientFilter, IServerFilter, IElectStateFilter, IApplyStateFilter
{
    private static readonly ActivitySource ActivitySource = new("Diquis.Hangfire", "1.0.0");
    private const string ActivityKey = "OtelActivity";

    // Client Filter - Job Creation
    public void OnCreating(CreatingContext context)
    {
        var activity = ActivitySource.StartActivity($"Hangfire Job Creating: {context.Job.Type.Name}.{context.Job.Method.Name}", ActivityKind.Producer);
        
        if (activity != null)
        {
            activity.SetTag("hangfire.job.type", context.Job.Type.FullName);
            activity.SetTag("hangfire.job.method", context.Job.Method.Name);
            activity.SetTag("hangfire.state", context.InitialState?.Name);
            
            context.SetJobParameter(ActivityKey, activity.Context);
        }
    }

    public void OnCreated(CreatedContext context)
    {
        Activity.Current?.Stop();
    }

    // Server Filter - Job Execution
    public void OnPerforming(PerformingContext context)
    {
        var parentContext = context.GetJobParameter<ActivityContext>(ActivityKey);
        
        var activity = parentContext != default
            ? ActivitySource.StartActivity($"Hangfire Job Execution: {context.BackgroundJob.Job.Type.Name}.{context.BackgroundJob.Job.Method.Name}", ActivityKind.Consumer, parentContext)
            : ActivitySource.StartActivity($"Hangfire Job Execution: {context.BackgroundJob.Job.Type.Name}.{context.BackgroundJob.Job.Method.Name}", ActivityKind.Consumer);

        if (activity != null)
        {
            activity.SetTag("hangfire.job.id", context.BackgroundJob.Id);
            activity.SetTag("hangfire.job.type", context.BackgroundJob.Job.Type.FullName);
            activity.SetTag("hangfire.job.method", context.BackgroundJob.Job.Method.Name);
            activity.SetTag("hangfire.server.name", context.ServerId);
            activity.SetTag("hangfire.retry.count", context.GetJobParameter<int>("RetryCount"));
            
            // Store activity in context for OnPerformed
            context.Items[ActivityKey] = activity;
        }
    }

    public void OnPerformed(PerformedContext context)
    {
        if (context.Items.TryGetValue(ActivityKey, out var activityObj) && activityObj is Activity activity)
        {
            if (context.Exception != null)
            {
                activity.SetStatus(ActivityStatusCode.Error, context.Exception.Message);
                activity.RecordException(context.Exception);
            }
            else
            {
                activity.SetStatus(ActivityStatusCode.Ok);
            }
            
            activity.Stop();
        }
    }

    // State Filter - Track State Changes
    public void OnStateElection(ElectStateContext context)
    {
        // No action needed for state election
    }

    public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
    {
        var activity = Activity.Current;
        if (activity != null)
        {
            activity.AddEvent(new ActivityEvent($"State Applied: {context.NewState.Name}"));
            activity.SetTag("hangfire.state.new", context.NewState.Name);
            
            if (context.OldStateName != null)
            {
                activity.SetTag("hangfire.state.old", context.OldStateName);
            }
        }
    }

    public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
    {
        // No action needed
    }
}
