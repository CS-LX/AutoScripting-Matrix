using Engine;
using Engine.Graphics;
using System;

namespace Game
{
    public class ASMTransGateBlock : ASMRotateableMountedElectricElementBlock
    {
        public const int Index = 606;

        public readonly CalcGateInfo[] Infos = [
            new CalcGateInfo("矩阵求逆", "求输入矩阵的逆矩阵并输出", "ASMInverter"),
            new CalcGateInfo("矩阵转置", "转置输入矩阵并输出", "ASMTransposer"),
            new CalcGateInfo("矩阵二次方", "将输入矩阵平方再输出", "ASMSquarer"),
            new CalcGateInfo("矩阵三次方", "将输入矩阵立方再输出", "ASMCuber"),
            new CalcGateInfo("矩阵转平均浮点数", "将矩阵内每一个非0元素相加再除以非0元素的个数，将所得浮点数结果输出", "ASMAverager"),
            new CalcGateInfo("浮点数转常数矩阵", "输出一个内部元素全为输入浮点数的矩阵", "ASMFloat2CMatrixConverter"),
            new CalcGateInfo("矩阵转三维横向量", "输出一个三维向量，内容是输入矩阵第一行第一列，第一行第二列，第一行第三列的值", "ASMMatrix2Vector3Converter"),
        ];

        public Texture2D[] textures;

        public ASMTransGateBlock()
            : base("Models/ASMTransGate", "TransGate", 0.3f)
        {
        }

        public override void Initialize() {
            base.Initialize();
            textures = new Texture2D[Infos.Length];
            for (int i = 0; i < Infos.Length; i++) {//获取所有门的材质
                textures[i] = ContentManager.Get<Texture2D>($"Textures/CalcGates/{Infos[i].Texture}");
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

        public override ASMElectricElement CreateElectricElement(SubsystemASMElectricity subsystemElectricity, int value, int x, int y, int z) => new ASMTransGateElectricElement(subsystemElectricity, new CellFace(x, y, z, GetFace(value)), value);
        public override ASMElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
        {
            int data = Terrain.ExtractData(value);
            if (GetFace(value) == face)
            {
                ElectricConnectorDirection? connectorDirection = SubsystemElectricity.GetConnectorDirection(GetFace(value), GetRotation(data), connectorFace);
                if (connectorDirection == ElectricConnectorDirection.Bottom)
                {
                    return ASMElectricConnectorType.Input;
                }
                if (connectorDirection == ElectricConnectorDirection.Top || connectorDirection == ElectricConnectorDirection.In)
                {
                    return ASMElectricConnectorType.Output;
                }
            }
            return null;
        }

        public static int GetType(int data) => (data >> 5) & 15;

        public static int SetType(int data, int type) => (data & -481) | ((type & 15) << 5);
    }
}