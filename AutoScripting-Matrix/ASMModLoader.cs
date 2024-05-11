using System.Xml.Linq;
using Engine;
using Engine.Graphics;

namespace Game {
    public class ASMModLoader : ModLoader {
        public override void __ModInitialize() {
            base.__ModInitialize();
            ModsManager.RegisterHook("GuiDraw", this);
            ModsManager.RegisterHook("OnLoadingFinished", this);
            ModsManager.RegisterHook("SaveSettings", this);
            ModsManager.RegisterHook("OnCameraChange", this);
        }

        public override void GuiDraw(ComponentGui componentGui, Camera camera, int drawOrder) {
            base.GuiUpdate(componentGui);
            ASMPerformanceManager.Draw(componentGui, camera);
        }

        public override void OnLoadingFinished(List<Action> actions) {
            base.OnLoadingFinished(actions);
            ASMSettingsManager.Init();
            ScreensManager.AddScreen("ASMSettings", new ASMSettingsScreen());
        }

        public override void OnCameraChange(ComponentPlayer m_componentPlayer, ComponentGui componentGui) {
            GameWidget gameWidget = m_componentPlayer.GameWidget;
            int playerIndex = m_componentPlayer.PlayerData.PlayerIndex;
            ASMComplexPerspectiveCamera camera = ASMPlayerCameraSetterManager.m_cameras[playerIndex];

            if (gameWidget.ActiveCamera is FppCamera) {
                if (!ASMPlayerCameraSetterManager.m_isCameraActive[playerIndex]) {
                    if (camera == null) return;
                    gameWidget.ActiveCamera = camera;
                    componentGui.DisplaySmallMessage("矩阵摄像机", Color.White, blinking: false, playNotificationSound: false);
                    ASMPlayerCameraSetterManager.m_isCameraActive[playerIndex] = true;
                }
                else {
                    ASMPlayerCameraSetterManager.m_isCameraActive[playerIndex] = false;
                }
            }
        }

        public override void SaveSettings(XElement xElement) {
            base.SaveSettings(xElement);
        }
    }
}