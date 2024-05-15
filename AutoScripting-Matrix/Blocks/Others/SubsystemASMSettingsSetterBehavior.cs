using Engine;

namespace Game {
    public class SubsystemASMSettingsSetterBehavior : SubsystemBlockBehavior {
        public override int[] HandledBlocks => [ASMSettingsSetterBlock.Index];

        public override bool OnEditBlock(int x, int y, int z, int value, ComponentPlayer componentPlayer) {
            ScreensManager.SwitchScreen("ASMSettings");
            return true;
        }

        public override bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer componentPlayer) {
            ScreensManager.SwitchScreen("ASMSettings");
            return true;
        }

        public override bool OnUse(Ray3 ray, ComponentMiner componentMiner) {
            ScreensManager.SwitchScreen("ASMSettings");
            return true;
        }
    }
}