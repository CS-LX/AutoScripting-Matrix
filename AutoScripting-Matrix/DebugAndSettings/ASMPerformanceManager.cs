using Engine;
using Engine.Graphics;
using Engine.Media;

namespace Game {
    public static class ASMPerformanceManager {
        public static PrimitivesRenderer2D m_primitivesRenderer;

        static ASMPerformanceManager() {
            m_primitivesRenderer = new PrimitivesRenderer2D();
        }

        public static void Draw(ComponentGui gui, Camera camera) {
            if(!ASMSettingsManager.Get<bool>("DisplayUpperLeft")) return;
            if(camera is ASMComplexPerspectiveCamera) return;//防止摄像机板画面出现调试信息
            Ray3 ray = new Ray3(gui.m_componentPlayer.GameWidget.ActiveCamera.ViewPosition, gui.m_componentPlayer.GameWidget.ActiveCamera.ViewDirection);
            var scale = new Vector2(MathUtils.Round(MathUtils.Clamp(ScreensManager.RootWidget.GlobalScale, 1.0f, 4.0f)));
            SubsystemASMElectricity asmElectricity = gui.Project.FindSubsystem<SubsystemASMElectricity>();
            string displayText = "";

            object obj = gui.m_componentPlayer.ComponentMiner.Raycast(ray, RaycastMode.Digging);
            if (obj is TerrainRaycastResult) {
                TerrainRaycastResult result = (TerrainRaycastResult)obj;
                int id = result.Value;
                CellFace cellFace = result.CellFace;
                displayText = $"P: ({result.CellFace})\r\n";
                if (BlocksManager.Blocks[Terrain.ExtractContents(id)] is IASMElectricElementBlock) {
                    try {
                        displayText += $"D: {Convert.ToString(Terrain.ExtractData(id), 2)} ({Terrain.ExtractData(id)})\r\n"
                            + $"E: {asmElectricity.m_electricElementsByCellFace[cellFace]}\r\n"
                            + $"Hash: {asmElectricity.m_electricElementsByCellFace[cellFace].GetHashCode()}\r\n";
                        displayText += "V: \r\n";
                        for (int i = 0; i < 6; i++) {
                            displayText += $"    {asmElectricity.m_electricElementsByCellFace[cellFace].GetOutputVoltage(i)}\r\n";
                        }
                    }
                    catch (Exception e) {
                        displayText = e.Message;
                    }
                }
            }

            FontBatch2D fontBatch2D = m_primitivesRenderer.FontBatch(BitmapFont.DebugFont, 0, null, null, null, SamplerState.PointClamp);
            fontBatch2D.QueueText(displayText, Vector2.Transform(Vector2.Zero, ScreensManager.RootWidget.GlobalTransform), 0f, Color.White, TextAnchor.Default, scale, Vector2.Zero);
            m_primitivesRenderer.Flush();
        }
    }
}