using Engine;

namespace Game {
    public class ASMDecomposerElectricElement : ASMRotateableElectricElement {

        public int m_type;

        public Matrix m_voltage_top;
        public Matrix m_voltage_bottom;
        public Matrix m_voltage_left;
        public Matrix m_voltage_right;
        public Matrix m_voltage_in;

        public ASMDecomposerElectricElement(SubsystemASMElectricity subsystemElectricity, CellFace cellFace, int value)
            : base(subsystemElectricity, cellFace) {
            m_type = ASMDecomposerBlock.GetType(Terrain.ExtractData(value));
        }

        public override Matrix GetOutputVoltage(int face) {
            ASMElectricConnectorDirection? connectorDirection = SubsystemASMElectricity.GetConnectorDirection(CellFaces[0].Face, Rotation, face);
            return connectorDirection switch {
                ASMElectricConnectorDirection.Bottom => m_voltage_bottom,
                ASMElectricConnectorDirection.Left => m_voltage_left,
                ASMElectricConnectorDirection.Right => m_voltage_right,
                ASMElectricConnectorDirection.In => m_voltage_in,
                ASMElectricConnectorDirection.Top => m_voltage_top,
                _ => m_voltage_top
            };
        }

        public override bool Simulate() {
            Matrix voltage_top = m_voltage_top;
            Matrix voltage_bottom = m_voltage_bottom;
            Matrix voltage_left = m_voltage_left;
            Matrix voltage_right = m_voltage_right;
            Matrix voltage_in = m_voltage_in;

            Matrix input_top = Matrix.Zero;
            Matrix input_bottom = Matrix.Zero;
            Matrix input_left = Matrix.Zero;
            Matrix input_right = Matrix.Zero;
            Matrix input_in = Matrix.Zero;

            foreach (ASMElectricConnection connection in Connections) {
                if (connection.ConnectorType != ASMElectricConnectorType.Output
                    && connection.NeighborConnectorType != 0) {
                    ASMElectricConnectorDirection? connectorDirection = SubsystemASMElectricity.GetConnectorDirection(CellFaces[0].Face, Rotation, connection.ConnectorFace);
                    if (connectorDirection.HasValue) {
                        switch (connectorDirection) {
                            case ASMElectricConnectorDirection.Bottom:
                                input_bottom = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                                break;
                            case ASMElectricConnectorDirection.In:
                                input_in = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                                break;
                            case ASMElectricConnectorDirection.Left:
                                input_left = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                                break;
                            case ASMElectricConnectorDirection.Right:
                                input_right = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                                break;
                            case ASMElectricConnectorDirection.Top:
                                input_top = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                                break;
                        }
                    }
                }
            }

            switch (m_type) {
                case 0://TO解构器
                    m_voltage_left = input_bottom.TranslationMatrix;
                    m_voltage_right = input_bottom.OrientationMatrix;
                    break;
                case 1: input_bottom.Decompose(out Vector3 scale, out Quaternion rotation, out Vector3 translation);
                    m_voltage_left.Right = translation;
                    m_voltage_top.Right = rotation.ToYawPitchRoll();
                    m_voltage_right.Right = scale;
                    break;
            }

            return m_voltage_top != voltage_top || m_voltage_left != voltage_left || m_voltage_right != voltage_right || m_voltage_bottom != voltage_bottom || m_voltage_in != voltage_in;
        }
    }
}