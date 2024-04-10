using Engine.Graphics;

namespace Game {
    public interface IASMGizmo {
        public abstract void GizmosDraw(FlatBatch3D flatBatch);
        public abstract void TopMostGizmosDraw(FlatBatch3D flatBatch);
    }
}