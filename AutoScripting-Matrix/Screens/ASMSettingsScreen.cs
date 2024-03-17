using System.Xml.Linq;
using Engine;

namespace Game {
    public class ASMSettingsScreen  : Screen {
        public ButtonWidget m_displayUpperLeft;
        public ButtonWidget m_displayBackRect;

        public ButtonWidget m_displayConnectorMatrix;
        public ButtonWidget m_displayConnectorMatrix_DisplayIn;

        public SliderWidget m_tpPlateVolume;
        float m_lastTPPlateVolume;
        public ASMSettingsScreen()
        {
            XElement node = ContentManager.Get<XElement>("Screens/ASMSettingsScreen");
            LoadContents(this, node);
            m_displayUpperLeft = Children.Find<BevelledButtonWidget>("DisplayUpperLeft");
            m_displayBackRect = Children.Find<BevelledButtonWidget>("DisplayBackRect");
            m_displayConnectorMatrix = Children.Find<BevelledButtonWidget>("DisplayConnectorMatrix");
            m_displayConnectorMatrix_DisplayIn = Children.Find<BevelledButtonWidget>("DisplayConnectorMatrix.DisplayIn");
            m_tpPlateVolume = Children.Find<SliderWidget>("TPPlateVolume");
            m_tpPlateVolume.Value = float.Parse(ASMSettingsManager.Get("TPPlateVolume").ToString());
        }

        public override void Update() {
            //显示左上角调试信息
            BoolDescriptor("DisplayUpperLeft", m_displayUpperLeft);
            //元件端口显示输出
            BoolDescriptor("DisplayConnectorMatrix", m_displayConnectorMatrix);
            BoolDescriptor("DisplayConnectorMatrix.DisplayIn", m_displayConnectorMatrix_DisplayIn);
            //显示背景矩形
            BoolDescriptor("DisplayBackRect", m_displayBackRect);
            //转移板音量
            if (m_lastTPPlateVolume != m_tpPlateVolume.Value) {
                m_lastTPPlateVolume = m_tpPlateVolume.Value;
                ASMSettingsManager.Set("TPPlateVolume", m_tpPlateVolume.Value);
            }

            if (Input.Back || Input.Cancel || Children.Find<ButtonWidget>("TopBar.Back").IsClicked)
            {
                ASMSettingsManager.Save();
                ScreensManager.SwitchScreen(ScreensManager.PreviousScreen);
            }
        }

        private void BoolDescriptor(string settingName, ButtonWidget button) {
            bool flag = (bool)ASMSettingsManager.Get(settingName);
            button.Text = flag ? "是" : "否";
            if(button.IsClicked) ASMSettingsManager.Set(settingName, !flag);
        }
    }
}