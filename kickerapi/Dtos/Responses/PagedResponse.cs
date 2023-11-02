namespace kickerapi.Dtos.Responses
{
    public class PagedResponse<T>
    {
        public int PageCount { get; set; }
        public List<T>? Data { get; set; }

        public PagedResponse(List<T>? data, int pageSize, int count)
        {
            Data = data;
            PageCount = (count + pageSize - 1) / pageSize;
        }
    }
}
