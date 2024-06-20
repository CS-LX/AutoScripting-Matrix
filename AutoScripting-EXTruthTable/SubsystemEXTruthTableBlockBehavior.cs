using Engine;
using Game;

namespace AutoScripting_EXTruthTable {
    public class SubsystemEXTruthTableBlockBehavior : SubsystemEditableItemBehavior<EXTruthTableData> {
        public override int[] HandledBlocks => new int[1]
        {
            EXTruthTableBlock.Index
        };

        public SubsystemEXTruthTableBlockBehavior()
            : base(EXTruthTableBlock.Index)
        {
        }

        public override bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer componentPlayer)
        {
            if (componentPlayer.DragHostWidget.IsDragInProgress) return false;
            int value = inventory.GetSlotValue(slotIndex);
            int count = inventory.GetSlotCount(slotIndex);
            int id = Terrain.ExtractData(value);
            EXTruthTableData truthTableData = GetItemData(id);
            truthTableData = truthTableData != null ? (EXTruthTableData)truthTableData.Copy() : new EXTruthTableData();
            DialogsManager.ShowDialog(componentPlayer.GuiWidget, new TextBoxDialog("输入表达式", truthTableData.Data, int.MaxValue,
                (s) => {
                    truthTableData.Data = s;
                    int data = StoreItemDataAtUniqueId(truthTableData);
                    int value2 = Terrain.ReplaceData(value, data);
                    inventory.RemoveSlotItems(slotIndex, count);
                    inventory.AddSlotItems(slotIndex, value2, count);
                }));
            return true;
        }

        public override bool OnEditBlock(int x, int y, int z, int value, ComponentPlayer componentPlayer)
        {
            EXTruthTableData truthTableData = GetBlockData(new Point3(x, y, z)) ?? new EXTruthTableData();
            DialogsManager.ShowDialog(componentPlayer.GuiWidget, new TextBoxDialog("输入表达式", truthTableData.Data, int.MaxValue, (s) => {
                truthTableData.Data = s;
                SetBlockData(new Point3(x, y, z), truthTableData);
                int face = ((EXTruthTableBlock)BlocksManager.Blocks[EXTruthTableBlock.Index]).GetFace(value);
                SubsystemElectricity subsystemElectricity = SubsystemTerrain.Project.FindSubsystem<SubsystemElectricity>(throwOnError: true);
                ElectricElement electricElement = subsystemElectricity.GetElectricElement(x, y, z, face);
                if (electricElement != null)
                {
                    subsystemElectricity.QueueElectricElementForSimulation(electricElement, subsystemElectricity.CircuitStep + 1);
                }
            }));
            return true;
        }
    }
}