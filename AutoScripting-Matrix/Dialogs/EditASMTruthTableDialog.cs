using System.Globalization;
using System.Xml.Linq;
using Engine;
using Engine.Graphics;

namespace Game {
    public class EditASMTruthTableDialog : Dialog {

		public ButtonWidget cancelButtonWidget;

		public ButtonWidget okButtonWidget;

		public GridPanelWidget inputMatrixGrid;

		public TextBoxWidget[] inputValues = new TextBoxWidget[16];

		public BitmapButtonWidget[] m_copyButtons = new BitmapButtonWidget[16];

		public BitmapButtonWidget[] m_pasteButtons = new BitmapButtonWidget[16];

		public BitmapButtonWidget[] m_applyAllButtons = new BitmapButtonWidget[16];

		public Action<string[]> callBack = _ => { };

		public ASMTruthTableData m_data;

		public LabelWidget m_lastOutputLabel;

		public EditASMTruthTableDialog(ASMTruthTableData m_data, Action<string[]> callBack) {
			this.callBack = callBack;
			this.m_data = m_data;
			XElement node = ContentManager.Get<XElement>("Dialogs/EditASMTruthTableDialog");
			LoadContents(this, node);
			cancelButtonWidget = Children.Find<ButtonWidget>("CancelButton");
			okButtonWidget = Children.Find<ButtonWidget>("OKButton");
			inputMatrixGrid = Children.Find<GridPanelWidget>("InputMatrix");
			m_lastOutputLabel = Children.Find<LabelWidget>("LastOutputText");

			for (int x = 0; x < 16; x++) {
					CanvasWidget canvasWidget = new() {Size = new Vector2(360, 32), HorizontalAlignment = WidgetAlignment.Center, VerticalAlignment = WidgetAlignment.Center, Margin = new Vector2(4, 4)};
					RectangleWidget rectangleWidget = new() {Size=new Vector2(float.MaxValue, 32), FillColor = new Color(0,0,0,0), OutlineColor = new Color(64, 64, 64, 64)};
					TextBoxWidget textBoxWidget = new(){Text = this.m_data.Expressions[x].ToString(CultureInfo.InvariantCulture), Color = new Color(255, 255, 255), Margin = new Vector2(4, 0), VerticalAlignment = WidgetAlignment.Center, HorizontalAlignment = WidgetAlignment.Stretch};
					Subtexture copyTexture = new (ContentManager.Get<Texture2D>("Textures/Icons/Copy"), Vector2.Zero, Vector2.One);
					BitmapButtonWidget copyButton = new() {
						Size = new Vector2(16),
						ClickedSubtexture = copyTexture,
						NormalSubtexture = copyTexture,
						HorizontalAlignment = WidgetAlignment.Far,
						VerticalAlignment = WidgetAlignment.Center,
						Margin = new Vector2(48, 0),

					};
					Subtexture pasteTexture = new (ContentManager.Get<Texture2D>("Textures/Icons/Paste"), Vector2.Zero, Vector2.One);
					BitmapButtonWidget pasteButton = new() {
						Size = new Vector2(16),
						ClickedSubtexture = pasteTexture,
						NormalSubtexture = pasteTexture,
						HorizontalAlignment = WidgetAlignment.Far,
						VerticalAlignment = WidgetAlignment.Center,
						Margin = new Vector2(28, 0),

					};
					Subtexture applyAllTexture = new (ContentManager.Get<Texture2D>("Textures/Icons/Link"), Vector2.Zero, Vector2.One);
					BitmapButtonWidget applyAllButton = new() {
						Size = new Vector2(16),
						ClickedSubtexture = applyAllTexture,
						NormalSubtexture = applyAllTexture,
						HorizontalAlignment = WidgetAlignment.Far,
						VerticalAlignment = WidgetAlignment.Center,
						Margin = new Vector2(8, 0),

					};
					LabelWidget indexLabel = new LabelWidget {Text = $"M{(int)(x / 4) + 1}{x % 4 + 1}", Color = Color.Gray, HorizontalAlignment = WidgetAlignment.Near, VerticalAlignment = WidgetAlignment.Center, Margin = new Vector2(-48, 0)};
					canvasWidget.Children.Add(rectangleWidget);
					canvasWidget.Children.Add(textBoxWidget);
					canvasWidget.Children.Add(copyButton);
					canvasWidget.Children.Add(pasteButton);
					canvasWidget.Children.Add(applyAllButton);
					canvasWidget.Children.Add(indexLabel);
					inputMatrixGrid.Children.Add(canvasWidget);
					inputMatrixGrid.SetWidgetCell(canvasWidget, new Point2(0, x));
					inputValues[x] = textBoxWidget;
					m_copyButtons[x] = copyButton;
					m_pasteButtons[x] = pasteButton;
					m_applyAllButtons[x] = applyAllButton;
			}
		}

		public override void Update() {
			m_lastOutputLabel.Text = "上一输出: " + m_data.LastOutputStatus;
			for (int i = 0; i < 16; i++) {
				if (m_copyButtons[i].IsClicked) {
					if (inputValues[i].Text == null || inputValues[i].Text.Length <= 0) continue;
					ClipboardManager.ClipboardString = inputValues[i].Text;
				}
				if (m_pasteButtons[i].IsClicked) {
					if (ClipboardManager.ClipboardString == null || ClipboardManager.ClipboardString.Length <= 0) continue;
					inputValues[i].Text = ClipboardManager.ClipboardString;
				}
				if (m_applyAllButtons[i].IsClicked) {
					int index = i;
					DialogsManager.ShowDialog(
						this,
						new MessageDialog(
							"",
							"复制粘贴至所有元素表达式？",
							"确定",
							"取消",
							button => {
								if (button == MessageDialogButton.Button1) {
									for (int j = 0; j < 16; j++) {
										string exp = inputValues[index].Text = inputValues[index].Text;
										inputValues[j].Text = exp;
									}
								}
							}
						)
					);
				}
				if (Input.Cancel
					|| cancelButtonWidget.IsClicked) {
					Dismiss(false);
				}
				else if (Input.Ok
					|| okButtonWidget.IsClicked) {
					Dismiss(true);
				}
			}
		}

		public void Dismiss(bool canInvoke)
		{
			if (canInvoke) {
				string[] newExpressions = GetExpressions();
				if (ASMTruthTableData.Check(newExpressions, out int index)) {
					if (!m_data.Expressions.SequenceEqual(newExpressions)) callBack.Invoke(newExpressions);
					DialogsManager.HideDialog(this);
				}
				else {
					DialogsManager.ShowDialog(this, new MessageDialog("错误", $"表达式{index}非法", "确定", null, null));
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