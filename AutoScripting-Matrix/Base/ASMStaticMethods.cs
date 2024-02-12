using Engine;

namespace Game {
    public static class ASMStaticMethods {
        public static float GetElement(this Matrix m, int index) {
            if (index < 0
                || index > 15)
                throw new ArgumentOutOfRangeException("矩阵元素序号必须为[0, 15]");
            switch (index) {
                case 0: return m.M11;
                case 1: return m.M12;
                case 2: return m.M13;
                case 3: return m.M14;
                case 4: return m.M21;
                case 5: return m.M22;
                case 6: return m.M23;
                case 7: return m.M24;
                case 8: return m.M31;
                case 9: return m.M32;
                case 10: return m.M33;
                case 11: return m.M34;
                case 12: return m.M41;
                case 13: return m.M42;
                case 14: return m.M43;
                case 15: return m.M44;
                default: return m.M11;
            }
        }
    }
}