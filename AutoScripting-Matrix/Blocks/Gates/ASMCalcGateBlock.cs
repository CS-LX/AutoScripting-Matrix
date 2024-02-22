using Engine;
using Engine.Graphics;
using System;

namespace Game
{
    public class ASMCalcGateBlock : ASMRotateableMountedElectricElementBlock
    {
        public const int Index = 605;

        public readonly CalcGateInfo[] Infos = [
            new CalcGateInfo("矩阵加法器", "", "ASMAdder", true),
            new CalcGateInfo("矩阵减法器", "", "ASMSubtracter", true),
            new CalcGateInfo("矩阵乘法器", "", "ASMMultiplier", true),
            new CalcGateInfo("矩阵除法器", "", "ASMDivider", true),
            new CalcGateInfo("矩阵乘方器", "", "ASMPower", true),
            new CalcGateInfo("矩阵除余器", "", "ASMRemainder", true),
            new CalcGateInfo("矩阵对数器", "", "ASMLogarithmer", true),
            new CalcGateInfo("矩阵乘法器", "", "ASMMultiplier", false),
            new CalcGateInfo("矩阵除法器", "", "ASMDivider", false),
            new CalcGateInfo("矩阵取小器", "", "ASMMinimumer", true),
            new CalcGateInfo("矩阵取大器", "", "ASMMaximumer", true),
        ];

        public Texture2D[] textures;

        public ASMCalcGateBlock()
            : base("Models/ASMCalcGate", "CalcGate", 0.5f, "CalcP2PGate")
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

        public override string GetDescription(int value) => Infos[GetType(Terrain.ExtractData(value))].Description;

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

        public override ASMElectricElement CreateElectricElement(SubsystemASMElectricity subsystemElectricity, int value, int x, int y, int z) => new ASMCalcGateElectricElement(subsystemElectricity, new CellFace(x, y, z, GetFace(value)), value);
        public override ASMElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
        {
            int data = Terrain.ExtractData(value);
            if (GetFace(value) == face)
            {
                ASMElectricConnectorDirection? connectorDirection = SubsystemASMElectricity.GetConnectorDirection(GetFace(value), GetRotation(data), connectorFace);
                if (connectorDirection == ASMElectricConnectorDirection.Right || connectorDirection == ASMElectricConnectorDirection.Left)
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

        public static int GetType(int data) => (data >> 5) & 15;

        public static int SetType(int data, int type) => (data & -481) | ((type & 15) << 5);
    }
}