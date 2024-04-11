using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game {
    public class ComponentASMSimCameraLED : Component, IDrawable, IUpdateable, IASMGizmo {
        public int[] DrawOrders => [202, 1101];

        public UpdateOrder UpdateOrder => UpdateOrder.Default;

        public SubsystemASMElectricity m_subsystemAsmElectricity;

        public SubsystemGameWidgets m_subsystemGameWidgets;

        public ASMSimCameraLEDElectricElement? m_simCameraElectricElement;

        public SubsystemTerrain m_subsystemTerrain;

        public SubsystemDrawing m_subsystemDrawing;

        public SubsystemASMCamerasGameWidgets m_subsystemCameraGameWidgets;

        public SubsystemSky m_subsystemSky;

        public ComponentPlayer m_componentPlayer;

        public ComponentBody m_componentBody;

        public ASMComplexPerspectiveCamera m_camera;

        public PrimitivesRenderer3D m_primitivesRenderer = new PrimitivesRenderer3D();

        public Vector3 m_position;

        public GameWidget m_gameWidget;

        public Vector3 m_lastTranslation;

        public int m_face;

        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap) {
            base.Load(valuesDictionary, idToEntityMap);
            m_subsystemAsmElectricity = Project.FindSubsystem<SubsystemASMElectricity>(true);
            m_subsystemGameWidgets = Project.FindSubsystem<SubsystemGameWidgets>(true);
            m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
            m_subsystemDrawing = Project.FindSubsystem<SubsystemDrawing>(true);
            m_subsystemSky = Project.FindSubsystem<SubsystemSky>(true);
            m_subsystemCameraGameWidgets = base.Project.FindSubsystem<SubsystemASMCamerasGameWidgets>(true);
            Point3 coordinates = Entity.FindComponent<ComponentBlockEntity>(true).Coordinates;
            m_position = new Vector3(coordinates);
            int blockValue = m_subsystemTerrain.Terrain.GetCellValueFast(coordinates.X, coordinates.Y, coordinates.Z);
            m_face = ASMSimCameraLEDBlock.GetMountingFace(Terrain.ExtractData(blockValue));
            m_simCameraElectricElement = m_subsystemAsmElectricity.GetElectricElement(coordinates.X, coordinates.Y, coordinates.Z, m_face) as ASMSimCameraLEDElectricElement;
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
                || m_camera == null)
                return;
            if (!CheckVisible()) return;
            DrawScreen(camera, drawOrder);
        }

        public void DrawView() {
            m_camera.PrepareForDrawing();
            Vector3 translation = m_simCameraElectricElement.GetInputMatrix().Translation;
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
            Vector3 position = v - (0.435f * CellFace.FaceToVector3(m_face)); //0.435f原本是0.4375f，但是会导致闪烁，所以加一些偏移量
            Vector3 forward = CellFace.FaceToVector3(m_face);
            Vector3 up = (m_face < 4) ? Vector3.UnitY : Vector3.UnitX;
            Vector3 right = Vector3.Cross(up, forward);
            Vector3 p1 = position + (-right - up) / 2;
            Vector3 p2 = position + right / 2 - up / 2;
            Vector3 p3 = position + right / 2 + up / 2;
            Vector3 p4 = position - right / 2 + up / 2;
            if(!IsInPlayerFrustum(camera, p1, p2, p3, p4)) return;
            ASMStaticMethods.CalcUV(m_camera.ViewTexture.Width, m_camera.ViewTexture.Height, out Vector2 uvMin, out Vector2 uvMax);
            Vector2 uv0 = uvMin;
            Vector2 uv1 = new Vector2(uvMax.X, uvMin.Y);
            Vector2 uv2 = uvMax;
            Vector2 uv3 = new Vector2(uvMin.X, uvMax.Y);
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
                    p4,
                    p3,
                    p2,
                    p1,
                    uv0,
                    uv1,
                    uv2,
                    uv3,
                    Color.White
                );
            m_primitivesRenderer.Flush(camera.ViewProjectionMatrix);
        }

        public void Update(float dt) {
            if (m_componentPlayer == null) {
                m_componentPlayer = FindInteractingPlayer();
            }
            else {
                if (m_simCameraElectricElement == null) {
                    FindLed();
                    return;
                }
                if (m_camera == null) {
                    AddCamera();
                }
                Matrix viewMatrix = m_camera.ViewMatrix.Invert();
                Matrix inputMatrix = m_simCameraElectricElement.GetInputMatrix();
                Matrix lerpMatrix = ASMStaticMethods.Lerp(viewMatrix, inputMatrix, 0.5f);
                m_camera.SetViewMatrix(lerpMatrix);

                if (m_subsystemTerrain.TerrainRenderer.m_chunksToDraw.Count > 0) {
                    DrawView();
                }
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
            Point3 coordinates = Entity.FindComponent<ComponentBlockEntity>(true).Coordinates;
            int blockValue = m_subsystemTerrain.Terrain.GetCellValueFast(coordinates.X, coordinates.Y, coordinates.Z);
            m_face = ASMSimCameraLEDBlock.GetMountingFace(Terrain.ExtractData(blockValue));
            m_simCameraElectricElement = m_subsystemAsmElectricity.GetElectricElement(coordinates.X, coordinates.Y, coordinates.Z, m_face) as ASMSimCameraLEDElectricElement;
        }

        private bool CheckVisible() {
            if (m_componentPlayer == null) return false;
            Vector3 playerPos = m_componentPlayer.ComponentBody.Position;
            float length = (playerPos - m_position).Length() - 16;
            return length < m_subsystemTerrain.m_subsystemsky.VisibilityRange;
        }

        public bool IsInPlayerFrustum(Camera camera, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4) {
            BoundingFrustum frustum = camera.ViewFrustum;
            return frustum.Intersection(p1) || frustum.Intersection(p2) || frustum.Intersection(p3) || frustum.Intersection(p4);
        }

        public void AddCamera() {
            int index = new Random().Int(4, int.MaxValue);
            while (m_subsystemTerrain.TerrainUpdater.m_pendingLocations.ContainsKey(index)) {
                index = new Random().Int(4, int.MaxValue);
            }
            m_gameWidget = new GameWidget(new PlayerData(Project) { PlayerIndex = index }, 0);
            m_subsystemGameWidgets.m_gameWidgets.Add(m_gameWidget);
            m_subsystemCameraGameWidgets.m_gameWidgets.Add(m_gameWidget);
            m_camera = new ASMComplexPerspectiveCamera(m_gameWidget, m_componentPlayer.GameWidget.ActiveCamera.ProjectionMatrix);
            m_gameWidget.m_activeCamera = m_camera;
            m_gameWidget.m_cameras = [m_camera];

            m_subsystemTerrain.TerrainUpdater.SetUpdateLocation(m_gameWidget.PlayerData.PlayerIndex, m_simCameraElectricElement.GetInputMatrix().Translation.XZ, MathUtils.Min(m_subsystemTerrain.m_subsystemsky.VisibilityRange, 64f), 64f);
        }

        public void GizmosDraw(FlatBatch3D flatBatch) {
        }

        public void TopMostGizmosDraw(FlatBatch3D flatBatch) {
            if(m_camera == null) return;
            flatBatch.QueueLine(m_position + Vector3.One * 0.5f, m_camera.ViewPosition, Color.Pink, Color.Violet);
            flatBatch.QueueBoundingBox(new BoundingBox(m_camera.ViewPosition - Vector3.One * 0.2f, m_camera.ViewPosition + Vector3.One * 0.2f), Color.Yellow);
            flatBatch.QueueCoordinate(m_camera.ViewMatrix.Invert(), 0.4f);
        }
    }
}