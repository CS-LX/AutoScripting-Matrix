using Engine;

namespace Game {
    public class ASMGlowPoint {
        public Vector3 Position;

        public Vector3 Right;

        public Vector3 Up;

        public Vector3 Forward;

        public Color Color;

        public float Size;

        public float FarSize;

        public float FarDistance;

        public GlowPointType Type;
    }

    public class ASMGlowBlock {
        public int index;

        public Matrix transform;

        public bool environmentallySusceptible;
    }
}