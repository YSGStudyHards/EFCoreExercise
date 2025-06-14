using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DatabaseFirst.NewModels;

[Table("studentinfo")]
[Index("ClassID", Name = "ClassID")]
public partial class studentinfo
{
    /// <summary>
    /// 学生ID
    /// </summary>
    [Key]
    public int StudentID { get; set; }

    /// <summary>
    /// 学生姓名
    /// </summary>
    [Required]
    [StringLength(80)]
    public string StudentName { get; set; }

    /// <summary>
    /// 学生性别（1男生，2女生）
    /// </summary>
    public int Gender { get; set; }

    /// <summary>
    /// 学生生日
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime Birthday { get; set; }

    /// <summary>
    /// 所属班级ID（关联ClassInfo表）
    /// </summary>
    public int ClassID { get; set; }

    /// <summary>
    /// 家长联系电话
    /// </summary>
    [Required]
    [StringLength(50)]
    public string ParentPhone { get; set; }

    /// <summary>
    /// 家庭住址
    /// </summary>
    [Required]
    [StringLength(255)]
    public string Address { get; set; }

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
