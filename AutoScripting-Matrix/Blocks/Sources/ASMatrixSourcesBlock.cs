using Engine;
using Engine.Graphics;
using System;

namespace Game
{
    public class ASMatrixSourcesBlock : ASMRotateableMountedElectricElementBlock
    {
        public const int Index = 608;

        public readonly MatrixSourceInfo[] Infos = [
            new MatrixSourceInfo("矩阵源: 世界方块坐标", "", "ASMatrixSourceWorldPosition", [], [ASMElectricConnectorDirection.Top]),
            new MatrixSourceInfo("矩阵源: 玩家变换", "", "ASMatrixSourcePlayerTransform", [ASMElectricConnectorDirection.Bottom], [ASMElectricConnectorDirection.Top]),
            new MatrixSourceInfo("矩阵源: 玩家摄像机视图", "输出最近玩家的摄像机视图矩阵，包含观测的位置，方向，垂直Y方向 \n(注意: 若要直接使用此矩阵作为绘制的变换，须对其求逆)", "ASMatrixSourcePlayerCamera", [ASMElectricConnectorDirection.Bottom], [ASMElectricConnectorDirection.Top]),
            new MatrixSourceInfo("矩阵源: 玩家摄像机投影", "输出最近玩家的摄像机投影矩阵(左)、屏幕投影矩阵(右)", "ASMatrixSourcePlayerCameraProjection", [], [ASMElectricConnectorDirection.Left, ASMElectricConnectorDirection.Right]),
            new MatrixSourceInfo("矩阵源: 从位移创建", "", "ASMatrixSourceFromTranslation", [ASMElectricConnectorDirection.Left, ASMElectricConnectorDirection.Bottom, ASMElectricConnectorDirection.Right], [ASMElectricConnectorDirection.Top]),
            new MatrixSourceInfo("矩阵源: 从欧拉角(三轴旋转)创建", "", "ASMatrixSourceFromYPR", [ASMElectricConnectorDirection.Left, ASMElectricConnectorDirection.Bottom, ASMElectricConnectorDirection.Right], [ASMElectricConnectorDirection.Top]),
            new MatrixSourceInfo("矩阵源: 从缩放创建", "", "ASMatrixSourceFromScale", [ASMElectricConnectorDirection.Bottom], [ASMElectricConnectorDirection.Top]),
            new MatrixSourceInfo("矩阵源: 从三轴缩放创建", "", "ASMatrixSourceFromScaleXYZ", [ASMElectricConnectorDirection.Left, ASMElectricConnectorDirection.Bottom, ASMElectricConnectorDirection.Right], [ASMElectricConnectorDirection.Top]),
            new MatrixSourceInfo("矩阵源: 从观察创建", "", "ASMatrixSourceFromLookAt", [ASMElectricConnectorDirection.Left, ASMElectricConnectorDirection.Bottom, ASMElectricConnectorDirection.Right], [ASMElectricConnectorDirection.Top]),
            new MatrixSourceInfo("矩阵源: 随机常数矩阵", "", "ASMatrixSourceRandomGenerator", [ASMElectricConnectorDirection.Bottom], [ASMElectricConnectorDirection.Top]),
        ];

        public Texture2D[] textures;

        public ASMatrixSourcesBlock()
            : base("Models/ASMatrixSource", "ASMatrixSource", 0.5f)
        {
        }

        public override void Initialize() {
            base.Initialize();
            textures = new Texture2D[Infos.Length];
            for (int i = 0; i < Infos.Length; i++) {//获取所有门的材质
                textures[i] = ContentManager.Get<Texture2D>($"Textures/MatrixSources/{Infos[i].Texture}");
            }
        }

        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData) {
            int type = GetType(Terrain.ExtractData(value));
            BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, textures[type], color, 2f * size, ref matrix, environmentData);
        }

        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
        {
            int num = Terrain.ExtractData(value) & 0x1F;
            int type = GetType(Terrain.ExtractData(value));
            generator.GenerateMeshVertices(this, x, y, z, m_blockMeshes[num], Color.White, null, geometry.GetGeometry(textures[type]).SubsetOpaque);
            GenerateASMWireVertices(generator, value, x, y, z, GetFace(value), m_centerBoxSize, Vector2.Zero, geometry.SubsetOpaque);
        }

        public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value) => Infos[GetType(Terrain.ExtractData(value))].DisplayName;

        public override string GetDescription(int value) => Infos[GetType(Terrain.ExtractData(value))].Description;

        public override IEnumerable<int> GetCreativeValues() {
            for (int i = 0; i < Infos.Length; i++) {
                yield return Terrain.MakeBlockValue(Index, 0, SetType(0, i));
            }
        }

        public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris) {
            int data = Terrain.ExtractData(oldValue);
            dropValues.Add(new BlockDropValue { Value = Terrain.MakeBlockValue(Index, 0, SetType(0, GetType(data))), Count = 1 });
            showDebris = true;
        }

        public override ASMElectricElement CreateElectricElement(SubsystemASMElectricity subsystemElectricity, int value, int x, int y, int z) => new ASMatrixSourcesElectricElement(subsystemElectricity, new CellFace(x, y, z, GetFace(value)), value);
        public override ASMElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
        {
            int data = Terrain.ExtractData(value);
            int type = GetType(data);
            if (GetFace(value) == face)
            {
                ASMElectricConnectorDirection? connectorDirection = SubsystemASMElectricity.GetConnectorDirection(GetFace(value), GetRotation(data), connectorFace);
                if(Infos[type].InputDirections.Any(e => e == connectorDirection)) return ASMElectricConnectorType.Input;
                if(Infos[type].OutputDirections.Any(e => e == connectorDirection)) return ASMElectricConnectorType.Output;
            }
            return null;
        }

        public static int GetType(int data) => (data >> 5) & 15;

        public static int SetType(int data, int type) => (data & -481) | ((type & 15) << 5);
    }
}