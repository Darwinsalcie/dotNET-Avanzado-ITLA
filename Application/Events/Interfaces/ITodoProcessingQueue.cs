

using Domain.Entities;

namespace Application.Events.Interfaces
{
    /// <summary>
    /// Abstracción para encolar y procesar Todo de forma secuencial.
    /// </summary>
    public interface ITodoProcessingQueue : IDisposable
    {
        /// <summary>
        /// Envía un nuevo Todo a la cola; retorna inmediatamente.
        /// </summary>
        void Enqueue(Todo todo);

        /// <summary>
        /// Flujo de todos los ítems procesados (para test, métricas, etc.).
        /// </summary>
        IObservable<Todo> ProcessedStream { get; }
    }
}
