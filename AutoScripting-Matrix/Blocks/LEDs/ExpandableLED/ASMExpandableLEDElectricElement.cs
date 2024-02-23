using Engine;

namespace Game {
    public class ASMExpandableLEDElectricElement : ASMMountedElectricElement {
        public ASMELEDController m_controller;
        public SubsystemTerrain m_subsystemTerrain;

        public ASMExpandableLEDElectricElement(SubsystemASMElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace) {
            m_subsystemTerrain = SubsystemElectricity.SubsystemTerrain;
        }

        public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner) {
            CellFace cellFace = CellFaces[0];
            Dictionary<CellFace, ASMExpandableLEDElectricElement> all = new();
            GetConnectedElectricElements(cellFace.X, cellFace.Y, cellFace.Z, all);
            SubsystemElectricity.SubsystemAudio.PlaySound("Audio/Click", 1f, 0f, new Vector3(cellFace.X, cellFace.Y, cellFace.Z), 2f, autoDelay: true);
            return true;
        }

        public void GetConnectedElectricElements(int x, int y, int z, Dictionary<CellFace, ASMExpandableLEDElectricElement> parent) {

            if (parent.ContainsKey(new CellFace(x, y, z, 4))) return;//有本地的，重复了，直接返回
            //先获取本地的电路元件
            ASMExpandableLEDElectricElement electricElement = SubsystemElectricity.GetElectricElement(x, y, z, 4) as ASMExpandableLEDElectricElement;
            parent.Add(new CellFace(x, y, z, 4), electricElement);

            int blockID_X = Terrain.ExtractContents(m_subsystemTerrain.Terrain.GetCellValueFast(x + 1, y, z));
            int blockID_Z = Terrain.ExtractContents(m_subsystemTerrain.Terrain.GetCellValueFast(x , y, z + 1));
            int blockID_NX = Terrain.ExtractContents(m_subsystemTerrain.Terrain.GetCellValueFast(x - 1, y, z));
            int blockID_NZ = Terrain.ExtractContents(m_subsystemTerrain.Terrain.GetCellValueFast(x, y, z - 1));

            if (blockID_X == ASMExpandableLEDBlock.Index) {//可以往x方向走
                GetConnectedElectricElements(x + 1, y, z, parent);
            }
            if (blockID_Z == ASMExpandableLEDBlock.Index) {//可以往z方向走
                GetConnectedElectricElements(x, y, z + 1, parent);
            }
            if (blockID_NX == ASMExpandableLEDBlock.Index) {//可以往-x方向走
                GetConnectedElectricElements(x - 1, y, z, parent);
            }
            if (blockID_NZ == ASMExpandableLEDBlock.Index) {//可以往-z方向走
                GetConnectedElectricElements(x, y, z - 1, parent);
            }
        }
    }
}