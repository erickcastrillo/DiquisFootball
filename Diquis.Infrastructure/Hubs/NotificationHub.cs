using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Diquis.Infrastructure.Hubs
{
    /// <summary>
    /// SignalR hub for pushing real-time notifications to authenticated users.
    /// Handles connections, disconnections, and message broadcasting.
    /// </summary>
    [AllowAnonymous] // Allow anonymous for now - tenant notifications are not sensitive
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationHub"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Called when a client connects to the hub.
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier ?? "Anonymous";
            _logger.LogInformation("User {UserId} connected to NotificationHub (ConnectionId: {ConnectionId})", userId, Context.ConnectionId);
            await Clients.Caller.SendAsync("Connected", $"Successfully connected to notification hub");
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Called when a client disconnects from the hub.
        /// </summary>
        /// <param name="exception">The exception that caused the disconnection, if any.</param>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier ?? "Anonymous";
            if (exception != null)
            {
                _logger.LogWarning(exception, "User {UserId} disconnected with error", userId);
            }
            else
            {
                _logger.LogInformation("User {UserId} disconnected from NotificationHub", userId);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
