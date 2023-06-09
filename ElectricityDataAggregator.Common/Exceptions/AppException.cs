namespace ElectricityDataAggregator.Common.Exceptions
{
    public class AppException : Exception
    {
        public AppException(string code, string title, string message = null, Exception innerException = null) : base(message ?? title ?? code, innerException)
        {
            Code = code;
            Title = title;
        }

        public string Code { get; set; }
        public string Title { get; set; }
        public AppException(string title) : this("AppException", title) { }

    }
}
