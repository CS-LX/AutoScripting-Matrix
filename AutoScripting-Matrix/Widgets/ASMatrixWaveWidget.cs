using Engine;
using Engine.Graphics;

namespace Game
{
	public class ASMatrixWaveWidget : Widget
	{
		public float m_timeOffset;

		public Texture2D Texture
		{
			get;
			set;
		}

		public ASMatrixWaveWidget()
		{
			Texture = ContentManager.Get<Texture2D>("Textures/Gui/ASMatrix");
			m_timeOffset = new Random().Float(0f, 1000f);
		}

		public void DrawImage(DrawContext dc)
		{
			Vector2 zero = Vector2.Zero;
			Vector2 actualSize = ActualSize;
			TexturedBatch2D texturedBatch2D = dc.PrimitivesRenderer2D.TexturedBatch(Texture, useAlphaTest: false, 0, DepthStencilState.DepthWrite, null, BlendState.AlphaBlend, SamplerState.LinearWrap);
			int count = texturedBatch2D.TriangleVertices.Count;
			zero = Vector2.Zero;
			Vector2 texCoord = new Vector2(ActualSize.X / Texture.Width, 0f);
			Vector2 texCoord2 = new Vector2(ActualSize.X / Texture.Width, ActualSize.Y / Texture.Height);
			Vector2 texCoord3 = new Vector2(0f, ActualSize.Y / Texture.Height);
			texturedBatch2D.QueueQuad(zero, actualSize, 1f, Vector2.Zero, texCoord2, GlobalColorTransform);
			texturedBatch2D.TransformTriangles(GlobalTransform, count);
		}

		public void DrawSquares(DrawContext dc)
		{
			FlatBatch2D flatBatch2D = dc.PrimitivesRenderer2D.FlatBatch(1, DepthStencilState.None, null, BlendState.AlphaBlend);
			int count = flatBatch2D.LineVertices.Count;
			int count2 = flatBatch2D.TriangleVertices.Count;
			float num = (float)MathUtils.Remainder(Time.FrameStartTime + m_timeOffset, 10000.0);
			float num2 = ActualSize.X / 12f;
			float num3 = GlobalColorTransform.A / 255f;
			for (float num4 = 0f; num4 < ActualSize.X; num4 += num2)
			{
				for (float num5 = 0f; num5 < ActualSize.Y; num5 += num2)
				{
					float num6 = 0.35f * MathUtils.Pow(MathUtils.Saturate(SimplexNoise.OctavedNoise(num4 + 1000f, num5, 0.7f * num, 0.5f, 1, 2f, 1f) - 0.1f), 1f) * num3;
					float num7 = 0.7f * MathUtils.Pow(SimplexNoise.OctavedNoise(num4, num5, 0.5f * num, 0.5f, 1, 2f, 1f), 3f) * num3;
					var corner = new Vector2(num4, num5);
					var corner2 = new Vector2(num4 + num2, num5 + num2);
					if (num6 > 0.01f)
					{
						flatBatch2D.QueueRectangle(corner, corner2, 0f, new Color(0f, 0f, 0f, num6));
					}
					if (num7 > 0.01f)
					{
						flatBatch2D.QueueQuad(corner, corner2, 0f, new Color(0f, 0f, 0f, num7));
					}
				}
			}
			flatBatch2D.TransformLines(GlobalTransform, count);
			flatBatch2D.TransformTriangles(GlobalTransform, count2);
		}

		public override void MeasureOverride(Vector2 parentAvailableSize)
		{
			IsDrawRequired = true;
		}

		public override void Draw(DrawContext dc)
		{
			DrawImage(dc);
			DrawSquares(dc);
		}
	}
}
