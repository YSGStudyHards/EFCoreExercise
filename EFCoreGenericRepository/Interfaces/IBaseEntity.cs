namespace EFCoreGenericRepository.Interfaces
{
    /// <summary>
    /// 实体标记接口
    /// </summary>
    public interface IEntity
    {
    }

    /// <summary>
    /// 具有主键的实体接口
    /// </summary>
    /// <typeparam name="TKey">主键类型</typeparam>
    public interface IEntity<TKey> : IEntity
    {
        /// <summary>
        /// 实体唯一标识符
        /// </summary>
        TKey Id { get; set; }
    }

    /// <summary>
    /// 常用的int主键实体接口
    /// </summary>
    public interface IIntEntity : IEntity<int>
    {
    }

    /// <summary>
    /// 常用的Guid主键实体接口
    /// </summary>
    public interface IGuidEntity : IEntity<Guid>
    {
    }
}
