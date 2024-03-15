using System.Xml.Linq;
using Engine;

namespace Game {
    public class ASMSettingsScreen  : Screen {
        public ButtonWidget m_displayUpperLeft;
        public ButtonWidget m_displayBackRect;

        public ASMSettingsScreen()
        {
            XElement node = ContentManager.Get<XElement>("Screens/ASMSettingsScreen");
            LoadContents(this, node);
            m_displayUpperLeft = Children.Find<BevelledButtonWidget>("DisplayUpperLeft");
            m_displayBackRect = Children.Find<BevelledButtonWidget>("DisplayBackRect");
        }

        public override void Update() {
            //显示左上角调试信息
            bool displayUpperLeft = (bool)ASMSettingsManager.Get("DisplayUpperLeft");
            m_displayUpperLeft.Text = displayUpperLeft ? "是" : "否";
            if(m_displayUpperLeft.IsClicked) ASMSettingsManager.Set("DisplayUpperLeft", !displayUpperLeft);
            //显示背景矩形
            bool displayBackRect = (bool)ASMSettingsManager.Get("DisplayBackRect");
            m_displayBackRect.Text = displayBackRect ? "是" : "否";
            if(m_displayBackRect.IsClicked) ASMSettingsManager.Set("DisplayBackRect", !displayBackRect);

            if (Input.Back || Input.Cancel || Children.Find<ButtonWidget>("TopBar.Back").IsClicked)
            {
                ScreensManager.SwitchScreen(ScreensManager.PreviousScreen);
            }
        }
    }
}