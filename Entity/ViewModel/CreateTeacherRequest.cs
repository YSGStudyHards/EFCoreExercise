namespace Entity.ViewModel
{
    public class CreateTeacherRequest
    {
        public string TeacherName { get; set; }

        /// <summary>
        /// 老师性别（1男生，2女生）
        /// </summary>
        public int Gender { get; set; }

        public DateTime Birthday { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? UpdateTime { get; set; }
    }
}
