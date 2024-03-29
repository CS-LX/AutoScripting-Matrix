using Engine;

namespace Game {
    public class ASMExpandableLEDElectricElement : ASMMountedElectricElement {
        public SubsystemTerrain m_subsystemTerrain;
        public SubsystemASMatrixDisplay m_subsystemMatrixDisplay;

        public SubsystemASMExpandableLEDControllers m_subsystemControllers;

        //public ASMatrixDisplayData m_matrixDisplayData;

        public Matrix m_outputMatrix;
        public Matrix OutputMatrixToController() => m_outputMatrix;

        public ASMExpandableLEDElectricElement(SubsystemASMElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace) {
            m_subsystemTerrain = SubsystemElectricity.SubsystemTerrain;
            m_subsystemMatrixDisplay = SubsystemElectricity.Project.FindSubsystem<SubsystemASMatrixDisplay>();
            m_subsystemControllers = SubsystemElectricity.Project.FindSubsystem<SubsystemASMExpandableLEDControllers>();
        }

        public override void OnAdded() {
            base.OnAdded();
            // m_matrixDisplayData = m_subsystemMatrixDisplay.Add(true);
            // m_matrixDisplayData.Height = 1;
            // m_matrixDisplayData.Width = 1;
            // m_matrixDisplayData.DisplayPoint = CellFaces[0];
            // m_matrixDisplayData.DisplayType = ASMatrixDisplayType.ColumnLines | ASMatrixDisplayType.RowLines;

            UpdateController();
        }

        public override void OnRemoved() {
            base.OnRemoved();
            if(m_subsystemControllers.IsThereAController(CellFaces[0])) m_subsystemControllers.RemoveController(m_subsystemControllers.GetController(CellFaces[0]));

            ASMUtils.FaceToAxesAndConner(CellFaces[0].Face, out Point3[] axes, out _);
            Point3 center = CellFaces[0].Point;
            for (int i = 0; i < 4; i++) {
                Point3 checkPoint = center + axes[i];
                int blockID = Terrain.ExtractContents(m_subsystemTerrain.Terrain.GetCellValueFast(checkPoint.X, checkPoint.Y, checkPoint.Z));
                if (blockID == ASMExpandableLEDBlock.Index) {
                    ASMExpandableLEDElectricElement? electricElement = SubsystemElectricity.GetElectricElement(checkPoint.X, checkPoint.Y, checkPoint.Z, CellFaces[0].Face) as ASMExpandableLEDElectricElement;
                    electricElement?.UpdateController();
                }
            }
        }


        public override bool Simulate() {
            Matrix voltage = m_outputMatrix;
            m_outputMatrix = Matrix.Zero;
            foreach (ASMElectricConnection connection in Connections)
            {
                if (connection.ConnectorType != ASMElectricConnectorType.Output && connection.NeighborConnectorType != 0)
                {
                    m_outputMatrix += connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                }
            }
            if(m_outputMatrix != voltage) m_subsystemControllers.GetController(CellFaces[0])?.CollectAndDisplayCellsMatrix();//如果输入的矩阵值发生变化，则告诉控制器重新收集各LED的矩阵电压

            if (m_subsystemControllers.IsThereAController(CellFaces[0])) {
                // m_matrixDisplayData.Matrix = m_subsystemControllers.GetController(CellFaces[0]).DisplayMatrix;
            }
            m_subsystemControllers.GetController(CellFaces[0])?.SimulateControlledElement();
            return false;
        }

        public async void UpdateController() {
            CellFace cellFace = CellFaces[0];
            Dictionary<CellFace, ASMExpandableLEDElectricElement> all = new();
            await GetConnectedElectricElements(cellFace.X, cellFace.Y, cellFace.Z, cellFace.Face, all);

            //已经有控制器，则删除旧的
            foreach (var cell in all.Keys) {
                if(m_subsystemControllers.IsThereAController(cell)) m_subsystemControllers.RemoveController(m_subsystemControllers.GetController(cell));
            }

            //为新的板子添加新控制器实例与对应关系
            m_subsystemControllers.AddControllerByPoints(all.Keys.ToArray(), new ASMExpandableLEDController(all, SubsystemElectricity));
        }

        public async Task GetConnectedElectricElements(int x, int y, int z, int face, Dictionary<CellFace, ASMExpandableLEDElectricElement> parent) {
            if (parent.ContainsKey(new CellFace(x, y, z, face))) return;
            //先获取本地的电路元件
            ASMExpandableLEDElectricElement electricElement = SubsystemElectricity.GetElectricElement(x, y, z, face) as ASMExpandableLEDElectricElement;
            parent.Add(new CellFace(x, y, z, face), electricElement);

            Point3 center = new(x, y, z);
            Point3[] axes = new Point3[4];

            ASMUtils.FaceToAxesAndConner(face, out axes, out _);

            for (int i = 0; i < 4; i++) {
                Point3 checkPoint = center + axes[i];
                int blockValue = m_subsystemTerrain.Terrain.GetCellValueFast(checkPoint.X, checkPoint.Y, checkPoint.Z);
                int blockID = Terrain.ExtractContents(blockValue);
                if (blockID == ASMExpandableLEDBlock.Index && GetFace(blockValue) == face) {
                    //可以往某方向走
                    await GetConnectedElectricElements(checkPoint.X, checkPoint.Y, checkPoint.Z, face, parent);
                }
            }
            int GetFace(int value) => (Terrain.ExtractData(value) >> 2) & 7;
        }
    }
}