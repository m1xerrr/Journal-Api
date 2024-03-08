using Journal.Domain.Enums;

namespace Journal.Domain.Responses
{
    public class BaseResponse<T>
    {
        public StatusCode StatusCode { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
    }
}
