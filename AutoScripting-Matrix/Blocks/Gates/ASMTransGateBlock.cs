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
            new CalcGateInfo("矩阵转三维横向量", "输出一个三维向量，内容是输入矩阵M11，M12，M13的值", "ASMMatrix2Vector3Converter", false, false),
            new CalcGateInfo("矩阵提取取向", "求输入矩阵的取向矩阵", "ASMExtractOrientation", false, false),
            new CalcGateInfo("矩阵提取平移", "求输入矩阵的平移矩阵", "ASMExtractTranslation", false, false),
            new CalcGateInfo("矩阵弧度转角度", "依据点对点计算，将输入矩阵内所有元素转为角度制。（使用弧度制）", "ASMRad2Deg", true),
            new CalcGateInfo("矩阵角度转弧度", "依据点对点计算，将输入矩阵内所有元素转为弧度制。（使用角度制）", "ASMDeg2Rad", true),
            new CalcGateInfo("矩阵正弦", "依据点对点计算，将输入矩阵内所有元素求正弦值。（使用弧度制）", "ASMSin", true),
            new CalcGateInfo("矩阵余弦", "依据点对点计算，将输入矩阵内所有元素求余弦值。（使用弧度制）", "ASMCos", true),
            new CalcGateInfo("矩阵正切", "依据点对点计算，将输入矩阵内所有元素求正切值。（使用弧度制）", "ASMTan", true),
            new CalcGateInfo("矩阵反正弦", "依据点对点计算，将输入矩阵内所有元素求反正弦值。（使用弧度制）", "ASMAsin", true),
            new CalcGateInfo("矩阵反余弦", "依据点对点计算，将输入矩阵内所有元素求反余弦值。（使用弧度制）", "ASMAcos", true),
            new CalcGateInfo("矩阵反正切", "依据点对点计算，将输入矩阵内所有元素求反正切值。（使用弧度制）", "ASMAtan", true),
            new CalcGateInfo("矩阵求行列式", "求输入矩阵的行列式，输出浮点数。", "ASMDeterminantor", extraDesc: "一个三行四列的行列式怎么算？"),
            new CalcGateInfo("矩阵平方开方器", "将矩阵内所有元素的平方和再进行开方，最终输出。（即勾股定理，只不过有16个项）", "ASMPythagorean"),
        ];

        public Texture2D[] textures;

        public ASMTransGateBlock()
            : base("Models/ASMTransGate", "TransGate", 0.3f, "TransP2PGate")
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
            BlocksManager.DrawMeshBlock(primitivesRenderer, Infos[type].IsPointToPoint ? m_standaloneBlockMesh2 : m_standaloneBlockMesh, textures[type], color, 2f * size, ref matrix, environmentData);
        }

        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
        {
            int num = Terrain.ExtractData(value) & 0x1F;
            int type = GetType(Terrain.ExtractData(value));
            generator.GenerateMeshVertices(this, x, y, z, Infos[type].IsPointToPoint ? m_blockMeshes2[num] : m_blockMeshes[num], Color.White, null, geometry.GetGeometry(textures[type]).SubsetOpaque);
            GenerateASMWireVertices(generator, value, x, y, z, GetFace(value), m_centerBoxSize, Vector2.Zero, geometry.SubsetOpaque);
        }

        public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value) => Infos[GetType(Terrain.ExtractData(value))].DisplayName + (Infos[GetType(Terrain.ExtractData(value))].IsPointToPoint ? "(点对点)" : string.Empty);

        public override string MainDescription(int value) => Infos[GetType(Terrain.ExtractData(value))].Description;

        public override string ExtraDescription(int value) => Infos[GetType(Terrain.ExtractData(value))].ExtraDescription;

        public override IEnumerable<int> GetCreativeValues() {
            for (int i = 0; i < Infos.Length; i++) {
                if (!Infos[i].IsDisplay) continue;
                yield return Terrain.MakeBlockValue(Index, 0, SetType(0, i));
            }
        }

        public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris) {
            int data = Terrain.ExtractData(oldValue);
            dropValues.Add(new BlockDropValue { Value = Terrain.MakeBlockValue(Index, 0, SetType(0, GetType(data))), Count = 1 });
            showDebris = true;
        }

        public override ASMElectricElement CreateElectricElement(SubsystemASMElectricity subsystemElectricity, int value, int x, int y, int z) => new ASMTransGateElectricElement(subsystemElectricity, new CellFace(x, y, z, GetFace(value)), value);
        public override ASMElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
        {
            int data = Terrain.ExtractData(value);
            if (GetFace(value) == face)
            {
                ASMElectricConnectorDirection? connectorDirection = SubsystemASMElectricity.GetConnectorDirection(GetFace(value), GetRotation(data), connectorFace);
                if (connectorDirection == ASMElectricConnectorDirection.Bottom)
                {
                    return ASMElectricConnectorType.Input;
                }
                if (connectorDirection == ASMElectricConnectorDirection.Top || connectorDirection == ASMElectricConnectorDirection.In)
                {
                    return ASMElectricConnectorType.Output;
                }
            }
            return null;
        }

        public static int GetType(int data) => (data >> 5) & 31;

        public static int SetType(int data, int type) => (data & 0b1111111111111111111110000011111) | ((type & 31) << 5);
    }
}