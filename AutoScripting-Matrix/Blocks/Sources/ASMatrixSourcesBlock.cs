using Engine;
using Engine.Graphics;
using System;

namespace Game
{
    public class ASMatrixSourcesBlock : ASMRotateableMountedElectricElementBlock
    {
        public const int Index = 608;

        public readonly MatrixSourceInfo[] Infos = [
            new MatrixSourceInfo("矩阵源: 世界方块坐标", "输出包含本元件所处的坐标的位移矩阵。", "ASMatrixSourceWorldPosition", [], [ASMElectricConnectorDirection.Top]),
            new MatrixSourceInfo("矩阵源: 玩家变换", "输出包含离此元件最近的玩家的世界变换矩阵（包含玩家坐标与旋转）。当时钟端没有接线时，此元件每时每刻输出；而当时钟端接线后，当时钟端输入矩阵M11上升沿（由0开始上升）时，获取一次矩阵并且输出。", "ASMatrixSourcePlayerTransform", [ASMElectricConnectorDirection.Bottom], [ASMElectricConnectorDirection.Top]),
            new MatrixSourceInfo("矩阵源: 玩家摄像机视图", "输出最近玩家的摄像机视图矩阵，包含观测的位置，方向，垂直Y方向 \n(注意: 若要直接使用此矩阵作为绘制的变换，须对其求逆)", "ASMatrixSourcePlayerCamera", [ASMElectricConnectorDirection.Bottom], [ASMElectricConnectorDirection.Top]),
            new MatrixSourceInfo("矩阵源: 玩家摄像机投影", "输出最近玩家的摄像机投影矩阵(左)、屏幕投影矩阵(右)", "ASMatrixSourcePlayerCameraProjection", [], [ASMElectricConnectorDirection.Left, ASMElectricConnectorDirection.Right]),
            new MatrixSourceInfo("矩阵源: 从位移创建", "创建一个位移矩阵，x为左端（浮点）+底端输入M11，y为后端（浮点）+底端输入M12，z为右端（浮点）+底端输入M13。", "ASMatrixSourceFromTranslation", [ASMElectricConnectorDirection.Left, ASMElectricConnectorDirection.Bottom, ASMElectricConnectorDirection.Right, ASMElectricConnectorDirection.In], [ASMElectricConnectorDirection.Top]),
            new MatrixSourceInfo("矩阵源: 从欧拉角(三轴旋转)创建", "根据欧拉角创建一个旋转矩阵，采用弧度制！ yaw为左端（浮点）+底端输入M11，pitch为后端（浮点）+底端输入M12，roll为右端（浮点）+底端输入M13。", "ASMatrixSourceFromYPR", [ASMElectricConnectorDirection.Left, ASMElectricConnectorDirection.Bottom, ASMElectricConnectorDirection.Right, ASMElectricConnectorDirection.In], [ASMElectricConnectorDirection.Top]),
            new MatrixSourceInfo("矩阵源: 从缩放创建", "根据缩放创建一个缩放矩阵。缩放矩阵的三轴缩放皆为输入值。", "ASMatrixSourceFromScale", [ASMElectricConnectorDirection.Bottom], [ASMElectricConnectorDirection.Top]),
            new MatrixSourceInfo("矩阵源: 从三轴缩放创建", "根据三轴缩放创建一个缩放矩阵。x为左端（浮点）+底端输入M11，y为后端（浮点）+底端输入M12，z为右端（浮点）+底端输入M13。", "ASMatrixSourceFromScaleXYZ", [ASMElectricConnectorDirection.Left, ASMElectricConnectorDirection.Bottom, ASMElectricConnectorDirection.Right, ASMElectricConnectorDirection.In], [ASMElectricConnectorDirection.Top]),
            new MatrixSourceInfo("矩阵源: 从观察创建", "详见\"https://github.com/CS-LX/AutoScripting-Matrix/blob/main/README.md\"", "ASMatrixSourceFromLookAt", [ASMElectricConnectorDirection.Left, ASMElectricConnectorDirection.Bottom, ASMElectricConnectorDirection.Right], [ASMElectricConnectorDirection.Top]),
            new MatrixSourceInfo("矩阵源: 随机常数矩阵", "输出一个介于0~1的随机常数矩阵。当时钟端没有接线时，此元件每时每刻输出；而当时钟端接线后，当时钟端输入矩阵M11上升沿（由0开始上升）时，获取一次矩阵并且输出。", "ASMatrixSourceRandomGenerator", [ASMElectricConnectorDirection.Bottom], [ASMElectricConnectorDirection.Top]),
            new MatrixSourceInfo("二阶方阵转矩阵", "详见\"https://github.com/CS-LX/AutoScripting-Matrix/blob/main/README.md\"", "ASM2X2ToMatrix", [ASMElectricConnectorDirection.Left, ASMElectricConnectorDirection.Right, ASMElectricConnectorDirection.Bottom, ASMElectricConnectorDirection.Top], [ASMElectricConnectorDirection.In], true),
            new MatrixSourceInfo("四维横向量转矩阵", "详见\"https://github.com/CS-LX/AutoScripting-Matrix/blob/main/README.md\"", "ASMVector4TToMatrix", [ASMElectricConnectorDirection.Left, ASMElectricConnectorDirection.Right, ASMElectricConnectorDirection.Bottom, ASMElectricConnectorDirection.Top], [ASMElectricConnectorDirection.In], true),
            new MatrixSourceInfo("浮点转四维横向量", "输出一个四维向量(x, y, z, w)。x：前端；y：右端；z：后端；w：左端。各输入端口都为浮点。", "ASMFloatToVector4T", [ASMElectricConnectorDirection.Left, ASMElectricConnectorDirection.Right, ASMElectricConnectorDirection.Bottom, ASMElectricConnectorDirection.Top], [ASMElectricConnectorDirection.In], true),
            new MatrixSourceInfo("矩阵源: 从正交投影创建", "输出一个正交投影矩阵。前端（浮点）和右端（浮点）分别代表投影平面的宽度和高度，后端（浮点）和左端（浮点）分别代表近裁剪面和远裁剪面的距离。", "ASMatrixSourceOrthographic", [ASMElectricConnectorDirection.Left, ASMElectricConnectorDirection.Right, ASMElectricConnectorDirection.Bottom, ASMElectricConnectorDirection.Top], [ASMElectricConnectorDirection.In], true),
            new MatrixSourceInfo("矩阵源: 从透视投影创建", "输出一个透视投影矩阵。前端（浮点）和右端（浮点）分别代表垂直视角的视野范围和宽高比，后端（浮点）和左端（浮点）分别代表近裁剪面和远裁剪面的距离。", "ASMatrixSourcePerspective", [ASMElectricConnectorDirection.Left, ASMElectricConnectorDirection.Right, ASMElectricConnectorDirection.Bottom, ASMElectricConnectorDirection.Top], [ASMElectricConnectorDirection.In], true),
            new MatrixSourceInfo("矩阵源: 实时钟", "详见\"https://github.com/CS-LX/AutoScripting-Matrix/blob/main/README.md\"", "ASMatrixSourceRealtimeClock", [ASMElectricConnectorDirection.In], [ASMElectricConnectorDirection.Left, ASMElectricConnectorDirection.Right, ASMElectricConnectorDirection.Bottom, ASMElectricConnectorDirection.Top], true),
        ];

        public Texture2D[] textures;

        public ASMatrixSourcesBlock()
            : base("Models/ASMatrixSource", "ASMatrixSource", 0.5f, "ASMatrixSourceLarger")
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
            BlocksManager.DrawMeshBlock(primitivesRenderer,Infos[type].IsLarger ? m_standaloneBlockMesh2 : m_standaloneBlockMesh, textures[type], color, 2f * size, ref matrix, environmentData);
        }

        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
        {
            int num = Terrain.ExtractData(value) & 0x1F;
            int type = GetType(Terrain.ExtractData(value));
            generator.GenerateMeshVertices(this, x, y, z, Infos[type].IsLarger ? m_blockMeshes2[num] : m_blockMeshes[num], Color.White, null, geometry.GetGeometry(textures[type]).SubsetOpaque);
            GenerateASMWireVertices(generator, value, x, y, z, GetFace(value), m_centerBoxSize, Vector2.Zero, geometry.SubsetOpaque);
        }

        public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value) => Infos[GetType(Terrain.ExtractData(value))].DisplayName;

        public override string GetDescription(int value) => Infos[GetType(Terrain.ExtractData(value))].Description;

        public override float GetIconViewScale(int value, DrawBlockEnvironmentData environmentData) => Infos[GetType(Terrain.ExtractData(value))].IsLarger ? 1 : 1.2f;

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