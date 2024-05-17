using Engine;

namespace Game {
    public class ASMTranslatePlateElectricElement : ASMRotateableElectricElement {

        public ComponentBody m_collideBody;

        public WorldItem m_worldItem;

        public int m_lastPressFrame;

        public bool m_pressed;

        public Vector3 m_prepressVelocity_Body;

        public Vector3 m_prepressVelocity_Item;

        public ASMTranslatePlateElectricElement(SubsystemASMElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace) { }

        public override bool Simulate() {
            if (Time.FrameIndex - m_lastPressFrame > 2) {//玩家退出，收尾用
                m_pressed = false;
                return false;
            }

            Matrix normalTransform = Matrix.Identity;
            Matrix velocityTransform = Matrix.Identity;

            foreach (ASMElectricConnection connection in Connections) {
                if (connection.ConnectorType != ASMElectricConnectorType.Output
                    && connection.NeighborConnectorType != 0) {
                    ASMElectricConnectorDirection? connectorDirection = SubsystemASMElectricity.GetConnectorDirection(CellFaces[0].Face, Rotation, connection.ConnectorFace);
                    if (connectorDirection.HasValue) {
                        if (connectorDirection == ASMElectricConnectorDirection.Left) {
                            normalTransform = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                        }
                        else if (connectorDirection == ASMElectricConnectorDirection.Right) {
                            velocityTransform = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                        }
                    }
                }
            }

            if (m_collideBody != null && !m_pressed) {
                m_pressed = true;
                Matrix bodyMatrix = m_collideBody.Matrix;
                Matrix transformedMatrix = bodyMatrix * normalTransform;
                transformedMatrix.Decompose(out _, out Quaternion quaternion, out Vector3 position);
                m_collideBody.Position = position;
                m_collideBody.Rotation = quaternion;
                m_collideBody.m_cachedMatrixValid = true;
                m_collideBody.Velocity = Vector3.Transform(m_prepressVelocity_Body, velocityTransform);
                SubsystemElectricity.SubsystemAudio.PlaySound(
                    "Audio/Throw",
                    ASMSettingsManager.Get<float>("TPPlateVolume"),
                    0f,
                    position,
                    2.5f,
                    autoDelay: true
                );
            }

            if (m_worldItem != null && !m_pressed) {
                m_pressed = true;
                Matrix itemMatrix = Matrix.CreateTranslation(m_worldItem.Position);
                Matrix itemTransformed = itemMatrix * normalTransform;
                itemTransformed.Decompose(out _, out _, out Vector3 position);
                m_worldItem.Position = position;
                m_worldItem.Velocity = Vector3.Transform(m_prepressVelocity_Item, velocityTransform);
                SubsystemElectricity.SubsystemAudio.PlaySound(
                    "Audio/Throw",
                    ASMSettingsManager.Get<float>("TPPlateVolume"),
                    0f,
                    position,
                    2.5f,
                    autoDelay: true
                );
            }

            m_collideBody = null;
            m_worldItem = null;
            SubsystemElectricity.QueueElectricElementForSimulation(this, SubsystemElectricity.CircuitStep + 10);//收尾用模拟

            return false;
        }

        public override void OnCollide(CellFace cellFace, float velocity, ComponentBody componentBody) {
            m_prepressVelocity_Body = componentBody.Velocity;
            m_collideBody = componentBody;
            m_lastPressFrame = Time.FrameIndex;
            SubsystemElectricity.QueueElectricElementForSimulation(this, SubsystemElectricity.CircuitStep + 1);
            componentBody.ApplyImpulse(new Vector3(0f, -2E-05f, 0f));
            ComponentCreature componentCreature = componentBody.Entity.FindComponent<ComponentCreature>(true);
        }

        public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem) {
            m_prepressVelocity_Item = worldItem.Velocity;
            m_worldItem = worldItem;
            m_lastPressFrame = Time.FrameIndex;
            SubsystemElectricity.QueueElectricElementForSimulation(this, SubsystemElectricity.CircuitStep + 1);
        }
    }
}