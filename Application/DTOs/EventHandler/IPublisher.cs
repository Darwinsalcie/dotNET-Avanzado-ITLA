using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.EventHandler
{
    public interface IPublisher
    {
        Task PublishAsync<TEvent>(TEvent @event);
    }
}
