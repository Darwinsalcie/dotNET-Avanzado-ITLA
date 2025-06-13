using Application.DTOs.EventHandler;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Events
{
    public class InMemoryPublisher : IPublisher
    {
        private readonly IServiceProvider _sp;

        public InMemoryPublisher(IServiceProvider sp) => _sp = sp;

        public async Task PublishAsync<TEvent>(TEvent @event)
        {
            var handlers = _sp.GetServices<IEventHandler<TEvent>>();
            foreach (var h in handlers)
                await h.HandleAsync(@event);
        }
    }
}
