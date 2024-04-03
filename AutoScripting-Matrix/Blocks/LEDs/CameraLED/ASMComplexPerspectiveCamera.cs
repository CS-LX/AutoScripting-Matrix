using Engine;
using Engine.Graphics;

namespace Game {
	public class ASMComplexPerspectiveCamera : BasePerspectiveCamera {
		public override bool UsesMovementControls => false;

		public override bool IsEntityControlEnabled => true;

		public Matrix m_projectionMatrix = Matrix.Zero;

		public RenderTarget2D ViewTexture = new RenderTarget2D(Window.Size.X, Window.Size.Y, 1, ColorFormat.Rgba8888, DepthFormat.Depth24Stencil8);

		public override void Activate(Camera previousCamera) => SetupPerspectiveCamera(previousCamera.ViewPosition, previousCamera.ViewDirection, previousCamera.ViewUp);

		public override Matrix ProjectionMatrix => m_projectionMatrix;

		public void SetViewMatrix(Matrix viewMatrix) {
			m_viewMatrix = null;
			SetupPerspectiveCamera(viewMatrix.Translation, viewMatrix.Forward, viewMatrix.Up);
		}

		public ASMComplexPerspectiveCamera(GameWidget gameWidget, Matrix projectionMatrix) : base(gameWidget) {
			m_projectionMatrix = projectionMatrix; }

		public override void Update(float dt) { }
	}
}