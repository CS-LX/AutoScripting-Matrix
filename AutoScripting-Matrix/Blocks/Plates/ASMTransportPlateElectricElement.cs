using Engine;

namespace Game {
    public class ASMTransportPlateElectricElement : ASMRotateableElectricElement {

        public ComponentBody m_collideBody;
        public bool m_clockAllowed;

        public ASMTransportPlateElectricElement(SubsystemASMElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace) { }

        public override bool Simulate() {
            Matrix transformMatrix = Matrix.Identity;
            bool hasClockConnection = false;
            bool needClockOutput = false;

            foreach (ASMElectricConnection connection in Connections) {
                if (connection.ConnectorType != ASMElectricConnectorType.Output
                    && connection.NeighborConnectorType != 0) {
                    ASMElectricConnectorDirection? connectorDirection = SubsystemASMElectricity.GetConnectorDirection(CellFaces[0].Face, Rotation, connection.ConnectorFace);
                    if (connectorDirection.HasValue) {
                        if (connectorDirection == ASMElectricConnectorDirection.Bottom) {//时钟端
                            if (connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace).ToFloat() > 0) {
                                if (m_clockAllowed) {
                                    needClockOutput = true;
                                    m_clockAllowed = false;
                                }
                            }
                            else {
                                m_clockAllowed = true;
                            }
                            hasClockConnection = true;
                        }
                        else transformMatrix = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                    }
                }
            }

            if (hasClockConnection) {
                if (needClockOutput) Transport(transformMatrix);
            }
            else {
                Transport(transformMatrix);
            }
            m_collideBody = null;
            return false;
        }

        public override void OnCollide(CellFace cellFace, float velocity, ComponentBody componentBody) {
            m_collideBody = componentBody;
            SubsystemElectricity.QueueElectricElementForSimulation(this, SubsystemElectricity.CircuitStep + 1);
        }

        private void Transport(Matrix transform) {
            if (m_collideBody != null) {
                transform.Decompose(out Vector3 _, out Quaternion rotation, out Vector3 position);
                m_collideBody.Position = position;
                m_collideBody.Rotation = rotation;
                SubsystemElectricity.SubsystemAudio.PlaySound("Audio/Throw", 1f, 0f, m_collideBody.Position, 2.5f, autoDelay: true);
            }
        }
    }
}