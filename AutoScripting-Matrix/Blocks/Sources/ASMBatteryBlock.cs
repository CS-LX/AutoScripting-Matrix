using Engine;
using Engine.Graphics;
using System.Collections.Generic;

namespace Game {
    public class ASMBatteryBlock : ASMElectricBaseBlock, IASMElectricElementBlock {
        public const int Index = 601;

        public Texture2D texture;

        public override void Initialize() {
            texture = ContentManager.Get<Texture2D>("Textures/ASMMatrixSource");
        }

        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z) {
            generator.GenerateCubeVertices(
                this,
                value,
                x,
                y,
                z,
                Color.White,
                geometry.GetGeometry(texture).OpaqueSubsetsByFace
            );
        }

        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData) {
            BlocksManager.DrawCubeBlock(
                primitivesRenderer,
                value,
                new Vector3(size),
                ref matrix,
                Color.White,
                Color.White,
                environmentData,
                texture
            );
        }

        public ASMElectricElement CreateElectricElement(SubsystemASMElectricity subsystemElectricity, int value, int x, int y, int z) {
            CellFace[] cellFaces = new CellFace[6];
            for (int i = 0; i < 6; i++) {
                cellFaces[i] = new CellFace(x, y, z, i);
            }
            return new ASMBatteryElectricElement(subsystemElectricity, cellFaces);
        }

        public override int GetTextureSlotCount(int value) => 1;

        public ASMElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z) {
            return ASMElectricConnectorType.Output;
        }

        public int GetConnectionMask(int value) {
            return int.MaxValue;
        }

        public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value) => "矩阵源";

        public override string MainDescription(int value) => "输出玩家拟定好的矩阵，初始放置时默认数据为单位阵，破坏后不会丢失数据。";

        public override string ExtraDescription(int value) => "本mod的标志性方块~";
    }
}