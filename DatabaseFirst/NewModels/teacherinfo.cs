using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DatabaseFirst.NewModels;

[Table("teacherinfo")]
public partial class teacherinfo
{
    /// <summary>
    /// 老师ID
    /// </summary>
    [Key]
    public int TeacherID { get; set; }

    /// <summary>
    /// 老师名称
    /// </summary>
    [Required]
    [StringLength(80)]
    public string TeacherName { get; set; }

    /// <summary>
    /// 老师性别（1男生，2女生）
    /// </summary>
    public int Gender { get; set; }

    /// <summary>
    /// 老师生日
    /// </summary>
    [Column(TypeName = "datetime")]
    public DateTime Birthday { get; set; }

    /// <summary>
    /// 老师联系手机
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Phone { get; set; }

    /// <summary>
    /// 老师联系邮箱
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Email { get; set; }

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
