using Engine;

namespace Game {
    public class SubsystemASMTruthTableBlockBehavior : SubsystemEditableItemBehavior<ASMTruthTableData> {

        public SubsystemASMTruthTableBlockBehavior() : base(ASMTruthTableBlock.Index) { }

        public override int[] HandledBlocks => [ASMTruthTableBlock.Index];

        public override bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer componentPlayer)
        {
            if (componentPlayer.DragHostWidget.IsDragInProgress) return false;
            int value = inventory.GetSlotValue(slotIndex);
            int count = inventory.GetSlotCount(slotIndex);
            int id = Terrain.ExtractData(value);
            ASMTruthTableData asmTruthTableData = GetItemData(id);
            asmTruthTableData = asmTruthTableData != null ? (ASMTruthTableData)asmTruthTableData.Copy() : new ASMTruthTableData();
            DialogsManager.ShowDialog(componentPlayer.GuiWidget, new EditASMTruthTableDialog(asmTruthTableData,
                 m => {
                    asmTruthTableData.Expressions = m;
                    int data = StoreItemDataAtUniqueId(asmTruthTableData);
                    int newBlockValue = Terrain.ReplaceData(value, data);
                    inventory.RemoveSlotItems(slotIndex, count);
                    inventory.AddSlotItems(slotIndex, newBlockValue, count);
                }));
            return true;
        }

        public override bool OnEditBlock(int x, int y, int z, int value, ComponentPlayer componentPlayer)
        {
            ASMTruthTableData asmTruthTableData = GetBlockData(new Point3(x, y, z)) ?? new ASMTruthTableData();
            DialogsManager.ShowDialog(componentPlayer.GuiWidget, new EditASMTruthTableDialog(asmTruthTableData,
                m => {
                    asmTruthTableData.Expressions = m;
                    SetBlockData(new Point3(x, y, z), asmTruthTableData);
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