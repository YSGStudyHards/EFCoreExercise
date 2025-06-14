using System;
using System.Collections.Generic;

namespace DatabaseFirst.Models;

public partial class Studentinfo
{
    /// <summary>
    /// 学生ID
    /// </summary>
    public int StudentId { get; set; }

    /// <summary>
    /// 学生姓名
    /// </summary>
    public string StudentName { get; set; } = null!;

    /// <summary>
    /// 学生性别（1男生，2女生）
    /// </summary>
    public int Gender { get; set; }

    /// <summary>
    /// 学生生日
    /// </summary>
    public DateTime Birthday { get; set; }

    /// <summary>
    /// 所属班级ID（关联ClassInfo表）
    /// </summary>
    public int ClassId { get; set; }

    /// <summary>
    /// 家长联系电话
    /// </summary>
    public string ParentPhone { get; set; } = null!;

    /// <summary>
    /// 家庭住址
    /// </summary>
    public string Address { get; set; } = null!;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdateTime { get; set; }
}
