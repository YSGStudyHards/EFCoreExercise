namespace Entity.ViewModel
{
    public class CreateTeacherWithStudentsRequest : CreateTeacherRequest
    {
        public List<StudentViewModel> Students { get; set; }
    }
}
