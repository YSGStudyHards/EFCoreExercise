
using DatabaseFirst.Context;
using DatabaseFirst.NewModels;

namespace DatabaseFirst
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (NewSchoolDBContext schoolContext = new NewSchoolDBContext())
            {
                var getTeachers = schoolContext.teacherinfos.Take(10).ToList();
            }

            Console.WriteLine("Hello, World!");
        }
    }
}
