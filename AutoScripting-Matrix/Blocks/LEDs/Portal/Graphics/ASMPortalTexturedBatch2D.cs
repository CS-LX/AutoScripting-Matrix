using Engine;
using Engine.Graphics;

namespace Game {
    public class ASMPortalTexturedBatch3D : TexturedBatch3D {
        public new static ASMPortalShader Shader = new();

        public void Flush() {
            Display.Clear(Color.Black);
            Display.DepthStencilState = DepthStencilState;
            Display.RasterizerState = RasterizerState;
            Display.BlendState = BlendState;
            Shader.TextureSize = new Vector2(Texture.Width, Texture.Height);
            Shader.Texture = Texture;
            Shader.SamplerState = SamplerState;
            Matrix matrix = PrimitivesRenderer2D.ViewportMatrix();
            Shader.Transforms.World[0] = matrix;
            Display.DrawUserIndexed(
                PrimitiveType.TriangleList,
                Shader,
                VertexPositionColorTexture.VertexDeclaration,
                TriangleVertices.Array,
                0,
                TriangleVertices.Count,
                TriangleIndices.Array,
                0,
                TriangleIndices.Count
            );
            Clear();
        }

        public override void Flush(Matrix matrix, bool clearAfterFlush = true) {
            Display.DepthStencilState = base.DepthStencilState;
            Display.RasterizerState = base.RasterizerState;
            Display.BlendState = base.BlendState;
            FlushWithCurrentState2(Texture, SamplerState, matrix, clearAfterFlush);
        }

        public void FlushWithCurrentState2(Texture2D texture, SamplerState samplerState, Matrix matrix, bool clearAfterFlush = true) {
            Shader.Texture = texture;
            Shader.SamplerState = samplerState;
            Shader.Transforms.World[0] = matrix;
            FlushWithCurrentStateAndShader(Shader, clearAfterFlush);
        }
    }
}