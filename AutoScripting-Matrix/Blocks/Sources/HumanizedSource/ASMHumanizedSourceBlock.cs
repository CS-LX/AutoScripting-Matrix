using Engine;
using Engine.Graphics;

namespace Game {
    public class ASMHumanizedSourceBlock : ASMRotateableMountedElectricElementBlock {

        public const int Index = 626;

        public Texture2D m_texture;

        public ASMHumanizedSourceBlock() : base("Models/ASMDecomposer", "Decomposer", 0.5f) { }

        public override void Initialize() {
            base.Initialize();
            m_texture = ContentManager.Get<Texture2D>("Textures/ASMHumanizedSource");
        }

        public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value) => "人性化矩阵源";

        public override string MainDescription(int value) => "";

        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData) {
            BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, m_texture, color, 2f * size, ref matrix, environmentData);
        }

        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
        {
            int num = Terrain.ExtractData(value) & 0x1F;
            generator.GenerateMeshVertices(this, x, y, z, m_blockMeshes[num], Color.White, null, geometry.GetGeometry(m_texture).SubsetOpaque);
            GenerateASMWireVertices(generator, value, x, y, z, GetFace(value), m_centerBoxSize, Vector2.Zero, geometry.SubsetOpaque);
        }

        public override ASMElectricElement CreateElectricElement(SubsystemASMElectricity subsystemASMElectricity, int value, int x, int y, int z) => new ASMTruthTableElectricElement(subsystemASMElectricity, new CellFace(x, y, z, GetFace(value)));

        public override ASMElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z) {
            int face2 = GetFace(value);
            if (face == face2) {
                ElectricConnectorDirection? connectorDirection = SubsystemElectricity.GetConnectorDirection(face2, GetRotation(Terrain.ExtractData(value)), connectorFace);
                if (connectorDirection.HasValue)
                    return connectorDirection.Value == ElectricConnectorDirection.Top ? ASMElectricConnectorType.Output : null;
            }
            return null;
        }
    }
}