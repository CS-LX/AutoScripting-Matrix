using Engine;
using Engine.Graphics;

namespace Game {
    public class ASMPlayerCameraSetter : ASMRotateableMountedElectricElementBlock {

        public const int Index = 624;

        public Texture2D m_texture;

        public ASMPlayerCameraSetter() : base("Models/ASMDecomposer", "Decomposer", 0.5f) { }

        public override void Initialize() {
            base.Initialize();
            m_texture = ContentManager.Get<Texture2D>("Textures/ASMPlayerCameraSetter");
        }

        public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value) => "玩家摄像机设置器";

        public override string GetDescription(int value) => "";

        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData) {
            BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, m_texture, color, 2f * size, ref matrix, environmentData);
        }

        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
        {
            int num = Terrain.ExtractData(value) & 0x1F;
            generator.GenerateMeshVertices(this, x, y, z, m_blockMeshes[num], Color.White, null, geometry.GetGeometry(m_texture).SubsetOpaque);
            GenerateASMWireVertices(generator, value, x, y, z, GetFace(value), m_centerBoxSize, Vector2.Zero, geometry.SubsetOpaque);
        }

        public override ASMElectricElement CreateElectricElement(SubsystemASMElectricity subsystemASMElectricity, int value, int x, int y, int z) => new ASMPlayerCameraSetterElectricElement(subsystemASMElectricity, new CellFace(x, y, z, GetFace(value)));

        public override ASMElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z) {
            int face2 = GetFace(value);
            int data = Terrain.ExtractData(value);
            if (face == face2) {
                ASMElectricConnectorDirection? connectorDirection = SubsystemASMElectricity.GetConnectorDirection(GetFace(value), GetRotation(data), connectorFace);
                if (connectorDirection == ASMElectricConnectorDirection.Right || connectorDirection == ASMElectricConnectorDirection.Left)
                {
                    return ASMElectricConnectorType.Input;
                }
            }
            return null;
        }

        public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult) {
            BlockPlacementData result;
            int playerIndex = componentMiner.ComponentPlayer.PlayerData.PlayerIndex;
            if (ASMPlayerCameraSetterManager.m_cameras[playerIndex].Item3 == null) {
                return GeneratePlacementData(subsystemTerrain, componentMiner, value, raycastResult);
            }
            componentMiner.ComponentPlayer?.ComponentGui.DisplaySmallMessage($"玩家{playerIndex}已经有对应的玩家摄像机设置器，位于({ASMPlayerCameraSetterManager.m_cameras[playerIndex].Item2})", Color.White, blinking: true, playNotificationSound: false);
            result = default;
            return result;
        }
    }
}