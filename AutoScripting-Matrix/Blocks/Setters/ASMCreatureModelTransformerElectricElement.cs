using Engine;

namespace Game {
    public class ASMCreatureModelTransformerElectricElement : ASMRotateableElectricElement {

        public ComponentBody m_collideBody;

        public ASMCreatureModelTransformerElectricElement(SubsystemASMElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace) { }

        public override bool Simulate() {
            Matrix modelIndex = Matrix.Zero;
            Matrix modelTransform = Matrix.Zero;

            foreach (ASMElectricConnection connection in Connections) {
                if (connection.ConnectorType != ASMElectricConnectorType.Output
                    && connection.NeighborConnectorType != 0) {
                    ASMElectricConnectorDirection? connectorDirection = SubsystemASMElectricity.GetConnectorDirection(CellFaces[0].Face, Rotation, connection.ConnectorFace);
                    if (connectorDirection.HasValue) {
                        if (connectorDirection == ASMElectricConnectorDirection.Left) modelIndex = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                        else if (connectorDirection == ASMElectricConnectorDirection.Right) modelTransform = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                    }
                }
            }

            if (m_collideBody != null) {
                ComponentModel componentCreatureModel = m_collideBody.Entity.FindComponent<ComponentModel>(true);
                if (componentCreatureModel != null)
                {
                    componentCreatureModel.SetBoneTransform((int)modelIndex.ToFloat(), modelTransform);
                    SubsystemElectricity.SubsystemAudio.PlaySound(
                        "Audio/Throw",
                        1f,
                        0f,
                        new Vector3(CellFaces[0].Point),
                        2.5f,
                        autoDelay: true
                    );
                }
            }

            m_collideBody = null;
            return false;
        }

        public override void OnCollide(CellFace cellFace, float velocity, ComponentBody componentBody) {
            m_collideBody = componentBody;
            SubsystemElectricity.QueueElectricElementForSimulation(this, SubsystemElectricity.CircuitStep + 1);
            componentBody.ApplyImpulse(new Vector3(0f, -2E-05f, 0f));
        }
    }
}