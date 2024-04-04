using Engine;
using Engine.Graphics;
using GameEntitySystem;

namespace Game {
    public class ASMPortal : IDisposable {
        public ASMComplexPerspectiveCamera m_camera;
        public GameWidget m_gameWidget;
        public Matrix m_transformMatrix;
        public Vector3 m_lastViewTranslation;
        public ComponentASMPortal m_controller;
        public ComponentPlayer m_player;
        public int m_screenUVSubdivision;

        public readonly Project Project;

        readonly SubsystemTerrain m_subsystemTerrain;
        readonly SubsystemASMCamerasGameWidgets m_subsystemCameraGameWidgets;
        readonly SubsystemGameWidgets m_subsystemGameWidgets;
        readonly SubsystemSky m_subsystemSky;
        readonly SubsystemDrawing m_subsystemDrawing;

        public ASMPortal LinkedPortal;

        public ASMPortal(ASMComplexPerspectiveCamera camera, GameWidget gameWidget, Matrix transformMatrix, int screenUVSubdivision, Project project, ComponentASMPortal controller, ComponentPlayer player) {
            m_camera = camera;
            m_gameWidget = gameWidget;
            m_transformMatrix = transformMatrix;
            m_screenUVSubdivision = screenUVSubdivision;
            m_controller = controller;
            m_player = player;
            Project = project;
            m_subsystemTerrain = project.FindSubsystem<SubsystemTerrain>(true);
            m_subsystemCameraGameWidgets = project.FindSubsystem<SubsystemASMCamerasGameWidgets>(true);
            m_subsystemGameWidgets = project.FindSubsystem<SubsystemGameWidgets>(true);
            m_subsystemSky = project.FindSubsystem<SubsystemSky>(true);
            m_subsystemDrawing = project.FindSubsystem<SubsystemDrawing>(true);
        }

        public void LinkPortal(ASMPortal portal) {
            LinkedPortal = portal;
        }

        public void SetTransformMatrix(Matrix transformMatrix) => m_transformMatrix = transformMatrix;

        public void DrawView(PrimitivesRenderer3D primitivesRenderer3D) {
            m_camera.PrepareForDrawing();
            Vector3 translation = m_camera.ViewMatrix.Translation;
            if ((m_lastViewTranslation.XZ - translation.XZ).Length() > 16) {
                m_lastViewTranslation = translation;
                m_subsystemTerrain.TerrainUpdater.SetUpdateLocation(m_camera.GameWidget.PlayerData.PlayerIndex, translation.XZ, MathUtils.Min(m_subsystemTerrain.m_subsystemsky.VisibilityRange, 64f), 64f);
            }

            //去雾
            foreach (TerrainChunk terrainChunk in m_subsystemTerrain.Terrain.AllocatedChunks) {
                if (Vector2.DistanceSquared(m_camera.ViewPosition.XZ, terrainChunk.Center) <= MathUtils.Sqr(m_subsystemSky.VisibilityRange)
                    && terrainChunk.State == TerrainChunkState.Valid) {
                    terrainChunk.FogEnds[0] = float.MaxValue;
                }
            }
            RenderTarget2D lastRenderTarget = Display.RenderTarget;
            Display.RenderTarget = m_camera.ViewTexture;
            Display.Clear(Color.Black, 1f, 0);
            try {
                m_subsystemDrawing.Draw(m_camera);
            }
            finally {
                Display.RenderTarget = lastRenderTarget;
            }
        }

        public void DrawPortal(PrimitivesRenderer3D primitivesRenderer3D, Camera camera) {
            if (LinkedPortal == null) return;
            if (camera == m_camera
                || camera == LinkedPortal.m_camera)
                return;
            //传送门1
            for (int i = 0; i < m_screenUVSubdivision; i++) {
                for (int j = 0; j < m_screenUVSubdivision; j++) {
                    float length = 1 / (float)m_screenUVSubdivision;
                    Vector3 wp1 = Vector3.Transform(new Vector3(length * i - 0.5f, 0, length * j - 0.5f), m_transformMatrix);
                    Vector3 wp2 = Vector3.Transform(new Vector3(length * (i + 1) - 0.5f, 0, length * j - 0.5f), m_transformMatrix);
                    Vector3 wp3 = Vector3.Transform(new Vector3(length * (i + 1) - 0.5f, 0, length * (j + 1) - 0.5f), m_transformMatrix);
                    Vector3 wp4 = Vector3.Transform(new Vector3(length * i - 0.5f, 0, length * (j + 1) - 0.5f), m_transformMatrix);
                    Vector3 sp1 = camera.WorldToScreen(wp1, Matrix.Identity);
                    Vector3 sp2 = camera.WorldToScreen(wp2, Matrix.Identity);
                    Vector3 sp3 = camera.WorldToScreen(wp3, Matrix.Identity);
                    Vector3 sp4 = camera.WorldToScreen(wp4, Matrix.Identity);
                    Vector2 sp11 = new Vector2(sp1.X, sp1.Y);
                    Vector2 sp21 = new Vector2(sp2.X, sp2.Y);
                    Vector2 sp31 = new Vector2(sp3.X, sp3.Y);
                    Vector2 sp41 = new Vector2(sp4.X, sp4.Y);
                    sp11 = new Vector2(sp11.X / camera.ViewportSize.X, sp11.Y / camera.ViewportSize.Y);
                    sp21 = new Vector2(sp21.X / camera.ViewportSize.X, sp21.Y / camera.ViewportSize.Y);
                    sp31 = new Vector2(sp31.X / camera.ViewportSize.X, sp31.Y / camera.ViewportSize.Y);
                    sp41 = new Vector2(sp41.X / camera.ViewportSize.X, sp41.Y / camera.ViewportSize.Y);
                    primitivesRenderer3D.TexturedBatch(
                            LinkedPortal.m_camera.ViewTexture, //传送门1显示传送门2对应的摄像机的画面
                            useAlphaTest: true,
                            0,
                            null,
                            RasterizerState.CullNone,
                            null,
                            SamplerState.LinearWrap
                        )
                        .QueueQuad(
                            wp1,
                            wp2,
                            wp3,
                            wp4,
                            sp11,
                            sp21,
                            sp31,
                            sp41,
                            Color.White
                        );
                }
            }
        }

        public void Update(float dt) {
            if(LinkedPortal == null) return;

            Matrix playerView = m_player.GameWidget.ActiveCamera.ViewMatrix.Invert();
            Matrix portal1Trans = m_transformMatrix;
            Matrix portal2Trans = LinkedPortal.m_transformMatrix;

            Matrix playerCamToPortal2 = playerView * portal2Trans.Invert();
            Matrix viewMatrix1 = playerCamToPortal2 * portal1Trans;
            m_camera.SetViewMatrix(viewMatrix1);
        }

        public void Dispose() {
            m_subsystemTerrain.TerrainUpdater.RemoveUpdateLocation(m_gameWidget.PlayerData.PlayerIndex);
            m_subsystemGameWidgets.m_gameWidgets.Remove(m_gameWidget);
            m_subsystemCameraGameWidgets.m_gameWidgets.Remove(m_gameWidget);
            m_gameWidget.Dispose();
            m_gameWidget = null;
            Utilities.Dispose(ref m_camera.ViewTexture);
            m_gameWidget = null;
        }
    }
}