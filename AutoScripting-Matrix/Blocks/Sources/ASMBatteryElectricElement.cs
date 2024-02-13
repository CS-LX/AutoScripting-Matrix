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
    }
}