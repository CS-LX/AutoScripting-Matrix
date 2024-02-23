using Engine;

namespace Game {
    public class ASMExpandableLEDElectricElement : ASMMountedElectricElement {
        public SubsystemTerrain m_subsystemTerrain;

        public ASMExpandableLEDElectricElement(SubsystemASMElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace) {
            m_subsystemTerrain = SubsystemElectricity.SubsystemTerrain;
        }

        public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner) {
            CellFace cellFace = CellFaces[0];
            Dictionary<CellFace, ASMExpandableLEDElectricElement> all = new();
            GetConnectedElectricElements(cellFace.X, cellFace.Y, cellFace.Z, cellFace.Face, all);
            SubsystemElectricity.SubsystemAudio.PlaySound("Audio/Click", 1f, 0f, new Vector3(cellFace.X, cellFace.Y, cellFace.Z), 2f, autoDelay: true);
            return true;
        }

        public void GetConnectedElectricElements(int x, int y, int z, int face, Dictionary<CellFace, ASMExpandableLEDElectricElement> parent) {

            if (parent.ContainsKey(new CellFace(x, y, z, face))) return;//有本地的，重复了，直接返回
            //先获取本地的电路元件
            ASMExpandableLEDElectricElement electricElement = SubsystemElectricity.GetElectricElement(x, y, z, 4) as ASMExpandableLEDElectricElement;
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