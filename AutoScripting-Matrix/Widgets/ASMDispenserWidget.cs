using Engine;
using System.Xml.Linq;

namespace Game {
    public class ASMDispenserWidget : CanvasWidget {
        public SubsystemTerrain m_subsystemTerrain;

        public ComponentASMDispenser m_componentDispenser;

        public ComponentBlockEntity m_componentBlockEntity;

        public ComponentPlayer m_componentPlayer;

        public GridPanelWidget m_inventoryGrid;

        public GridPanelWidget m_dispenserGrid;

        public CheckboxWidget m_acceptsDropsBox;

        public ButtonWidget m_lastMatrixButton;

        public ASMDispenserElectricElement m_element;

        public ASMDispenserWidget(IInventory inventory, ComponentASMDispenser componentDispenser, ComponentPlayer componentPlayer, ASMDispenserElectricElement electricElement) {
            m_componentDispenser = componentDispenser;
            m_componentPlayer = componentPlayer;
            m_element = electricElement;
            m_componentBlockEntity = componentDispenser.Entity.FindComponent<ComponentBlockEntity>(throwOnError: true);
            m_subsystemTerrain = componentDispenser.Project.FindSubsystem<SubsystemTerrain>(throwOnError: true);
            XElement node = ContentManager.Get<XElement>("Widgets/ASMDispenserWidget");
            LoadContents(this, node);
            m_inventoryGrid = Children.Find<GridPanelWidget>("InventoryGrid");
            m_dispenserGrid = Children.Find<GridPanelWidget>("DispenserGrid");
            m_acceptsDropsBox = Children.Find<CheckboxWidget>("AcceptsDropsBox");
            m_lastMatrixButton = Children.Find<BevelledButtonWidget>("LastMatrixButton");
            int num = 0;
            for (int i = 0; i < m_dispenserGrid.RowsCount; i++) {
                for (int j = 0; j < m_dispenserGrid.ColumnsCount; j++) {
                    var inventorySlotWidget = new InventorySlotWidget();
                    inventorySlotWidget.AssignInventorySlot(componentDispenser, num++);
                    m_dispenserGrid.Children.Add(inventorySlotWidget);
                    m_dispenserGrid.SetWidgetCell(inventorySlotWidget, new Point2(j, i));
                }
            }
            num = 10;
            for (int k = 0; k < m_inventoryGrid.RowsCount; k++) {
                for (int l = 0; l < m_inventoryGrid.ColumnsCount; l++) {
                    var inventorySlotWidget2 = new InventorySlotWidget();
                    inventorySlotWidget2.AssignInventorySlot(inventory, num++);
                    m_inventoryGrid.Children.Add(inventorySlotWidget2);
                    m_inventoryGrid.SetWidgetCell(inventorySlotWidget2, new Point2(l, k));
                }
            }
        }

        public override void Update() {
            int value = m_subsystemTerrain.Terrain.GetCellValue(m_componentBlockEntity.Coordinates.X, m_componentBlockEntity.Coordinates.Y, m_componentBlockEntity.Coordinates.Z);
            int data = Terrain.ExtractData(value);
            if (m_acceptsDropsBox.IsClicked) {
                data = ASMDispenserBlock.SetAcceptsDrops(data, !ASMDispenserBlock.GetAcceptsDrops(data));
                value = Terrain.ReplaceData(value, data);
                m_subsystemTerrain.ChangeCell(m_componentBlockEntity.Coordinates.X, m_componentBlockEntity.Coordinates.Y, m_componentBlockEntity.Coordinates.Z, value);
            }
            if(m_lastMatrixButton.IsClicked) DialogsManager.ShowDialog(m_componentPlayer.GuiWidget, new MessageDialog("上一次输入电压", m_element?.m_voltage.ToFormatTable(2), "确定", null, null));
            m_acceptsDropsBox.IsChecked = ASMDispenserBlock.GetAcceptsDrops(data);
            if (!m_componentDispenser.IsAddedToProject) {
                ParentWidget.Children.Remove(this);
            }
        }
    }
}