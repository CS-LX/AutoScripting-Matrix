using System.Globalization;
using System.Xml.Linq;
using Engine;

namespace Game {
    public class EditASMGizmosDialog : ASMDialog {
        public ButtonWidget ignoreAllButtonWidget;

        public ButtonWidget unignoreButtonWidget;

        public SubsystemASMGizmos m_subsystemAsmGizmos;

        public EditASMGizmosDialog(SubsystemASMGizmos subsystemAsmGizmos) : base("Dialogs/EditASMGizmosDialog") {
            m_subsystemAsmGizmos = subsystemAsmGizmos;
            ignoreAllButtonWidget = Children.Find<ButtonWidget>("IgnoreAllButton");
            unignoreButtonWidget = Children.Find<ButtonWidget>("UnignoreAllButton");
        }

        public override void Update() {
            if (ignoreAllButtonWidget.IsClicked) {
                m_subsystemAsmGizmos.IgnoreAll();
                Dismiss();
            }
            else if (unignoreButtonWidget.IsClicked) {
                m_subsystemAsmGizmos.UnignoreAll();
                Dismiss();
            }
            else if(Input.Ok || Input.Cancel) {
                Dismiss();
            }
        }

        public void Dismiss() {
            DialogsManager.HideDialog(this);
        }
    }
}