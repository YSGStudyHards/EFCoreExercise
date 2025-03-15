using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCoreExercise.DBModel
{
    [Table("ClassInfo")]
    public class ClassInfo
    {
        [Key]
        public int ClassID { get; set; }

        [Required]
        [MaxLength(100)]
        public string ClassName { get; set; }

        [Required]
        public int Grade { get; set; }

        [Required]
        public int TeacherID { get; set; }

        [Required]
        public DateTime CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }
    }
}
