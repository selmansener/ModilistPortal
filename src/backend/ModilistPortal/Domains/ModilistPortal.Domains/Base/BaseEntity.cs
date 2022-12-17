namespace ModilistPortal.Domains.Base
{
    public interface IBaseEntity { }

    public abstract class BaseEntity : IBaseEntity
    {
        protected BaseEntity()
        {
            CreatedAt = DateTime.UtcNow;
        }

        public int Id { get; }

        public DateTime? DeletedAt { get; private set; }

        public DateTime? UpdatedAt { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public int DeletedById { get; protected set; }

        public int UpdatedById { get; protected set; }

        public int CreatedById { get; protected set; }

        public virtual void Update()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        public virtual void Delete()
        {
            DeletedAt = DateTime.UtcNow;
        }
    }
}
