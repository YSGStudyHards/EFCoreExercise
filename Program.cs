using EFCoreExercise.DBModel;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using System.Reflection;

namespace EFCoreExercise
{
    internal class Program
    {
        static void Main(string[] args)
        {
            #region 数据CRUD简单操作

            SimpleOperation.AddData();
            SimpleOperation.DataQueryOperation();
            SimpleOperation.UpdateData();
            SimpleOperation.DeleteData();

            #endregion
        }
    }
}
