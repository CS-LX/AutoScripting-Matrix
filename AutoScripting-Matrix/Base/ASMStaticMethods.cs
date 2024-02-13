using Engine;

namespace Game {
    public static class ASMStaticMethods {
        public static float GetElement(this Matrix m, int index) {
            if (index < 0
                || index > 15)
                throw new ArgumentOutOfRangeException(nameof(index));
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

        public static Matrix ToMatrix(this string serializedMatrix) {
            try {
            var values = serializedMatrix.Trim().Split(',');
            Matrix matrix = new Matrix();
            matrix.M11 = float.Parse(values[0]);
            matrix.M21 = float.Parse(values[1]);
            matrix.M31 = float.Parse(values[2]);
            matrix.M41 = float.Parse(values[3]);
            matrix.M12 = float.Parse(values[4]);
            matrix.M22 = float.Parse(values[5]);
            matrix.M32 = float.Parse(values[6]);
            matrix.M42 = float.Parse(values[7]);
            matrix.M13 = float.Parse(values[8]);
            matrix.M23 = float.Parse(values[9]);
            matrix.M33 = float.Parse(values[10]);
            matrix.M43 = float.Parse(values[11]);
            matrix.M14 = float.Parse(values[12]);
            matrix.M24 = float.Parse(values[13]);
            matrix.M34 = float.Parse(values[14]);
            matrix.M44 = float.Parse(values[15]);
            return matrix;
            }
            catch (Exception e) {
                Log.Warning($"矩阵反序列化存在潜在问题:{e}");
                return Matrix.Identity;
            }
        }
    }
}