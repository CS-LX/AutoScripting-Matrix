using System.Xml.Linq;
using Engine;

namespace Game {
    public class ASMSettingsScreen  : Screen {
        public ButtonWidget m_displayUpperLeft;

        public ASMSettingsScreen()
        {
            XElement node = ContentManager.Get<XElement>("Screens/ASMSettingsScreen");
            LoadContents(this, node);
            m_displayUpperLeft = Children.Find<BevelledButtonWidget>("DisplayUpperLeft");
        }

        public override void Update() {
            //显示左上角调试信息
            bool displayUpperLeft = (bool)ASMSettingsManager.Get("DisplayUpperLeft");
            m_displayUpperLeft.Text = displayUpperLeft ? "是" : "否";
            if(m_displayUpperLeft.IsClicked) ASMSettingsManager.Set("DisplayUpperLeft", !displayUpperLeft);

            if (Input.Back || Input.Cancel || Children.Find<ButtonWidget>("TopBar.Back").IsClicked)
            {
                ScreensManager.SwitchScreen(ScreensManager.PreviousScreen);
            }
        }
    }
}