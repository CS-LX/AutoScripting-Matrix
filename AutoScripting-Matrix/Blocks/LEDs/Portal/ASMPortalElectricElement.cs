using Engine;

namespace Game {
    public class ASMPortalElectricElement : ASMRotateableElectricElement {
        public ASMPortalElectricElement(SubsystemASMElectricity subsystemElectricity, CellFace cellFace, int blockValue) : base(subsystemElectricity, cellFace) {
            m_blockValue = blockValue;
        }

        public Matrix m_voltage_top;
        public Matrix m_voltage_bottom;
        public Matrix m_voltage_left;
        public Matrix m_voltage_right;
        public Matrix m_voltage_in;

        public int m_blockValue;

        public Matrix GetPortal1Transform() => m_voltage_top;

        public Matrix GetControl1() => m_voltage_right;

        public Matrix GetPortal2Transform() => m_voltage_bottom;

        public Matrix GetControl2() => m_voltage_left;

        public override bool Simulate() {
            m_voltage_bottom = m_voltage_in = m_voltage_right = m_voltage_top = m_voltage_left = Matrix.Zero;
            foreach (ASMElectricConnection connection in Connections) {
                if (connection.ConnectorType != ASMElectricConnectorType.Output
                    && connection.NeighborConnectorType != 0) {
                    ASMElectricConnectorDirection? connectorDirection = SubsystemASMElectricity.GetConnectorDirection(CellFaces[0].Face, Rotation, connection.ConnectorFace);
                    if (connectorDirection.HasValue) {
                        switch (connectorDirection) {
                            case ASMElectricConnectorDirection.Bottom:
                                m_voltage_bottom = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                                break;
                            case ASMElectricConnectorDirection.In:
                                m_voltage_in = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                                break;
                            case ASMElectricConnectorDirection.Left:
                                m_voltage_left = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                                break;
                            case ASMElectricConnectorDirection.Right:
                                m_voltage_right = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                                break;
                            case ASMElectricConnectorDirection.Top:
                                m_voltage_top = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                                break;
                        }
                    }
                }
            }
            return false;
        }
    }
}