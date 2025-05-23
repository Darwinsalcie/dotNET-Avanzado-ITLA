

namespace Domain.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException() { }
        public DomainException(string message) : base(message) { }
        public DomainException(string message, Exception innerException) : base(message, innerException) { }
        public DomainException(IEnumerable<string> errors)
            : base("Ha ocurrido un error de validación de la entidad Dominio") 
        {
            Errors = errors.ToList().AsReadOnly();
        }

        public IReadOnlyCollection<string> Errors { get; }

    }
}
