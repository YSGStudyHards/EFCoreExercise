using System;
using System.Collections.Generic;

namespace DatabaseFirst.Models;

public partial class Teacherinfo
{
    /// <summary>
    /// 老师ID
    /// </summary>
    public int TeacherId { get; set; }

    /// <summary>
    /// 老师名称
    /// </summary>
    public string TeacherName { get; set; } = null!;

    /// <summary>
    /// 老师性别（1男生，2女生）
    /// </summary>
    public int Gender { get; set; }

    /// <summary>
    /// 老师生日
    /// </summary>
    public DateTime Birthday { get; set; }

    /// <summary>
    /// 老师联系手机
    /// </summary>
    public string Phone { get; set; } = null!;

    /// <summary>
    /// 老师联系邮箱
    /// </summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdateTime { get; set; }
}
