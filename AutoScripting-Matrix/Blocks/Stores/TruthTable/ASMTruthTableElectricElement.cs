using Engine;

namespace Game {
    public class ASMTruthTableElectricElement : ASMRotateableElectricElement {

        public SubsystemASMTruthTableBlockBehavior m_subsystemTruthTableBehavior;

        public Matrix m_voltage;

        public ASMTruthTableElectricElement(SubsystemASMElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace) {
            m_subsystemTruthTableBehavior = subsystemElectricity.Project.FindSubsystem<SubsystemASMTruthTableBlockBehavior>(true);
        }

        public override Matrix GetOutputVoltage(int face) => m_voltage;

        public override bool Simulate() {
            Matrix voltage = m_voltage;
            ASMTruthTableData data = m_subsystemTruthTableBehavior.GetBlockData(CellFaces[0].Point);
            if (data == null) return false;
            Matrix i1 = Matrix.Zero;
            Matrix i2 = Matrix.Zero;
            Matrix i3 = Matrix.Zero;
            Matrix i4 = Matrix.Zero;
            foreach (ASMElectricConnection connection in Connections) {
                if (connection.ConnectorType != ASMElectricConnectorType.Output
                    && connection.NeighborConnectorType != 0) {
                    ASMElectricConnectorDirection? connectorDirection = SubsystemASMElectricity.GetConnectorDirection(CellFaces[0].Face, Rotation, connection.ConnectorFace);
                    if (connectorDirection.HasValue) {
                        if (connectorDirection == ASMElectricConnectorDirection.Top) {
                            i1 = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                        }
                        else if (connectorDirection == ASMElectricConnectorDirection.Right) {
                            i2 = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                        }
                        else if (connectorDirection == ASMElectricConnectorDirection.Bottom) {
                            i3 = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                        }
                        else if (connectorDirection == ASMElectricConnectorDirection.Left) {
                            i4 = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                        }
                    }
                }
            }
            try {
                data.Calculate(
                    i1,
                    i2,
                    i3,
                    i4,
                    out m_voltage
                );
            }
            catch (Exception e) {
                data.LastOutputStatus = e.Message;
            }
            return m_voltage != voltage;
        }
    }
}