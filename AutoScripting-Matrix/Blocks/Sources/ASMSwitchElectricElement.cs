

using Engine;

namespace Game {
    public class ASMSwitchElectricElement : ASMMountedElectricElement {
        public Matrix m_voltage = Matrix.Identity;
        public SubsystemASMSwitchBlockBehavior subsystemASMSwitchBlockBehavior;
        public ASMSwitchElectricElement(SubsystemASMElectricity subsystemElectricity, CellFace cellFace, int value)
            : base(subsystemElectricity, cellFace)
        {
            subsystemASMSwitchBlockBehavior = subsystemElectricity.Project.FindSubsystem<SubsystemASMSwitchBlockBehavior>(true);
            ASMatrixData matrixData = subsystemASMSwitchBlockBehavior.GetBlockData(cellFace.Point);
            Matrix voltageLevel = matrixData == null ? Matrix.Identity : matrixData.Data;
            m_voltage = ASMSwitchBlock.GetLeverState(value) ? voltageLevel : Matrix.Zero;
        }

        public override Matrix GetOutputVoltage(int face)
        {
            return m_voltage;
        }

        public override bool Simulate() {//以便编辑数据后模拟
            Matrix voltage = m_voltage;
            ASMatrixData matrixData = subsystemASMSwitchBlockBehavior.GetBlockData(CellFaces[0].Point);
            Matrix voltageLevel = matrixData == null ? Matrix.Identity : matrixData.Data;
            m_voltage = ASMSwitchBlock.GetLeverState(SubsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(CellFaces[0].X, CellFaces[0].Y, CellFaces[0].Z)) ? voltageLevel : Matrix.Zero;
            return m_voltage != voltage;
        }

        public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
        {
            CellFace cellFace = base.CellFaces[0];
            int cellValue = base.SubsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z);
            int value = ASMSwitchBlock.SetLeverState(cellValue, !ASMSwitchBlock.GetLeverState(cellValue));
            base.SubsystemElectricity.SubsystemTerrain.ChangeCell(cellFace.X, cellFace.Y, cellFace.Z, value);
            base.SubsystemElectricity.SubsystemAudio.PlaySound("Audio/Click", 1f, 0f, new Vector3(cellFace.X, cellFace.Y, cellFace.Z), 2f, autoDelay: true);
            return true;
        }
    }
}