using Engine;
using Engine.Graphics;
using System;

namespace Game {
    public class ASMMotionDetectorBlock : ASMMountedElectricElementBlock {
        public const int Index = 618;

        public BlockMesh m_standaloneBlockMesh = new BlockMesh();

        public BlockMesh[] m_blockMeshesByData = new BlockMesh[6];

        public BoundingBox[][] m_collisionBoxesByData = new BoundingBox[6][];

        public Texture2D m_texture;

        public override void Initialize() {
            Model model = ContentManager.Get<Model>("Models/ASMMotionDetector");
            m_texture = ContentManager.Get<Texture2D>("Textures/ASMMotionDetector");
            Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("MotionDetector").ParentBone);
            for (int i = 0; i < 6; i++) {
                int num = i;
                Matrix m = (i >= 4) ? ((i != 4) ? (Matrix.CreateRotationX((float)Math.PI) * Matrix.CreateTranslation(0.5f, 1f, 0.5f)) : Matrix.CreateTranslation(0.5f, 0f, 0.5f)) : (Matrix.CreateRotationX((float)Math.PI / 2f) * Matrix.CreateTranslation(0f, 0f, -0.5f) * Matrix.CreateRotationY(i * (float)Math.PI / 2f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f));
                m_blockMeshesByData[num] = new BlockMesh();
                m_blockMeshesByData[num]
                .AppendModelMeshPart(
                    model.FindMesh("MotionDetector").MeshParts[0],
                    boneAbsoluteTransform * m,
                    makeEmissive: false,
                    flipWindingOrder: false,
                    doubleSided: false,
                    flipNormals: false,
                    Color.White
                );
                m_collisionBoxesByData[num] = new BoundingBox[1] { m_blockMeshesByData[num].CalculateBoundingBox() };
            }
            Matrix m2 = Matrix.CreateRotationY(-(float)Math.PI / 2f) * Matrix.CreateRotationZ((float)Math.PI / 2f);
            m_standaloneBlockMesh.AppendModelMeshPart(
                model.FindMesh("MotionDetector").MeshParts[0],
                boneAbsoluteTransform * m2,
                makeEmissive: false,
                flipWindingOrder: false,
                doubleSided: false,
                flipNormals: false,
                Color.White
            );
        }

        public override int GetFace(int value) {
            return Terrain.ExtractData(value) & 7;
        }

        public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult) {
            BlockPlacementData result = default;
            result.Value = Terrain.ReplaceData(value, raycastResult.CellFace.Face);
            result.CellFace = raycastResult.CellFace;
            return result;
        }

        public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value) {
            int num = Terrain.ExtractData(value);
            if (num >= m_collisionBoxesByData.Length) {
                return null;
            }
            return m_collisionBoxesByData[num];
        }

        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z) {
            int num = Terrain.ExtractData(value);
            if (num < m_blockMeshesByData.Length) {
                generator.GenerateMeshVertices(
                    this,
                    x,
                    y,
                    z,
                    m_blockMeshesByData[num],
                    Color.White,
                    null,
                    geometry.GetGeometry(m_texture).SubsetOpaque
                );
                GenerateASMWireVertices(
                    generator,
                    value,
                    x,
                    y,
                    z,
                    GetFace(value),
                    0.25f,
                    Vector2.Zero,
                    geometry.SubsetOpaque
                );
            }
        }

        public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value) => "矩阵移动传感器";

        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData) {
            BlocksManager.DrawMeshBlock(
                primitivesRenderer,
                m_standaloneBlockMesh,
                m_texture,
                color,
                2f * size,
                ref matrix,
                environmentData
            );
        }

        public override ASMElectricElement CreateElectricElement(SubsystemASMElectricity subsystemElectricity, int value, int x, int y, int z) {
            return new ASMMotionDetectorElectricElement(subsystemElectricity, new CellFace(x, y, z, GetFace(value)));
        }

        public override ASMElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z) {
            int face2 = GetFace(value);
            if (face == face2
                && SubsystemASMElectricity.GetConnectorDirection(face2, 0, connectorFace).HasValue) {
                return ASMElectricConnectorType.Output;
            }
            return null;
        }
    }
}