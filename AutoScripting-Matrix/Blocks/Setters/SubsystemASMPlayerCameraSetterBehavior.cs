using Engine;

namespace Game {
    public class SubsystemASMPlayerCameraSetterBehavior : SubsystemEditableItemBehavior<ASMPlayerCameraSetterData> {

        public SubsystemASMPlayerCameraSetterBehavior() : base(ASMPlayerCameraSetter.Index) { }

        public override int[] HandledBlocks => [ASMPlayerCameraSetter.Index];

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
                    asmTruthTableData.Name = m.Length > 0 ? m : ASMPlayerCameraSetterData.DefaultName;
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
                    asmTruthTableData.Name = m.Length > 0 ? m : ASMPlayerCameraSetterData.DefaultName;
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

        public static int GetFace(int value)
        {
            return (Terrain.ExtractData(value) >> 2) & 7;
        }
    }
}