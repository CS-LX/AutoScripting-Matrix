using Engine;
using Engine.Graphics;
using GameEntitySystem;
using System.Collections.Generic;
using Engine.Media;
using TemplatesDatabase;

namespace Game {
    public class SubsystemASMGlow : Subsystem, IDrawable {
        public SubsystemSky m_subsystemSky;

        public SubsystemTerrain m_subsystemTerrain;

        public Dictionary<ASMGlowPoint, bool> m_glowPoints = new Dictionary<ASMGlowPoint, bool>();

        public Dictionary<ASMGlowBlock, bool> m_glowBlocks = new Dictionary<ASMGlowBlock, bool>();

        public Dictionary<IASMGlowGeometry, bool> m_glowGeometries = new Dictionary<IASMGlowGeometry, bool>();

        public Dictionary<ASMGlowText, bool> m_glowTexts = new Dictionary<ASMGlowText, bool>();

        public PrimitivesRenderer3D m_primitivesRenderer = new PrimitivesRenderer3D();

        public TexturedBatch3D[] m_batchesByType = new TexturedBatch3D[4];

        public FlatBatch3D m_geometryBatch;

        public FontBatch3D m_textBatch;

        public static int[] m_drawOrders = new int[1] { 110 };

        public int[] DrawOrders => m_drawOrders;

        public ASMGlowPoint AddGlowPoint() {
            var glowPoint = new ASMGlowPoint();
            m_glowPoints.Add(glowPoint, value: true);
            return glowPoint;
        }

        public ASMGlowBlock AddGlowBlock() {
            var glowBlock = new ASMGlowBlock();
            m_glowBlocks.Add(glowBlock, true);
            return glowBlock;
        }

        public ASMGlowCuboid AddGlowCuboid() {
            var glowBlock = new ASMGlowCuboid();
            m_glowGeometries.Add(glowBlock, true);
            return glowBlock;
        }

        public ASMGlowLine AddGlowLine() {
            var glowBlock = new ASMGlowLine();
            m_glowGeometries.Add(glowBlock, true);
            return glowBlock;
        }

        public ASMGlowText AddGlowText() {
            var glowBlock = new ASMGlowText();
            m_glowTexts.Add(glowBlock, true);
            return glowBlock;
        }

        public void RemoveGlowPoint(ASMGlowPoint glowPoint) {
            m_glowPoints.Remove(glowPoint);
        }

        public void RemoveGlowBlock(ASMGlowBlock glowBlock) {
            m_glowBlocks.Remove(glowBlock);
        }

        public void RemoveGlowGeometry(IASMGlowGeometry glowGeometry) {
            m_glowGeometries.Remove(glowGeometry);
        }

        public void RemoveGlowText(ASMGlowText glowText) {
            m_glowTexts.Remove(glowText);
        }

        public void Draw(Camera camera, int drawOrder) {
            //绘制点
            foreach (ASMGlowPoint key in m_glowPoints.Keys) {
                if (key.Color.A > 0) {
                    Vector3 vector = key.Position - camera.ViewPosition;
                    float num = Vector3.Dot(vector, camera.ViewDirection);
                    if (num > 0.01f) {
                        float num2 = vector.Length();
                        if (num2 < m_subsystemSky.ViewFogRange.Y) {
                            float num3 = key.Size;
                            if (key.FarDistance > 0f) {
                                num3 += (key.FarSize - key.Size) * MathUtils.Saturate(num2 / key.FarDistance);
                            }
                            Vector3 v = (0f - (0.01f + (0.02f * num))) / num2 * vector;
                            Vector3 p = key.Position + (num3 * (-key.Right - key.Up)) + v;
                            Vector3 p2 = key.Position + (num3 * (key.Right - key.Up)) + v;
                            Vector3 p3 = key.Position + (num3 * (key.Right + key.Up)) + v;
                            Vector3 p4 = key.Position + (num3 * (-key.Right + key.Up)) + v;
                            m_batchesByType[(int)key.Type]
                            .QueueQuad(
                                p,
                                p2,
                                p3,
                                p4,
                                new Vector2(0f, 0f),
                                new Vector2(1f, 0f),
                                new Vector2(1f, 1f),
                                new Vector2(0f, 1f),
                                key.Color
                            );
                        }
                    }
                }
            }
            //绘制方块
            foreach (var glowBlock in m_glowBlocks.Keys) {
                Vector3 position = glowBlock.transform.Translation;
                Point3 pos = new ((int)position.X, (int)position.Y, (int)position.Z);

                //处理环境
                DrawBlockEnvironmentData environmentData = new();
                environmentData.SubsystemTerrain = m_subsystemTerrain;
                environmentData.InWorldMatrix = Matrix.Identity;
                environmentData.DrawBlockMode = DrawBlockMode.FirstPerson;//第一人称下绘制的方块，可具有厚度
                if (glowBlock.environmentallySusceptible) {
                    try {
                        environmentData.Humidity = m_subsystemTerrain.Terrain.GetSeasonalHumidity(pos.X, pos.Z);
                        environmentData.Temperature = m_subsystemTerrain.Terrain.GetSeasonalTemperature(pos.X, pos.Z) + SubsystemWeather.GetTemperatureAdjustmentAtHeight(pos.Y);
                        environmentData.Light = m_subsystemTerrain.Terrain.GetCellLightFast(pos.X, pos.Y, pos.Z);
                    }
                    catch { }
                }

                //处理ID
                int id = Terrain.ExtractContents(glowBlock.index);
                Block block = BlocksManager.Blocks[id];

                //绘制方块
                block.DrawBlock(m_primitivesRenderer, glowBlock.index, Color.White, (float)(block.InHandScale * 1), ref glowBlock.transform, glowBlock.environmentallySusceptible ? environmentData : BlocksManager.m_defaultEnvironmentData);
            }
            //绘制几何方面
            foreach (var glowGeometry in m_glowGeometries) {
                glowGeometry.Key.Draw(m_geometryBatch);
            }
            //绘制文字
            foreach (var glowText in m_glowTexts) {
                Vector3 viewDirection = camera.ViewDirection;
                var vector = Vector3.Normalize(Vector3.Cross(viewDirection, Vector3.UnitY));
                Vector3 v = -Vector3.Normalize(Vector3.Cross(vector, viewDirection));
                float s = 0.006f;
                m_textBatch.QueueText(glowText.Key.m_text, glowText.Key.m_position, glowText.Key.m_billBoard ? vector * s : glowText.Key.m_right, glowText.Key.m_billBoard ? v * s : glowText.Key.m_down, glowText.Key.m_color, TextAnchor.HorizontalCenter | TextAnchor.VerticalCenter, Vector2.Zero);
            }
            m_primitivesRenderer.Flush(camera.ViewProjectionMatrix);
        }

        public override void Load(ValuesDictionary valuesDictionary) {
            m_subsystemSky = Project.FindSubsystem<SubsystemSky>(true);
            m_subsystemTerrain = Project.FindSubsystem<SubsystemTerrain>(true);
            m_batchesByType[0] = m_primitivesRenderer.TexturedBatch(
                ContentManager.Get<Texture2D>("Textures/RoundGlow"),
                useAlphaTest: false,
                0,
                DepthStencilState.DepthRead,
                RasterizerState.CullCounterClockwiseScissor,
                BlendState.AlphaBlend,
                SamplerState.LinearClamp
            );
            m_batchesByType[1] = m_primitivesRenderer.TexturedBatch(
                ContentManager.Get<Texture2D>("Textures/SquareGlow"),
                useAlphaTest: false,
                0,
                DepthStencilState.DepthRead,
                RasterizerState.CullCounterClockwiseScissor,
                BlendState.AlphaBlend,
                SamplerState.LinearClamp
            );
            m_batchesByType[2] = m_primitivesRenderer.TexturedBatch(
                ContentManager.Get<Texture2D>("Textures/HorizontalRectGlow"),
                useAlphaTest: false,
                0,
                DepthStencilState.DepthRead,
                RasterizerState.CullCounterClockwiseScissor,
                BlendState.AlphaBlend,
                SamplerState.LinearClamp
            );
            m_batchesByType[3] = m_primitivesRenderer.TexturedBatch(
                ContentManager.Get<Texture2D>("Textures/VerticalRectGlow"),
                useAlphaTest: false,
                0,
                DepthStencilState.DepthRead,
                RasterizerState.CullCounterClockwiseScissor,
                BlendState.AlphaBlend,
                SamplerState.LinearClamp
            );
            m_geometryBatch = m_primitivesRenderer.FlatBatch();
            m_textBatch = m_primitivesRenderer.FontBatch(BitmapFont.DebugFont, 0, DepthStencilState.None);
        }
    }
}