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

        public override void OnItemHarvested(int x, int y, int z, int blockValue, ref BlockDropValue dropValue, ref int newBlockValue) {
            ASMDelayData blockData = this.GetBlockData(new Point3(x, y, z));
            if ((object)blockData == null) return;
            int freeItemId = this.FindFreeItemId();
            this.m_itemsData.Add(freeItemId, (ASMDelayData)blockData.Copy());
            dropValue.Value = Terrain.ReplaceData(dropValue.Value, (freeItemId << 7) | (Terrain.ExtractData(blockValue) & 0b1100000));
        }

        public override void OnItemPlaced(int x, int y, int z, ref BlockPlacementData placementData, int itemValue) {
            ASMDelayData itemData = this.GetItemData(Terrain.ExtractData(itemValue) >> 7);
            if ((object)itemData == null) return;
            this.m_blocksData[new Point3(x, y, z)] = (ASMDelayData)itemData.Copy();
        }
    }
}