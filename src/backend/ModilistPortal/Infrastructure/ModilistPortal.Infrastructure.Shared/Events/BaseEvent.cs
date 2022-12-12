namespace ModilistPortal.Infrastructure.Shared.Events
{
    public abstract class BaseEvent
    {
        protected BaseEvent(string publisherId, PublisherType publisherType)
        {
            PublisherId = publisherId;
            PublisherType = publisherType;
            PublishedAt = DateTime.UtcNow;
        }

        public string PublisherId { get; set; }

        public PublisherType PublisherType { get; set; }

        public abstract string Version { get; }

        public DateTime PublishedAt { get; }
    }

    public enum PublisherType
    {
        Account = 0,
        System = 1,
        Application = 2,
    }

    public static class EventPublishers
    {
        public const string WebAPI = "WebAPI";
    }
}
