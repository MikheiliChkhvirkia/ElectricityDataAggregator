namespace ElectricityDataAggregator.Common.Exceptions
{
    public class ObjectNotFoundException : AppException
    {
        public string ObjectType { get; private set; }
        public object ObjectId { get; private set; }
        public ObjectNotFoundException(string objectType, object objectId)
            : base("ObjectNotFoundException", "Object not found", $"{objectType}{(objectId != null ? $":{objectId}" : null)} not found") { }

        public ObjectNotFoundException(string title)
            : base("ObjectNotFoundException", title) { }
    }
}
