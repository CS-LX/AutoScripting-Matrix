namespace Game {
    public class ASMTruthTableData : IEditableItemData {
        public string[] Expressions = new string[16] {
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty
        };


        public IEditableItemData Copy() => new ASMTruthTableData() { Expressions = (string[])Expressions.Clone() };

        public void LoadString(string data) {
            string[] expressions = data.Split('|');
            if (expressions.Length != 16) throw new Exception("非法数据: 表达式个数必须是16");
            for (int i = 0; i < 16; i++) {
                Expressions[i] = expressions[i];
            }
        }

        public string SaveString() {
            string res = string.Empty;
            for (int i = 0; i < 16; i++) {
                string expression = Expressions[i];
                if (expression.Contains('|')) throw new Exception($"储存失败, 原因: 第{i}个表达式存在非法字符\'|\'");
                res += expression + "|";
            }
            return res.Substring(0, res.Length - 1);
        }

        public bool Check(out int index) => Check(Expressions, out index);

        public static bool Check(string[] expressions, out int index) {
            for (int i = 0; i < 16; i++) {
                index = i;
                if (expressions[i].Contains('|')) return false;
            }
            index = -1;
            return true;
        }

        public bool Check_Linq() => Check_Linq(Expressions);

        public static bool Check_Linq(string[] expressions) => !expressions.Take(16).Any(expressions => expressions.Contains('|'));
    }
}