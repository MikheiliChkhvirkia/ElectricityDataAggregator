namespace ElectricityDataAggregator.Common.Domain.Contracts
{
    public interface ITrackedEntity
    {
        DateTime? DeleteDate { get; protected set; }
        void UpdateCreateCredentials(DateTime createDate, string? createdBy);
        void UpdateLastModifiedCredentials(DateTime lastModifiedDate, string? modifiedBy);
        void UpdateDeleteCredentials(DateTime deleteDate, string? deletedBy);
    }
}
