

namespace Domain.DTOs
{
    public class Response
    {
        public bool ThereIsError => Errors.Any();
        public long EntityId { get; set; }
        public bool Successful { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>(0);
    }

    public class Response<T> : Response
    {
        public IEnumerable<T> DataList { get; set; }
        public T SingleData { get; set; }
    }
}