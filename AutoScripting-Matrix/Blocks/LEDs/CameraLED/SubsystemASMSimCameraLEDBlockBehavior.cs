using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game {
    public class SubsystemASMSimCameraLEDBlockBehavior : SubsystemBlockBehavior {

        public SubsystemBlockEntities m_subsystemBlockEntities;

        public override int[] HandledBlocks => [ASMSimCameraLEDBlock.Index];

        public override void Load(ValuesDictionary valuesDictionary) {
            base.Load(valuesDictionary);
            m_subsystemBlockEntities = Project.FindSubsystem<SubsystemBlockEntities>(throwOnError: true);
        }

        public override void OnBlockAdded(int value, int oldValue, int x, int y, int z) {
            base.OnBlockAdded(
                value,
                oldValue,
                x,
                y,
                z
            );
            DatabaseObject databaseObject = Project.GameDatabase.Database.FindDatabaseObject("ASMSimCameraLED", Project.GameDatabase.EntityTemplateType, throwIfNotFound: true);
            var valuesDictionary = new ValuesDictionary();
            valuesDictionary.PopulateFromDatabaseObject(databaseObject);
            valuesDictionary.GetValue<ValuesDictionary>("BlockEntity").SetValue("Coordinates", new Point3(x, y, z));
            Entity entity = Project.CreateEntity(valuesDictionary);
            Project.AddEntity(entity);
        }

        public override void OnBlockRemoved(int value, int newValue, int x, int y, int z) {
            base.OnBlockRemoved(
                value,
                newValue,
                x,
                y,
                z
            );
            ComponentBlockEntity blockEntity = m_subsystemBlockEntities.GetBlockEntity(x, y, z);
            if (blockEntity != null)
            {
                Project.RemoveEntity(blockEntity.Entity, disposeEntity: true);
            }
        }
    }
}