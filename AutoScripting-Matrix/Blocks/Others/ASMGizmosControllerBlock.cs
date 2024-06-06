using Engine;
using Engine.Graphics;

namespace Game {
    public class ASMGizmosControllerBlock : ASMElectricBaseBlock {
        public const int Index = 623;

        public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value) => "Gizmos控制器";

        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z) { }

        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData) {
            FlatBatch3D flatBatch3D = primitivesRenderer.FlatBatch();
            Vector3 translation = matrix.Translation;
            Vector3 vector = matrix.Right * size;
            Vector3 v = matrix.Up * size;
            Vector3 v2 = matrix.Forward * size;
            Vector3 v3 = translation + (0.5f * (-vector - v - v2));
            Vector3 v4 = translation + (0.5f * (vector - v - v2));
            Vector3 v5 = translation + (0.5f * (-vector + v - v2));
            Vector3 v6 = translation + (0.5f * (vector + v - v2));
            Vector3 v7 = translation + (0.5f * (-vector - v + v2));
            Vector3 v8 = translation + (0.5f * (vector - v + v2));
            Vector3 v9 = translation + (0.5f * (-vector + v + v2));
            Vector3 v10 = translation + (0.5f * (vector + v + v2));
            if (environmentData.ViewProjectionMatrix.HasValue) {
                Matrix m = environmentData.ViewProjectionMatrix.Value;
                Vector3.Transform(ref v3, ref m, out v3);
                Vector3.Transform(ref v4, ref m, out v4);
                Vector3.Transform(ref v5, ref m, out v5);
                Vector3.Transform(ref v6, ref m, out v6);
                Vector3.Transform(ref v7, ref m, out v7);
                Vector3.Transform(ref v8, ref m, out v8);
                Vector3.Transform(ref v9, ref m, out v9);
                Vector3.Transform(ref v10, ref m, out v10);
            }
            flatBatch3D.QueueLineStrip([v3, v5, v6, v4], Color.Yellow);
            flatBatch3D.QueueLineStrip([v7, v8, v10, v9], Color.Yellow);
            flatBatch3D.QueueLineStrip([v3, v4, v8, v7], Color.Yellow);
            flatBatch3D.QueueLineStrip([v5, v9, v10, v6], Color.Yellow);
            flatBatch3D.QueueLineStrip([v3, v7, v9, v5], Color.Yellow);
            flatBatch3D.QueueLineStrip([v4, v6, v10, v8], Color.Yellow);
        }
    }
}