using Engine;

namespace Game {
    public class SubsystemASMDelayGateBlockBehavior : SubsystemEditableItemBehavior<ASMDelayData> {

        public SubsystemASMDelayGateBlockBehavior() : base(ASMDelayGateBlock.Index) { }

        public override int[] HandledBlocks => [ASMDelayGateBlock.Index];

        public override bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer componentPlayer)
        {
            if (componentPlayer.DragHostWidget.IsDragInProgress) return false;
            int value = inventory.GetSlotValue(slotIndex);
            int count = inventory.GetSlotCount(slotIndex);
            if (ASMDelayGateBlock.GetType(Terrain.ExtractData(value)) != 1) return false;
            int id = Terrain.ExtractData(value) >> 7;
            ASMDelayData delayData = GetItemData(id);
            delayData = delayData != null ? (ASMDelayData)delayData.Copy() : new ASMDelayData();
            DialogsManager.ShowDialog(
                componentPlayer.GuiWidget,
                new EditASMAdjustableDelayGateDialog(
                    delayData,
                    m => {
                        delayData.Data = m;
                        int data = (StoreItemDataAtUniqueId(delayData) << 7) | (Terrain.ExtractData(value) & 0b1111111);
                        int newBlockValue = Terrain.ReplaceData(value, data);
                        inventory.RemoveSlotItems(slotIndex, count);
                        inventory.AddSlotItems(slotIndex, newBlockValue, count);
                    }
                )
            );
            return true;
        }

        public override bool OnEditBlock(int x, int y, int z, int value, ComponentPlayer componentPlayer)
        {
            if (ASMDelayGateBlock.GetType(Terrain.ExtractData(value)) != 1) return false;
            ASMDelayData delayData = GetBlockData(new Point3(x, y, z)) ?? new ASMDelayData();
            DialogsManager.ShowDialog(componentPlayer.GuiWidget, new EditASMAdjustableDelayGateDialog(delayData,
                m => {
                    delayData.Data = m;
                    SetBlockData(new Point3(x, y, z), delayData);
                    SubsystemASMElectricity subsystemElectricity = SubsystemTerrain.Project.FindSubsystem<SubsystemASMElectricity>(throwOnError: true);
                    ASMElectricElement electricElement = subsystemElectricity.GetElectricElement(x, y, z, GetFace(value));
                    if (electricElement != null)
                    {
                        subsystemElectricity.QueueElectricElementForSimulation(electricElement, subsystemElectricity.CircuitStep + 1);
                    }
                }));
            return true;
        }

        public static int GetFace(int value)
        {
            return (Terrain.ExtractData(value) >> 1) & 7;
        }
    }
}