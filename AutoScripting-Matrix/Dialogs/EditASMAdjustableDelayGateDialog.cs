using Engine;
using System.Xml.Linq;

namespace Game
{
	public class EditASMAdjustableDelayGateDialog : Dialog
	{
		public Action<int> m_handler;

		public SliderWidget m_delaySlider;

		public ButtonWidget m_plusButton;

		public ButtonWidget m_minusButton;

		public LabelWidget m_delayLabel;

		public ButtonWidget m_okButton;

		public ButtonWidget m_cancelButton;

		public int m_delay;

		public EditASMAdjustableDelayGateDialog(ASMDelayData delay, Action<int> handler)
		{
			XElement node = ContentManager.Get<XElement>("Dialogs/EditASMAdjustableDelayGateDialog");
			LoadContents(this, node);
			m_delaySlider = Children.Find<SliderWidget>("EditAdjustableDelayGateDialog.DelaySlider");
			m_plusButton = Children.Find<ButtonWidget>("EditAdjustableDelayGateDialog.PlusButton");
			m_minusButton = Children.Find<ButtonWidget>("EditAdjustableDelayGateDialog.MinusButton");
			m_delayLabel = Children.Find<LabelWidget>("EditAdjustableDelayGateDialog.Label");
			m_okButton = Children.Find<ButtonWidget>("EditAdjustableDelayGateDialog.OK");
			m_cancelButton = Children.Find<ButtonWidget>("EditAdjustableDelayGateDialog.Cancel");
			m_handler = handler;
			m_delay = delay.Data;
			UpdateControls();
		}

		public override void Update()
		{
			if (m_delaySlider.IsSliding)
			{
				m_delay = (int)m_delaySlider.Value;
			}
			if (m_minusButton.IsClicked)
			{
				m_delay = MathUtils.Max(m_delay - 1, (int)m_delaySlider.MinValue);
			}
			if (m_plusButton.IsClicked)
			{
				m_delay = MathUtils.Min(m_delay + 1, (int)m_delaySlider.MaxValue);
			}
			if (m_okButton.IsClicked)
			{
				Dismiss(m_delay);
			}
			if (Input.Cancel || m_cancelButton.IsClicked)
			{
				Dismiss(null);
			}
			UpdateControls();
		}

		public void UpdateControls()
		{
			m_delaySlider.Value = m_delay;
			m_minusButton.IsEnabled = m_delay > m_delaySlider.MinValue;
			m_plusButton.IsEnabled = m_delay < m_delaySlider.MaxValue;
			m_delayLabel.Text = string.Format("延迟: " + Math.Round((m_delay + 1) * 0.01f, 2));
		}

		public void Dismiss(int? result)
		{
			DialogsManager.HideDialog(this);
			if (m_handler != null && result.HasValue)
			{
				m_handler(result.Value);
			}
		}
	}
}
