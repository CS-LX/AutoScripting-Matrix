using Engine;
using Engine.Graphics;

namespace Game {
    public static class ASMGizmosDrawUtils {
        public static void QueueCoordinate(this FlatBatch3D flatBatch3D, Matrix transform, float axesLength = 1) {
            flatBatch3D.QueueLine(transform.Translation, transform.Translation + transform.Right * axesLength, Color.Red);
            flatBatch3D.QueueLine(transform.Translation, transform.Translation + transform.Up * axesLength, Color.Green);
            flatBatch3D.QueueLine(transform.Translation, transform.Translation + transform.Forward * axesLength, Color.Blue);
        }
    }
}