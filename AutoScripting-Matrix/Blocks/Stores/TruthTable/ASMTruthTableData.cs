namespace Game {
    public class ASMTruthTableData : IEditableItemData {

        public string[] Expressions = new string[16];


        public IEditableItemData Copy() => new ASMTruthTableData(){Expressions = (string[])Expressions.Clone()};

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
            return res.TrimEnd('|');
        }

        public bool Check() {
            for (int i = 0; i < 16; i++) {
                if (Expressions[i].Contains('|')) return false;
            }
            return true;
        }

        public bool Check_Linq() => !Expressions.Take(16).Any(expression => expression.Contains('|'));
    }
}