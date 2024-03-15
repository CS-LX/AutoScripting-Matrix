using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using Engine;

namespace Game {
    public static class ASMStaticMethods {
        /// <summary>
        /// 获取矩阵指定索引对应值
        /// </summary>
        /// <param name="m"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static float GetElement(this Matrix m, int index) {
            if (index is < 0 or > 15) throw new ArgumentOutOfRangeException(nameof(index));
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

        /// <summary>
        /// 将序列化得到的十六进制字符串反序列化为矩阵
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 设置指定矩阵指定索引的值
        /// </summary>
        /// <param name="m"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void SetElement(ref Matrix m, int index, float value) {
            if (index is < 0 or > 15) throw new ArgumentOutOfRangeException(nameof(index));
            switch (index) {
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
                default: throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
        }

        /// <summary>
        /// 将矩阵序列化为十六进制字符串
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static string ToHexString(this Matrix m) {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < 16; i++) {
                string hex = m.GetElement(i).ToHexString();
                builder.Append(hex + ",");
            }
            builder.Remove(builder.Length - 1, 1); //删除末尾分号
            return builder.ToString();
        }

        /// <summary>
        /// 将浮点数转为十六进制字符串
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static string ToHexString(this float f) {
            return BitConverter.ToString(BitConverter.GetBytes(f)).Replace("-", "");
        }

        public static string ToFormatString(this Matrix m) {
            DefaultInterpolatedStringHandler interpolatedStringHandler = new(18, 16);
            interpolatedStringHandler.AppendFormatted<float>(m.M11);
            interpolatedStringHandler.AppendLiteral(",");
            interpolatedStringHandler.AppendFormatted<float>(m.M12);
            interpolatedStringHandler.AppendLiteral(",");
            interpolatedStringHandler.AppendFormatted<float>(m.M13);
            interpolatedStringHandler.AppendLiteral(",");
            interpolatedStringHandler.AppendFormatted<float>(m.M14);
            interpolatedStringHandler.AppendLiteral("\r\n");
            interpolatedStringHandler.AppendFormatted<float>(m.M21);
            interpolatedStringHandler.AppendLiteral(",");
            interpolatedStringHandler.AppendFormatted<float>(m.M22);
            interpolatedStringHandler.AppendLiteral(",");
            interpolatedStringHandler.AppendFormatted<float>(m.M23);
            interpolatedStringHandler.AppendLiteral(",");
            interpolatedStringHandler.AppendFormatted<float>(m.M24);
            interpolatedStringHandler.AppendLiteral("\r\n");
            interpolatedStringHandler.AppendFormatted<float>(m.M31);
            interpolatedStringHandler.AppendLiteral(",");
            interpolatedStringHandler.AppendFormatted<float>(m.M32);
            interpolatedStringHandler.AppendLiteral(",");
            interpolatedStringHandler.AppendFormatted<float>(m.M33);
            interpolatedStringHandler.AppendLiteral(",");
            interpolatedStringHandler.AppendFormatted<float>(m.M34);
            interpolatedStringHandler.AppendLiteral("\r\n");
            interpolatedStringHandler.AppendFormatted<float>(m.M41);
            interpolatedStringHandler.AppendLiteral(",");
            interpolatedStringHandler.AppendFormatted<float>(m.M42);
            interpolatedStringHandler.AppendLiteral(",");
            interpolatedStringHandler.AppendFormatted<float>(m.M43);
            interpolatedStringHandler.AppendLiteral(",");
            interpolatedStringHandler.AppendFormatted<float>(m.M44);
            return interpolatedStringHandler.ToStringAndClear();
        }

        public static string ToFormatTable(this Matrix m, int decimalPlaces) {
            string table = $"";
            for (int i = 0; i < 4; i++) {
                for (int j = 0; j < 4; j++) {
                    int index = i * 4 + j;
                    table += m.GetElement(index).ToString($"F{decimalPlaces}").PadLeft(10);
                }
                table += "\n";
            }
            return table;
        }

        /// <summary>
        /// 将十六进制字符串转为浮点数
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static float HexStringToFloat(this string hex) {
            byte[] array = new byte[4];
            for (int i = 0; i < 4; i++) {
                array[i] = byte.Parse(hex.Substring(i * 2, 2), NumberStyles.HexNumber);
            }
            return BitConverter.ToSingle(array, 0);
        }

        /// <summary>
        /// 提取矩阵第一个元素作为浮点数: m.M11
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static float ToFloat(this Matrix m) => m.M11;

        /// <summary>
        /// 将矩阵内每一个非0元素相加再除以非0元素的个数，将所得浮点数结果输出
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static float ToAverageFloat(this Matrix m) {
            float result = 0;
            int num = 0;
            for (int i = 0; i < 16; i++) {
                result += m.GetElement(i);
                num += m.GetElement(i) != 0 ? 1 : 0;
            }
            return result / num;
        }

        /// <summary>
        /// 输出一个三维横向量，内容是输入矩阵第一行第一列，第一行第二列，第一行第三列的值
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Vector3 ToVector3T(this Matrix m) => m.Right;

        /// <summary>
        /// 输出一个三维纵向量，内容是输入矩阵M11,M21,M31
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Vector3 ToVector3(this Matrix m) => Matrix.Transpose(m).Right;

        /// <summary>
        /// 输出一个内部元素全为输入浮点数的矩阵: m[0...15] = f
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Matrix ToCMatrix(this float f) {
            Matrix m = Matrix.Zero;
            for (int i = 0; i < 16; i++) {
                SetElement(ref m, i, f);
            }
            return m;
        }

        /// <summary>
        /// 输出一个内部元素全为输入整数的矩阵: m[0...15] = f
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Matrix ToCMatrix(this int f) {
            Matrix m = Matrix.Zero;
            for (int i = 0; i < 16; i++) {
                SetElement(ref m, i, f);
            }
            return m;
        }

        /// <summary>
        /// 返回一个矩阵，内部每个元素都为传入矩阵x对应元素的传入矩阵y的对应元素次方：m[i] = pow(x[i], y[i])
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Matrix Power(Matrix x, Matrix y) {
            Matrix m = Matrix.Zero;
            for (int i = 0; i < 16; i++) {
                SetElement(ref m, i, MathUtils.Pow(x.GetElement(i), y.GetElement(i)));
            }
            return m;
        }

        /// <summary>
        /// 返回一个矩阵，内部每个元素都为传入矩阵x对应元素除传入矩阵y的对应元素：m[i] = x[i] % y[i]
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Matrix Remain(Matrix x, Matrix y) {
            Matrix m = Matrix.Zero;
            for (int i = 0; i < 16; i++) {
                SetElement(ref m, i, MathUtils.Remainder(x.GetElement(i), y.GetElement(i)));
            }
            return m;
        }

        /// <summary>
        /// 点对点的矩阵乘法
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Matrix Multiply(Matrix x, Matrix y) {
            Matrix m = Matrix.Zero;
            for (int i = 0; i < 16; i++) {
                SetElement(ref m, i, x.GetElement(i) * y.GetElement(i));
            }
            return m;
        }


        /// <summary>
        /// 返回一个矩阵，内部每个元素都为以传入矩阵y对应元素为底，传入矩阵x的对应元素的对数 m[i] = log(x[i], y[i])
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Matrix Logarithm(Matrix x, Matrix y) {
            Matrix m = Matrix.Zero;
            for (int i = 0; i < 16; i++) {
                SetElement(ref m, i, MathUtils.Log(x.GetElement(i)) / MathUtils.Log(y.GetElement(i)));
            }
            return m;
        }

        public static Matrix Max(Matrix x, Matrix y) {
            Matrix m = Matrix.Zero;
            for (int i = 0; i < 16; i++) {
                SetElement(ref m, i, MathUtils.Max(x.GetElement(i), y.GetElement(i)));
            }
            return m;
        }

        public static Matrix Min(Matrix x, Matrix y) {
            Matrix m = Matrix.Zero;
            for (int i = 0; i < 16; i++) {
                SetElement(ref m, i, MathUtils.Min(x.GetElement(i), y.GetElement(i)));
            }
            return m;
        }

        /// <summary>
        /// 返回一个矩阵，内部每个元素都为以e为底，传入矩阵x的对应元素的对数 m[i] = log(x[i], e)
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static Matrix Logarithm(Matrix x) {
            Matrix m = Matrix.Zero;
            for (int i = 0; i < 16; i++) {
                SetElement(ref m, i, MathUtils.Log(x.GetElement(i)));
            }
            return m;
        }

        /// <summary>
        /// 返回一个矩阵，内部每个元素都将角度制转为弧度制 m[i] = deg2rad(x[i])
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static Matrix Deg2Rad(Matrix x) {
            Matrix m = Matrix.Zero;
            for (int i = 0; i < 16; i++) {
                SetElement(ref m, i, MathUtils.DegToRad(x.GetElement(i)));
            }
            return m;
        }

        /// <summary>
        /// 返回一个矩阵，内部每个元素都将弧度制转为角度制 m[i] = rad2deg(x[i])
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static Matrix Rad2Deg(Matrix x) {
            Matrix m = Matrix.Zero;
            for (int i = 0; i < 16; i++) {
                SetElement(ref m, i, MathUtils.RadToDeg(x.GetElement(i)));
            }
            return m;
        }

        public static Matrix Sin(Matrix x) {
            Matrix m = Matrix.Zero;
            for (int i = 0; i < 16; i++) {
                SetElement(ref m, i, MathUtils.Sin(x.GetElement(i)));
            }
            return m;
        }

        public static Matrix Cos(Matrix x) {
            Matrix m = Matrix.Zero;
            for (int i = 0; i < 16; i++) {
                SetElement(ref m, i, MathUtils.Cos(x.GetElement(i)));
            }
            return m;
        }

        public static Matrix Tan(Matrix x) {
            Matrix m = Matrix.Zero;
            for (int i = 0; i < 16; i++) {
                SetElement(ref m, i, MathUtils.Tan(x.GetElement(i)));
            }
            return m;
        }

        public static Matrix ASin(Matrix x) {
            Matrix m = Matrix.Zero;
            for (int i = 0; i < 16; i++) {
                SetElement(ref m, i, MathUtils.Asin(x.GetElement(i)));
            }
            return m;
        }

        public static Matrix ACos(Matrix x) {
            Matrix m = Matrix.Zero;
            for (int i = 0; i < 16; i++) {
                SetElement(ref m, i, MathUtils.Acos((x.GetElement(i))));
            }
            return m;
        }

        public static Matrix ATan(Matrix x) {
            Matrix m = Matrix.Zero;
            for (int i = 0; i < 16; i++) {
                SetElement(ref m, i, MathUtils.Atan(x.GetElement(i)));
            }
            return m;
        }

        public static Matrix CreateScaleTranslation(float sx, float sy, float tx, float ty) {
            return new Matrix(
                sx,
                0f,
                0f,
                0f,
                0f,
                sy,
                0f,
                0f,
                0f,
                0f,
                1f,
                0f,
                tx,
                ty,
                0f,
                1f
            );
        }

        /// <summary>
        /// 根据矩形计算其自适应正方形uv
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="uvMin"></param>
        /// <param name="uvMax"></param>
        public static void CalcUV(float w, float h, out Vector2 uvMin, out Vector2 uvMax) {
            uvMin = Vector2.Zero;
            uvMax = Vector2.One;
            if (w > h) {
                uvMin.Y = 0f;
                uvMin.X = 0.5f - 0.5f * h / w;
                uvMax.Y = 1f;
                uvMax.X = 0.5f + 0.5f * h / w;
            }
            else {
                uvMin.Y = 0.5f - 0.5f * w / h;
                uvMin.X = 0f;
                uvMax.Y = 0.5f + 0.5f * w / h;
                uvMax.X = 1f;
            }
        }

        /// <summary>
        /// 矩阵插值
        /// </summary>
        /// <param name="m1">起始矩阵</param>
        /// <param name="m2">终止矩阵</param>
        /// <param name="v">速度</param>
        /// <returns></returns>
        public static Matrix Lerp(Matrix m1, Matrix m2, float v) {
            Matrix result = new Matrix();
            if (m1.HasNaN()) return m2;
            if (m2.HasNaN()) return m1;
            result.M11 = m1.M11 + (m2.M11 - m1.M11) * v;
            result.M12 = m1.M12 + (m2.M12 - m1.M12) * v;
            result.M13 = m1.M13 + (m2.M13 - m1.M13) * v;
            result.M14 = m1.M14 + (m2.M14 - m1.M14) * v;
            result.M21 = m1.M21 + (m2.M21 - m1.M21) * v;
            result.M22 = m1.M22 + (m2.M22 - m1.M22) * v;
            result.M23 = m1.M23 + (m2.M23 - m1.M23) * v;
            result.M24 = m1.M24 + (m2.M24 - m1.M24) * v;
            result.M31 = m1.M31 + (m2.M31 - m1.M31) * v;
            result.M32 = m1.M32 + (m2.M32 - m1.M32) * v;
            result.M33 = m1.M33 + (m2.M33 - m1.M33) * v;
            result.M34 = m1.M34 + (m2.M34 - m1.M34) * v;
            result.M41 = m1.M41 + (m2.M41 - m1.M41) * v;
            result.M42 = m1.M42 + (m2.M42 - m1.M42) * v;
            result.M43 = m1.M43 + (m2.M43 - m1.M43) * v;
            result.M44 = m1.M44 + (m2.M44 - m1.M44) * v;
            return result;
        }

        /// <summary>
        /// 矩阵是否有元素为NaN
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static bool HasNaN(this Matrix m) {
            if (float.IsNaN(m.M11)
                || float.IsNaN(m.M12)
                || float.IsNaN(m.M13)
                || float.IsNaN(m.M14)
                || float.IsNaN(m.M21)
                || float.IsNaN(m.M22)
                || float.IsNaN(m.M23)
                || float.IsNaN(m.M24)
                || float.IsNaN(m.M31)
                || float.IsNaN(m.M32)
                || float.IsNaN(m.M33)
                || float.IsNaN(m.M34)
                || float.IsNaN(m.M41)
                || float.IsNaN(m.M42)
                || float.IsNaN(m.M43)
                || float.IsNaN(m.M44))
                return true;
            return false;
        }

        /// <summary>
        /// 求逆矩阵
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Matrix Invert(this Matrix m) => Matrix.Invert(m);

        /// <summary>
        /// 求转置矩阵
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Matrix Transpose(this Matrix m) => Matrix.Transpose(m);
    }
}