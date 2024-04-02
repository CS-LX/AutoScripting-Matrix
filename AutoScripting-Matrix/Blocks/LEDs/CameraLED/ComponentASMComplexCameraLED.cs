using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game {
    public class ComponentASMComplexCameraLED : Component, IDrawable, IUpdateable {
        public int[] DrawOrders => [202, 1101];

        public UpdateOrder UpdateOrder => UpdateOrder.Default;

        public SubsystemASMElectricity m_subsystemAsmElectricity;

        public SubsystemGameWidgets m_subsystemGameWidgets;

        public ASMComplexCameraLEDElectricElement? m_complexCameraElectricElement;

        public SubsystemTerrain m_subsystemTerrain;

        public SubsystemDrawing m_subsystemDrawing;

        public SubsystemSky m_subsystemSky;

        public SubsystemASMCamerasGameWidgets m_subsystemCameraGameWidgets;

        public ComponentPlayer m_componentPlayer;

        public ComponentBody m_componentBody;

        public ASMComplexPerspectiveCamera m_camera;

        public ComponentBlockEntity m_componentEntity;

        public PrimitivesRenderer3D m_primitivesRenderer = new PrimitivesRenderer3D();

        public Vector3 m_position;

        public GameWidget m_gameWidget;

        public Vector3 m_lastTranslation;

        public int m_face;

        //控制部分
        public bool m_isAutoClip;

        public int m_screenUVSubdivision;//屏幕uv细分，小于等于0，则不用屏幕纹理。大于0，则用屏幕纹理

        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap) {
            base.Load(valuesDictionary, idToEntityMap);
            m_subsystemAsmElectricity = Project.FindSubsystem<SubsystemASMElectricity>(true);
            m_subsystemGameWidgets = Project.FindSubsystem<SubsystemGameWidgets>(true);
            m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
            m_subsystemDrawing = Project.FindSubsystem<SubsystemDrawing>(true);
            m_subsystemSky = Project.FindSubsystem<SubsystemSky>(true);
            m_subsystemCameraGameWidgets = base.Project.FindSubsystem<SubsystemASMCamerasGameWidgets>(true);
            m_componentEntity = Entity.FindComponent<ComponentBlockEntity>(true);
            Point3 coordinates = Entity.FindComponent<ComponentBlockEntity>(true).Coordinates;
            m_position = new Vector3(coordinates);
            int blockValue = m_subsystemTerrain.Terrain.GetCellValueFast(coordinates.X, coordinates.Y, coordinates.Z);
            m_face = ASMSimCameraLEDBlock.GetMountingFace(Terrain.ExtractData(blockValue));
            m_complexCameraElectricElement = m_subsystemAsmElectricity.GetElectricElement(coordinates.X, coordinates.Y, coordinates.Z, m_face) as ASMComplexCameraLEDElectricElement;
        }

        public override void OnEntityRemoved() {
            base.OnEntityRemoved();
            m_subsystemTerrain.TerrainUpdater.RemoveUpdateLocation(m_gameWidget.PlayerData.PlayerIndex);
            m_subsystemGameWidgets.m_gameWidgets.Remove(m_gameWidget);
            m_subsystemCameraGameWidgets.m_gameWidgets.Remove(m_gameWidget);
            m_gameWidget.Dispose();
            m_gameWidget = null;
            Utilities.Dispose(ref m_camera.ViewTexture);
            m_camera = null;
        }

        public void Draw(Camera camera, int drawOrder) {
            if (m_componentPlayer == null
                || m_camera == null
                || m_complexCameraElectricElement == null)
                return;
            if (!CheckVisible()) return;
            DrawScreen(camera, drawOrder);
        }

        public void DrawView() {
            m_camera.PrepareForDrawing();
            m_camera.m_projectionMatrix = m_complexCameraElectricElement.GetProjectionMatrix();
            Vector3 translation = m_complexCameraElectricElement.GetInputMatrix().Translation;
            if ((m_lastTranslation.XZ - translation.XZ).Length() > 16) {
                m_lastTranslation = translation;
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


        public void DrawScreen(Camera camera, int drawOrder) {
            //显示的面的单位向量，满足笛卡尔坐标系，right是x轴，up是y轴
            Vector3 v = new(m_position.X + 0.5f, m_position.Y + 0.5f, m_position.Z + 0.5f);
            if (m_screenUVSubdivision <= 0) {
                Vector3 offset1 = new Vector3(-0.5f, 0, -0.5f);
                Vector3 offset2 = new Vector3(0.5f, 0, -0.5f);
                Vector3 offset3 = new Vector3(0.5f, 0, 0.5f);
                Vector3 offset4 = new Vector3(-0.5f, 0, 0.5f);
                Vector3 p1 = v + Vector3.Transform(offset1, m_complexCameraElectricElement.GetDisplayTransformMatrix());
                Vector3 p2 = v + Vector3.Transform(offset2, m_complexCameraElectricElement.GetDisplayTransformMatrix());
                Vector3 p3 = v + Vector3.Transform(offset3, m_complexCameraElectricElement.GetDisplayTransformMatrix());
                Vector3 p4 = v + Vector3.Transform(offset4, m_complexCameraElectricElement.GetDisplayTransformMatrix());
                ASMStaticMethods.CalcUV(m_camera.ViewTexture.Width, m_camera.ViewTexture.Height, out Vector2 uvMin, out Vector2 uvMax);
                Vector2 uv0 = m_isAutoClip ? uvMin : Vector2.Zero;
                Vector2 uv1 = m_isAutoClip ? new Vector2(uvMax.X, uvMin.Y) : Vector2.UnitX;
                Vector2 uv2 = m_isAutoClip ? uvMax : Vector2.One;
                Vector2 uv3 = m_isAutoClip ? new Vector2(uvMin.X, uvMax.Y) : Vector2.UnitY;
                m_primitivesRenderer.TexturedBatch(
                        m_camera.ViewTexture,
                        useAlphaTest: true,
                        0,
                        null,
                        RasterizerState.CullCounterClockwiseScissor,
                        null,
                        SamplerState.PointClamp
                    )
                    .QueueQuad(
                        p1,
                        p2,
                        p3,
                        p4,
                        uv0,
                        uv1,
                        uv2,
                        uv3,
                        Color.White
                    );
            }
            else {
                for (int i = 0; i < m_screenUVSubdivision; i++) {
                    for (int j = 0; j < m_screenUVSubdivision; j++) {
                        float length = 1 / (float)m_screenUVSubdivision;
                        Vector3 wp1 = v + Vector3.Transform(new Vector3(length * i - 0.5f, 0, length * j - 0.5f), m_complexCameraElectricElement.GetDisplayTransformMatrix());
                        Vector3 wp2 = v + Vector3.Transform(new Vector3(length * (i + 1) - 0.5f, 0, length * j - 0.5f), m_complexCameraElectricElement.GetDisplayTransformMatrix());
                        Vector3 wp3 = v + Vector3.Transform(new Vector3(length * (i + 1) - 0.5f, 0, length * (j + 1) - 0.5f), m_complexCameraElectricElement.GetDisplayTransformMatrix());
                        Vector3 wp4 = v + Vector3.Transform(new Vector3(length * i - 0.5f, 0, length * (j + 1) - 0.5f), m_complexCameraElectricElement.GetDisplayTransformMatrix());
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
                                m_camera.ViewTexture,
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
            m_primitivesRenderer.Flush(camera.ViewProjectionMatrix);
        }

        public void Update(float dt) {
            if (m_componentPlayer == null) {
                m_componentPlayer = FindInteractingPlayer();
            }
            else {
                Point3 coordinates = m_componentEntity.Coordinates;
                int blockValue = m_subsystemTerrain.Terrain.GetCellValueFast(coordinates.X, coordinates.Y, coordinates.Z);
                m_face = ASMComplexCameraLEDBlock.GetFace_Static(blockValue);
                if (m_complexCameraElectricElement == null
                    || ASMComplexCameraLEDBlock.GetRotation(Terrain.ExtractData(m_complexCameraElectricElement.m_blockValue)) != ASMComplexCameraLEDBlock.GetRotation(Terrain.ExtractData(blockValue))) {
                    FindLed();
                    return;
                }
                if (m_camera == null) {
                    AddCamera();
                }
                Matrix viewMatrix = m_camera.ViewMatrix.Invert();
                Matrix inputMatrix = m_complexCameraElectricElement.GetInputMatrix();
                Matrix lerpMatrix = ASMStaticMethods.Lerp(viewMatrix, inputMatrix, 0.5f);
                m_camera.SetViewMatrix(lerpMatrix);
                Matrix projectionMatrix = m_complexCameraElectricElement.GetProjectionMatrix();
                m_camera.m_projectionMatrix = projectionMatrix;
                if (m_subsystemTerrain.TerrainRenderer.m_chunksToDraw.Count > 0) {
                    DrawView();
                }

                //控制部分
                m_isAutoClip = m_complexCameraElectricElement.GetControlMatrix().M11 > 0;
                m_screenUVSubdivision = (int)MathUtils.Floor(m_complexCameraElectricElement.GetControlMatrix().M12);
            }
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

        public void FindLed() {
            Point3 coordinates = m_componentEntity.Coordinates;
            int blockValue = m_subsystemTerrain.Terrain.GetCellValueFast(coordinates.X, coordinates.Y, coordinates.Z);
            m_face = ASMComplexCameraLEDBlock.GetFace_Static(blockValue);
            m_complexCameraElectricElement = m_subsystemAsmElectricity.GetElectricElement(coordinates.X, coordinates.Y, coordinates.Z, m_face) as ASMComplexCameraLEDElectricElement;
        }

        public void AddCamera() {
            int index = new Random().Int(4, int.MaxValue);
            while (m_subsystemTerrain.TerrainUpdater.m_pendingLocations.ContainsKey(index)) {
                index = new Random().Int(4, int.MaxValue);
            }
            m_gameWidget = new GameWidget(new PlayerData(Project) { PlayerIndex = index }, 0);
            m_subsystemGameWidgets.m_gameWidgets.Add(m_gameWidget);
            m_subsystemCameraGameWidgets.m_gameWidgets.Add(m_gameWidget);
            m_camera = new ASMComplexPerspectiveCamera(m_gameWidget, m_complexCameraElectricElement.GetProjectionMatrix());
            m_gameWidget.m_activeCamera = m_camera;
            m_gameWidget.m_cameras = [m_camera];

            m_subsystemTerrain.TerrainUpdater.SetUpdateLocation(m_gameWidget.PlayerData.PlayerIndex, m_complexCameraElectricElement.GetInputMatrix().Translation.XZ, MathUtils.Min(m_subsystemTerrain.m_subsystemsky.VisibilityRange, 64f), 64f);
        }

        private bool CheckVisible() {
            if (m_componentPlayer == null) return false;
            Vector3 playerPos = m_componentPlayer.ComponentBody.Position;
            float length = (playerPos - m_position).Length() - 16;
            return length < m_subsystemTerrain.m_subsystemsky.VisibilityRange;
        }
    }
}