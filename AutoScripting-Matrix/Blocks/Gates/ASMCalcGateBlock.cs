using Engine;
using Engine.Graphics;
using System;

namespace Game
{
    public class ASMCalcGateBlock : ASMRotateableMountedElectricElementBlock
    {
        public const int Index = 605;

        public readonly CalcGateInfo[] Infos = [
            new CalcGateInfo("矩阵加法器", "", "ASMAdder"),
            new CalcGateInfo("矩阵减法器", "", "ASMSubtracter"),
            new CalcGateInfo("矩阵乘法器", "", "ASMMultiplier"),
            new CalcGateInfo("矩阵除法器", "", "ASMDivider")
        ];

        public Texture2D[] textures;

        public ASMCalcGateBlock()
            : base("Models/ASMCalcGate", "CalcGate", 0.5f)
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

        public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris) {
            int data = Terrain.ExtractData(oldValue);
            dropValues.Add(new BlockDropValue { Value = Terrain.MakeBlockValue(Index, 0, SetType(data, GetType(data))), Count = 1 });
            showDebris = true;
        }

        public override ASMElectricElement CreateElectricElement(SubsystemASMElectricity subsystemElectricity, int value, int x, int y, int z) => new ASMCalcGateElectricElement(subsystemElectricity, new CellFace(x, y, z, GetFace(value)), value);
        public override ASMElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
        {
            int data = Terrain.ExtractData(value);
            if (GetFace(value) == face)
            {
                ElectricConnectorDirection? connectorDirection = SubsystemElectricity.GetConnectorDirection(GetFace(value), GetRotation(data), connectorFace);
                if (connectorDirection == ElectricConnectorDirection.Right || connectorDirection == ElectricConnectorDirection.Left)
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