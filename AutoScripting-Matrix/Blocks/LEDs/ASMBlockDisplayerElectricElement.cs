using Engine;
using Engine.Graphics;

namespace Game {
    public class ASMBlockDisplayerElectricElement : ASMRotateableElectricElement, IASMGizmo {
        public SubsystemASMGlow m_subsystemGlow;

        public Matrix m_leftInput = Matrix.Zero;

        public Matrix m_rightInput = Matrix.Zero;

        public ASMGlowBlock m_glowBlock;

        public Matrix m_transform;

        public ASMBlockDisplayerElectricElement(SubsystemASMElectricity subsystemElectricity, CellFace cellFace, int value) : base(subsystemElectricity, cellFace) {
            m_subsystemGlow = subsystemElectricity.Project.FindSubsystem<SubsystemASMGlow>(true);
        }

        public override void OnAdded() => m_glowBlock = m_subsystemGlow.AddGlowBlock();

        public override void OnRemoved() => m_subsystemGlow.RemoveGlowBlock(m_glowBlock);

        public override bool Simulate() {
            Matrix leftInput = m_leftInput;
            Matrix rightInput = m_rightInput;
            m_leftInput = Matrix.Zero;
            m_rightInput = Matrix.Zero;
            int rotation = Rotation;
            foreach (ASMElectricConnection connection in Connections) {
                if (connection.ConnectorType != ASMElectricConnectorType.Output
                    && connection.NeighborConnectorType != 0) {
                    ASMElectricConnectorDirection? connectorDirection = SubsystemASMElectricity.GetConnectorDirection(CellFaces[0].Face, rotation, connection.ConnectorFace);
                    if (connectorDirection.HasValue) {
                        if (connectorDirection == ASMElectricConnectorDirection.Right) {
                            m_rightInput = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                        }
                        else if (connectorDirection == ASMElectricConnectorDirection.Left) {
                            m_leftInput = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                        }
                    }
                }
            }

            if (rightInput != m_rightInput
                || leftInput != m_leftInput) {
                CellFace cellFace = CellFaces[0];
                Vector3 position = new Vector3(cellFace.Point.X + 0.5f, cellFace.Point.Y + 0.5f, cellFace.Point.Z + 0.5f);

                Matrix baseMatrix = Matrix.CreateTranslation(position);

                int index = (int)m_leftInput.ToFloat();
                bool affectable = m_leftInput.M12 > 0 ? true : false;
                bool useRelativeCoordinates = m_leftInput.M13 > 0 ? true : false;

                //变换
                m_transform = useRelativeCoordinates ? m_rightInput * baseMatrix : m_rightInput;

                m_glowBlock.index = index;
                m_glowBlock.transform = m_transform;
                m_glowBlock.environmentallySusceptible = affectable;
            }
            return false;
        }

        public void GizmosDraw(FlatBatch3D flatBatch) {
        }

        public void TopMostGizmosDraw(FlatBatch3D flatBatch) {
            flatBatch.QueueCoordinate(m_transform, 0.5f);
        }
    }
}