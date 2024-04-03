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
                Matrix viewMatrix1 = playerView;
                if(m_camera1 == null) AddCamera(ref m_camera1, ref m_gameWidget1, viewMatrix1);
                m_camera1.SetViewMatrix(viewMatrix1);//拍原始传送门画面的相机用玩家的视角

                Matrix portal1Trans = m_portalElectricElement.GetPortal1Transform().OrientationMatrix * (m_portalElectricElement.GetPortal1Transform().TranslationMatrix * Matrix.CreateTranslation(m_position));
                Matrix portal2Trans = m_portalElectricElement.GetPortal2Transform().OrientationMatrix * (m_portalElectricElement.GetPortal2Transform().TranslationMatrix * Matrix.CreateTranslation(m_position));
                Matrix playerCamToPortal1 = playerView * portal1Trans.Invert();//传送门1为参考系，玩家视角的变换矩阵
                Matrix viewMatrix2 = playerCamToPortal1 * portal2Trans;
                if(m_camera2 == null) AddCamera(ref m_camera2, ref m_gameWidget2, viewMatrix2);
                m_camera2.SetViewMatrix(viewMatrix2);
            }
        }

        public void Draw(Camera camera, int drawOrder) {
            if (m_componentPlayer == null
                || m_camera1 == null
                || m_camera2 == null
                || m_portalElectricElement == null)
                return;
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