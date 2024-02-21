using Engine;
using Engine.Graphics;
using System;

namespace Game
{
    public class ASMDecomposerBlock : ASMRotateableMountedElectricElementBlock
    {
        public const int Index = 613;

        public readonly ASMDecomposerInfo[] Infos = [
            new ASMDecomposerInfo("矩阵TO解构器", "", "ASMatrixToTO", [ASMElectricConnectorDirection.Bottom], [ASMElectricConnectorDirection.Left, ASMElectricConnectorDirection.Right]),
            new ASMDecomposerInfo("矩阵TRS解构器", "", "ASMatrixToTRS", [ASMElectricConnectorDirection.Bottom], [ASMElectricConnectorDirection.Left, ASMElectricConnectorDirection.Right, ASMElectricConnectorDirection.Top]),
            new ASMDecomposerInfo("矩阵转二阶方阵", "", "ASMatrixTo2X2", [ASMElectricConnectorDirection.In], [ASMElectricConnectorDirection.Left, ASMElectricConnectorDirection.Right, ASMElectricConnectorDirection.Top, ASMElectricConnectorDirection.Bottom]),
            new ASMDecomposerInfo("矩阵转四维纵向量", "", "ASMatrixToVector4", [ASMElectricConnectorDirection.In], [ASMElectricConnectorDirection.Left, ASMElectricConnectorDirection.Right, ASMElectricConnectorDirection.Top, ASMElectricConnectorDirection.Bottom], false),
            new ASMDecomposerInfo("四维纵向量转浮点", "", "ASMVector4ToFloat", [ASMElectricConnectorDirection.In], [ASMElectricConnectorDirection.Left, ASMElectricConnectorDirection.Right, ASMElectricConnectorDirection.Top, ASMElectricConnectorDirection.Bottom], false),
            new ASMDecomposerInfo("矩阵转四维横向量", "", "ASMatrixToVector4T", [ASMElectricConnectorDirection.In], [ASMElectricConnectorDirection.Left, ASMElectricConnectorDirection.Right, ASMElectricConnectorDirection.Top, ASMElectricConnectorDirection.Bottom]),
            new ASMDecomposerInfo("四维横向量转浮点", "", "ASMVector4TToFloat", [ASMElectricConnectorDirection.In], [ASMElectricConnectorDirection.Left, ASMElectricConnectorDirection.Right, ASMElectricConnectorDirection.Top, ASMElectricConnectorDirection.Bottom]),
        ];

        public Texture2D[] textures;

        public ASMDecomposerBlock()
            : base("Models/ASMDecomposer", "Decomposer", 0.5f)
        {
        }

        public override void Initialize() {
            base.Initialize();
            textures = new Texture2D[Infos.Length];
            for (int i = 0; i < Infos.Length; i++) {//获取所有门的材质
                textures[i] = ContentManager.Get<Texture2D>($"Textures/Decomposers/{Infos[i].Texture}");
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

        public override ASMElectricElement CreateElectricElement(SubsystemASMElectricity subsystemElectricity, int value, int x, int y, int z) => new ASMDecomposerElectricElement(subsystemElectricity, new CellFace(x, y, z, GetFace(value)), value);
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