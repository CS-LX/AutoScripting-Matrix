using Engine;

namespace Game {
    public class ASMMotionDetectorElectricElement : ASMMountedElectricElement {
        public SubsystemBodies m_subsystemBodies;

        public SubsystemMovingBlocks m_subsystemMovingBlocks;

        public SubsystemProjectiles m_subsystemProjectiles;

        public SubsystemPickables m_subsystemPickables;

        public Matrix m_voltage;

        public Vector3 m_center;

        public Vector3 m_direction;

        public Vector2 m_corner1;

        public Vector2 m_corner2;

        public DynamicArray<ComponentBody> m_bodies = new DynamicArray<ComponentBody>();

        public ASMMotionDetectorElectricElement(SubsystemASMElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace) {
            m_subsystemBodies = subsystemElectricity.Project.FindSubsystem<SubsystemBodies>(throwOnError: true);
            m_subsystemMovingBlocks = subsystemElectricity.Project.FindSubsystem<SubsystemMovingBlocks>(throwOnError: true);
            m_subsystemProjectiles = subsystemElectricity.Project.FindSubsystem<SubsystemProjectiles>(throwOnError: true);
            m_subsystemPickables = subsystemElectricity.Project.FindSubsystem<SubsystemPickables>(throwOnError: true);
            m_center = new Vector3(cellFace.X, cellFace.Y, cellFace.Z) + new Vector3(0.5f) - (0.25f * m_direction);
            m_direction = CellFace.FaceToVector3(cellFace.Face);
            Vector3 vector = Vector3.One - new Vector3(MathUtils.Abs(m_direction.X), MathUtils.Abs(m_direction.Y), MathUtils.Abs(m_direction.Z));
            Vector3 vector2 = m_center - (8f * vector);
            Vector3 vector3 = m_center + (8f * (vector + m_direction));
            m_corner1 = new Vector2(vector2.X, vector2.Z);
            m_corner2 = new Vector2(vector3.X, vector3.Z);
        }

        public override Matrix GetOutputVoltage(int face) {
            return m_voltage;
        }

        public override bool Simulate() {
            Matrix voltage = m_voltage;
            m_voltage = CalculateMotionVoltage();
            if (m_voltage != Matrix.Zero
                && voltage == Matrix.Zero) {
                base.SubsystemElectricity.SubsystemAudio.PlaySound(
                    "Audio/MotionDetectorClick",
                    1f,
                    0f,
                    m_center,
                    1f,
                    autoDelay: true
                );
            }
            base.SubsystemElectricity.QueueElectricElementForSimulation(this, SubsystemElectricity.CircuitStep + 1);
            return m_voltage != voltage;
        }

        public Matrix CalculateMotionVoltage() {
            Matrix num = Matrix.Zero;
            m_bodies.Clear();
            m_subsystemBodies.FindBodiesInArea(m_corner1, m_corner2, m_bodies);
            for (int i = 0; i < m_bodies.Count; i++) {
                ComponentBody componentBody = m_bodies.Array[i];
                if (!(componentBody.Velocity.LengthSquared() < 0.0625f)) {
                    num = componentBody.Matrix;
                }
            }
            foreach (IMovingBlockSet movingBlockSet in m_subsystemMovingBlocks.MovingBlockSets) {
                if (movingBlockSet.CurrentVelocity.LengthSquared() < 0.0625f
                    || BoundingBox.Distance(movingBlockSet.BoundingBox(extendToFillCells: false), m_center) > 8f) {
                    continue;
                }
                foreach (MovingBlock block in movingBlockSet.Blocks) {
                    num = Matrix.CreateTranslation(movingBlockSet.Position);
                }
            }
            foreach (Projectile projectile in m_subsystemProjectiles.Projectiles) {
                if (!(projectile.Velocity.LengthSquared() < 0.0625f)) {
                    num = Matrix.CreateFromAxisAngle(Vector3.Normalize(projectile.Rotation), projectile.Rotation.Length());
                    num.Translation = projectile.Position;
                }
            }
            foreach (Pickable pickable in m_subsystemPickables.Pickables) {
                if (!(pickable.Velocity.LengthSquared() < 0.0625f)) {
                    num = Matrix.CreateTranslation(pickable.Position);
                }
            }
            return num;
        }
    }
}