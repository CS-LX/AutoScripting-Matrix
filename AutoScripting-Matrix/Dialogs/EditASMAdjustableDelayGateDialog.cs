using Engine;
using System.Xml.Linq;

namespace Game
{
	public class EditASMAdjustableDelayGateDialog : ASMDialog
	{
		public Action<int> m_handler;

		public SliderWidget m_delaySlider;

		public ButtonWidget m_plusButton;
		public ButtonWidget m_plus10Button;
		public ButtonWidget m_plus100Button;

		public ButtonWidget m_minusButton;
		public ButtonWidget m_minus10Button;
		public ButtonWidget m_minus100Button;

		public LabelWidget m_delayLabel;

		public ButtonWidget m_okButton;

		public ButtonWidget m_cancelButton;

		public int m_delay;

		public int m_lastDelay;

		public EditASMAdjustableDelayGateDialog(ASMDelayData delay, Action<int> handler) : base("Dialogs/EditASMAdjustableDelayGateDialog")
		{
			m_delaySlider = Children.Find<SliderWidget>("EditAdjustableDelayGateDialog.DelaySlider");
			m_plusButton = Children.Find<ButtonWidget>("EditAdjustableDelayGateDialog.PlusButton");
			m_plus10Button = Children.Find<ButtonWidget>("EditAdjustableDelayGateDialog.Plus10Button");
			m_plus100Button = Children.Find<ButtonWidget>("EditAdjustableDelayGateDialog.Plus100Button");
			m_minusButton = Children.Find<ButtonWidget>("EditAdjustableDelayGateDialog.MinusButton");
			m_minus10Button = Children.Find<ButtonWidget>("EditAdjustableDelayGateDialog.Minus10Button");
			m_minus100Button = Children.Find<ButtonWidget>("EditAdjustableDelayGateDialog.Minus100Button");
			m_delayLabel = Children.Find<LabelWidget>("EditAdjustableDelayGateDialog.Label");
			m_okButton = Children.Find<ButtonWidget>("EditAdjustableDelayGateDialog.OK");
			m_cancelButton = Children.Find<ButtonWidget>("EditAdjustableDelayGateDialog.Cancel");
			m_handler = handler;
			m_delay = m_lastDelay = delay.Data;
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
			if (m_minus10Button.IsClicked)
			{
				m_delay = MathUtils.Max(m_delay - 10, (int)m_delaySlider.MinValue);
			}
			if (m_minus100Button.IsClicked)
			{
				m_delay = MathUtils.Max(m_delay - 100, (int)m_delaySlider.MinValue);
			}
			if (m_plusButton.IsClicked)
			{
				m_delay = MathUtils.Min(m_delay + 1, (int)m_delaySlider.MaxValue);
			}
			if (m_plus10Button.IsClicked)
			{
				m_delay = MathUtils.Min(m_delay + 10, (int)m_delaySlider.MaxValue);
			}
			if (m_plus100Button.IsClicked)
			{
				m_delay = MathUtils.Min(m_delay + 100, (int)m_delaySlider.MaxValue);
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
			m_minusButton.IsEnabled = m_minus10Button.IsEnabled = m_minus100Button.IsEnabled = m_delay > m_delaySlider.MinValue;
			m_plusButton.IsEnabled = m_plus10Button.IsEnabled = m_plus100Button.IsEnabled = m_delay < m_delaySlider.MaxValue;
			m_delayLabel.Text = string.Format("延迟: " + Math.Round((m_delay + 1) * 0.01f, 2));
		}

		public void Dismiss(int? result)
		{
			DialogsManager.HideDialog(this);
			if (m_handler != null && result.HasValue)
			{
				if(m_lastDelay != result.Value) m_handler(result.Value);
			}
		}
	}
}
