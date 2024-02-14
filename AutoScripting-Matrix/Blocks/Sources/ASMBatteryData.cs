using Engine;
using System.Collections.Generic;
using System.Text;

namespace Game {
    public class ASMBatteryData : IEditableItemData {

        public Matrix Data = Matrix.Identity;

        public IEditableItemData Copy() => new ASMBatteryData { Data = this.Data };

        public void LoadString(string data) => Data = data.ToMatrix();

        public string SaveString() => Data.ToString();
    }
}