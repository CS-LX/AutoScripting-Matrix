using Engine;

namespace Game {
    public class SubsystemASMButtonBlockBehavior : SubsystemEditableItemBehavior<ASMatrixData> {

        public SubsystemASMButtonBlockBehavior() : base(ASMButtonBlock.Index) { }

        public override int[] HandledBlocks => [ASMButtonBlock.Index];

        public override bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer componentPlayer)
        {
            if (componentPlayer.DragHostWidget.IsDragInProgress) return false;
            int value = inventory.GetSlotValue(slotIndex);
            int count = inventory.GetSlotCount(slotIndex);
            int id = Terrain.ExtractData(value);
            ASMatrixData asMatrixData = GetItemData(id);
            asMatrixData = asMatrixData != null ? (ASMatrixData)asMatrixData.Copy() : new ASMatrixData();
            DialogsManager.ShowDialog(componentPlayer.GuiWidget, new EditASMatrixDialog(asMatrixData.Data,
                 m => {
                    asMatrixData.Data = m;
                    int id = StoreItemDataAtUniqueId(asMatrixData);
                    int newBlockValue = Terrain.ReplaceData(value, id);
                    inventory.RemoveSlotItems(slotIndex, count);
                    inventory.AddSlotItems(slotIndex, newBlockValue, count);
                }));
            return true;
        }

        public override bool OnEditBlock(int x, int y, int z, int value, ComponentPlayer componentPlayer)
        {
            ASMatrixData asMatrixData = GetBlockData(new Point3(x, y, z)) ?? new ASMatrixData();
            DialogsManager.ShowDialog(componentPlayer.GuiWidget, new EditASMatrixDialog(asMatrixData.Data,
                m => {
                    asMatrixData.Data = m;
                    SetBlockData(new Point3(x, y, z), asMatrixData);
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