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
                int rotation = 0;
                if (raycastResult.CellFace.Face >= 4) {
                    Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
                    float num = Vector3.Dot(forward, Vector3.UnitZ);
                    float num2 = Vector3.Dot(forward, Vector3.UnitX);
                    float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
                    float num4 = Vector3.Dot(forward, -Vector3.UnitX);
                    if (num == MathUtils.Max(num, num2, num3, num4)) {
                        rotation = 2;
                    }
                    else if (num2 == MathUtils.Max(num, num2, num3, num4)) {
                        rotation = 1;
                    }
                    else if (num3 == MathUtils.Max(num, num2, num3, num4)) {
                        rotation = 0;
                    }
                    else if (num4 == MathUtils.Max(num, num2, num3, num4)) {
                        rotation = 3;
                    }
                }
                int num5 = Terrain.ExtractData(value);
                num5 &= -29;
                num5 |= raycastResult.CellFace.Face << 2;
                result = default;
                result.Value = Terrain.MakeBlockValue(BlockIndex, 0, SetRotation(num5, rotation));
                result.CellFace = raycastResult.CellFace;
                return result;
            }
            componentMiner.ComponentPlayer?.ComponentGui.DisplaySmallMessage($"玩家{playerIndex}已经有对应的玩家摄像机设置器，位于({ASMPlayerCameraSetterManager.m_cameras[playerIndex].Item2})", Color.White, blinking: true, playNotificationSound: false);
            result = default;
            return result;
        }
    }
}