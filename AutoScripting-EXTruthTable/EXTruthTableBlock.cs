using Engine;
using Engine.Graphics;
using Game;

namespace AutoScripting_EXTruthTable {
    public class EXTruthTableBlock : RotateableMountedElectricElementBlock
    {
        public const int Index = 670;

        public Texture2D m_texture;

        public EXTruthTableBlock()
            : base("Models/ASEXTruthTable", "Decomposer", 0.5f)
        {
        }

        public override void Initialize() {
            base.Initialize();
            m_texture = ContentManager.Get<Texture2D>("Textures/ASEXTruthTable");
        }

        public override ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
        {
            return new EXTruthTableElectricElement(subsystemElectricity, new CellFace(x, y, z, GetFace(value)));
        }

        public override ElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
        {
            int data = Terrain.ExtractData(value);
            if (GetFace(value) == face)
            {
                ElectricConnectorDirection? connectorDirection = SubsystemElectricity.GetConnectorDirection(GetFace(value), GetRotation(data), connectorFace);
                if (connectorDirection == ElectricConnectorDirection.Right || connectorDirection == ElectricConnectorDirection.Left || connectorDirection == ElectricConnectorDirection.Bottom || connectorDirection == ElectricConnectorDirection.Top)
                {
                    return ElectricConnectorType.Input;
                }
                if (connectorDirection == ElectricConnectorDirection.In)
                {
                    return ElectricConnectorType.Output;
                }
            }
            return null;
        }

        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, m_texture, color, 2f * size, ref matrix, environmentData);
        }

        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
        {
            int num = Terrain.ExtractData(value) & 0x1F;
            generator.GenerateMeshVertices(this, x, y, z, m_blockMeshes[num], Color.White, null, geometry.GetGeometry(m_texture).SubsetOpaque);
            generator.GenerateWireVertices(value, x, y, z, GetFace(value), m_centerBoxSize, Vector2.Zero, geometry.SubsetOpaque);
        }
    }
}