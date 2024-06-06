using Engine;
using Engine.Graphics;

namespace Game {
    public class ASMDelayGateBlock : ASMRotateableMountedElectricElementBlock {
        public const int Index = 616;

        public Texture2D[] m_textures;

        public readonly ASMDelayGateInfo[] Infos = [
            new ASMDelayGateInfo("矩阵延迟门", "与原版延迟门功能几乎一样。", "ASMDelayGate"),
            new ASMDelayGateInfo("矩阵可调延迟门", "与原版延迟门功能几乎一样，但延迟上限增大至655.35秒。", "ASMAdjustableDelayGate")
        ];

        public ASMDelayGateBlock() : base("Models/ASMDelayGate", "ASMDelayGate", 0.375f) { }

        public override void Initialize() {
            base.Initialize();
            m_textures = new Texture2D[Infos.Length];
            for (int i = 0; i < Infos.Length; i++) {//获取所有门的材质
                m_textures[i] = ContentManager.Get<Texture2D>($"Textures/DelayGates/{Infos[i].TextureName}");
            }
        }

        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData) {
            int type = GetType(Terrain.ExtractData(value));
            BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, m_textures[type], color, 2f * size, ref matrix, environmentData);
        }

        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
        {
            int num = Terrain.ExtractData(value) & 0x1F;
            int type = GetType(Terrain.ExtractData(value));
            generator.GenerateMeshVertices(this, x, y, z, m_blockMeshes[num], Color.White, null, geometry.GetGeometry(m_textures[type]).SubsetOpaque);
            GenerateASMWireVertices(generator, value, x, y, z, GetFace(value), m_centerBoxSize, Vector2.Zero, geometry.SubsetOpaque);
        }

        public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value) => Infos[GetType(Terrain.ExtractData(value))].Name;

        public override string MainDescription(int value) => Infos[GetType(Terrain.ExtractData(value))].Description;

        public override bool IsNonDuplicable_(int value) => GetType(Terrain.ExtractData(value)) == 1;

        public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris) {
            int data = Terrain.ExtractData(oldValue);
            dropValues.Add(new BlockDropValue { Value = Terrain.MakeBlockValue(Index, 0, SetType(data, GetType(data))), Count = 1 });
            showDebris = true;
        }

        public override IEnumerable<int> GetCreativeValues() {
            for (int i = 0; i < Infos.Length; i++) {
                yield return Terrain.MakeBlockValue(Index, 0, SetType(0, i));
            }
        }

        public override ASMElectricElement CreateElectricElement(SubsystemASMElectricity subsystemASMElectricity, int value, int x, int y, int z) => GetType(Terrain.ExtractData(value)) == 1 ? new ASMAdjustableDelayGateElectricElement(subsystemASMElectricity, new CellFace(x, y, z, GetFace(value))) : new ASMDelayGateElectricElement(subsystemASMElectricity, new CellFace(x, y, z, GetFace(value)));

        public override ASMElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z) {
            int data = Terrain.ExtractData(value);
            if (GetFace(value) == face) {
                ASMElectricConnectorDirection? connectorDirection = SubsystemASMElectricity.GetConnectorDirection(GetFace(value), GetRotation(data), connectorFace);
                if (connectorDirection == ASMElectricConnectorDirection.Bottom) {
                    return ASMElectricConnectorType.Input;
                }
                if (connectorDirection == ASMElectricConnectorDirection.Top
                    || connectorDirection == ASMElectricConnectorDirection.In) {
                    return ASMElectricConnectorType.Output;
                }
            }
            return null;
        }

        //从右往左数，1~5：旋转   6~7：不同延迟门ID   8~14：不同延迟值
        public static int GetType(int data) => (data >> 5) & 3;

        public static int SetType(int data, int type) {
            int a = data & -128;
            int b = (type & 3) << 5;
            return a | b;
        }
    }
}