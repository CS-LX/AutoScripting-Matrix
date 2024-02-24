using Engine;

namespace Game {
    public class ASMExpandableLEDElectricElement : ASMMountedElectricElement {
        public SubsystemTerrain m_subsystemTerrain;
        public SubsystemASMGlow m_subsystemGlow;

        public List<ASMGlowCuboid> m_glowCuboids = new List<ASMGlowCuboid>();

        public ASMExpandableLEDElectricElement(SubsystemASMElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace) {
            m_subsystemTerrain = SubsystemElectricity.SubsystemTerrain;
            m_subsystemGlow = SubsystemElectricity.Project.FindSubsystem<SubsystemASMGlow>();
        }

        public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner) {
            CellFace cellFace = CellFaces[0];

            RemoveGlows();//删除发光点（引用类型，其他LED内的一并会删除）
            Dictionary<CellFace, ASMExpandableLEDElectricElement> all = new();
            GetConnectedElectricElements(cellFace.X, cellFace.Y, cellFace.Z, cellFace.Face, all);
            foreach (var cell in all) {
                if (cell.Value.m_glowCuboids != m_glowCuboids) cell.Value.RemoveGlows();//相邻的LED发光点不是引用自自己这的
                ASMGlowCuboid m_glowCuboid = m_subsystemGlow.AddGlowCuboid();
                m_glowCuboid = m_subsystemGlow.AddGlowCuboid();
                m_glowCuboid.m_start = new Vector3(cell.Key.Point) + Vector3.One * 0.2f;
                m_glowCuboid.m_end = new Vector3(cell.Key.Point) + Vector3.One * 0.8f;
                m_glowCuboid.m_color = Color.Yellow;
                m_glowCuboids.Add(m_glowCuboid);
                cell.Value.m_glowCuboids = m_glowCuboids;//设置相邻LED的发光点为自己这引用，一整块LED共用一个发光点
            }
            SubsystemElectricity.SubsystemAudio.PlaySound("Audio/Click", 1f, 0f, new Vector3(cellFace.X, cellFace.Y, cellFace.Z), 2f, autoDelay: true);
            return true;
        }

        public override void OnRemoved() {
            base.OnRemoved();
            RemoveGlows();
        }

        public void RemoveGlows() {
            for (int i = 0; i < m_glowCuboids.Count; i++) {
                m_subsystemGlow.RemoveGlowGeometry(m_glowCuboids[i]);
            }
            m_glowCuboids.Clear();
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