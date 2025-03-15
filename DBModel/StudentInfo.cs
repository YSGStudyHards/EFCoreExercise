using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreExercise.DBModel
{
    public class StudentInfo
    {
        [Key]
        public int StudentID { get; set; }

        [Required]
        [MaxLength(80)]
        public string StudentName { get; set; }

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
    }
}
