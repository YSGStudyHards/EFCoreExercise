namespace EFCoreGenericRepository.Models
{
    /// <summary>
    /// 分页查询结果
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// 当前页数据
        /// </summary>
        public List<T> PageDatas { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 页索引（从0开始）
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; }
    }
}
