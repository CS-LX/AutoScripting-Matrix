using Engine;

namespace Game {
    public class ASMTransGateElectricElement : ASMRotateableElectricElement {
        public Matrix m_voltage;
        public int m_type;

        public ASMTransGateElectricElement(SubsystemASMElectricity subsystemElectricity, CellFace cellFace, int value)
            : base(subsystemElectricity, cellFace) {
            m_type = ASMTransGateBlock.GetType(Terrain.ExtractData(value));
        }

        public override Matrix GetOutputVoltage(int face)
        {
            return m_voltage;
        }

        public override bool Simulate() {
            Matrix voltage = m_voltage;
            Matrix input = Matrix.Zero;
            int rotation = Rotation;
            foreach (ASMElectricConnection connection in Connections) {
                if (connection.ConnectorType != ASMElectricConnectorType.Output
                    && connection.NeighborConnectorType != 0) {
                    ASMElectricConnectorDirection? connectorDirection = SubsystemASMElectricity.GetConnectorDirection(CellFaces[0].Face, rotation, connection.ConnectorFace);
                    if (connectorDirection.HasValue) {
                        if (connectorDirection == ASMElectricConnectorDirection.Bottom) {
                            input = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                        }
                    }
                }
            }

            switch (m_type) {
                case 0: m_voltage = Matrix.Invert(input);//求逆
                    break;
                case 1: m_voltage = Matrix.Transpose(input);//转置
                    break;
                case 2: m_voltage = input * input;
                    break;
                case 3: m_voltage = input * input * input;
                    break;
                case 4: m_voltage = new Matrix { M11 = input.ToAverageFloat() };
                    break;
                case 5: m_voltage = input.ToFloat().ToCMatrix();
                    break;
                case 6: m_voltage = new Matrix { Right = input.ToVector3T() };
                    break;
                case 7: m_voltage = input.OrientationMatrix;
                    break;
                case 8: m_voltage = input.TranslationMatrix;
                    break;
                case 9: m_voltage = ASMStaticMethods.Rad2Deg(input);
                    break;
                case 10: m_voltage = ASMStaticMethods.Deg2Rad(input);
                    break;
            }

            return m_voltage != voltage;
        }
    }
}