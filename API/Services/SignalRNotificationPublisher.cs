using API.Hubs;
using Application.DTOs.EventHandler;
using Microsoft.AspNetCore.SignalR;

namespace API.Services
{
    public class SignalRNotificationPublisher : INotificationPublisher
    {
        private readonly IHubContext<NotificationHub> _hub;
        public SignalRNotificationPublisher(IHubContext<NotificationHub> hub)
            => _hub = hub;

        public Task PublishTodoCreatedAsync(int taskId, string title, DateTime createdAt)
        {
            Console.WriteLine($"[SignalRPublisher] Enviando notificación de tarea #{taskId}");  // <-- traza

            var payload = new { Id = taskId, Title = title, CreatedAt = createdAt };
            return _hub.Clients.All.SendAsync("ReceiveNotificacion", payload);
        }
    }
}
