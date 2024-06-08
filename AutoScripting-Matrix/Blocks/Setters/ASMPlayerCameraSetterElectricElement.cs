using Engine;

namespace Game {
    public class ASMPlayerCameraSetterElectricElement : ASMRotateableElectricElement {
        private int m_playerIndex = 3;
        public int PlayerIndex => m_playerIndex;
        public SubsystemASMPlayerCameraSetterBehavior m_subsystemAsmPlayerCameraSetterBehavior;
        public ASMComplexPerspectiveCamera m_camera;

        private Matrix m_viewMatrix = Matrix.Zero;
        public Matrix ViewMatrix => m_viewMatrix;
        private Matrix m_projectionMatrix = Matrix.Zero;
        public Matrix ProjectionMatrix => m_projectionMatrix;

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
            m_viewMatrix = Matrix.Zero;
            m_projectionMatrix = Matrix.Zero;

            foreach (ASMElectricConnection connection in Connections) {
                if (connection.ConnectorType != ASMElectricConnectorType.Output
                    && connection.NeighborConnectorType != 0) {
                    ASMElectricConnectorDirection? connectorDirection = SubsystemASMElectricity.GetConnectorDirection(CellFaces[0].Face, Rotation, connection.ConnectorFace);
                    if (connectorDirection.HasValue) {
                        if (connectorDirection == ASMElectricConnectorDirection.Left) {
                            m_viewMatrix = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                        }
                        else if (connectorDirection == ASMElectricConnectorDirection.Right) {
                            m_projectionMatrix = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                        }
                    }
                }
            }

            string currentName = m_subsystemAsmPlayerCameraSetterBehavior.GetBlockData(CellFaces[0].Point)?.Name ?? ASMPlayerCameraSetterData.DefaultName;
            ASMPlayerCameraSetterManager.m_cameras[m_playerIndex].Item1 = currentName;

            SubsystemElectricity.QueueElectricElementForSimulation(this, SubsystemElectricity.CircuitStep + 1);//收尾用模拟

            return false;
        }
    }
}