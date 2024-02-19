using Engine;

namespace Game {
    public class ASMRelayElectricElement : ASMRotateableElectricElement {

        public Matrix m_voltage;
        public ASMRelayElectricElement(SubsystemASMElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace) { }

        public override Matrix GetOutputVoltage(int face) => m_voltage;

        public override bool Simulate() {
            Matrix voltage = m_voltage;

            Matrix leftInput = Matrix.Zero;
            Matrix rightInput = Matrix.Zero;
            Matrix bottomInput = Matrix.Zero;

            foreach (ASMElectricConnection connection in Connections) {
                if (connection.ConnectorType != ASMElectricConnectorType.Output
                    && connection.NeighborConnectorType != 0) {
                    ASMElectricConnectorDirection? connectorDirection = SubsystemASMElectricity.GetConnectorDirection(CellFaces[0].Face, Rotation, connection.ConnectorFace);
                    if (connectorDirection.HasValue) {
                        if (connectorDirection == ASMElectricConnectorDirection.Left) leftInput = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                        else if (connectorDirection == ASMElectricConnectorDirection.Right) rightInput = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                        else if (connectorDirection == ASMElectricConnectorDirection.Bottom) bottomInput = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                    }
                }
            }

            m_voltage = (leftInput.ToFloat() > 0 || rightInput.ToFloat() > 0) ? bottomInput : Matrix.Zero;

            return m_voltage != voltage;
        }
    }
}