namespace ElectricityDataAggregator.Common.Exceptions
{
    public class HttpException : AppException
    {
        public string Endpoint { get; private set; }

        public HttpException(string title)
            : base("HttpException", title) { }

        public HttpException(string title, string message)
            : base("HttpException", title, message) { }

        public HttpException(string title, string message, string endpoint)
            : base("HttpException", title, message)
        {
            Endpoint = endpoint;
        }

        public HttpException(string title, Exception innerException)
            : base("HttpException", title, null, innerException) { }

        public HttpException(string title, Exception innerException, string endpoint)
            : base("HttpException", title, null, innerException)
        {
            Endpoint = endpoint;
        }

        public HttpException(string title, string message, Exception innerException, string endpoint)
            : base("HttpException", title, message, innerException)
        {
            Endpoint = endpoint;
        }
    }
}
