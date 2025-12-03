using Diquis.Application.Common.Marker;

namespace Diquis.WebApi.Extensions
{
    public static class DynamicServiceRegistrationExtensions
    {
        // auto registration of services with lifecycles Transient/Scoped
        // -- instead of having to manually register each service, this will find the classes that implement ITransientService or IScopedService interfaces and register them

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            Type transientServiceType = typeof(ITransientService);
            Type scopedServiceType = typeof(IScopedService);

            var transientServices = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(transientServiceType.IsAssignableFrom)
                .Where(t => t.IsClass && !t.IsAbstract && !t.IsNested) // Exclude nested classes
                .Select(t => new
                {
                    Service = t.GetInterfaces().FirstOrDefault(),
                    Implementation = t
                })
                .Where(t => t.Service != null);

            var scopedServices = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(scopedServiceType.IsAssignableFrom)
                .Where(t => t.IsClass && !t.IsAbstract && !t.IsNested) // Exclude nested classes
                .Select(t => new
                {
                    Service = t.GetInterfaces().FirstOrDefault(),
                    Implementation = t
                })
                .Where(t => t.Service != null);


            foreach (var transientService in transientServices)
            {
                if (transientServiceType.IsAssignableFrom(transientService.Service))
                {
                    _ = services.AddTransient(transientService.Service, transientService.Implementation);
                }
            }

            foreach (var scopedService in scopedServices)
            {
                if (scopedServiceType.IsAssignableFrom(scopedService.Service))
                {
                    _ = services.AddScoped(scopedService.Service, scopedService.Implementation);
                }
            }

            return services;
        }

    }
}
