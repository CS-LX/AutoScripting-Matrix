using Engine;
using Engine.Graphics;

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

    public interface IASMGlowGeometry {
        public abstract void Draw(FlatBatch3D flatBatch3D);
    }

    public class ASMGlowCuboid : IASMGlowGeometry {
        public Vector3 m_start;
        public Vector3 m_end;
        public Color m_color;

        public void Draw(FlatBatch3D flatBatch3D) {
            flatBatch3D.QueueBoundingBox(new BoundingBox(m_start, m_end), m_color);
        }
    }

    public class ASMGlowLine : IASMGlowGeometry {
        public Vector3 m_start;
        public Vector3 m_end;
        public Color m_color1;
        public Color m_color2;

        public void Draw(FlatBatch3D flatBatch3D) {
            flatBatch3D.QueueLine(m_start, m_end, m_color1, m_color2);
        }
    }
}