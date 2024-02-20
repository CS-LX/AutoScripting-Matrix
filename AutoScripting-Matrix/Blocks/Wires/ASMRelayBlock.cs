using Engine;
using Engine.Graphics;

namespace Game {
    public class ASMRelayBlock : ASMRotateableMountedElectricElementBlock {

        public const int Index = 611;

        public Texture2D texture;

        public override void Initialize() {
            base.Initialize();
            texture = ContentManager.Get<Texture2D>("Textures/ASMRelay");
        }

        public ASMRelayBlock() : base("Models/ASMRelay", "Relay", 0.2f) {}

        public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value) => "矩阵继电器";

        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData) {
            BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, texture, color, 2f * size, ref matrix, environmentData);
        }

        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
        {
            int num = Terrain.ExtractData(value) & 0x1F;
            generator.GenerateMeshVertices(this, x, y, z, m_blockMeshes[num], Color.White, null, geometry.GetGeometry(texture).SubsetOpaque);
            GenerateASMWireVertices(generator, value, x, y, z, GetFace(value), m_centerBoxSize, Vector2.Zero, geometry.SubsetOpaque);
        }
        public override ASMElectricElement CreateElectricElement(SubsystemASMElectricity subsystemASMElectricity, int value, int x, int y, int z) => new ASMRelayElectricElement(subsystemASMElectricity, new CellFace(x, y, z, GetFace(value)));

        public override ASMElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)		{
            int data = Terrain.ExtractData(value);
            if (GetFace(value) == face)
            {
                ASMElectricConnectorDirection? connectorDirection = SubsystemASMElectricity.GetConnectorDirection(GetFace(value), GetRotation(data), connectorFace);
                if (connectorDirection == ASMElectricConnectorDirection.Bottom || connectorDirection == ASMElectricConnectorDirection.Left || connectorDirection == ASMElectricConnectorDirection.Right)
                {
                    return ASMElectricConnectorType.Input;
                }
                if (connectorDirection == ASMElectricConnectorDirection.Top)
                {
                    return ASMElectricConnectorType.Output;
                }
            }
            return null;
        }
    }
}