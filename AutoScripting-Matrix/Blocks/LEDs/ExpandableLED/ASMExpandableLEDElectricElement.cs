using Engine;

namespace Game {
    public class ASMExpandableLEDElectricElement : ASMMountedElectricElement {
        public SubsystemTerrain m_subsystemTerrain;
        public SubsystemASMGlow m_subsystemGlow;

        public SubsystemASMExpandableLEDControllers m_subsystemControllers;

        public ASMGlowCuboid m_glowCubiod;

        public ASMExpandableLEDElectricElement(SubsystemASMElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace) {
            m_subsystemTerrain = SubsystemElectricity.SubsystemTerrain;
            m_subsystemGlow = SubsystemElectricity.Project.FindSubsystem<SubsystemASMGlow>();
            m_subsystemControllers = SubsystemElectricity.Project.FindSubsystem<SubsystemASMExpandableLEDControllers>();
        }

        public override void OnAdded() {
            base.OnAdded();
            m_glowCubiod = m_subsystemGlow.AddGlowCuboid();
            m_glowCubiod.m_start = new Vector3(CellFaces[0].Point) + Vector3.One * 0.2f;
            m_glowCubiod.m_end = new Vector3(CellFaces[0].Point) + Vector3.One * 0.8f;
        }

        public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner) {
            CellFace cellFace = CellFaces[0];
            Dictionary<CellFace, ASMExpandableLEDElectricElement> all = new();
            GetConnectedElectricElements(cellFace.X, cellFace.Y, cellFace.Z, cellFace.Face, all);

            //已经有控制器，则删除旧的
            foreach (var cell in all.Keys) {
                if(m_subsystemControllers.IsThereAController(cell)) m_subsystemControllers.RemoveController(m_subsystemControllers.GetController(cell));
            }

            //为新的板子添加新控制器实例与对应关系
            m_subsystemControllers.AddControllerByPoints(all.Keys.ToArray(), new ASMExpandableLEDController());

            SubsystemElectricity.SubsystemAudio.PlaySound("Audio/Click", 1f, 0f, new Vector3(cellFace.X, cellFace.Y, cellFace.Z), 2f, autoDelay: true);
            return true;
        }

        public override void OnRemoved() {
            base.OnRemoved();
            m_subsystemControllers.RemovePoint(CellFaces[0]);
            m_subsystemGlow.RemoveGlowGeometry(m_glowCubiod);
        }


        public override bool Simulate() {
            Console.WriteLine(m_subsystemControllers.GetControllersCount());
            if (m_subsystemControllers.IsThereAController(CellFaces[0])) {
                m_glowCubiod.m_color = Color.Yellow;
            }
            else {
                m_glowCubiod.m_color = Color.Transparent;
            }
            SubsystemElectricity.QueueElectricElementForSimulation(this, SubsystemElectricity.CircuitStep + 10);
            return false;
        }

        public void GetConnectedElectricElements(int x, int y, int z, int face, Dictionary<CellFace, ASMExpandableLEDElectricElement> parent) {

            if (parent.ContainsKey(new CellFace(x, y, z, face))) return;//有本地的，重复了，直接返回
            //先获取本地的电路元件
            ASMExpandableLEDElectricElement electricElement = SubsystemElectricity.GetElectricElement(x, y, z, face) as ASMExpandableLEDElectricElement;
            parent.Add(new CellFace(x, y, z, face), electricElement);

            Point3 center = new(x, y, z);
            Point3[] axes = new Point3[4];

            ASMELEDUtils.FaceToAxesAndConner(face, out axes, out _);

            for (int i = 0; i < 4; i++) {
                Point3 checkPoint = center + axes[i];
                int blockID = Terrain.ExtractContents(m_subsystemTerrain.Terrain.GetCellValueFast(checkPoint.X, checkPoint.Y, checkPoint.Z));
                if (blockID == ASMExpandableLEDBlock.Index) {
                    //可以往某方向走
                    GetConnectedElectricElements(checkPoint.X, checkPoint.Y, checkPoint.Z, face, parent);
                }
            }
        }
    }
}