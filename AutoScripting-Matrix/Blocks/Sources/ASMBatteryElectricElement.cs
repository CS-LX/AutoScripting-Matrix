using Engine;

namespace Game {
    public class ASMBatteryElectricElement : ASMElectricElement {
        public SubsystemASMBatteryBlockBehavior asmBatteryBlockBehavior;
        public Matrix m_voltage = Matrix.Identity;
        public ASMBatteryElectricElement(SubsystemASMElectricity subsystemElectricity, CellFace cellFace)
            : base(subsystemElectricity, cellFace) {
            asmBatteryBlockBehavior = subsystemElectricity.Project.FindSubsystem<SubsystemASMBatteryBlockBehavior>(true);
        }

        public override Matrix GetOutputVoltage(int face) {
            return m_voltage;
        }

        public override bool Simulate() {
            Matrix voltage = m_voltage;
            ASMBatteryData data = asmBatteryBlockBehavior.GetBlockData(CellFaces[0].Point);
            m_voltage = data == null ? Matrix.Identity : data.Data;
            return m_voltage != voltage;
        }
    }
}