using Engine;

namespace Game {
    public static class ASMStaticMethods {
        public static float GetElement(this Matrix m, int index) {
            if (index is < 0 or > 15)
                throw new ArgumentOutOfRangeException(nameof(index));
            return index switch {
                0 => m.M11,
                1 => m.M12,
                2 => m.M13,
                3 => m.M14,
                4 => m.M21,
                5 => m.M22,
                6 => m.M23,
                7 => m.M24,
                8 => m.M31,
                9 => m.M32,
                10 => m.M33,
                11 => m.M34,
                12 => m.M41,
                13 => m.M42,
                14 => m.M43,
                15 => m.M44,
                _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
            };
        }

        public static Matrix ToMatrix(this string serializedMatrix) {
            try {
                var values = serializedMatrix.Trim().Split(',');
                Matrix matrix = new Matrix();
                matrix.M11 = float.Parse(values[0]);
                matrix.M12 = float.Parse(values[1]);
                matrix.M13 = float.Parse(values[2]);
                matrix.M14 = float.Parse(values[3]);
                matrix.M21 = float.Parse(values[4]);
                matrix.M22 = float.Parse(values[5]);
                matrix.M23 = float.Parse(values[6]);
                matrix.M24 = float.Parse(values[7]);
                matrix.M31 = float.Parse(values[8]);
                matrix.M32 = float.Parse(values[9]);
                matrix.M33 = float.Parse(values[10]);
                matrix.M34 = float.Parse(values[11]);
                matrix.M41 = float.Parse(values[12]);
                matrix.M42 = float.Parse(values[13]);
                matrix.M43 = float.Parse(values[14]);
                matrix.M44 = float.Parse(values[15]);
                return matrix;
            }
            catch (Exception e) {
                Log.Warning($"矩阵反序列化存在潜在问题:{e}");
                return Matrix.Zero;
            }
        }

        public static void SetElement(ref Matrix m, int index, float value)
        {
            if (index is < 0 or > 15)
                throw new ArgumentOutOfRangeException(nameof(index));

            switch (index)
            {
                case 0:
                    m.M11 = value;
                    break;
                case 1:
                    m.M12 = value;
                    break;
                case 2:
                    m.M13 = value;
                    break;
                case 3:
                    m.M14 = value;
                    break;
                case 4:
                    m.M21 = value;
                    break;
                case 5:
                    m.M22 = value;
                    break;
                case 6:
                    m.M23 = value;
                    break;
                case 7:
                    m.M24 = value;
                    break;
                case 8:
                    m.M31 = value;
                    break;
                case 9:
                    m.M32 = value;
                    break;
                case 10:
                    m.M33 = value;
                    break;
                case 11:
                    m.M34 = value;
                    break;
                case 12:
                    m.M41 = value;
                    break;
                case 13:
                    m.M42 = value;
                    break;
                case 14:
                    m.M43 = value;
                    break;
                case 15:
                    m.M44 = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
        }
    }
}