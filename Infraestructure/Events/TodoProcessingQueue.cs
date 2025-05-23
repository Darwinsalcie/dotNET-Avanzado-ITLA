using Application.Events.Interfaces;
using Domain.Entities;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Infrastructure.Events
{
    /// <summary>
    /// Cola reactive que procesa cada TodoItem de forma estrictamente secuencial.
    /// </summary>
    public class TodoProcessingQueue : ITodoProcessingQueue, IDisposable
    {
        private readonly Subject<Todo> _input = new();
        private readonly IObservable<Todo> _processed;
        private readonly IDisposable _subscription;


        public TodoProcessingQueue(
            IScheduler subscribeOnScheduler,
            IScheduler observeOnScheduler)
        {
            _processed = _input
                .Synchronize()
                .ObserveOn(subscribeOnScheduler)
                .Select(todo =>
                    // Aquí retornamos el todo tras procesarlo
                    Observable.FromAsync(async () =>
                    {
                        await ProcessTodoAsync(todo);
                        return todo;            // <— devolvemos el Todo
                    })
                )
                .Concat()                       // concatena uno a uno
                .ObserveOn(observeOnScheduler);

            _subscription = _processed.Subscribe(
                _ => { /* opcionalmente loggear cada todo procesado */ },
                ex => Console.Error.WriteLine($"Error en cola Rx: {ex}")
            );
        }


        public void Enqueue(Todo todo) => _input.OnNext(todo);


        public IObservable<Todo> ProcessedStream => _processed;

        /// <summary>
        /// Lógica real de “procesar” un Todo: persistir, notificar, etc.
        /// </summary>
        private async Task ProcessTodoAsync(Todo todo)
        {
            // Simulamos un trabajo asincrónico (persistir, notificar, etc.)
            await Task.Delay(TimeSpan.FromSeconds(1));
       
        }


        public void Dispose()
        {
            _subscription.Dispose();
            _input.OnCompleted();
            _input.Dispose();
        }


    }
}
