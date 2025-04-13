using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Entity.DBModel
{
    [Table("ClassInfo")]
    public class ClassInfo
    {
        [Key]
        public int ClassID { get; set; }

        [Required]
        [MaxLength(100)]
        public string ClassName { get; set; }

        /// <summary>
        /// 年级（如：1表示一年级、2表示二年级...）
        /// </summary>
        [Required]
        public int Grade { get; set; }

        [Required]
        public int TeacherID { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }

        // 导航属性
        // 标记为virtual以启用延迟加载
        public /*virtual*/ TeacherInfo Teacher { get; set; }

        public List<StudentInfo> Students { get; set; }
    }
}
