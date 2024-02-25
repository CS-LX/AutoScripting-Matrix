using Engine;

namespace Game {
    public class ASMatrixDisplayData {
        public Matrix Matrix;
        public float Width;
        public float Height;
        public CellFace DisplayPoint;
        public float RowLinesWidth = 0.01f;
        public float ColumnLinesWidth = 0.01f;
        public ASMatrixDisplayType DisplayType;
    }

    [Flags]
    public enum ASMatrixDisplayType : int{
        Brackets = 1,
        RowLines = 2,
        ColumnLines = 4
    }
}
