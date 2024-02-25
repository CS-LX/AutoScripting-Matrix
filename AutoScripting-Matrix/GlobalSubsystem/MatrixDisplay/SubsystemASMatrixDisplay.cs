using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game {
    public class SubsystemASMatrixDisplay : Subsystem, IDrawable {
        public int[] DrawOrders => [110];
        public PrimitivesRenderer3D m_primitivesRenderer = new PrimitivesRenderer3D();
        public FlatBatch3D m_geometryBatch;
        public FontBatch3D m_numbersBatch;

        public Dictionary<ASMatrixDisplayData, bool> m_displayMatrices = new();

        public ASMatrixDisplayData Add(bool flag) {
            ASMatrixDisplayData displayData = new();
            m_displayMatrices.Add(displayData, flag);
            return displayData;
        }

        public bool Remove(ASMatrixDisplayData data) => m_displayMatrices.Remove(data);

        public void Draw(Camera camera, int drawOrder) {
            foreach (var displayMatrix in m_displayMatrices) {
                ASMatrixDisplayData matrixDisplay = displayMatrix.Key;
                CellFace cellFace = matrixDisplay.DisplayPoint;
                //显示的面的单位向量，满足笛卡尔坐标系，right是x轴，up是y轴
                Vector3 v = new(cellFace.X + 0.5f, cellFace.Y + 0.5f, cellFace.Z + 0.5f);
                Vector3 position = v - (0.435f * CellFace.FaceToVector3(cellFace.Face));//0.435f原本是0.4375f，但是会导致闪烁，所以加一些偏移量
                Vector3 forward = CellFace.FaceToVector3(cellFace.Face);
                Vector3 up = (cellFace.Face < 4) ? Vector3.UnitY : Vector3.UnitX;
                Vector3 right = Vector3.Cross(up, forward);

                Vector3 p1 = position + (-right - up) / 2;
                Vector3 p2 = position + right * (matrixDisplay.Width - 0.5f) - up / 2;
                Vector3 p3 = position + right * (matrixDisplay.Width - 0.5f) + up * (matrixDisplay.Height - 0.5f);
                Vector3 p4 = position - right / 2 + up * (matrixDisplay.Height - 0.5f);

                //绘制方横向分割线
                if ((matrixDisplay.DisplayType & ASMatrixDisplayType.RowLines) == ASMatrixDisplayType.RowLines) {
                    for (int i = 1; i < 4; i++) {
                        float height = matrixDisplay.Height * (i / 4f);
                        m_geometryBatch.QueueQuad(
                            p1 + up * (height - matrixDisplay.RowLinesWidth / 2),
                            p2 + up * (height - matrixDisplay.RowLinesWidth / 2),
                            p2 + up * (height + matrixDisplay.RowLinesWidth / 2),
                            p1 + up * (height + matrixDisplay.RowLinesWidth / 2),
                            matrixDisplay.RowLinesColor
                        );
                    }
                }

                //绘制方纵向分割线
                if ((matrixDisplay.DisplayType & ASMatrixDisplayType.ColumnLines) == ASMatrixDisplayType.ColumnLines) {
                    for (int i = 1; i < 4; i++) {
                        float width = matrixDisplay.Width * (i / 4f);
                        m_geometryBatch.QueueQuad(
                            p1 + right * (width - matrixDisplay.ColumnLinesWidth / 2),
                            p1 + right * (width + matrixDisplay.ColumnLinesWidth / 2),
                            p4 + right * (width + matrixDisplay.ColumnLinesWidth / 2),
                            p4 + right * (width - matrixDisplay.ColumnLinesWidth / 2),
                            matrixDisplay.ColumnLinesColor
                        );
                    }
                }

                //文字
                for (int i = 0; i < 4; i++) {
                    for (int j = 0; j < 4; j++) {
                        float element = matrixDisplay.Matrix.GetElement(j * 4 + i);
                        string displayNum = element.ToString("0." + new string('0', matrixDisplay.NumRoundLength));//保留NumRoundLength位小数
                        float width = matrixDisplay.Width * (i / 4f) + matrixDisplay.Width / 8f;
                        float height = matrixDisplay.Height * ((4 - j) / 4f) - matrixDisplay.Height / 8f;
                        Vector3 displayNumPos = p1 + right * width + up * height;

                        float offsetScale = 0.005f * matrixDisplay.FontScale;
                        m_numbersBatch.QueueText(displayNum, displayNumPos, right * offsetScale, -up * offsetScale, matrixDisplay.FontColor, TextAnchor.HorizontalCenter | TextAnchor.VerticalCenter, Vector2.Zero);
                    }
                }
            }
            m_primitivesRenderer.Flush(camera.ViewProjectionMatrix);
        }

        public override void Load(ValuesDictionary valuesDictionary) {
            base.Load(valuesDictionary);
            m_geometryBatch = m_primitivesRenderer.FlatBatch();
            m_numbersBatch = m_primitivesRenderer.FontBatch(LabelWidget.BitmapFont, 1, DepthStencilState.DepthRead, RasterizerState.CullNoneScissor, BlendState.AlphaBlend, SamplerState.LinearClamp);
        }
    }
}