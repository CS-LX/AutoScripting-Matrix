using System.Globalization;
using System.Text;
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

        public static Matrix ToMatrix(this string hexString) {
            try {
                var values = hexString.Trim().Split(',');
                Matrix matrix = new Matrix();
                matrix.M11 = values[0].HexStringToFloat();
                matrix.M12 = values[1].HexStringToFloat();
                matrix.M13 = values[2].HexStringToFloat();
                matrix.M14 = values[3].HexStringToFloat();
                matrix.M21 = values[4].HexStringToFloat();
                matrix.M22 = values[5].HexStringToFloat();
                matrix.M23 = values[6].HexStringToFloat();
                matrix.M24 = values[7].HexStringToFloat();
                matrix.M31 = values[8].HexStringToFloat();
                matrix.M32 = values[9].HexStringToFloat();
                matrix.M33 = values[10].HexStringToFloat();
                matrix.M34 = values[11].HexStringToFloat();
                matrix.M41 = values[12].HexStringToFloat();
                matrix.M42 = values[13].HexStringToFloat();
                matrix.M43 = values[14].HexStringToFloat();
                matrix.M44 = values[15].HexStringToFloat();
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

        public static string ToHexString(this Matrix m) {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < 16; i++) {
                string hex = m.GetElement(i).ToHexString();
                builder.Append(hex + ",");
            }
            builder.Remove(builder.Length - 1, 1);//删除末尾分号
            return builder.ToString();
        }

        public static string ToHexString(this float f) {
            return BitConverter.ToString(BitConverter.GetBytes(f)).Replace("-", "");
        }

        public static float HexStringToFloat(this string hex) {
            byte[] array = new byte[4];
            for (int i = 0; i < 4; i++) {
                array[i] = byte.Parse(hex.Substring(i * 2, 2), NumberStyles.HexNumber);
            }
            return BitConverter.ToSingle(array, 0);
        }

        public static float ToFloat(this Matrix m) => m.M11;

        public static float ToAverageFloat(this Matrix m) {
            float result = 0;
            for (int i = 0; i < 16; i++) {
                result += m.GetElement(i);
            }
            return result / 16;
        }

        public static Vector3 ToVector3(this Matrix m) => m.Right;
    }
}