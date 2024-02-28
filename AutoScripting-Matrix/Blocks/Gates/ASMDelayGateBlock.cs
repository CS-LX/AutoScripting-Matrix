using Engine;
using Engine.Graphics;

namespace Game {
    public class ASMDelayGateBlock : ASMRotateableMountedElectricElementBlock {
        public const int Index = 616;

        public Texture2D[] m_textures;

        public readonly ASMDelayGateInfo[] Infos = [
            new ASMDelayGateInfo("矩阵延迟门", "", "ASMDelayGate"),
            new ASMDelayGateInfo("矩阵可调延迟门", "", "ASMAdjustableDelayGate")
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

        public override string GetDescription(int value) => Infos[GetType(Terrain.ExtractData(value))].Description;

        public override IEnumerable<int> GetCreativeValues() {
            for (int i = 0; i < Infos.Length; i++) {
                yield return Terrain.MakeBlockValue(Index, 0, SetType(0, i));
            }
        }

        public override ASMElectricElement CreateElectricElement(SubsystemASMElectricity subsystemASMElectricity, int value, int x, int y, int z) => new ASMDelayGateElectricElement(subsystemASMElectricity, new CellFace(x, y, z, GetFace(value)));

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

        public static int GetType(int data) => (data >> 5) & 15;

        public static int SetType(int data, int type) => (data & -481) | ((type & 15) << 5);
    }
}