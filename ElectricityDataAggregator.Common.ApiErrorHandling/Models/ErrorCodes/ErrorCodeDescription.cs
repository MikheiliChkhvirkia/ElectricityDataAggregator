namespace ElectricityDataAggregator.Common.ApiErrorHandling.Models.ErrorCodes
{
    public class ErrorCodeDescription
    {
        public string Code { get; }
        public string Title { get; }
        public string Description { get; }
        public List<ErrorCodeField> ExtensionFields { get; }

        public ErrorCodeDescription(string code, string title,
            string description = null,
            List<ErrorCodeField> extensionFields = null)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentNullException(nameof(code));

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException(nameof(title));

            Code = code;
            Title = title;
            Description = description;
            ExtensionFields = extensionFields;
        }
    }

    public class ErrorCodeField
    {
        public string Name { get; }
        public Type Type { get; }
        public string Description { get; }
        public object ExampleValue { get; }

        public ErrorCodeField(string name, Type type, string description, object exampleValue = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            Name = name;
            Type = type ?? typeof(string);
            Description = description;
            ExampleValue = exampleValue;
        }
    }

    public class ErrorCodeField<TField> : ErrorCodeField
    {
        public ErrorCodeField(string name, string description)
         : base(name, typeof(TField), description)
        {

        }

        public ErrorCodeField(string name, string description, TField exampleValue)
            : base(name, typeof(TField), description, exampleValue)
        {

        }
    }
}