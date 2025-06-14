using System;
using System.Collections.Generic;

namespace DatabaseFirst.Models;

public partial class Classinfo
{
    /// <summary>
    /// 班级ID
    /// </summary>
    public int ClassId { get; set; }

    /// <summary>
    /// 班级名称
    /// </summary>
    public string ClassName { get; set; } = null!;

    /// <summary>
    /// 年级（如：1表示一年级、2表示二年级...）
    /// </summary>
    public int Grade { get; set; }

    /// <summary>
    /// 班主任ID（关联TeacherInfo表）
    /// </summary>
    public int TeacherId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdateTime { get; set; }
}
