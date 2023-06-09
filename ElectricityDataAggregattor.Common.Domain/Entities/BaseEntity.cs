namespace ElectricityDataAggregator.Common.Domain.Entities
{
    public abstract class BaseEntity<T>
    {
        /// <summary>
        /// A uniquie identifier.
        /// </summary>
        public T Id { get; set; }
    }
}
