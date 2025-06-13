

namespace Application.DTOs.EventHandler
{
    public interface INotificationPublisher
    {
        Task PublishTodoCreatedAsync(int taskId, string title, DateTime createdAt);
    }
}
