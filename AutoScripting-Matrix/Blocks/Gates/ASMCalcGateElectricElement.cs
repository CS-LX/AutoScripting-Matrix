using Engine;

namespace Game {
    public class ASMCalcGateElectricElement : ASMRotateableElectricElement {
        public Matrix m_voltage;
        public int m_type;

        public ASMCalcGateElectricElement(SubsystemASMElectricity subsystemElectricity, CellFace cellFace, int value)
            : base(subsystemElectricity, cellFace) {
            m_type = ASMCalcGateBlock.GetType(Terrain.ExtractData(value));
        }

        public override Matrix GetOutputVoltage(int face)
        {
            return m_voltage;
        }

        public override bool Simulate() {
            Matrix voltage = m_voltage;
            Matrix leftInput = Matrix.Zero;
            Matrix rightInput = Matrix.Zero;
            int rotation = Rotation;
            foreach (ASMElectricConnection connection in Connections) {
                if (connection.ConnectorType != ASMElectricConnectorType.Output
                    && connection.NeighborConnectorType != 0) {
                    ASMElectricConnectorDirection? connectorDirection = SubsystemASMElectricity.GetConnectorDirection(CellFaces[0].Face, rotation, connection.ConnectorFace);
                    if (connectorDirection.HasValue) {
                        if (connectorDirection == ASMElectricConnectorDirection.Right) {
                            rightInput = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                        }
                        else if (connectorDirection == ASMElectricConnectorDirection.Left) {
                            leftInput = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                        }
                    }
                }
            }

            switch (m_type) {
                case 0: m_voltage = leftInput + rightInput;//加法器
                    break;
                case 1: m_voltage = leftInput - rightInput;//减法器
                    break;
                case 2: m_voltage = leftInput * rightInput;//乘法器
                    break;
            }

            return m_voltage != voltage;
        }
    }
}