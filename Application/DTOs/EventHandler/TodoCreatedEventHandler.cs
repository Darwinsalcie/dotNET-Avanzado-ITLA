

using Domain.Events;

namespace Application.DTOs.EventHandler
{
    public class TodoCreatedEventHandler : IEventHandler<TodoCreatedEvent>
    {
        private readonly INotificationPublisher _notifier;
        public TodoCreatedEventHandler(INotificationPublisher notifier)
            => _notifier = notifier;

        public Task HandleAsync(TodoCreatedEvent evt)
            => _notifier.PublishTodoCreatedAsync(evt.TaskId, evt.Title, evt.CreatedAt);
    }

}
