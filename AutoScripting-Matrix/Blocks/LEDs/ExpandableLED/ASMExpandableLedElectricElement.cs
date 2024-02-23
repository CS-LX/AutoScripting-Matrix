using Engine;

namespace Game {
    public class ASMExpandableLedElectricElement : ASMMountedElectricElement {

        SubsystemASMGlow m_subsystemGlow;
        SubsystemTerrain m_subsystemTerrain;
        SubsystemASMExpandableLEDController m_subsystemASMExpandableLEDController;

        //debug
        ASMGlowCuboid m_glowCuboid;
        ASMGlowLine m_glowLine;

        public ASMELEDController m_controller;

        public ASMExpandableLedElectricElement(SubsystemASMElectricity subsystemElectricity, CellFace cellFace)
            : base(subsystemElectricity, cellFace)
        {
            m_subsystemGlow = subsystemElectricity.Project.FindSubsystem<SubsystemASMGlow>(throwOnError: true);
            m_subsystemTerrain = subsystemElectricity.Project.FindSubsystem<SubsystemTerrain>(throwOnError: true);
            m_subsystemASMExpandableLEDController = subsystemElectricity.Project.FindSubsystem<SubsystemASMExpandableLEDController>(throwOnError: true);
        }

        public override void OnAdded() {
            m_glowCuboid = m_subsystemGlow.AddGlowCuboid();
            m_glowCuboid.m_start = new Vector3(CellFaces[0].Point) + new Vector3(0.2f, 0.2f, 0.2f);
            m_glowCuboid.m_end = new Vector3(CellFaces[0].Point) + new Vector3(0.8f, 0.8f, 0.8f);
            m_glowCuboid.m_color = Color.Transparent;

            m_glowLine = m_subsystemGlow.AddGlowLine();

            ASMELEDController[] neighborControllers = FindNeighborControllers();
            if (neighborControllers.Length == 0) {//如果四周没有LED控制器
                m_controller = m_subsystemASMExpandableLEDController.FindController(CellFaces[0].Point) ?? m_subsystemASMExpandableLEDController.AddController(CellFaces[0].Point);
            }
            else {
                m_controller = neighborControllers[0];
            }
        }

        public override bool Simulate() {
            if (m_controller == null) {
                m_glowLine.m_color1 = m_glowLine.m_color2 = Color.Transparent;
                return false;
            }
            m_glowLine.m_start = new Vector3(m_subsystemASMExpandableLEDController.FindPoint(m_controller)) + Vector3.One * 0.5f;
            m_glowLine.m_end = new Vector3(CellFaces[0].Point) + Vector3.One * 0.5f;
            m_glowLine.m_color1 = Color.Blue;
            m_glowLine.m_color2 = Color.Cyan;
            m_glowCuboid.m_color = m_subsystemASMExpandableLEDController.FindPoint(m_controller) == CellFaces[0].Point ? Color.Yellow : Color.Transparent;

            SubsystemElectricity.QueueElectricElementForSimulation(this, SubsystemElectricity.CircuitStep + 10);
            return false;
        }

        public override void OnRemoved() {
            m_subsystemGlow.RemoveGlowGeometry(m_glowCuboid);
            m_subsystemGlow.RemoveGlowGeometry(m_glowLine);
            if (m_subsystemASMExpandableLEDController.FindPoint(m_controller) == CellFaces[0].Point) {//移除的LED在自己位置存在控制器
                m_subsystemASMExpandableLEDController.RemoveController(CellFaces[0].Point);
            }
        }

        public ASMELEDController[] FindNeighborControllers() {
            CellFace cellFace = CellFaces[0];
            List<ASMELEDController> controllers = new List<ASMELEDController>();
            Point3[] axes = new Point3[4];
            switch (cellFace.Face) {
                case 4:
                    axes = [Point3.UnitX, Point3.UnitZ, -Point3.UnitX, -Point3.UnitZ];
                    break;
            }
            //获取四个相邻面的LED，不重复
            for (int i = 0; i < 4; i++) {
                Point3 point = cellFace.Point;
                Point3 findPoint = point + axes[i];
                int neighborValue = m_subsystemTerrain.Terrain.GetCellValueFast(findPoint.X, findPoint.Y, findPoint.Z);
                if (Terrain.ExtractContents(neighborValue) != ASMExpandableLEDBlock.Index) continue;
                int neighborFace = GetFace(neighborValue);
                if (neighborFace != cellFace.Face) continue;

                ASMExpandableLedElectricElement? ledElement = SubsystemElectricity.GetElectricElement(findPoint.X, findPoint.Y, findPoint.Z, cellFace.Face) as ASMExpandableLedElectricElement;
                if(ledElement == null) continue;
                ASMELEDController controller = ledElement.m_controller;
                if(!controllers.Contains(controller)) controllers.Add(controller);
            }
            return controllers.ToArray();
        }

        public int GetFace(int value)
        {
            return (Terrain.ExtractData(value) >> 2) & 7;
        }
    }
}