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

    public class ResponseQuiz<T>
    {
        public ResponseQuiz()
        {
        }

        public ResponseQuiz(int code, string message, T content, CustomerViewModel customer, QuizIndexViewModel indexes , int TotlalQuestion)
        {
            Code = code;
            Message = message;
            Content = content;
            CurrentCustomer = customer;
            QuizIndexes = indexes;
            TotalQuestions = TotlalQuestion;
        }

        public int Code { get; set; }

        public string Message { get; set; }

        public T Content { get; set; }

        public CustomerViewModel CurrentCustomer { get; set; }

        public QuizIndexViewModel QuizIndexes { get; set; }

        public int TotalQuestions { get; set; }
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

    public class ResponseNew
    {
        public ResponseNew()
        {
        }

        public ResponseNew (int code, string message, CustomerViewModel customer)
        {
            Code = code;
            Message = message;
            
            CurrentCustomer = customer;
        }

        public int Code { get; set; }

        public string Message { get; set; }

        

        public CustomerViewModel CurrentCustomer { get; set; }
    }

    

}
