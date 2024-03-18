using Engine;
using Engine.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Game {
    public class ASMComplexCameraLEDBlock : ASMRotateableMountedElectricElementBlock {
        public const int Index = 620;

        public Texture2D texture;

        public ASMComplexCameraLEDBlock() : base("Models/ASMLed", "ASMLed", 0.9f) { }

        public override void Initialize() {
            base.Initialize();
            texture = ContentManager.Get<Texture2D>("Textures/ASMComplexCameraLED");
        }

        public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value) => "复杂摄像机LED";

        public override string GetDescription(int value) => "创建一个摄像机，并且将摄像机捕捉到的画面显示出来。\r\n详见https://github.com/CS-LX/AutoScripting-Matrix";

        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z) {
            int num = Terrain.ExtractData(value) & 0x1F;
            generator.GenerateMeshVertices(
                this,
                x,
                y,
                z,
                m_blockMeshes[num],
                Color.White,
                null,
                geometry.GetGeometry(texture).SubsetOpaque
            );
            GenerateASMWireVertices(
                generator,
                value,
                x,
                y,
                z,
                GetFace(value),
                m_centerBoxSize,
                Vector2.Zero,
                geometry.SubsetOpaque
            );
        }

        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData) {
            BlocksManager.DrawMeshBlock(
                primitivesRenderer,
                m_standaloneBlockMesh,
                texture,
                color,
                2f * size,
                ref matrix,
                environmentData
            );
        }

        public override ASMElectricElement CreateElectricElement(SubsystemASMElectricity subsystemElectricity, int value, int x, int y, int z) => new ASMComplexCameraLEDElectricElement(subsystemElectricity, new CellFace(x, y, z, GetFace(value)), value);

        public override ASMElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z) {
            int face2 = GetFace(value);
            if (face == face2
                && SubsystemElectricity.GetConnectorDirection(face2, 0, connectorFace).HasValue) {
                return ASMElectricConnectorType.Input;
            }
            return null;
        }
    }
}