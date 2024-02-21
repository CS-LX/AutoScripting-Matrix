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

            Matrix m = input_in;

            switch (m_type) {
                case 0://TO解构器
                    m_voltage_left = input_bottom.TranslationMatrix;
                    m_voltage_right = input_bottom.OrientationMatrix;
                    break;
                case 1: input_bottom.Decompose(out Vector3 scale, out Quaternion rotation, out Vector3 translation);//TRS解构器
                    m_voltage_left.Right = translation;
                    m_voltage_top.Right = rotation.ToYawPitchRoll();
                    m_voltage_right.Right = scale;
                    break;
                case 2: //RUF(XYZ)解构器
                    m_voltage_left.Right = input_bottom.Right;
                    m_voltage_top.Right = input_bottom.Up;
                    m_voltage_right.Right = input_bottom.Forward;
                    break;
                case 3:
                    m_voltage_top = new Matrix(m.M11, m.M12, 0, 0, m.M21, m.M22, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                    m_voltage_right = new Matrix(m.M13, m.M14, 0, 0, m.M23, m.M24, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                    m_voltage_bottom = new Matrix(m.M31, m.M32, 0, 0, m.M41, m.M42, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                    m_voltage_left = new Matrix(m.M33, m.M34, 0, 0, m.M43, m.M44, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                    break;
                case 4:
                    m_voltage_top = new Matrix(m.M11, 0, 0, 0, m.M21, 0, 0, 0, m.M31, 0, 0, 0, m.M41, 0, 0, 0);
                    m_voltage_right = new Matrix(m.M12, 0, 0, 0, m.M22, 0, 0, 0, m.M32, 0, 0, 0, m.M42, 0, 0, 0);
                    m_voltage_bottom = new Matrix(m.M13, 0, 0, 0, m.M23, 0, 0, 0, m.M33, 0, 0, 0, m.M43, 0, 0, 0);
                    m_voltage_left = new Matrix(m.M14, 0, 0, 0, m.M24, 0, 0, 0, m.M34, 0, 0, 0, m.M44, 0, 0, 0);
                    break;
                case 5:
                    m_voltage_top.M11 = m.M11;
                    m_voltage_right.M11 = m.M21;
                    m_voltage_bottom.M11 = m.M31;
                    m_voltage_left.M11 = m.M41;
                    break;
                case 6:
                    m_voltage_top = new Matrix(m.M11, m.M12, m.M13, m.M14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                    m_voltage_right = new Matrix(m.M21, m.M22, m.M23, m.M24, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                    m_voltage_bottom = new Matrix(m.M31, m.M32, m.M33, m.M34, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                    m_voltage_left = new Matrix(m.M41, m.M42, m.M43, m.M44, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                    break;
                case 7:
                    m_voltage_top.M11 = m.M11;
                    m_voltage_right.M11 = m.M12;
                    m_voltage_bottom.M11 = m.M13;
                    m_voltage_left.M11 = m.M14;
                    break;
            }

            return m_voltage_top != voltage_top || m_voltage_left != voltage_left || m_voltage_right != voltage_right || m_voltage_bottom != voltage_bottom || m_voltage_in != voltage_in;
        }
    }
}