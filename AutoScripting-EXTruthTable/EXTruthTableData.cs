
using System.Globalization;
using Game;
using NCalc;

namespace AutoScripting_EXTruthTable {
    public class EXTruthTableData : IEditableItemData {
        public string Data = string.Empty;

        public IEditableItemData Copy() => new EXTruthTableData() { Data = this.Data };

        public void LoadString(string data) => Data = data;

        public string SaveString() => Data;

        public bool Calc(float i1, float i2, float i3, float i4, out float result, out string calcState) {
            result = 0;
            try {
                string exp = Data.Replace("i1", i1.ToString(CultureInfo.InvariantCulture)).Replace("i2", i2.ToString(CultureInfo.InvariantCulture)).Replace("i3", i3.ToString(CultureInfo.InvariantCulture)).Replace("i4", i4.ToString(CultureInfo.InvariantCulture));
                Expression expression = new(exp);
                Func<float> calculator = expression.ToLambda<float>();
                result = calculator();
                calcState = "正常";
                return true;

            }
            catch(Exception e) {
                calcState = e.Message;
                result = 0;
                return false;
            }
        }
    }
}