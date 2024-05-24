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
        public ASMPortalPrimitivesRenderere3D m_primitivesRenderer = new ASMPortalPrimitivesRenderere3D();

        public ASMPortal m_portal1;

        public ASMPortal m_portal2;

        public Vector3 m_cam1Translation;

        public Vector3 m_cam2Translation;

        public int m_screenUVSubdivision = 1;

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
            m_portal1.Dispose();
            m_portal2.Dispose();
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

                if (m_portal1 == null) {
                    Matrix playerCamToPortal2 = playerView * portal2Trans.Invert();
                    Matrix viewMatrix1 = playerCamToPortal2 * portal1Trans;

                    ASMComplexPerspectiveCamera camera1 = null;
                    GameWidget gameWidget1 = null;
                    AddCamera(ref camera1, ref gameWidget1, viewMatrix1);
                    m_portal1 = new ASMPortal(
                        camera1,
                        gameWidget1,
                        portal1Trans,
                        m_screenUVSubdivision,
                        Project,
                        this,
                        m_componentPlayer
                    );
                }

                if (m_portal2 == null) {
                    Matrix playerCamToPortal1 = playerView * portal1Trans.Invert(); //传送门1为参考系，玩家视角的变换矩阵
                    Matrix viewMatrix2 = playerCamToPortal1 * portal2Trans;

                    ASMComplexPerspectiveCamera camera2 = null;
                    GameWidget gameWidget2 = null;
                    AddCamera(ref camera2, ref gameWidget2, viewMatrix2);
                    m_portal2 = new ASMPortal(
                        camera2,
                        gameWidget2,
                        portal2Trans,
                        m_screenUVSubdivision,
                        Project,
                        this,
                        m_componentPlayer
                    );
                }



                UpdateControl();

                m_portal1.LinkPortal(m_portal2);
                m_portal1.SetTransformMatrix(portal1Trans);
                m_portal1.Update(dt);

                m_portal2.LinkPortal(m_portal1);
                m_portal2.SetTransformMatrix(portal2Trans);
                m_portal2.Update(dt);

                if (m_subsystemTerrain.TerrainRenderer.m_chunksToDraw.Count > 0) {
                    m_portal1.DrawView(m_primitivesRenderer);
                    m_portal2.DrawView(m_primitivesRenderer);
                }
            }
        }

        public void Draw(Camera camera, int drawOrder) {
            if (m_componentPlayer == null
                || m_portal1 == null
                || m_portal2 == null
                || m_portalElectricElement == null)
                return;
            m_portal1.DrawPortal(m_primitivesRenderer, camera);
            m_portal2.DrawPortal(m_primitivesRenderer, camera);
            m_primitivesRenderer.Flush(camera.ViewProjectionMatrix);
        }

        public void UpdateControl() {
            int M11_1 = (int)MathUtils.Floor(m_portalElectricElement.GetControl1().M11);
            int M11_2 = (int)MathUtils.Floor(m_portalElectricElement.GetControl2().M11);
            switch (M11_1) {
                case 0:
                    m_portal1.m_visible = false;
                    m_portal1.m_teleportEnable = false;
                    break;
                case 1:
                    m_portal1.m_visible = m_portalElectricElement.GetPortal1Transform() != Matrix.Zero;
                    m_portal1.m_teleportEnable = true;
                    break;
                case 2:
                    m_portal1.m_visible = m_portalElectricElement.GetPortal1Transform() != Matrix.Zero;
                    m_portal1.m_teleportEnable = false;
                    break;
                case 3:
                    m_portal1.m_visible = false;
                    m_portal1.m_teleportEnable = true;
                    break;
            }
            switch (M11_2) {
                case 0:
                    m_portal2.m_visible = false;
                    m_portal2.m_teleportEnable = false;
                    break;
                case 1:
                    m_portal2.m_visible = m_portalElectricElement.GetPortal2Transform() != Matrix.Zero;
                    m_portal2.m_teleportEnable = true;
                    break;
                case 2:
                    m_portal2.m_visible = m_portalElectricElement.GetPortal2Transform() != Matrix.Zero;
                    m_portal2.m_teleportEnable = false;
                    break;
                case 3:
                    m_portal2.m_visible = false;
                    m_portal2.m_teleportEnable = true;
                    break;
            }

            m_portal1.m_autoAntiClipping = m_portalElectricElement.GetControl1().M12 > 0;
            m_portal2.m_autoAntiClipping = m_portalElectricElement.GetControl2().M12 > 0;
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
    }
}