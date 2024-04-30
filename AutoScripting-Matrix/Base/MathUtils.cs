namespace Game {
  public static class MathUtils
  {
    public const float PI = 3.1415927f;
    public const float E = 2.7182817f;

    public static int Min(int x1, int x2) => x1 >= x2 ? x2 : x1;

    public static int Min(int x1, int x2, int x3) => MathUtils.Min(MathUtils.Min(x1, x2), x3);

    public static int Min(int x1, int x2, int x3, int x4)
    {
      return MathUtils.Min(MathUtils.Min(MathUtils.Min(x1, x2), x3), x4);
    }

    public static int Max(int x1, int x2) => x1 <= x2 ? x2 : x1;

    public static int Max(int x1, int x2, int x3) => MathUtils.Max(MathUtils.Max(x1, x2), x3);

    public static int Max(int x1, int x2, int x3, int x4)
    {
      return MathUtils.Max(MathUtils.Max(MathUtils.Max(x1, x2), x3), x4);
    }

    public static int Clamp(int x, int min, int max)
    {
      if (x < min)
        return min;
      return x <= max ? x : max;
    }

    public static int Sign(int x) => Math.Sign(x);

    public static int Abs(int x) => Math.Abs(x);

    public static int Sqr(int x) => x * x;

    public static bool IsPowerOf2(uint x) => x > 0U && ((int) x & (int) x - 1) == 0;

    public static uint NextPowerOf2(uint x)
    {
      --x;
      x |= x >> 1;
      x |= x >> 2;
      x |= x >> 4;
      x |= x >> 8;
      x |= x >> 16;
      ++x;
      return x;
    }

    public static int Hash(int key) => (int) MathUtils.Hash((uint) key);

    public static uint Hash(uint key)
    {
      key ^= key >> 16;
      key *= 2146121005U;
      key ^= key >> 15;
      key *= 2221713035U;
      key ^= key >> 16;
      return key;
    }

    public static long Min(long x1, long x2) => x1 >= x2 ? x2 : x1;

    public static long Min(long x1, long x2, long x3) => MathUtils.Min(MathUtils.Min(x1, x2), x3);

    public static long Min(long x1, long x2, long x3, long x4)
    {
      return MathUtils.Min(MathUtils.Min(MathUtils.Min(x1, x2), x3), x4);
    }

    public static long Max(long x1, long x2) => x1 <= x2 ? x2 : x1;

    public static long Max(long x1, long x2, long x3) => MathUtils.Max(MathUtils.Max(x1, x2), x3);

    public static long Max(long x1, long x2, long x3, long x4)
    {
      return MathUtils.Max(MathUtils.Max(MathUtils.Max(x1, x2), x3), x4);
    }

    public static long Clamp(long x, long min, long max)
    {
      if (x < min)
        return min;
      return x <= max ? x : max;
    }

    public static long Sign(long x) => (long) Math.Sign(x);

    public static long Abs(long x) => Math.Abs(x);

    public static long Sqr(long x) => x * x;

    public static bool IsPowerOf2(long x) => x > 0L && (x & x - 1L) == 0L;

    public static ulong NextPowerOf2(ulong x)
    {
      --x;
      x |= x >> 1;
      x |= x >> 2;
      x |= x >> 4;
      x |= x >> 8;
      x |= x >> 16;
      x |= x >> 32;
      ++x;
      return x;
    }

    public static float Min(float x1, float x2) => (double) x1 >= (double) x2 ? x2 : x1;

    public static float Min(float x1, float x2, float x3)
    {
      return MathUtils.Min(MathUtils.Min(x1, x2), x3);
    }

    public static float Min(float x1, float x2, float x3, float x4)
    {
      return MathUtils.Min(MathUtils.Min(MathUtils.Min(x1, x2), x3), x4);
    }

    public static float Max(float x1, float x2) => (double) x1 <= (double) x2 ? x2 : x1;

    public static float Max(float x1, float x2, float x3)
    {
      return MathUtils.Max(MathUtils.Max(x1, x2), x3);
    }

    public static float Max(float x1, float x2, float x3, float x4)
    {
      return MathUtils.Max(MathUtils.Max(MathUtils.Max(x1, x2), x3), x4);
    }

    public static float Clamp(float x, float min, float max)
    {
      if ((double) x < (double) min)
        return min;
      return (double) x <= (double) max ? x : max;
    }

    public static float Saturate(float x)
    {
      if ((double) x < 0.0)
        return 0.0f;
      return (double) x <= 1.0 ? x : 1f;
    }

    public static float Sign(float x) => (float) Math.Sign(x);

    public static float Abs(float x) => Math.Abs(x);

    public static float Floor(float x) => (float) Math.Floor((double) x);

    public static float Ceiling(float x) => (float) Math.Ceiling((double) x);

    public static float Round(float x) => (float) Math.Round((double) x);

    public static float Remainder(float x, float y) => x - MathUtils.Floor(x / y) * y;

    public static float Sqr(float x) => x * x;

    public static float Sqrt(float x) => (float) Math.Sqrt((double) x);

    public static float Sin(float x) => (float) Math.Sin((double) x);

    public static float Cos(float x) => (float) Math.Cos((double) x);

    public static float Tan(float x) => (float) Math.Tan((double) x);

    public static float Asin(float x) => (float) Math.Asin((double) x);

    public static float Acos(float x) => (float) Math.Acos((double) x);

    public static float Atan(float x) => (float) Math.Atan((double) x);

    public static float Atan2(float y, float x) => (float) Math.Atan2((double) y, (double) x);

    public static float Exp(float n) => (float) Math.Exp((double) n);

    public static float Log(float x) => (float) Math.Log((double) x);

    public static float Log10(float x) => (float) Math.Log10((double) x);

    public static float Pow(float x, float n) => (float) Math.Pow((double) x, (double) n);

    public static float PowSign(float x, float n)
    {
      return MathUtils.Sign(x) * MathUtils.Pow(MathUtils.Abs(x), n);
    }

    public static float Lerp(float x1, float x2, float f) => x1 + (x2 - x1) * f;

    public static float SmoothStep(float min, float max, float x)
    {
      x = MathUtils.Clamp((float) (((double) x - (double) min) / ((double) max - (double) min)), 0.0f, 1f);
      return (float) ((double) x * (double) x * (3.0 - 2.0 * (double) x));
    }

    public static float CatmullRom(float v1, float v2, float v3, float v4, float f)
    {
      float num1 = f * f;
      float num2 = num1 * f;
      return (float) (0.5 * (2.0 * (double) v2 + ((double) v3 - (double) v1) * (double) f + (2.0 * (double) v1 - 5.0 * (double) v2 + 4.0 * (double) v3 - (double) v4) * (double) num1 + (3.0 * (double) v2 - (double) v1 - 3.0 * (double) v3 + (double) v4) * (double) num2));
    }

    public static float NormalizeAngle(float angle)
    {
      angle = (float) Math.IEEERemainder((double) angle, 6.2831854820251465);
      if ((double) angle > 3.1415927410125732)
        angle -= 6.2831855f;
      else if ((double) angle <= -3.1415927410125732)
        angle += 6.2831855f;
      return angle;
    }

    public static float Sigmoid(float x, float steepness)
    {
      if ((double) x <= 0.0)
        return 0.0f;
      if ((double) x >= 1.0)
        return 1f;
      float num1 = MathUtils.Exp(steepness);
      float num2 = MathUtils.Exp(2f * steepness * x);
      return (float) ((double) num1 * ((double) num2 - 1.0) / (((double) num1 - 1.0) * ((double) num2 + (double) num1)));
    }

    public static float DegToRad(float degrees)
    {
      return (float) ((double) degrees / 180.0 * 3.1415927410125732);
    }

    public static float RadToDeg(float radians)
    {
      return (float) ((double) radians * 180.0 / 3.1415927410125732);
    }

    public static double Min(double x1, double x2) => x1 >= x2 ? x2 : x1;

    public static double Min(double x1, double x2, double x3)
    {
      return MathUtils.Min(MathUtils.Min(x1, x2), x3);
    }

    public static double Min(double x1, double x2, double x3, double x4)
    {
      return MathUtils.Min(MathUtils.Min(MathUtils.Min(x1, x2), x3), x4);
    }

    public static double Max(double x1, double x2) => x1 <= x2 ? x2 : x1;

    public static double Max(double x1, double x2, double x3)
    {
      return MathUtils.Max(MathUtils.Max(x1, x2), x3);
    }

    public static double Max(double x1, double x2, double x3, double x4)
    {
      return MathUtils.Max(MathUtils.Max(MathUtils.Max(x1, x2), x3), x4);
    }

    public static double Clamp(double x, double min, double max)
    {
      if (x < min)
        return min;
      return x <= max ? x : max;
    }

    public static double Saturate(double x)
    {
      if (x < 0.0)
        return 0.0;
      return x <= 1.0 ? x : 1.0;
    }

    public static double Sign(double x) => (double) Math.Sign(x);

    public static double Abs(double x) => Math.Abs(x);

    public static double Floor(double x) => Math.Floor(x);

    public static double Ceiling(double x) => Math.Ceiling(x);

    public static double Round(double x) => Math.Round(x);

    public static double Remainder(double x, double y) => x - MathUtils.Floor(x / y) * y;

    public static double Sqr(double x) => x * x;

    public static double Sqrt(double x) => Math.Sqrt(x);

    public static double Sin(double x) => Math.Sin(x);

    public static double Cos(double x) => Math.Cos(x);

    public static double Tan(double x) => Math.Tan(x);

    public static double Asin(double x) => Math.Asin(x);

    public static double Acos(double x) => Math.Acos(x);

    public static double Atan(double x) => Math.Atan(x);

    public static double Atan2(double y, double x) => Math.Atan2(y, x);

    public static double Exp(double n) => Math.Exp(n);

    public static double Log(double x) => Math.Log(x);

    public static double Log10(double x) => Math.Log10(x);

    public static double Pow(double x, double n) => Math.Pow(x, n);

    public static double PowSign(double x, double n)
    {
      return MathUtils.Sign(x) * MathUtils.Pow(MathUtils.Abs(x), n);
    }

    public static double Lerp(double x1, double x2, double f) => x1 + (x2 - x1) * f;

    public static double SmoothStep(double min, double max, double x)
    {
      x = MathUtils.Clamp((x - min) / (max - min), 0.0, 1.0);
      return x * x * (3.0 - 2.0 * x);
    }

    public static double CatmullRom(double v1, double v2, double v3, double v4, double f)
    {
      double num1 = f * f;
      double num2 = num1 * f;
      return 0.5 * (2.0 * v2 + (v3 - v1) * f + (2.0 * v1 - 5.0 * v2 + 4.0 * v3 - v4) * num1 + (3.0 * v2 - v1 - 3.0 * v3 + v4) * num2);
    }

    public static double NormalizeAngle(double angle)
    {
      angle = Math.IEEERemainder(angle, 2.0 * Math.PI);
      if (angle > 3.1415927410125732)
        angle -= 2.0 * Math.PI;
      else if (angle <= -1.0 * Math.PI)
        angle += 2.0 * Math.PI;
      return angle;
    }

    public static double DegToRad(double degrees) => degrees / 180.0 * Math.PI;

    public static double RadToDeg(double radians) => radians * 180.0 / Math.PI;

    public static float LinearStep(float zero, float one, float f)
    {
      return MathUtils.Saturate((float) (((double) f - (double) zero) / ((double) one - (double) zero)));
    }
  }
}