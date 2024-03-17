using Engine;
using Engine.Media;

namespace Game {
    public class ASMatrixDisplayData {
        public Matrix Matrix;
        public float Width;
        public float Height;
        public Vector2 Offset;
        public Vector3 Offset3 = Vector3.Zero;
        public CellFace DisplayPoint;
        public float RowLinesWidth = 0.01f;
        public float ColumnLinesWidth = 0.01f;
        public Color RowLinesColor = Color.Gray;
        public Color ColumnLinesColor = Color.Gray;
        public float FontScale = 1;
        public int NumRoundLength = 1;
        public Color FontColor = Color.White;
        public ASMatrixDisplayType DisplayType;
        public bool UseDebugFont = false;
        public bool TopMost = false;
    }

    [Flags]
    public enum ASMatrixDisplayType : int{
        RowLines = 1,
        ColumnLines = 2
    }
}
