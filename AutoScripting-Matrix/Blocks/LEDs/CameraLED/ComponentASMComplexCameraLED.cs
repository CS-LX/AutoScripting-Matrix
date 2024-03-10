using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game {
    public class ComponentASMComplexCameraLED : Component, IDrawable, IUpdateable {
        public int[] DrawOrders => [202, 1101];

        public UpdateOrder UpdateOrder => UpdateOrder.Default;

        public SubsystemASMElectricity m_subsystemAsmElectricity;

        public SubsystemPlayers m_subsystemPlayers;

        public SubsystemGameWidgets m_subsystemGameWidgets;

        public ASMComplexCameraLEDElectricElement? m_complexCameraElectricElement;

        public SubsystemTime m_subsystemTime;

        public SubsystemTerrain m_subsystemTerrain;

        public SubsystemModelsRenderer m_subsystemModelsRenderer;

        public SubsystemGameInfo m_subsystemGameInfo;

        public ComponentPlayer m_componentPlayer;

        public ComponentBody m_componentBody;

        public ASMComplexPerspectiveCamera m_camera;

        public ComponentBlockEntity m_componentEntity;

        public PrimitivesRenderer3D m_primitivesRenderer = new PrimitivesRenderer3D();

        public Vector3 m_position;

        public int m_face;

        //控制部分
        public bool m_isAutoClip;

        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap) {
            base.Load(valuesDictionary, idToEntityMap);
            m_subsystemAsmElectricity = Project.FindSubsystem<SubsystemASMElectricity>(true);
            m_subsystemGameWidgets = Project.FindSubsystem<SubsystemGameWidgets>(true);
            m_subsystemPlayers = Project.FindSubsystem<SubsystemPlayers>(true);
            m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
            m_subsystemTerrain = base.Project.FindSubsystem<SubsystemTerrain>(true);
            m_subsystemModelsRenderer = base.Project.FindSubsystem<SubsystemModelsRenderer>(true);
            m_subsystemGameInfo = base.Project.FindSubsystem<SubsystemGameInfo>(true);
            m_componentEntity = Entity.FindComponent<ComponentBlockEntity>(true);
            Point3 coordinates = Entity.FindComponent<ComponentBlockEntity>(true).Coordinates;
            m_position = new Vector3(coordinates);
            int blockValue = m_subsystemTerrain.Terrain.GetCellValueFast(coordinates.X, coordinates.Y, coordinates.Z);
            m_face = ASMSimCameraLEDBlock.GetMountingFace(Terrain.ExtractData(blockValue));
            m_complexCameraElectricElement = m_subsystemAsmElectricity.GetElectricElement(coordinates.X, coordinates.Y, coordinates.Z, m_face) as ASMComplexCameraLEDElectricElement;
        }

        public void Draw(Camera camera, int drawOrder) {
            if (m_componentPlayer == null
                || m_camera == null
                || m_complexCameraElectricElement == null)
                return;
            DrawView(camera, drawOrder);
            DrawScreen(camera, drawOrder);
        }

        public void DrawView(Camera camera, int drawOrder) {
            if (drawOrder == this.DrawOrders[0]) {
                if (this.m_componentPlayer.GameWidget != camera.GameWidget) {
                    return;
                }
                RenderTarget2D renderTarget = Display.RenderTarget;
                try {
                    float num = camera.GameWidget.ViewWidget.GlobalTransform.Right.Length();
                    float num2 = camera.GameWidget.ViewWidget.GlobalTransform.Up.Length();
                    Vector2 vector = new Vector2(camera.GameWidget.ViewWidget.ActualSize.X * num, camera.GameWidget.ViewWidget.ActualSize.Y * num2);
                    Point2 point = new Point2 { X = (int)MathUtils.Round(vector.X), Y = (int)MathUtils.Round(vector.Y) };
                    if (m_camera.ViewTexture.Width != point.X
                        || m_camera.ViewTexture.Height != point.Y) {
                        Utilities.Dispose<RenderTarget2D>(ref m_camera.ViewTexture);
                        m_camera.ViewTexture = new RenderTarget2D(
                            point.X,
                            point.Y,
                            1,
                            ColorFormat.Rgba8888,
                            DepthFormat.Depth24Stencil8
                        );
                    }
                    Display.RenderTarget = m_camera.ViewTexture;
                    this.m_camera.PrepareForDrawing();
                    Display.Clear(new Color?(Color.Black), new float?(1f), null);
                    this.m_subsystemTerrain.TerrainRenderer.PrepareForDrawing(this.m_camera);
                    this.DrawOpaque(this.m_camera);
                    this.DrawAlphaTested(this.m_camera);
                    this.DrawTransparent(this.m_camera);
                    if (!SubsystemModelsRenderer.DisableDrawingModels) {
                        this.PrepareModels(this.m_camera);
                        this.DrawModels(this.m_camera, this.m_subsystemModelsRenderer.m_modelsToDraw);
                    }
                }
                catch (Exception ex) {
                    Log.Error(ex.ToString());
                    return;
                }
                finally {
                    Display.RenderTarget = renderTarget;
                }
            }
        }


        public void DrawScreen(Camera camera, int drawOrder) {
            //显示的面的单位向量，满足笛卡尔坐标系，right是x轴，up是y轴
            Vector3 v = new(m_position.X + 0.5f, m_position.Y + 0.5f, m_position.Z + 0.5f);
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
            m_primitivesRenderer.Flush(camera.ViewProjectionMatrix);
        }

        public void Update(float dt) {
            if (m_componentPlayer == null
                || m_camera == null) {
                m_componentPlayer = FindInteractingPlayer();
                m_camera = new ASMComplexPerspectiveCamera(m_componentPlayer.GameWidget, Matrix.Zero);
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
                Matrix viewMatrix = m_camera.ViewMatrix.Invert();
                Matrix inputMatrix = m_complexCameraElectricElement.GetInputMatrix();
                Matrix lerpMatrix = ASMStaticMethods.Lerp(viewMatrix, inputMatrix, 0.5f);
                m_camera.SetViewMatrix(lerpMatrix);
                Matrix projectionMatrix = m_complexCameraElectricElement.GetProjectionMatrix();
                m_camera.m_projectionMatrix = projectionMatrix;
                m_isAutoClip = m_complexCameraElectricElement.GetControlMatrix().M11 > 0;
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

        #region 绘制方法

        public void DrawOpaque(Camera camera) {
            int gameWidgetIndex = camera.GameWidget.GameWidgetIndex;
            Vector3 viewPosition = camera.ViewPosition;
            Vector3 v = new Vector3(MathUtils.Floor(viewPosition.X), 0f, MathUtils.Floor(viewPosition.Z));
            Matrix value = Matrix.CreateTranslation(v - viewPosition) * camera.ViewMatrix.OrientationMatrix * camera.ProjectionMatrix;
            Display.BlendState = BlendState.Opaque;
            Display.DepthStencilState = DepthStencilState.Default;
            Display.RasterizerState = RasterizerState.CullCounterClockwiseScissor;
            TerrainRenderer.OpaqueShader.GetParameter("u_origin", false).SetValue(v.XZ);
            TerrainRenderer.OpaqueShader.GetParameter("u_viewProjectionMatrix", false).SetValue(value);
            TerrainRenderer.OpaqueShader.GetParameter("u_viewPosition", false).SetValue(viewPosition);
            TerrainRenderer.OpaqueShader.GetParameter("u_samplerState", false).SetValue(SettingsManager.TerrainMipmapsEnabled ? this.m_subsystemTerrain.TerrainRenderer.m_samplerStateMips : this.m_subsystemTerrain.TerrainRenderer.m_samplerState);
            TerrainRenderer.OpaqueShader.GetParameter("u_fogYMultiplier", false).SetValue(this.m_subsystemTerrain.TerrainRenderer.m_subsystemSky.VisibilityRangeYMultiplier);
            TerrainRenderer.OpaqueShader.GetParameter("u_fogColor", false).SetValue(new Vector3(Color.Black));
            ShaderParameter parameter = TerrainRenderer.OpaqueShader.GetParameter("u_fogStartInvLength", false);
            for (int i = 0; i < this.m_subsystemTerrain.TerrainRenderer.m_chunksToDraw.Count; i++) {
                TerrainChunk terrainChunk = this.m_subsystemTerrain.TerrainRenderer.m_chunksToDraw[i];
                float num = MathUtils.Min(terrainChunk.FogEnds[gameWidgetIndex], this.m_subsystemTerrain.TerrainRenderer.m_subsystemSky.ViewFogRange.Y);
                float num2 = MathUtils.Min(this.m_subsystemTerrain.TerrainRenderer.m_subsystemSky.ViewFogRange.X, num - 1f);
                if (true) {
                    num = MathUtils.Max(camera.ViewPosition.Y, num);
                }
                parameter.SetValue(new Vector2(num2, 1f / (num - num2)));
                int num3 = 16;
                if (viewPosition.Z > terrainChunk.BoundingBox.Min.Z) {
                    num3 |= 1;
                }
                if (viewPosition.X > terrainChunk.BoundingBox.Min.X) {
                    num3 |= 2;
                }
                if (viewPosition.Z < terrainChunk.BoundingBox.Max.Z) {
                    num3 |= 4;
                }
                if (viewPosition.X < terrainChunk.BoundingBox.Max.X) {
                    num3 |= 8;
                }
                this.m_subsystemTerrain.TerrainRenderer.DrawTerrainChunkGeometrySubsets(TerrainRenderer.OpaqueShader, terrainChunk.Geometry, num3, true);
            }
        }

        public void DrawAlphaTested(Camera camera) {
            int gameWidgetIndex = camera.GameWidget.GameWidgetIndex;
            Vector3 viewPosition = camera.ViewPosition;
            Vector3 v = new Vector3(MathUtils.Floor(viewPosition.X), 0f, MathUtils.Floor(viewPosition.Z));
            Matrix value = Matrix.CreateTranslation(v - viewPosition) * camera.ViewMatrix.OrientationMatrix * camera.ProjectionMatrix;
            Display.BlendState = BlendState.Opaque;
            Display.DepthStencilState = DepthStencilState.Default;
            Display.RasterizerState = RasterizerState.CullCounterClockwiseScissor;
            TerrainRenderer.AlphatestedShader.GetParameter("u_origin", false).SetValue(v.XZ);
            TerrainRenderer.AlphatestedShader.GetParameter("u_viewProjectionMatrix", false).SetValue(value);
            TerrainRenderer.AlphatestedShader.GetParameter("u_viewPosition", false).SetValue(viewPosition);
            TerrainRenderer.AlphatestedShader.GetParameter("u_samplerState", false).SetValue(SettingsManager.TerrainMipmapsEnabled ? this.m_subsystemTerrain.TerrainRenderer.m_samplerStateMips : this.m_subsystemTerrain.TerrainRenderer.m_samplerState);
            TerrainRenderer.AlphatestedShader.GetParameter("u_fogYMultiplier", false).SetValue(this.m_subsystemTerrain.TerrainRenderer.m_subsystemSky.VisibilityRangeYMultiplier);
            TerrainRenderer.AlphatestedShader.GetParameter("u_fogColor", false).SetValue(new Vector3(Color.Black));
            ShaderParameter parameter = TerrainRenderer.AlphatestedShader.GetParameter("u_fogStartInvLength", false);
            for (int i = 0; i < this.m_subsystemTerrain.TerrainRenderer.m_chunksToDraw.Count; i++) {
                TerrainChunk terrainChunk = this.m_subsystemTerrain.TerrainRenderer.m_chunksToDraw[i];
                float num = MathUtils.Min(terrainChunk.FogEnds[gameWidgetIndex], this.m_subsystemTerrain.TerrainRenderer.m_subsystemSky.ViewFogRange.Y);
                float num2 = MathUtils.Min(this.m_subsystemTerrain.TerrainRenderer.m_subsystemSky.ViewFogRange.X, num - 1f);
                if (true) {
                    num = MathUtils.Max(camera.ViewPosition.Y, num);
                }
                parameter.SetValue(new Vector2(num2, 1f / (num - num2)));
                int subsetsMask = 32;
                this.m_subsystemTerrain.TerrainRenderer.DrawTerrainChunkGeometrySubsets(TerrainRenderer.AlphatestedShader, terrainChunk.Geometry, subsetsMask, true);
            }
        }

        public void DrawTransparent(Camera camera) {
            int gameWidgetIndex = camera.GameWidget.GameWidgetIndex;
            Vector3 viewPosition = camera.ViewPosition;
            Vector3 v = new Vector3(MathUtils.Floor(viewPosition.X), 0f, MathUtils.Floor(viewPosition.Z));
            Matrix value = Matrix.CreateTranslation(v - viewPosition) * camera.ViewMatrix.OrientationMatrix * camera.ProjectionMatrix;
            Display.BlendState = BlendState.AlphaBlend;
            Display.DepthStencilState = DepthStencilState.Default;
            Display.RasterizerState = RasterizerState.CullCounterClockwiseScissor;
            TerrainRenderer.TransparentShader.GetParameter("u_origin", false).SetValue(v.XZ);
            TerrainRenderer.TransparentShader.GetParameter("u_viewProjectionMatrix", false).SetValue(value);
            TerrainRenderer.TransparentShader.GetParameter("u_viewPosition", false).SetValue(viewPosition);
            TerrainRenderer.TransparentShader.GetParameter("u_samplerState", false).SetValue(SettingsManager.TerrainMipmapsEnabled ? this.m_subsystemTerrain.TerrainRenderer.m_samplerStateMips : this.m_subsystemTerrain.TerrainRenderer.m_samplerState);
            TerrainRenderer.TransparentShader.GetParameter("u_fogYMultiplier", false).SetValue(this.m_subsystemTerrain.TerrainRenderer.m_subsystemSky.VisibilityRangeYMultiplier);
            TerrainRenderer.TransparentShader.GetParameter("u_fogColor", false).SetValue(new Vector3(Color.Black));
            ShaderParameter parameter = TerrainRenderer.TransparentShader.GetParameter("u_fogStartInvLength", false);
            for (int i = 0; i < this.m_subsystemTerrain.TerrainRenderer.m_chunksToDraw.Count; i++) {
                TerrainChunk terrainChunk = this.m_subsystemTerrain.TerrainRenderer.m_chunksToDraw[i];
                float num = MathUtils.Min(terrainChunk.FogEnds[gameWidgetIndex], this.m_subsystemTerrain.TerrainRenderer.m_subsystemSky.ViewFogRange.Y);
                float num2 = MathUtils.Min(this.m_subsystemTerrain.TerrainRenderer.m_subsystemSky.ViewFogRange.X, num - 1f);
                if (true) {
                    num = MathUtils.Max(camera.ViewPosition.Y, num);
                }
                parameter.SetValue(new Vector2(num2, 1f / (num - num2)));
                int subsetsMask = 64;
                this.m_subsystemTerrain.TerrainRenderer.DrawTerrainChunkGeometrySubsets(TerrainRenderer.TransparentShader, terrainChunk.Geometry, subsetsMask, true);
            }
        }

        public void PrepareModels(Camera camera) {
            for (int i = 0; i < this.m_subsystemModelsRenderer.m_modelsToDraw.Length; i++) {
                this.m_subsystemModelsRenderer.m_modelsToDraw[i].Clear();
            }
            this.m_subsystemModelsRenderer.m_modelsToPrepare.Clear();
            foreach (SubsystemModelsRenderer.ModelData modelData in this.m_subsystemModelsRenderer.m_componentModels.Values) {
                if (modelData.ComponentModel.Model != null) {
                    if (modelData.ComponentModel.Entity == camera.GameWidget.PlayerData.ComponentPlayer.Entity) {
                        modelData.ComponentModel.IsVisibleForCamera = true;
                    }
                    else {
                        modelData.ComponentModel.CalculateIsVisible(camera);
                    }
                    if (modelData.ComponentModel.IsVisibleForCamera) {
                        this.m_subsystemModelsRenderer.m_modelsToPrepare.Add(modelData);
                    }
                }
            }
            this.m_subsystemModelsRenderer.m_modelsToPrepare.Sort();
            foreach (SubsystemModelsRenderer.ModelData modelData2 in this.m_subsystemModelsRenderer.m_modelsToPrepare) {
                this.m_subsystemModelsRenderer.PrepareModel(modelData2, camera);
                this.m_subsystemModelsRenderer.m_modelsToDraw[(int)modelData2.ComponentModel.RenderingMode].Add(modelData2);
            }
        }

        public void DrawModels(Camera camera, List<SubsystemModelsRenderer.ModelData>[] modelsData) {
            Display.DepthStencilState = DepthStencilState.Default;
            Display.RasterizerState = RasterizerState.CullCounterClockwiseScissor;
            Display.BlendState = BlendState.Opaque;
            this.m_subsystemModelsRenderer.DrawInstancedModels(camera, modelsData[0], null);
            Display.RasterizerState = RasterizerState.CullNoneScissor;
            this.m_subsystemModelsRenderer.DrawInstancedModels(camera, modelsData[1], new float?(0f));
            Display.RasterizerState = RasterizerState.CullCounterClockwiseScissor;
            this.m_subsystemModelsRenderer.m_primitivesRenderer.Flush(camera.ProjectionMatrix, true, 0);
        }

        #endregion
    }
}