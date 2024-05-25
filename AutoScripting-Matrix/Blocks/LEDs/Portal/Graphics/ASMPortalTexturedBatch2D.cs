using Engine;
using Engine.Graphics;

namespace Game {
    public class ASMPortalTexturedBatch3D : TexturedBatch3D {
        public new static ASMPortalShader Shader = new();

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

        public void SetPortalFrame(float thickness, float width, float height) {
            Shader.m_frameThickness.SetValue(thickness);
            Shader.m_frameWidth.SetValue(width);
            Shader.m_frameHeight.SetValue(height);
        }
    }
}