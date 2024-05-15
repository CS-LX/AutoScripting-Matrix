using Engine;

namespace Game {
    public class ASMPlayerCameraSetterElectricElement : ASMRotateableElectricElement {
        public int m_playerIndex = 3;
        public SubsystemASMPlayerCameraSetterBehavior m_subsystemAsmPlayerCameraSetterBehavior;
        public ASMComplexPerspectiveCamera m_camera;

        public ASMPlayerCameraSetterElectricElement(SubsystemASMElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace) {
            m_subsystemAsmPlayerCameraSetterBehavior = SubsystemElectricity.Project.FindSubsystem<SubsystemASMPlayerCameraSetterBehavior>(true);
        }

        public override void OnAdded() {
            base.OnAdded();
            SubsystemPlayers subsystemPlayers = SubsystemElectricity.Project.FindSubsystem<SubsystemPlayers>(true);
            string name = m_subsystemAsmPlayerCameraSetterBehavior.GetBlockData(CellFaces[0].Point)?.Name ?? ASMPlayerCameraSetterData.DefaultName;
            m_camera = new ASMComplexPerspectiveCamera(subsystemPlayers.m_componentPlayers[3 - m_playerIndex].GameWidget, Matrix.Zero);
            ASMPlayerCameraSetterManager.m_cameras[m_playerIndex] = (name, CellFaces[0].Point, m_camera);
        }

        public override void OnRemoved() {
            base.OnRemoved();
            SubsystemPlayers subsystemPlayers = SubsystemElectricity.Project.FindSubsystem<SubsystemPlayers>(true);
            ComponentPlayer componentPlayer = subsystemPlayers.m_componentPlayers[3 - m_playerIndex];
            if (componentPlayer.GameWidget.ActiveCamera == ASMPlayerCameraSetterManager.m_cameras[m_playerIndex].Item3) {
                ModsManager.HookAction("OnCameraChange", modLoader =>
                {
                    modLoader.OnCameraChange(componentPlayer, componentPlayer.ComponentGui);
                    return false;
                });
            }
            ASMPlayerCameraSetterManager.m_cameras[m_playerIndex] = (string.Empty, Point3.Zero, null);
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

            ASMPlayerCameraSetterManager.m_cameras[m_playerIndex].Item3.SetViewMatrix(view);
            ASMPlayerCameraSetterManager.m_cameras[m_playerIndex].Item3.m_projectionMatrix = projection;

            string currentName = m_subsystemAsmPlayerCameraSetterBehavior.GetBlockData(CellFaces[0].Point)?.Name ?? ASMPlayerCameraSetterData.DefaultName;
            ASMPlayerCameraSetterManager.m_cameras[m_playerIndex].Item1 = currentName;

            SubsystemElectricity.QueueElectricElementForSimulation(this, SubsystemElectricity.CircuitStep + 1);//收尾用模拟

            return false;
        }
    }
}