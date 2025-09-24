namespace Entity.ViewModel
{
    public class CreateTeacherRequest
    {
        public string TeacherName { get; set; }
        public int Age { get; set; }
        public string CourseName { get; set; }
        public List<CreateStudentItem> Students { get; set; }
    }

    public class CreateStudentItem
    {
        public string StudentName { get; set; }
        public int Age { get; set; }
        public string ClassName { get; set; }
    }
}
