using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DatabaseFirst.NewModels;

[Table("classinfo")]
public partial class classinfo
{
    /// <summary>
    /// 班级ID
    /// </summary>
    [Key]
    public int ClassID { get; set; }

    /// <summary>
    /// 班级名称
    /// </summary>
    [Required]
    [StringLength(100)]
    public string ClassName { get; set; }

    /// <summary>
    /// 年级（如：1表示一年级、2表示二年级...）
    /// </summary>
    public int Grade { get; set; }

    /// <summary>
    /// 班主任ID（关联TeacherInfo表）
    /// </summary>
    public int TeacherID { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime? UpdateTime { get; set; }
}
