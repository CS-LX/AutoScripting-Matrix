using System.Globalization;
using System.Xml.Linq;
using Engine;

namespace Game {
    public class EditASMatrixDialog : ASMDialog {

		public ButtonWidget cancelButtonWidget;

		public ButtonWidget okButtonWidget;

		public GridPanelWidget inputMatrixGrid;

		public TextBoxWidget[] inputValues = new TextBoxWidget[16];

		public Action<Matrix> callBack = _ => { };

		public Matrix dataMatrix;

		public EditASMatrixDialog(Matrix dataMatrix, Action<Matrix> callBack) : base("Dialogs/EditASMatrixDialog") {
			this.callBack = callBack;
			this.dataMatrix = dataMatrix;
			cancelButtonWidget = Children.Find<ButtonWidget>("CancelButton");
			okButtonWidget = Children.Find<ButtonWidget>("OKButton");
			inputMatrixGrid = Children.Find<GridPanelWidget>("InputMatrix");

			for (int x = 0; x < 4; x++) {
				for (int y = 0; y < 4; y++) {
					int index = x * 4 + y;
					CanvasWidget canvasWidget = new() {Size = new Vector2(60, 40), HorizontalAlignment = WidgetAlignment.Center, VerticalAlignment = WidgetAlignment.Center, Margin = new Vector2(4, 4)};
					RectangleWidget rectangleWidget = new() {Size=new Vector2(float.MaxValue, 40), FillColor = new Color(0,0,0,0), OutlineColor = new Color(64, 64, 64, 64)};
					TextBoxWidget textBoxWidget = new(){Text = dataMatrix.GetElement(index).ToString(CultureInfo.InvariantCulture), Color = new Color(255, 255, 255), Margin = new Vector2(4, 0), VerticalAlignment=WidgetAlignment.Center, HorizontalAlignment = WidgetAlignment.Stretch};
					canvasWidget.Children.Add(rectangleWidget);
					canvasWidget.Children.Add(textBoxWidget);
					inputMatrixGrid.Children.Add(canvasWidget);
					inputMatrixGrid.SetWidgetCell(canvasWidget, new Point2(y, x));
					inputValues[index] = textBoxWidget;
				}
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

		public bool InputToMatrix(out Matrix m) {
			bool success = true;
			Matrix result = Matrix.Zero;
			for (int i = 0; i < 16; i++) {
				if (float.TryParse(inputValues[i].Text, out float elementValue)) {
					ASMStaticMethods.SetElement(ref result, i, elementValue);
				}
				else {
					m = Matrix.Zero;
					return false;
				}
			}
			m = result;
			return success;
		}

		public void Dismiss(bool canInvoke)
		{
			if (canInvoke) {
				if (InputToMatrix(out Matrix newData)) {
					if (newData != dataMatrix) {
						dataMatrix = newData;
						callBack.Invoke(dataMatrix);
					}
					DialogsManager.HideDialog(this);
				}
				else {
					DialogsManager.ShowDialog(this, new MessageDialog("错误", "矩阵元素值必须为浮点数", null, "确定", null));
				}
			}
			else {
				DialogsManager.HideDialog(this);
			}
		}
    }
}