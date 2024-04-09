

using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game {
    public class SubsystemASMGizmos : Subsystem, IDrawable {

        public int[] DrawOrders => [1];

        public PrimitivesRenderer3D m_primitivesRenderer = new PrimitivesRenderer3D();

        public FlatBatch3D m_normalBatch;

        public FlatBatch3D m_topMostBatch;

        public SubsystemASMElectricity m_subsystemASMElectricity;

        public SubsystemBlockEntities m_subsystemBlockEntities;

        public void Draw(Camera camera, int drawOrder) {
            foreach (var element in m_subsystemASMElectricity.m_electricElements) {
                if (element.Key is IASMGizmos elementGizmos) {
                    elementGizmos.GizmosDraw(m_normalBatch);
                    elementGizmos.TopMostGizmosDraw(m_topMostBatch);
                }
            }
            foreach (var entity in GameManager.Project.Entities) {
                foreach (var component in entity.Components) {
                    if (component is IASMGizmos componentGizmos) {
                        componentGizmos.GizmosDraw(m_normalBatch);
                        componentGizmos.TopMostGizmosDraw(m_topMostBatch);
                    }
                }
            }
            m_primitivesRenderer.Flush(camera.ViewProjectionMatrix);
        }

        public override void Load(ValuesDictionary valuesDictionary) {
            base.Load(valuesDictionary);
            m_normalBatch = m_primitivesRenderer.FlatBatch();
            m_topMostBatch = m_primitivesRenderer.FlatBatch(0, DepthStencilState.None);
            m_subsystemASMElectricity = Project.FindSubsystem<SubsystemASMElectricity>(true);
            m_subsystemBlockEntities = Project.FindSubsystem<SubsystemBlockEntities>(true);
        }
    }
}