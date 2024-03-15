namespace Game {
    public class SubsystemASMWireThroughBlockBehavior : SubsystemBlockBehavior {
        public override int[] HandledBlocks => [ASMWireThroughBlock.Index];

        public override bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer componentPlayer) {
            if (componentPlayer.DragHostWidget.IsDragInProgress) return false;
            int value = inventory.GetSlotValue(slotIndex);
            int count = inventory.GetSlotCount(slotIndex);
            int data = Terrain.ExtractData(value);
            int type = ASMWireThroughBlock.GetType(data);
            DialogsManager.ShowDialog(componentPlayer.GuiWidget, new EditASMWireThroughBlockDialog(type,
                (newType) => {
                    int newValue = Terrain.MakeBlockValue(ASMWireThroughBlock.Index, 0, ASMWireThroughBlock.SetWiredFaceAndType(0, 0, newType));
                    if (newValue != value)
                    {
                        inventory.RemoveSlotItems(slotIndex, count);
                        inventory.AddSlotItems(slotIndex, newValue, count);
                    }
                }));
            return true;
        }

        public override bool OnEditBlock(int x, int y, int z, int value, ComponentPlayer componentPlayer) {
            int data = Terrain.ExtractData(value);
            int type = ASMWireThroughBlock.GetType(data);
            DialogsManager.ShowDialog(componentPlayer.GuiWidget, new EditASMWireThroughBlockDialog(type,
                (newType) => {
                    int wiredFace = ASMWireThroughBlock.GetWiredFace(data);
                    if (newType != type) {
                        int newValue = Terrain.MakeBlockValue(ASMWireThroughBlock.Index, 0, ASMWireThroughBlock.SetWiredFaceAndType(0, wiredFace, newType));
                        SubsystemTerrain.ChangeCell(x, y, z, newValue);
                    }
                }));
            return true;
        }
    }
}