using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game {
    public class ComponentASMDispenser : ComponentInventoryBase {
        public SubsystemTerrain m_subsystemTerrain;

        public SubsystemAudio m_subsystemAudio;

        public SubsystemProjectiles m_subsystemProjectiles;

        public ComponentBlockEntity m_componentBlockEntity;

        public virtual void Dispense(Matrix velocityTransform) {
            Point3 coordinates = m_componentBlockEntity.Coordinates;
            int data = Terrain.ExtractData(m_subsystemTerrain.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z));
            int direction = ASMDispenserBlock.GetDirection(data);
            int num = 0;
            int slotValue;
            while (true) {
                if (num < SlotsCount) {
                    slotValue = GetSlotValue(num);
                    int slotCount = GetSlotCount(num);
                    if (slotValue != 0
                        && slotCount > 0) {
                        break;
                    }
                    num++;
                    continue;
                }
                return;
            }
            int num2 = RemoveSlotItems(num, 1);
            for (int i = 0; i < num2; i++) {
                DispenseItem(coordinates, direction, slotValue, velocityTransform);
            }
        }

        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap) {
            base.Load(valuesDictionary, idToEntityMap);
            m_subsystemTerrain = Project.FindSubsystem<SubsystemTerrain>(throwOnError: true);
            m_subsystemAudio = Project.FindSubsystem<SubsystemAudio>(throwOnError: true);
            m_subsystemProjectiles = Project.FindSubsystem<SubsystemProjectiles>(throwOnError: true);
            m_componentBlockEntity = Entity.FindComponent<ComponentBlockEntity>(throwOnError: true);
        }

        public virtual void DispenseItem(Point3 point, int face, int value, Matrix velocityTransform) {
            Vector3 baseVelocity = CellFace.FaceToVector3(face);
            Vector3 position = new Vector3(point.X + 0.5f, point.Y + 0.5f, point.Z + 0.5f) + (0.6f * baseVelocity);
            if (m_subsystemProjectiles.FireProjectile(
                    value,
                    position,
                    Vector3.Transform(baseVelocity, velocityTransform),
                    Vector3.Zero,
                    null
                )
                != null) {
                m_subsystemAudio.PlaySound(
                    "Audio/DispenserShoot",
                    1f,
                    0f,
                    new Vector3(position.X, position.Y, position.Z),
                    4f,
                    autoDelay: true
                );
            }
        }
    }
}