using Engine;
using Engine.Graphics;

namespace Game {
    public class ASMGeBlock : GenerateASMWireVerticesBlock {
        public const int Index = 599;

        public Texture2D texture;

        public override void Initialize() {
            base.Initialize();
            texture = ContentManager.Get<Texture2D>("Textures/ASMGeBlock");
        }

        public override string GetCategory(int value) => "[智械]矩阵";

        public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value) => "矩阵变体锗块";

        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
        {
            generator.GenerateCubeVertices(this, value, x, y, z, Color.White, geometry.GetGeometry(texture).OpaqueSubsetsByFace);
        }

        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            BlocksManager.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, Color.White, Color.White, environmentData, texture);
        }

        public override int GetTextureSlotCount(int value) => 1;
    }
}