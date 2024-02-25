using Engine;

namespace Game {
    public class ASMatrixDisplayData {
        public Matrix Matrix;
        public float Width;
        public float Height;
        public CellFace DisplayPoint;
        public float RowLinesWidth = 0.01f;
        public float ColumnLinesWidth = 0.01f;
        public Color RowLinesColor = Color.Gray;
        public Color ColumnLinesColor = Color.Gray;
        public float FontScale = 1;
        public int NumRoundLength = 1;
        public Color FontColor = Color.White;
        public ASMatrixDisplayType DisplayType;
    }

    [Flags]
    public enum ASMatrixDisplayType : int{
        RowLines = 1,
        ColumnLines = 2
    }
}
