

using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game {
    public class SubsystemASMGizmos : Subsystem, IDrawable {

        public int[] DrawOrders => [2000];

        public PrimitivesRenderer3D m_primitivesRenderer = new PrimitivesRenderer3D();

        public FlatBatch3D m_normalBatch;

        public FlatBatch3D m_topMostBatch;

        public SubsystemASMElectricity m_subsystemASMElectricity;

        public SubsystemBlockEntities m_subsystemBlockEntities;

        public List<IASMGizmos> m_ignoredGizmos = new List<IASMGizmos>();

        public void Draw(Camera camera, int drawOrder) {
            if(!(bool)ASMSettingsManager.Get("GizmosEnable")) return;
            foreach (var element in m_subsystemASMElectricity.m_electricElements) {
                if (element.Key is IASMGizmos elementGizmos && !m_ignoredGizmos.Contains(elementGizmos)) {
                    elementGizmos.GizmosDraw(m_normalBatch);
                    elementGizmos.TopMostGizmosDraw(m_topMostBatch);
                }
            }
            foreach (var entity in GameManager.Project.Entities) {
                foreach (var component in entity.Components) {
                    if (component is IASMGizmos componentGizmos && !m_ignoredGizmos.Contains(componentGizmos)) {
                        componentGizmos.GizmosDraw(m_normalBatch);
                        componentGizmos.TopMostGizmosDraw(m_topMostBatch);
                    }
                }
            }
            m_primitivesRenderer.Flush(camera.ViewProjectionMatrix);
        }

        public void IgnoreGizmos(IASMGizmos gizmos) {
            if(!m_ignoredGizmos.Contains(gizmos)) m_ignoredGizmos.Add(gizmos);
        }

        public void UnignoreGizmos(IASMGizmos gizmos) {
            if (m_ignoredGizmos.Contains(gizmos)) m_ignoredGizmos.Remove(gizmos);
        }

        public bool IsIgnore(IASMGizmos gizmos) => m_ignoredGizmos.Contains(gizmos);

        public void IgnoreAll() {
            foreach (var element in m_subsystemASMElectricity.m_electricElements) {
                if (element.Key is IASMGizmos elementGizmos && !m_ignoredGizmos.Contains(elementGizmos)) {
                    m_ignoredGizmos.Add(elementGizmos);
                }
            }
            foreach (var entity in GameManager.Project.Entities) {
                foreach (var component in entity.Components) {
                    if (component is IASMGizmos componentGizmos && !m_ignoredGizmos.Contains(componentGizmos)) {
                        m_ignoredGizmos.Add(componentGizmos);
                    }
                }
            }
        }

        public void UnignoreAll() {
            m_ignoredGizmos.Clear();
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