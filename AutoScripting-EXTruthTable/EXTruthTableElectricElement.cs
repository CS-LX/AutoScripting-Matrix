using Engine;
using Game;

namespace AutoScripting_EXTruthTable {
    public class EXTruthTableElectricElement : RotateableElectricElement {
        public SubsystemEXTruthTableBlockBehavior m_subsystemTruthTableCircuitBlockBehavior;

        public float m_voltage;

        public EXTruthTableElectricElement(SubsystemElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace) {
            m_subsystemTruthTableCircuitBlockBehavior = subsystemElectricity.Project.FindSubsystem<SubsystemEXTruthTableBlockBehavior>(throwOnError: true);
        }

        public override float GetOutputVoltage(int face) {
            return m_voltage;
        }

        public override bool Simulate() {
            float voltage = m_voltage;
            int num = 0;
            int rotation = Rotation;
            float i1 = 0;
            float i2 = 0;
            float i3 = 0;
            float i4 = 0;
            foreach (ElectricConnection connection in Connections) {
                if (connection.ConnectorType != ElectricConnectorType.Output
                    && connection.NeighborConnectorType != 0) {
                    ElectricConnectorDirection? connectorDirection = SubsystemElectricity.GetConnectorDirection(CellFaces[0].Face, rotation, connection.ConnectorFace);
                    if (connectorDirection.HasValue) {
                        if (connectorDirection == ElectricConnectorDirection.Top) {
                            i1 = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace) * 15f;
                        }
                        else if (connectorDirection == ElectricConnectorDirection.Right) {
                            i2 = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace) * 15f;
                        }
                        else if (connectorDirection == ElectricConnectorDirection.Bottom) {
                            i3 = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace) * 15f;
                        }
                        else if (connectorDirection == ElectricConnectorDirection.Left) {
                            i4 = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace) * 15f;
                        }
                    }
                }
            }
            EXTruthTableData blockData = m_subsystemTruthTableCircuitBlockBehavior.GetBlockData(CellFaces[0].Point);
            if (blockData != null) {
                float result = 0;
                bool isSucceed = blockData.Calc(
                    i1,
                    i2,
                    i3,
                    i4,
                    out result,
                    out string calcState
                );
                if (!isSucceed) {
                    SubsystemElectricity.Project.FindSubsystem<SubsystemPlayers>(true).FindNearestPlayer(new Vector3(CellFaces[0].Point)).ComponentGui.DisplaySmallMessage($"[表达式真值表]运行错误：{calcState}", Color.White, true, false);
                }
                result = Math.Clamp(result, 0, 15);
                m_voltage = Convert.ToInt32(result) / 15f;
            }
            return m_voltage != voltage;
        }
    }
}