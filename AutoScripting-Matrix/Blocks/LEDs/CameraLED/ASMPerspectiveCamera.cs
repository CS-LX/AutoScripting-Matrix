using Engine;
using Engine.Graphics;

namespace Game {
	public class ASMPerspectiveCamera : BasePerspectiveCamera {
		public override bool UsesMovementControls => false;

		public override bool IsEntityControlEnabled => true;

		public RenderTarget2D ViewTexture = new RenderTarget2D(Window.Size.X, Window.Size.Y, 1, ColorFormat.Rgba8888, DepthFormat.Depth24Stencil8);

		public override void Activate(Camera previousCamera) => SetupPerspectiveCamera(previousCamera.ViewPosition, previousCamera.ViewDirection, previousCamera.ViewUp);

		public void SetViewMatrix(Matrix viewMatrix) {
			SetupPerspectiveCamera(viewMatrix.Translation, viewMatrix.Forward, viewMatrix.Up);
		}

		public ASMPerspectiveCamera(GameWidget gameWidget) : base(gameWidget) { }

		public override void Update(float dt) { }
	}
}