using Service;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            #region LINQ中常用方法

            //LinqExercise.CommonMethodsInLINQ();

            #endregion

            #region EF Core数据CRUD简单操作

            //SimpleOperation.AddData();
            //SimpleOperation.DataQueryOperation();
            //SimpleOperation.UpdateData();
            //SimpleOperation.DeleteData();

            #endregion

            #region 数据库测试数据生成

            //TestDataCreate.GenerateDBTestData();

            #endregion

            #region EF Core高级查询技巧与实操

            //AdvancedQuery.RelationalQueryExample();

            //var queryStudents = AdvancedQuery.ComplexFilterExample("周", null, null).Result;

            //var groupByAndOrderByData = AdvancedQuery.GroupByAndOrderByExample().Result;

            //var getStudentInfo = AdvancedQuery.PaginationExample().Result;

            //AdvancedQuery.NavigationPropertyLoading();

            //AdvancedQuery.AsSplitQueryExample();

            AdvancedQuery.NativeSQLQuery();

            #endregion
        }
    }
}
