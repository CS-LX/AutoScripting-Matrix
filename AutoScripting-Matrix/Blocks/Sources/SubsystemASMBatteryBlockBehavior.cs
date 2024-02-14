using Engine;

namespace Game {
    public class SubsystemASMBatteryBlockBehavior : SubsystemEditableItemBehavior<ASMBatteryData> {

        public SubsystemASMBatteryBlockBehavior() : base(ASMBatteryBlock.Index) { }

        public override int[] HandledBlocks => [ASMBatteryBlock.Index];

        public override bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer componentPlayer)
        {
            if (componentPlayer.DragHostWidget.IsDragInProgress) return false;
            int value = inventory.GetSlotValue(slotIndex);
            int count = inventory.GetSlotCount(slotIndex);
            int id = Terrain.ExtractData(value);
            ASMBatteryData asmBatteryData = GetItemData(id);
            asmBatteryData = asmBatteryData != null ? (ASMBatteryData)asmBatteryData.Copy() : new ASMBatteryData();
            DialogsManager.ShowDialog(componentPlayer.GuiWidget, new EditASMatrixDialog(asmBatteryData.Data,
                 m => {
                    asmBatteryData.Data = m;
                    int data = StoreItemDataAtUniqueId(asmBatteryData);
                    int newBlockValue = Terrain.ReplaceData(value, data);
                    inventory.RemoveSlotItems(slotIndex, count);
                    inventory.AddSlotItems(slotIndex, newBlockValue, count);
                }));
            return true;
        }

        public override bool OnEditBlock(int x, int y, int z, int value, ComponentPlayer componentPlayer)
        {
            ASMBatteryData asmBatteryData = GetBlockData(new Point3(x, y, z)) ?? new ASMBatteryData();
            DialogsManager.ShowDialog(componentPlayer.GuiWidget, new EditASMatrixDialog(asmBatteryData.Data,
                m => {
                    asmBatteryData.Data = m;
                    SetBlockData(new Point3(x, y, z), asmBatteryData);
                    SubsystemASMElectricity subsystemElectricity = SubsystemTerrain.Project.FindSubsystem<SubsystemASMElectricity>(throwOnError: true);
                    ASMElectricElement electricElement = subsystemElectricity.GetElectricElement(x, y, z, 4);
                    if (electricElement != null)
                    {
                        subsystemElectricity.QueueElectricElementForSimulation(electricElement, subsystemElectricity.CircuitStep + 1);
                    }
                }));
            return true;
        }
    }
}