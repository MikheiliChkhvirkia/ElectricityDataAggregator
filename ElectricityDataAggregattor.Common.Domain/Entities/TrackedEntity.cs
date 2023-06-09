using ElectricityDataAggregator.Common.Domain.Contracts;

namespace ElectricityDataAggregator.Common.Domain.Entities
{
    public abstract class TrackedEntity<T> : BaseEntity<T>, ITrackedEntity
    {
        public string CreatedBy { get; protected set; }

        public DateTime CreateDate { get; protected set; }

        public string LastModifiedBy { get; protected set; }

        public DateTime? LastModifiedDate { get; protected set; }

        public string DeletedBy { get; protected set; }

        public DateTime? DeleteDate { get; set; }

        public void Delete()
        {
            DeleteDate = DateTime.Now;
        }

        public void UpdateCreateCredentials(DateTime createDate, string createdBy)
        {
            CreateDate = createDate;
            CreatedBy = createdBy;
        }

        public void UpdateLastModifiedCredentials(DateTime lastModifiedDate, string modifiedBy)
        {
            LastModifiedDate = lastModifiedDate;
            LastModifiedBy = modifiedBy;
        }

        public void UpdateDeleteCredentials(DateTime deleteDate, string deletedBy)
        {
            DeleteDate = deleteDate;
            DeletedBy = deletedBy;
        }
    }
}
