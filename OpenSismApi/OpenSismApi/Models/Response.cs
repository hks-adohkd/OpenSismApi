using DBContext.ViewModels;

namespace OpenSismApi.Models
{
    public class Response<T>
    {
        public Response()
        {
        }

        public Response(int code, string message, T content, CustomerViewModel customer)
        {
            Code = code;
            Message = message;
            Content = content;
            CurrentCustomer = customer;
        }

        public int Code { get; set; }

        public string Message { get; set; }

        public T Content { get; set; }

        public CustomerViewModel CurrentCustomer { get; set; }
    }

    public class Response
    {
        public int code;
        public string msg;

        public Response()
        {
        }

        public Response(int code, string msg)
        {
            this.code = code;
            this.msg = msg;
        }
    }
}
