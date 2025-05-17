

namespace Domain.Validation
{
    public class ValidationBehaviors
    {
        public delegate (bool IsValid, string ErrorMessage) Validator<T>(T entity);
    }
}
