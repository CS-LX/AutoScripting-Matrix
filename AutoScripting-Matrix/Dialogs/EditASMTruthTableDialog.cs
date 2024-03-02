using System.Globalization;
using System.Xml.Linq;
using Engine;

namespace Game {
    public class EditASMTruthTableDialog : Dialog {

		public ButtonWidget cancelButtonWidget;

		public ButtonWidget okButtonWidget;

		public GridPanelWidget inputMatrixGrid;

		public TextBoxWidget[] inputValues = new TextBoxWidget[16];

		public Action<string[]> callBack = _ => { };

		public string[] m_data;

		public EditASMTruthTableDialog(string[] m_data, Action<string[]> callBack) {
			this.callBack = callBack;
			this.m_data = m_data;
			XElement node = ContentManager.Get<XElement>("Dialogs/EditASMTruthTableDialog");
			LoadContents(this, node);
			cancelButtonWidget = Children.Find<ButtonWidget>("CancelButton");
			okButtonWidget = Children.Find<ButtonWidget>("OKButton");
			inputMatrixGrid = Children.Find<GridPanelWidget>("InputMatrix");

			for (int x = 0; x < 16; x++) {
					CanvasWidget canvasWidget = new() {Size = new Vector2(300, 32), HorizontalAlignment = WidgetAlignment.Center, VerticalAlignment = WidgetAlignment.Center, Margin = new Vector2(4, 4)};
					RectangleWidget rectangleWidget = new() {Size=new Vector2(float.MaxValue, 32), FillColor = new Color(0,0,0,0), OutlineColor = new Color(64, 64, 64, 64)};
					TextBoxWidget textBoxWidget = new(){Text = this.m_data[x].ToString(CultureInfo.InvariantCulture), Color = new Color(255, 255, 255), Margin = new Vector2(4, 0), VerticalAlignment = WidgetAlignment.Center, HorizontalAlignment = WidgetAlignment.Stretch};
					canvasWidget.Children.Add(rectangleWidget);
					canvasWidget.Children.Add(textBoxWidget);
					inputMatrixGrid.Children.Add(canvasWidget);
					inputMatrixGrid.SetWidgetCell(canvasWidget, new Point2(0, x));
					inputValues[x] = textBoxWidget;
			}
		}

		public override void Update()
		{
			if (Input.Cancel || cancelButtonWidget.IsClicked) {
				Dismiss(false);
			}
			else if (Input.Ok || okButtonWidget.IsClicked)
			{
				Dismiss(true);
			}
		}

		public void Dismiss(bool canInvoke)
		{
			if (canInvoke) {
				string[] newExpressions = GetExpressions();
				if (ASMTruthTableData.Check(newExpressions, out int index)) {
					if (!m_data.SequenceEqual(newExpressions)) callBack.Invoke(newExpressions);
					DialogsManager.HideDialog(this);
				}
				else {
					DialogsManager.ShowDialog(this, new MessageDialog("错误", $"表达式{index}存在非法字符", "确定", null, null));
				}
			}
			else {
				DialogsManager.HideDialog(this);
			}
		}

		public string[] GetExpressions() {
			string[] res = new string[16];
			for (int i = 0; i < 16; i++) {
				res[i] = inputValues[i].Text;
			}
			return res;
		}
    }
}