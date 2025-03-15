using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCoreExercise.DBModel
{
    [Table("TeacherInfo")]
    public class TeacherInfo
    {
        [Key]
        public int TeacherID { get; set; }

        [Required]
        [MaxLength(80)]
        public string TeacherName { get; set; }

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
