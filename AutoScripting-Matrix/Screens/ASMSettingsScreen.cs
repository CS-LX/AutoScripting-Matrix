using System.Xml.Linq;
using Engine;

namespace Game {
    public class ASMSettingsScreen  : Screen {
        public ButtonWidget m_displayUpperLeft;
        public ButtonWidget m_displayBackRect;

        public ButtonWidget m_displayConnectorMatrix;
        public ButtonWidget m_displayConnectorMatrix_DisplayIn;
        public SliderWidget m_displayConnectorMatrix_Thickness;

        public SliderWidget m_tpPlateVolume;
        public ASMSettingsScreen()
        {
            XElement node = ContentManager.Get<XElement>("Screens/ASMSettingsScreen");
            LoadContents(this, node);
            m_displayUpperLeft = Children.Find<BevelledButtonWidget>("DisplayUpperLeft");
            m_displayBackRect = Children.Find<BevelledButtonWidget>("DisplayBackRect");
            m_displayConnectorMatrix = Children.Find<BevelledButtonWidget>("DisplayConnectorMatrix");
            m_displayConnectorMatrix_DisplayIn = Children.Find<BevelledButtonWidget>("DisplayConnectorMatrix.DisplayIn");
            m_displayConnectorMatrix_Thickness = Children.Find<SliderWidget>("DisplayConnectorMatrix.Thickness");
            m_displayConnectorMatrix_Thickness.Value = float.Parse(ASMSettingsManager.Get("DisplayConnectorMatrix.Thickness").ToString());
            m_tpPlateVolume = Children.Find<SliderWidget>("TPPlateVolume");
            m_tpPlateVolume.Value = float.Parse(ASMSettingsManager.Get("TPPlateVolume").ToString());
        }

        public override void Update() {
            //显示左上角调试信息
            BoolDescriptor("DisplayUpperLeft", m_displayUpperLeft);
            //元件端口显示输出
            BoolDescriptor("DisplayConnectorMatrix", m_displayConnectorMatrix);
            BoolDescriptor("DisplayConnectorMatrix.DisplayIn", m_displayConnectorMatrix_DisplayIn);
            FloatDescriptor("DisplayConnectorMatrix.Thickness", m_displayConnectorMatrix_Thickness);
            //显示背景矩形
            BoolDescriptor("DisplayBackRect", m_displayBackRect);
            //转移板音量
            FloatDescriptor("TPPlateVolume", m_tpPlateVolume);

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

        private void FloatDescriptor(string settingName, SliderWidget slider) {
            if (slider.IsSliding) {
                ASMSettingsManager.Set(settingName, slider.Value);
            }
        }

        public override void Enter(object[] parameters) {
            base.Enter(parameters);
            Color topBarColor = SubsystemASMManager.ThemeColor;
            CanvasWidget topBar = Children.Find<CanvasWidget>("TopBar");
            topBar.Children.Find<BevelledRectangleWidget>("BackRectangle").CenterColor = topBarColor;
            topBar.Children.Find<BevelledRectangleWidget>("BackRectangle").BevelColor = topBarColor;
            topBar.Children.Find<BevelledRectangleWidget>("TopBarBevelledButton.Rectangle").CenterColor = topBarColor;
            topBar.Children.Find<BevelledRectangleWidget>("TopBarBevelledButton.Rectangle").BevelColor = topBarColor;
        }

        public bool IsEvening()
        {
            DateTime now = DateTime.Now;
            return now.Hour >= 18 || now.Hour < 7;
        }
    }
}