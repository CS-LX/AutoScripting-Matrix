using Engine;

namespace Game {
    public class ASMPlayerCameraSetterElectricElement : ASMRotateableElectricElement {
        public int m_playerIndex = 3;
        public ASMPlayerCameraSetterElectricElement(SubsystemASMElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace) { }

        public override void OnAdded() {
            base.OnAdded();
            SubsystemPlayers subsystemPlayers = SubsystemElectricity.Project.FindSubsystem<SubsystemPlayers>(true);
            ASMPlayerCameraSetterManager.m_cameras[m_playerIndex] = new ASMComplexPerspectiveCamera(subsystemPlayers.m_componentPlayers[3 - m_playerIndex].GameWidget, Matrix.Zero);
        }

        public override void OnRemoved() {
            base.OnRemoved();
            SubsystemPlayers subsystemPlayers = SubsystemElectricity.Project.FindSubsystem<SubsystemPlayers>(true);
            ComponentPlayer componentPlayer = subsystemPlayers.m_componentPlayers[3 - m_playerIndex];
            if (componentPlayer.GameWidget.ActiveCamera == ASMPlayerCameraSetterManager.m_cameras[m_playerIndex]) {
                ModsManager.HookAction("OnCameraChange", modLoader =>
                {
                    modLoader.OnCameraChange(componentPlayer, componentPlayer.ComponentGui);
                    return false;
                });
            }
            ASMPlayerCameraSetterManager.m_cameras[m_playerIndex] = null;
        }

        public override bool Simulate() {
            Matrix view = Matrix.Zero;
            Matrix projection = Matrix.Zero;

            foreach (ASMElectricConnection connection in Connections) {
                if (connection.ConnectorType != ASMElectricConnectorType.Output
                    && connection.NeighborConnectorType != 0) {
                    ASMElectricConnectorDirection? connectorDirection = SubsystemASMElectricity.GetConnectorDirection(CellFaces[0].Face, Rotation, connection.ConnectorFace);
                    if (connectorDirection.HasValue) {
                        if (connectorDirection == ASMElectricConnectorDirection.Left) {
                            view = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                        }
                        else if (connectorDirection == ASMElectricConnectorDirection.Right) {
                            projection = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                        }
                    }
                }
            }

            ASMPlayerCameraSetterManager.m_cameras[m_playerIndex].SetViewMatrix(view);
            ASMPlayerCameraSetterManager.m_cameras[m_playerIndex].m_projectionMatrix = projection;

            SubsystemElectricity.QueueElectricElementForSimulation(this, SubsystemElectricity.CircuitStep + 1);//收尾用模拟

            return false;
        }
    }
}