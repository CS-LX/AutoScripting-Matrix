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
                case 0: m_voltage = leftInput + rightInput;//加法器(点对点)
                    break;
                case 1: m_voltage = leftInput - rightInput;//减法器(点对点)
                    break;
                case 2: m_voltage = ASMStaticMethods.Multiply(leftInput, rightInput);//乘法器(点对点)
                    break;
                case 3: m_voltage = leftInput / rightInput;//除法器(点对点)
                    break;
                case 4: m_voltage = ASMStaticMethods.Power(leftInput, rightInput);
                    break;
                case 5: m_voltage = ASMStaticMethods.Remain(leftInput, rightInput);
                    break;
                case 6: m_voltage = ASMStaticMethods.Logarithm(leftInput, rightInput);
                    break;
                case 7: Matrix.MultiplyRestricted(ref leftInput, ref rightInput, out m_voltage);//矩阵乘法器
                    break;
                case 8: m_voltage = leftInput * Matrix.Invert(rightInput);//矩阵除法器
                    break;
                case 9: m_voltage = ASMStaticMethods.Min(leftInput, rightInput);
                    break;
                case 10: m_voltage = ASMStaticMethods.Max(leftInput, rightInput);
                    break;
            }

            return m_voltage != voltage;
        }
    }
}