namespace Game {
    public class SubsystemASMSettingsSetterBehavior : SubsystemBlockBehavior {
        public override int[] HandledBlocks => [ASMGeBlock.Index];

        public override bool OnEditBlock(int x, int y, int z, int value, ComponentPlayer componentPlayer){
            ScreensManager.SwitchScreen("ASMSettings");
            return true;
        }

        public override bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer componentPlayer) {
            ScreensManager.SwitchScreen("ASMSettings");
            return true;
        }
    }
}