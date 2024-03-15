using Engine;
using System.Xml.Linq;

namespace Game {
    public class EditASMWireThroughBlockDialog : ASMDialog {
        public ButtonWidget m_okButton;

        public ButtonWidget m_cancelButton;

        public ButtonWidget m_leftButton;

        public ButtonWidget m_rightButton;

        public LabelWidget m_blockName;

        public BlockIconWidget m_blockIcon;

        public int m_type;

        public Action<int> m_callback;

        public EditASMWireThroughBlockDialog(int type, Action<int> callback) : base("Dialogs/EditASMWireThroughBlockDialog") {
            m_callback = callback;
            m_okButton = Children.Find<ButtonWidget>("EditAdjustableDelayGateDialog.OK");
            m_cancelButton = Children.Find<ButtonWidget>("EditAdjustableDelayGateDialog.Cancel");
            m_leftButton = Children.Find<ButtonWidget>("LeftButton");
            m_rightButton = Children.Find<ButtonWidget>("RightButton");
            m_type = MathUtils.Clamp(type, 0, ASMWireThroughBlock.Infos.Length - 1);
            m_blockIcon = Children.Find<BlockIconWidget>("BlockIcon");
            m_blockName = Children.Find<LabelWidget>("BlockName");
            UpdateControls();
        }

        public override void Update() {
            if (m_leftButton.IsClicked) {
                m_type--;
                m_type = (int)MathUtils.Remainder(m_type, ASMWireThroughBlock.Infos.Length);
            }
            if (m_rightButton.IsClicked) {
                m_type++;
                m_type %= ASMWireThroughBlock.Infos.Length;
            }
            if (m_okButton.IsClicked) {
                Dismiss(m_type);
            }
            if (Input.Cancel
                || m_cancelButton.IsClicked) {
                Dismiss(null);
            }
            UpdateControls();
        }

        public void UpdateControls() {
            int blockValue = Terrain.MakeBlockValue(ASMWireThroughBlock.Index, 0, ASMWireThroughBlock.SetWiredFaceAndType(0, 0, m_type));
            m_blockIcon.Value = blockValue;
            m_blockName.Text = BlocksManager.Blocks[ASMWireThroughBlock.Index].GetDisplayName(null, blockValue);
        }

        public void Dismiss(int? result) {
            DialogsManager.HideDialog(this);
            if (result.HasValue) m_callback?.Invoke(result.Value);
        }
    }
}