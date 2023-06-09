namespace ElectricityDataAggregator.Common.Exceptions
{
    public class FileIsNotFoundException : Exception
    {
        public FileIsNotFoundException(string code, string title, string message = null, Exception innerException = null) : base(message ?? title ?? code, innerException)
        {
            Code = code;
            Title = title;
        }

        public string Code { get; set; }
        public string Title { get; set; }
        public FileIsNotFoundException(string title) : this("FileNotFoundException", title) { }

    }
}
