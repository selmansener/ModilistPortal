namespace ModilistPortal.Infrastructure.Shared.Events
{
    public class ProductImagesChanged : BaseEvent
    {
        public ProductImagesChanged(string publisherId, PublisherType publisherType, int productId, IEnumerable<string> imageUrls)
            : base(publisherId, publisherType)
        {
            ProductId = productId;
            ImageUrls = imageUrls;
        }

        public int ProductId { get; set; }

        public IEnumerable<string> ImageUrls { get; set; }

        public override string Version => "1.0";
    }
}
