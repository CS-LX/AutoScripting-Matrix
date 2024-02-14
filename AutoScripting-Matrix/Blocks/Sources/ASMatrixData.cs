using Engine;
using System.Collections.Generic;
using System.Text;

namespace Game {
    public class ASMatrixData : IEditableItemData {

        public Matrix Data = Matrix.Identity;

        public IEditableItemData Copy() => new ASMatrixData { Data = this.Data };

        public void LoadString(string data) => Data = data.ToMatrix();

        public string SaveString() => Data.ToHexString();
    }
}