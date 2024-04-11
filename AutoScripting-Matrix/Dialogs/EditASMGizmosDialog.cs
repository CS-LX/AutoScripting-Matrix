using System.Globalization;
using System.Xml.Linq;
using Engine;

namespace Game {
    public class EditASMGizmosDialog : ASMDialog {
        public ButtonWidget m_ignoreAllButtonWidget;

        public ButtonWidget m_unignoreButtonWidget;

        public ButtonWidget m_closeButtonWidget;

        public SubsystemASMGizmos m_subsystemAsmGizmos;

        public EditASMGizmosDialog(SubsystemASMGizmos subsystemAsmGizmos) : base("Dialogs/EditASMGizmosDialog") {
            m_subsystemAsmGizmos = subsystemAsmGizmos;
            m_ignoreAllButtonWidget = Children.Find<ButtonWidget>("IgnoreAllButton");
            m_unignoreButtonWidget = Children.Find<ButtonWidget>("UnignoreAllButton");
            m_closeButtonWidget = Children.Find<ButtonWidget>("CloseButton");
        }

        public override void Update() {
            m_ignoreAllButtonWidget.IsEnabled = m_subsystemAsmGizmos.CanOperate();
            m_unignoreButtonWidget.IsEnabled = m_subsystemAsmGizmos.CanOperate();
            if (m_ignoreAllButtonWidget.IsClicked) {
                m_subsystemAsmGizmos.IgnoreAll();
                Dismiss();
            }
            else if (m_unignoreButtonWidget.IsClicked) {
                m_subsystemAsmGizmos.UnignoreAll();
                Dismiss();
            }
            else if(Input.Ok || Input.Cancel || m_closeButtonWidget.IsClicked) {
                Dismiss();
            }
        }

        public void Dismiss() {
            DialogsManager.HideDialog(this);
        }
    }
}