namespace Game {
    public struct ASMELEDFacialData : IEquatable<ASMELEDFacialData> {
        public bool[] m_points = new bool[4];//边缘的点，第一个是左上方的点
        public bool[] m_sides = new bool[4];//边缘的黑边，第一个是上方的黑边

        public ASMELEDFacialData(bool[] points, bool[] sides) {
            if (points.Length is (> 4 or < 0)
                || sides.Length is (> 4 or < 0))
                throw new IndexOutOfRangeException();

            m_points = points;
            m_sides = sides;
        }

        public bool Equals(ASMELEDFacialData other) => m_points.Equals(other.m_points) && m_sides.Equals(other.m_sides);

        public override bool Equals(object? obj) => obj is ASMELEDFacialData other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(m_points, m_sides);

        public static bool operator ==(ASMELEDFacialData left, ASMELEDFacialData right) => left.Equals(right);

        public static bool operator !=(ASMELEDFacialData left, ASMELEDFacialData right) => !left.Equals(right);
    }
}