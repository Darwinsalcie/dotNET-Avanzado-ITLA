using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.EventHandler
{
    /// <summary>
    /// Manejador genérico para eventos de dominio.
    /// </summary>
    public interface IEventHandler<TEvent>
    {
        /// <summary>
        /// Se invoca cuando se publica un evento de tipo TEvent.
        /// </summary>
        Task HandleAsync(TEvent @event);
    }
}
