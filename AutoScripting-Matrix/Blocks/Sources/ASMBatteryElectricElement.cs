using Engine;

namespace Game {
    public class ASMBatteryElectricElement : ASMElectricElement {
        public ASMBatteryElectricElement(SubsystemASMElectricity subsystemElectricity, CellFace cellFace)
            : base(subsystemElectricity, cellFace)
        {
        }

        public override Matrix GetOutputVoltage(int face)
        {
            return new Matrix(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1);
        }

        public override void OnNeighborBlockChanged(CellFace cellFace, int neighborX, int neighborY, int neighborZ)
        {
            int cellValue = SubsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y - 1, cellFace.Z);
            Block block = BlocksManager.Blocks[Terrain.ExtractContents(cellValue)];
            if (!block.IsCollidable_(cellValue) || block.IsTransparent_(cellValue))
            {
                SubsystemElectricity.SubsystemTerrain.DestroyCell(0, cellFace.X, cellFace.Y, cellFace.Z, 0, noDrop: false, noParticleSystem: false);
            }
        }
    }
}