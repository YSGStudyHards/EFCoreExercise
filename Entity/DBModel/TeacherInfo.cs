using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Entity.DBModel
{
    [Table("TeacherInfo")]
    public class TeacherInfo
    {
        [Key]
        public int TeacherID { get; set; }

        [Required]
        [MaxLength(80)]
        public string TeacherName { get; set; }

        /// <summary>
        /// 老师性别（1男生，2女生）
        /// </summary>
        [Required]
        public int Gender { get; set; }

        [Required]
        public DateTime Birthday { get; set; }

        [Required]
        [MaxLength(50)]
        public string Phone { get; set; }

        [Required]
        [MaxLength(50)]
        public string Email { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }
    }
}
