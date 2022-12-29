namespace ModilistPortal.Business.DTOs
{
    public class DQBResultDTO<T>
    {
        public IEnumerable<T> Data { get; set; }

        public int Count { get; set; }
    }
}
