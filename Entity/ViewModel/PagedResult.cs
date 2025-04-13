namespace Entity.ViewModel
{
    public class PagedResult<T>
    {
        public int TotalCount { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public List<T> Items { get; set; }

        public PagedResult(int totalCount, int pageIndex, int pageSize, List<T> items)
        {
            TotalCount = totalCount;
            PageIndex = pageIndex;
            PageSize = pageSize;
            Items = items;
        }
    }
}
