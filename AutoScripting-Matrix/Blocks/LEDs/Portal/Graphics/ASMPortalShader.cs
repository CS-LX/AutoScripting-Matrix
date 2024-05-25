using Engine;
using Engine.Graphics;

namespace Game {
    public class ASMPortalShader : Shader {
        public ShaderParameter m_worldViewProjectionMatrixParameter;

        public ShaderParameter m_horizontal;
        public ShaderParameter m_textureSizeParameter;
        public ShaderParameter m_textureParameter;
        public ShaderParameter m_samplerStateParameter;

        public ShaderParameter m_frameThickness;
        public ShaderParameter m_frameWidth;
        public ShaderParameter m_frameHeight;

        public readonly ShaderTransforms Transforms;

        public bool Horizontal {
            set => m_horizontal.SetValue(value ? 1.0f : 0.0f);
        }

        public Vector2 TextureSize {
            set => m_textureSizeParameter.SetValue(value);
        }

        public Texture2D Texture {
            set => m_textureParameter.SetValue(value);
        }

        public SamplerState SamplerState {
            set => m_samplerStateParameter.SetValue(value);
        }

        public ASMPortalShader() : base(ShaderCodeManager.GetFast("Shaders/ASMPortalShader.vsh"), ShaderCodeManager.GetFast("Shaders/ASMPortalShader.psh")) {
            m_worldViewProjectionMatrixParameter = GetParameter("u_worldViewProjectionMatrix", true);
            m_horizontal = GetParameter("u_horizontal", true);
            m_textureSizeParameter = GetParameter("u_textureSize", true);
            m_textureParameter = GetParameter("u_texture", true);
            m_samplerStateParameter = GetParameter("u_samplerState", true);
            Transforms = new ShaderTransforms(1);
            m_frameThickness = GetParameter("u_frameThickness");
            m_frameWidth = GetParameter("u_frameWidth");
            m_frameHeight = GetParameter("u_frameHeight");
        }

        public override void PrepareForDrawingOverride() {
            Transforms.UpdateMatrices(1, false, false, true);
            m_worldViewProjectionMatrixParameter.SetValue(Transforms.WorldViewProjection, 1);
        }

    }
}