using Engine;
using Engine.Graphics;

namespace Game {
    public class ASMGeBlock : ASMElectricBaseBlock {
        public const int Index = 599;

        public Texture2D texture;

        public override void Initialize() {
            base.Initialize();
            texture = ContentManager.Get<Texture2D>("Textures/ASMGeBlock");
        }

        public override string GetCategory(int value) => SubsystemASMManager.CategoryName;

        public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value) => "矩阵变体锗块";

        public override string MainDescription(int value) => "更适合矩阵玩家与线代学者的锗块。\r\n可以用来作装饰，作电路载体。\r\n手持它时点击编辑按钮即可打开矩阵设置界面。";

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