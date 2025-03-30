namespace EFCoreExercise
{
    public static class ChineseNameGenerator
    {
        private static readonly Random _random = new Random();

        //现代常见姓氏（含复姓）
        private static readonly string[] _surnames = {
        "李", "王", "张", "刘", "陈", "杨", "黄", "赵", "周", "吴",
        "徐", "孙", "马", "朱", "胡", "郭", "何", "高", "林", "郑",
        "欧阳", "上官", "司马", "诸葛", "东方", "独孤", "慕容"};

        // 现代常用名字字符
        private static readonly string[] _nameChars = {
        // 中性字
        "辰", "宇", "轩", "梓", "泽", "思", "睿", "涵", "欣", "雨",
        // 偏男性字
        "浩", "昊", "博", "杰", "毅", "硕", "峻", "驰", "锐", "哲",
        // 偏女性字
        "萱", "婷", "雯", "怡", "悦", "萌", "雅", "璇", "彤", "琪"};

        public enum Gender { Random, Male, Female }

        /// <summary>
        /// 生成随机中文姓名
        /// </summary>
        /// <param name="gender">性别倾向</param>
        /// <param name="allowCompoundSurname">是否允许复姓</param>
        /// <returns>生成的完整姓名</returns>
        public static string Generate(Gender gender = Gender.Random, bool allowCompoundSurname = true)
        {
            // 1. 选择姓氏
            string surname = SelectSurname(allowCompoundSurname);

            // 2. 生成名字（1-2个字）
            string givenName = GenerateGivenName(gender);

            return surname + givenName;
        }

        private static string SelectSurname(bool allowCompound)
        {
            // 控制复姓出现概率（约5%）
            var pool = allowCompound && _random.NextDouble() < 0.05
                ? _surnames
                : _surnames.Where(s => s.Length == 1).ToArray();

            return pool[_random.Next(pool.Length)];
        }

        private static string GenerateGivenName(Gender gender)
        {
            // 1. 过滤符合性别的字符
            var filteredChars = gender switch
            {
                Gender.Male => _nameChars.Except(new[] { "萱", "婷", "雯", "怡", "悦", "萌", "雅", "璇", "彤", "琪" }),
                Gender.Female => _nameChars.Except(new[] { "浩", "昊", "博", "杰", "毅", "硕", "峻", "驰", "锐", "哲" }),
                _ => _nameChars.AsEnumerable()
            };

            // 2. 确定名字长度（70%概率2个字）
            int length = _random.NextDouble() < 0.7 ? 2 : 1;

            // 3. 组合名字字符
            var charPool = filteredChars.ToArray();
            return Enumerable.Range(0, length)
                .Select(_ => charPool[_random.Next(charPool.Length)])
                .Aggregate("", (acc, c) => acc + c);
        }
    }
}
