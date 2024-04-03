using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game {
    public class ComponentASMPortal : Component, IUpdateable, IDrawable {
        public int[] DrawOrders => [202, 1101];

        public UpdateOrder UpdateOrder => UpdateOrder.Default;

        public SubsystemASMElectricity m_subsystemAsmElectricity;

        public SubsystemGameWidgets m_subsystemGameWidgets;

        public ASMPortalElectricElement m_portalElectricElement;

        public SubsystemTerrain m_subsystemTerrain;

        public SubsystemDrawing m_subsystemDrawing;

        public SubsystemSky m_subsystemSky;

        public SubsystemASMCamerasGameWidgets m_subsystemCameraGameWidgets;

        public ComponentPlayer m_componentPlayer;

        public ComponentBody m_componentBody;

        public ComponentBlockEntity m_componentEntity;

        public Vector3 m_position;

        public int m_face;

        //绘制所需
        public PrimitivesRenderer3D m_primitivesRenderer = new PrimitivesRenderer3D();

        public ASMComplexPerspectiveCamera m_camera1;

        public ASMComplexPerspectiveCamera m_camera2;

        public GameWidget m_gameWidget1;

        public GameWidget m_gameWidget2;

        public Vector3 m_cam1Translation;

        public Vector3 m_cam2Translation;

        public int m_screenUVSubdivision = 24;

        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap) {
            base.Load(valuesDictionary, idToEntityMap);
            m_subsystemAsmElectricity = Project.FindSubsystem<SubsystemASMElectricity>(true);
            m_subsystemGameWidgets = Project.FindSubsystem<SubsystemGameWidgets>(true);
            m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
            m_subsystemDrawing = Project.FindSubsystem<SubsystemDrawing>(true);
            m_subsystemSky = Project.FindSubsystem<SubsystemSky>(true);
            m_subsystemCameraGameWidgets = base.Project.FindSubsystem<SubsystemASMCamerasGameWidgets>(true);
            m_componentEntity = Entity.FindComponent<ComponentBlockEntity>(true);
        }

        public override void OnEntityRemoved() {
            base.OnEntityRemoved();
            DisposeCamera(ref m_camera1, ref m_gameWidget1);
            DisposeCamera(ref m_camera2, ref m_gameWidget2);
        }

        public void Update(float dt) {
            //若玩家没有数据，则寻找玩家
            if (m_componentPlayer == null) {
                m_componentPlayer = FindInteractingPlayer();
            }
            else {
                Point3 coordinates = m_componentEntity.Coordinates;
                int blockValue = m_subsystemTerrain.Terrain.GetCellValueFast(coordinates.X, coordinates.Y, coordinates.Z);
                m_face = ASMPortalBlock.GetFace_Static(blockValue);
                //如果没有电子元件，就寻找
                if (m_portalElectricElement == null
                    || ASMPortalBlock.GetRotation(Terrain.ExtractData(m_portalElectricElement.m_blockValue)) != ASMPortalBlock.GetRotation(Terrain.ExtractData(blockValue))) {
                    FindElectricElement();
                    return;
                }

                Matrix playerView = m_componentPlayer.GameWidget.ActiveCamera.ViewMatrix.Invert();
                Matrix portal1Trans = m_portalElectricElement.GetPortal1Transform().OrientationMatrix * (m_portalElectricElement.GetPortal1Transform().TranslationMatrix * Matrix.CreateTranslation(m_position));
                Matrix portal2Trans = m_portalElectricElement.GetPortal2Transform().OrientationMatrix * (m_portalElectricElement.GetPortal2Transform().TranslationMatrix * Matrix.CreateTranslation(m_position));

                Matrix playerCamToPortal2 = playerView * portal2Trans.Invert();
                Matrix viewMatrix1 = playerCamToPortal2 * portal1Trans;
                if(m_camera1 == null) AddCamera(ref m_camera1, ref m_gameWidget1, viewMatrix1);
                m_camera1.SetViewMatrix(viewMatrix1);//拍原始传送门画面的相机用玩家的视角

                Matrix playerCamToPortal1 = playerView * portal1Trans.Invert();//传送门1为参考系，玩家视角的变换矩阵
                Matrix viewMatrix2 = playerCamToPortal1 * portal2Trans;
                if(m_camera2 == null) AddCamera(ref m_camera2, ref m_gameWidget2, viewMatrix2);
                m_camera2.SetViewMatrix(viewMatrix2);

                if (m_subsystemTerrain.TerrainRenderer.m_chunksToDraw.Count > 0) {
                    DrawView(ref m_camera1, m_camera1.ViewMatrix.Invert(), ref m_cam1Translation);
                    DrawView(ref m_camera2, m_camera2.ViewMatrix.Invert(), ref m_cam2Translation);
                }
            }
        }

        public void Draw(Camera camera, int drawOrder) {
            if (m_componentPlayer == null
                || m_camera1 == null
                || m_camera2 == null
                || m_portalElectricElement == null)
                return;
            DrawPortal(camera);
        }

        public void DrawView(ref ASMComplexPerspectiveCamera camera, Matrix viewMatrix, ref Vector3 lastTranslation) {
            camera.PrepareForDrawing();
            Vector3 translation = viewMatrix.Translation;
            if ((lastTranslation.XZ - translation.XZ).Length() > 16) {
                lastTranslation = translation;
                m_subsystemTerrain.TerrainUpdater.SetUpdateLocation(camera.GameWidget.PlayerData.PlayerIndex, translation.XZ, MathUtils.Min(m_subsystemTerrain.m_subsystemsky.VisibilityRange, 64f), 64f);
            }

            //去雾
            foreach (TerrainChunk terrainChunk in m_subsystemTerrain.Terrain.AllocatedChunks) {
                if (Vector2.DistanceSquared(camera.ViewPosition.XZ, terrainChunk.Center) <= MathUtils.Sqr(m_subsystemSky.VisibilityRange)
                    && terrainChunk.State == TerrainChunkState.Valid) {
                    terrainChunk.FogEnds[0] = float.MaxValue;
                }
            }

            RenderTarget2D lastRenderTarget = Display.RenderTarget;
            Display.RenderTarget = camera.ViewTexture;
            Display.Clear(Color.Black, 1f, 0);
            try {
                m_subsystemDrawing.Draw(camera);
            }
            finally {
                Display.RenderTarget = lastRenderTarget;
            }
        }

        public void DrawPortal(Camera camera) {
            Vector3 offset1 = new Vector3(-0.5f, 0, -0.5f);
            Vector3 offset2 = new Vector3(0.5f, 0, -0.5f);
            Vector3 offset3 = new Vector3(0.5f, 0, 0.5f);
            Vector3 offset4 = new Vector3(-0.5f, 0, 0.5f);
            //传送门1
            for (int i = 0; i < m_screenUVSubdivision; i++) {
                if (camera.GetHashCode() == m_camera2.GetHashCode() || camera.GetHashCode() == m_camera1.GetHashCode()) continue;
                for (int j = 0; j < m_screenUVSubdivision; j++) {
                    float length = 1 / (float)m_screenUVSubdivision;
                    Vector3 wp1 = m_position + Vector3.Transform(new Vector3(length * i - 0.5f, 0, length * j - 0.5f), m_portalElectricElement.GetPortal1Transform());
                    Vector3 wp2 = m_position + Vector3.Transform(new Vector3(length * (i + 1) - 0.5f, 0, length * j - 0.5f), m_portalElectricElement.GetPortal1Transform());
                    Vector3 wp3 = m_position + Vector3.Transform(new Vector3(length * (i + 1) - 0.5f, 0, length * (j + 1) - 0.5f), m_portalElectricElement.GetPortal1Transform());
                    Vector3 wp4 = m_position + Vector3.Transform(new Vector3(length * i - 0.5f, 0, length * (j + 1) - 0.5f), m_portalElectricElement.GetPortal1Transform());
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
                    m_primitivesRenderer.TexturedBatch(
                            m_camera2.ViewTexture,//传送门1显示传送门2对应的摄像机的画面
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
            //传送门2
            for (int i = 0; i < m_screenUVSubdivision; i++) {
                if (camera.GetHashCode() == m_camera2.GetHashCode() || camera.GetHashCode() == m_camera1.GetHashCode()) continue;
                for (int j = 0; j < m_screenUVSubdivision; j++) {
                    float length = 1 / (float)m_screenUVSubdivision;
                    Vector3 wp1 = m_position + Vector3.Transform(new Vector3(length * i - 0.5f, 0, length * j - 0.5f), m_portalElectricElement.GetPortal2Transform());
                    Vector3 wp2 = m_position + Vector3.Transform(new Vector3(length * (i + 1) - 0.5f, 0, length * j - 0.5f), m_portalElectricElement.GetPortal2Transform());
                    Vector3 wp3 = m_position + Vector3.Transform(new Vector3(length * (i + 1) - 0.5f, 0, length * (j + 1) - 0.5f), m_portalElectricElement.GetPortal2Transform());
                    Vector3 wp4 = m_position + Vector3.Transform(new Vector3(length * i - 0.5f, 0, length * (j + 1) - 0.5f), m_portalElectricElement.GetPortal2Transform());
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
                    m_primitivesRenderer.TexturedBatch(
                            m_camera1.ViewTexture,//传送门2显示传送门1对应画面
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
            m_primitivesRenderer.Flush(camera.ViewProjectionMatrix);
        }

        public ComponentPlayer FindInteractingPlayer() {
            ComponentPlayer componentPlayer = Entity.FindComponent<ComponentPlayer>();
            if (componentPlayer == null) {
                ComponentBlockEntity componentBlockEntity = Entity.FindComponent<ComponentBlockEntity>();
                if (componentBlockEntity != null) {
                    var position = new Vector3(componentBlockEntity.Coordinates);
                    componentPlayer = Project.FindSubsystem<SubsystemPlayers>(throwOnError: true).FindNearestPlayer(position);
                }
            }
            return componentPlayer;
        }

        public void FindElectricElement() {
            Point3 coordinates = m_componentEntity.Coordinates;
            m_position = new Vector3(coordinates) + 0.5f * Vector3.One;
            int blockValue = m_subsystemTerrain.Terrain.GetCellValueFast(coordinates.X, coordinates.Y, coordinates.Z);
            m_face = ASMComplexCameraLEDBlock.GetFace_Static(blockValue);
            m_portalElectricElement = m_subsystemAsmElectricity.GetElectricElement(coordinates.X, coordinates.Y, coordinates.Z, m_face) as ASMPortalElectricElement;
        }

        public void AddCamera(ref ASMComplexPerspectiveCamera camera, ref GameWidget gameWidget, Matrix viewMatrix) {
            int index = new Random().Int(4, int.MaxValue);
            while (m_subsystemTerrain.TerrainUpdater.m_pendingLocations.ContainsKey(index)) {
                index = new Random().Int(4, int.MaxValue);
            }
            gameWidget = new GameWidget(new PlayerData(Project) { PlayerIndex = index }, 0);
            m_subsystemGameWidgets.m_gameWidgets.Add(gameWidget);
            m_subsystemCameraGameWidgets.m_gameWidgets.Add(gameWidget);
            camera = new ASMComplexPerspectiveCamera(gameWidget, m_componentPlayer.GameWidget.ActiveCamera.ProjectionMatrix);
            camera.SetViewMatrix(viewMatrix);
            gameWidget.m_activeCamera = camera;
            gameWidget.m_cameras = [camera];

            m_subsystemTerrain.TerrainUpdater.SetUpdateLocation(gameWidget.PlayerData.PlayerIndex, viewMatrix.Translation.XZ, MathUtils.Min(m_subsystemTerrain.m_subsystemsky.VisibilityRange, 64f), 64f);
        }

        public void DisposeCamera(ref ASMComplexPerspectiveCamera camera, ref GameWidget gameWidget) {
            m_subsystemTerrain.TerrainUpdater.RemoveUpdateLocation(gameWidget.PlayerData.PlayerIndex);
            m_subsystemGameWidgets.m_gameWidgets.Remove(gameWidget);
            m_subsystemCameraGameWidgets.m_gameWidgets.Remove(gameWidget);
            gameWidget.Dispose();
            gameWidget = null;
            Utilities.Dispose(ref camera.ViewTexture);
            camera = null;
        }
    }
}