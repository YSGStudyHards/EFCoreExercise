namespace Entity.ViewModel
{
    public class StudentViewModel
    {
        /// <summary>
        /// 学生编号
        /// </summary>
        public int StudentID { get; set; }

        /// <summary>
        /// 学生姓名
        /// </summary>
        public string StudentName { get; set; }

        /// <summary>
        /// 学生性别（1男生，2女生）
        /// </summary>
        public int Gender { get; set; }

        /// <summary>
        /// 学生生日
        /// </summary>
        public DateTime Birthday { get; set; }

        /// <summary>
        /// 班级编号
        /// </summary>
        public int ClassID { get; set; }

        /// <summary>
        /// 家长电话
        /// </summary>
        public string ParentPhone { get; set; }

        /// <summary>
        /// 家庭住址
        /// </summary>
        public string Address { get; set; }
    }
}
