using Engine;

namespace Game {
    public class ASMELEDController {
        public Matrix Matrix;

        public string SaveToString() {
            return Matrix.ToHexString();
        }

        public static ASMELEDController CreateByString(string data) {
            ASMELEDController asmeledController = new();
            asmeledController.Matrix = data.ToMatrix();
            return asmeledController;
        }
    }
}