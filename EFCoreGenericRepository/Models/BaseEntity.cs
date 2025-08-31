using EFCoreGenericRepository.Interfaces;

namespace EFCoreGenericRepository.Models
{
    /// <summary>
    /// 基础实体抽象类
    /// </summary>
    /// <typeparam name="TKey">主键类型</typeparam>
    public abstract class BaseEntity<TKey> : IEntity<TKey>
    {
        /// <summary>
        /// 实体唯一标识符
        /// </summary>
        public virtual TKey Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 创建者ID
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// 最后修改时间【审计字段】
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// 最后修改者ID【审计字段】
        /// </summary>
        public string UpdatedBy { get; set; }
    }
}
