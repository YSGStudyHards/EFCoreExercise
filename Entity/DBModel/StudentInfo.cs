using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using EFCoreGenericRepository.Interfaces;

namespace Entity.DBModel
{
    [Table("StudentInfo")]
    public class StudentInfo
    {
        [Key]
        public int StudentID { get; set; }

        [Required]
        [MaxLength(80)]
        public string StudentName { get; set; }

        /// <summary>
        /// 学生性别（1男生，2女生）
        /// </summary>
        [Required]
        public int Gender { get; set; }

        [Required]
        public DateTime Birthday { get; set; }

        [Required]
        public int ClassID { get; set; }

        [Required]
        [MaxLength(50)]
        public string ParentPhone { get; set; }

        [Required]
        [MaxLength(255)]
        public string Address { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 导航属性
        /// </summary>
        public ClassInfo ClassInfo { get; set; }
    }
}
