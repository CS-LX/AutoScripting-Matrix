using Engine;

namespace Game
{
    public class ASMButtonElectricElement : ASMMountedElectricElement
    {
        public Matrix m_pressedVoltage;

        public Matrix m_voltage;

        public bool m_wasPressed;

        public SubsystemASMButtonBlockBehavior subsystemAsmButtonBlockBehavior;

        public ASMButtonElectricElement(SubsystemASMElectricity subsystemElectricity, CellFace cellFace, int value)
            : base(subsystemElectricity, cellFace) {
            subsystemAsmButtonBlockBehavior = subsystemElectricity.Project.FindSubsystem<SubsystemASMButtonBlockBehavior>();
            ASMatrixData matrixData = subsystemAsmButtonBlockBehavior.GetBlockData(cellFace.Point);
            Matrix voltageLevel = matrixData == null ? Matrix.Identity : matrixData.Data;
            m_pressedVoltage = voltageLevel;
        }

        public void Press()
        {
            if (!m_wasPressed && m_voltage == Matrix.Zero)
            {
                m_wasPressed = true;
                CellFace cellFace = base.CellFaces[0];
                base.SubsystemElectricity.SubsystemAudio.PlaySound("Audio/Click", 1f, 0f, new Vector3(cellFace.X, cellFace.Y, cellFace.Z), 2f, autoDelay: true);
                base.SubsystemElectricity.QueueElectricElementForSimulation(this, base.SubsystemElectricity.CircuitStep + 1);
            }
        }

        public override Matrix GetOutputVoltage(int face)
        {
            return m_voltage;
        }

        public override bool Simulate()
        {
            Matrix voltage = m_voltage;
            ASMatrixData matrixData = subsystemAsmButtonBlockBehavior.GetBlockData(CellFaces[0].Point);
            Matrix voltageLevel = matrixData == null ? Matrix.Identity : matrixData.Data;
            m_pressedVoltage = voltageLevel;
            if (m_wasPressed)
            {
                m_wasPressed = false;
                m_voltage = m_pressedVoltage;
                base.SubsystemElectricity.QueueElectricElementForSimulation(this, base.SubsystemElectricity.CircuitStep + 10);
            }
            else
            {
                m_voltage = Matrix.Zero;
            }
            return m_voltage != voltage;
        }

        public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
        {
            Press();
            return true;
        }

        public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
        {
            Press();
        }
    }
}