using Engine;

namespace Game {
    public class ASMExpandableLEDController {
        /// <summary>
        /// 表示在内存中的位置（唯一ID）
        /// </summary>
        public int ID => GetHashCode();

        /// <summary>
        /// 所有控制的元件的数量
        /// </summary>
        public int ControlledCount => m_ledElements.Count;

        /// <summary>
        /// 所控制的所有LED元件以及坐标
        /// </summary>
        public Dictionary<CellFace, ASMExpandableLEDElectricElement> m_ledElements = new Dictionary<CellFace, ASMExpandableLEDElectricElement>();

        public Matrix DisplayMatrix;

        SubsystemASMElectricity m_subsystemElectricity;

        SubsystemASMatrixDisplay m_subsystemMatrixDisplay;

        public List<ASMatrixDisplayData> m_matrixDisplayDatas;

        /// <summary>
        /// 通过指定一个控制器所管辖的LED来实例化一个控制器（实例化中控制器会执行CollectCellsMatrix）
        /// </summary>
        /// <param name="ledElectricElements"></param>
        public ASMExpandableLEDController(Dictionary<CellFace, ASMExpandableLEDElectricElement> ledElectricElements, SubsystemASMElectricity subsystemAsmElectricity) {
            m_ledElements = ledElectricElements;
            m_subsystemElectricity = subsystemAsmElectricity;
            //初始化时添加要显示的矩阵
            m_subsystemMatrixDisplay = subsystemAsmElectricity.Project.FindSubsystem<SubsystemASMatrixDisplay>(true);
            CollectAndDisplayCellsMatrix();
        }

        ~ASMExpandableLEDController() {
            RemoveDisplayMatrix();
        }

        /// <summary>
        /// 重新获取管辖的所有LED方块的显示电压，并且加在一起作为最终的显示电压
        /// </summary>
        public void CollectAndDisplayCellsMatrix() {
            DisplayMatrix = Matrix.Zero;
            foreach (var ledElement in m_ledElements.Values) {
                if(ledElement == null) continue;
                DisplayMatrix += ledElement.OutputMatrixToController();
            }
            UpdateDisplayMatrix();
        }

        /// <summary>
        /// 使所有管辖的LED组件进行电路模拟一次
        /// </summary>
        public void SimulateControlledElement() {
            foreach (var ledElement in m_ledElements.Values) {
                if(ledElement == null) continue;
                m_subsystemElectricity.QueueElectricElementForSimulation(ledElement, m_subsystemElectricity.CircuitStep + 1);
            }
        }

        public void RemoveDisplayMatrix() {
            if (m_matrixDisplayDatas == null) return;
            foreach (var data in m_matrixDisplayDatas) {
                m_subsystemMatrixDisplay.Remove(data);
            }
            m_matrixDisplayDatas.Clear();
        }

        private void UpdateDisplayMatrix() {
            RemoveDisplayMatrix();
            m_matrixDisplayDatas = new List<ASMatrixDisplayData>();
            Point3[] ledPoints = m_ledElements.Keys.Select(c => c.Point).ToArray();
            Point2[] flatPoints = ASMUtils.Point3ToPoint2(ledPoints, m_ledElements.Keys.First().Face);
            if (ASMUtils.CheckRectangle(
                    flatPoints,
                    out Point2 minPoint,
                    out int w,
                    out int h
                )) {//检测到为矩形
                ASMatrixDisplayData matrixDisplayData = m_subsystemMatrixDisplay.Add(true);
                m_matrixDisplayDatas.Add(matrixDisplayData);
                CellFace firstLedCell = m_ledElements.Keys.First();
                Point3 displayPoint = Point3.Zero;
                int oriW = w;
                int oriH = h;
                //这段代码的坐标偏移量是看实际效果慢慢修正过来的
                //所以这段代码写得依托答辩
                switch (firstLedCell.Face) {
                    case 0:
                        (w, h) = (h, w);
                        displayPoint = new Point3(minPoint.X , minPoint.Y, firstLedCell.Z);
                        break;
                    case 1:
                        displayPoint = new Point3(firstLedCell.X, minPoint.X, minPoint.Y + h - 1);
                        break;
                    case 2:
                        displayPoint = new Point3(minPoint.X + w - 1, minPoint.Y, firstLedCell.Z);
                        (w, h) = (h, w);
                        break;
                    case 3:
                        displayPoint = new Point3(firstLedCell.X, minPoint.X, minPoint.Y);
                        break;
                    case 4:
                        displayPoint = new Point3(minPoint.X, firstLedCell.Y, minPoint.Y );
                        break;
                    case 5:
                        displayPoint = new Point3(minPoint.X, firstLedCell.Y, minPoint.Y + h - 1);
                        break;
                }
                matrixDisplayData.DisplayPoint = new CellFace(displayPoint.X, displayPoint.Y, displayPoint.Z, firstLedCell.Face);
                matrixDisplayData.Width = h - 2 / 16f;
                matrixDisplayData.Height = w - 2 / 16f;
                matrixDisplayData.Offset = new Vector2(1 / 16f, 1 / 16f);
                matrixDisplayData.DisplayType = ASMatrixDisplayType.RowLines | ASMatrixDisplayType.ColumnLines;
                matrixDisplayData.Matrix = DisplayMatrix;
                matrixDisplayData.NumRoundLength = oriW;
                matrixDisplayData.FontScale = MathUtils.Min(1 + (oriW - 1) * 0.25f, 1 + (oriH - 1) * 0.25f);
            }
            else {
                foreach (var ledElement in m_ledElements) {
                    CellFace cellFace = ledElement.Key;
                    ASMatrixDisplayData matrixDisplayData = m_subsystemMatrixDisplay.Add(true);
                    m_matrixDisplayDatas.Add(matrixDisplayData);
                    matrixDisplayData.Width = 14f / 16f;
                    matrixDisplayData.Height = 14f / 16f;
                    matrixDisplayData.Offset = new Vector2(1f / 16f, 1f / 16f);
                    matrixDisplayData.DisplayPoint = cellFace;
                    matrixDisplayData.DisplayType = ASMatrixDisplayType.RowLines | ASMatrixDisplayType.ColumnLines;
                    matrixDisplayData.Matrix = DisplayMatrix;
                    matrixDisplayData.NumRoundLength = 1;
                    matrixDisplayData.FontScale = 1;
                }
            }
        }
    }
}