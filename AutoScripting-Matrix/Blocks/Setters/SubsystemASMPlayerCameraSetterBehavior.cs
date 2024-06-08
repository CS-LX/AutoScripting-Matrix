using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game {
    public class SubsystemASMPlayerCameraSetterBehavior : SubsystemEditableItemBehavior<ASMPlayerCameraSetterData> {

        public SubsystemASMPlayerCameraSetterBehavior() : base(ASMPlayerCameraSetter.Index) { }

        public override int[] HandledBlocks => [ASMPlayerCameraSetter.Index];

        public SubsystemBlockEntities m_subsystemBlockEntities;

        public override void Load(ValuesDictionary valuesDictionary) {
            base.Load(valuesDictionary);
            m_subsystemBlockEntities = Project.FindSubsystem<SubsystemBlockEntities>(true);
        }

        public override bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer componentPlayer)
        {
            if (componentPlayer.DragHostWidget.IsDragInProgress) return false;
            int value = inventory.GetSlotValue(slotIndex);
            int count = inventory.GetSlotCount(slotIndex);
            int id = Terrain.ExtractData(value);
            ASMPlayerCameraSetterData asmTruthTableData = GetItemData(id);
            asmTruthTableData = asmTruthTableData != null ? (ASMPlayerCameraSetterData)asmTruthTableData.Copy() : new ASMPlayerCameraSetterData();
            DialogsManager.ShowDialog(componentPlayer.GuiWidget, new ASMTextBoxDialog("编辑相机名称", asmTruthTableData.Name, int.MaxValue,
                 m => {
                    asmTruthTableData.Name = m != null && m.Length > 0 ? m : ASMPlayerCameraSetterData.DefaultName;
                    int data = StoreItemDataAtUniqueId(asmTruthTableData);
                    int newBlockValue = Terrain.ReplaceData(value, data);
                    inventory.RemoveSlotItems(slotIndex, count);
                    inventory.AddSlotItems(slotIndex, newBlockValue, count);
                }));
            return true;
        }

        public override bool OnEditBlock(int x, int y, int z, int value, ComponentPlayer componentPlayer)
        {
            ASMPlayerCameraSetterData asmTruthTableData = GetBlockData(new Point3(x, y, z)) ?? new ASMPlayerCameraSetterData();
            DialogsManager.ShowDialog(componentPlayer.GuiWidget, new ASMTextBoxDialog("编辑相机名称", asmTruthTableData.Name, int.MaxValue,
                m => {
                    asmTruthTableData.Name = m != null && m.Length > 0 ? m : ASMPlayerCameraSetterData.DefaultName;
                    SetBlockData(new Point3(x, y, z), asmTruthTableData);
                    SubsystemASMElectricity subsystemElectricity = SubsystemTerrain.Project.FindSubsystem<SubsystemASMElectricity>(throwOnError: true);
                    ASMElectricElement electricElement = subsystemElectricity.GetElectricElement(x, y, z, GetFace(value));
                    if (electricElement != null)
                    {
                        subsystemElectricity.QueueElectricElementForSimulation(electricElement, subsystemElectricity.CircuitStep + 1);
                    }
                }));
            return true;
        }

        public override void OnBlockAdded(int value, int oldValue, int x, int y, int z) {
            base.OnBlockAdded(
                value,
                oldValue,
                x,
                y,
                z
            );
            DatabaseObject databaseObject = Project.GameDatabase.Database.FindDatabaseObject("ASMPlayerCameraSetter", Project.GameDatabase.EntityTemplateType, throwIfNotFound: true);
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

        public static int GetFace(int value)
        {
            return (Terrain.ExtractData(value) >> 2) & 7;
        }
    }
}