using Engine.Graphics;

namespace Game {
    public class ASMModLoader : ModLoader {
        public override void __ModInitialize() {
            base.__ModInitialize();
            ModsManager.RegisterHook("GuiDraw", this);
        }

        public override void GuiDraw(ComponentGui componentGui, Camera camera, int drawOrder) {
            base.GuiUpdate(componentGui);
            ASMPerformanceManager.Draw(componentGui);
        }
    }
}