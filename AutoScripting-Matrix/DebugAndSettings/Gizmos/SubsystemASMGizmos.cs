

using System.Globalization;
using System.Text;
using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game {
    public class SubsystemASMGizmos : Subsystem, IDrawable, IUpdateable {

        public int[] DrawOrders => [2000];

        public PrimitivesRenderer3D m_primitivesRenderer = new PrimitivesRenderer3D();

        public FlatBatch3D m_normalBatch;

        public FlatBatch3D m_topMostBatch;

        public SubsystemASMElectricity m_subsystemASMElectricity;

        public SubsystemBlockEntities m_subsystemBlockEntities;

        public Dictionary<IASMGizmo, Point3> m_ignoredGizmos = new Dictionary<IASMGizmo, Point3>();

        Point3[] m_ignoredPoints;

        public void Draw(Camera camera, int drawOrder) {
            if(!ASMSettingsManager.Get<bool>("GizmosEnable")) return;
            foreach (var element in m_subsystemASMElectricity.m_electricElements) {
                if (element.Key is IASMGizmo elementGizmos && !m_ignoredGizmos.Keys.Contains(elementGizmos)) {
                    elementGizmos.GizmosDraw(m_normalBatch);
                    elementGizmos.TopMostGizmosDraw(m_topMostBatch);
                }
            }
            foreach (var entity in GameManager.Project.Entities) {
                foreach (var component in entity.Components) {
                    if (component is IASMGizmo componentGizmos && !m_ignoredGizmos.Keys.Contains(componentGizmos)) {
                        componentGizmos.GizmosDraw(m_normalBatch);
                        componentGizmos.TopMostGizmosDraw(m_topMostBatch);
                    }
                }
            }
            m_primitivesRenderer.Flush(camera.ViewProjectionMatrix);
        }

        public void IgnoreGizmos(IASMGizmo gizmos, Point3 position) {
            if(!m_ignoredGizmos.Keys.Contains(gizmos)) m_ignoredGizmos.Add(gizmos, position);
        }

        public void UnignoreGizmos(IASMGizmo gizmos) {
            if (m_ignoredGizmos.Keys.Contains(gizmos)) m_ignoredGizmos.Remove(gizmos);
        }

        public bool IsIgnore(IASMGizmo gizmos) => m_ignoredGizmos.Keys.Contains(gizmos);

        public bool IsIgnore(Point3 position) => m_ignoredGizmos.Values.Contains(position);

        public void IgnoreAll() {
            if(!CanOperate()) return;
            foreach (var element in m_subsystemASMElectricity.m_electricElementsByCellFace) {
                if (element.Value is IASMGizmo elementGizmos && !m_ignoredGizmos.ContainsKey(elementGizmos)) {
                    m_ignoredGizmos.Add(elementGizmos, new Point3(element.Key.X, element.Key.Y, element.Key.Z));
                }
            }
            foreach (var entity in GameManager.Project.Entities) {
                foreach (var component in entity.Components) {
                    ComponentBlockEntity componentBlockEntity = entity.FindComponent<ComponentBlockEntity>();
                    if (component is IASMGizmo componentGizmos && !m_ignoredGizmos.ContainsKey(componentGizmos) && componentBlockEntity != null) {
                        m_ignoredGizmos.Add(componentGizmos, componentBlockEntity.Coordinates);
                    }
                }
            }
        }

        public void UnignoreAll() {
            if(!CanOperate()) return;
            m_ignoredGizmos.Clear();
        }

        public bool CanOperate() => m_ignoredPoints == null;

        public override void Load(ValuesDictionary valuesDictionary) {
            base.Load(valuesDictionary);
            m_normalBatch = m_primitivesRenderer.FlatBatch();
            m_topMostBatch = m_primitivesRenderer.FlatBatch(0, DepthStencilState.None);
            m_subsystemASMElectricity = Project.FindSubsystem<SubsystemASMElectricity>(true);
            m_subsystemBlockEntities = Project.FindSubsystem<SubsystemBlockEntities>(true);

            try {
                string ignoredGizmos = valuesDictionary.GetValue<string>("IgnoredGizmos");
                string[] pointsText = ignoredGizmos.Split('|', StringSplitOptions.RemoveEmptyEntries);
                m_ignoredPoints = pointsText.Select(ASMStaticMethods.ParseToPoint).ToArray();
            }
            catch (Exception e) {
                Log.Warning($"Gizmos管理器: 加载已忽略的Gizmos出错，原因: {e}");
            }

        }

        private KeyValuePair<Component, Point3>[] FindComponentByPoints(Point3[] points) {
            List<KeyValuePair<Component, Point3>> components = new List<KeyValuePair<Component, Point3>>();
            foreach (var entity in GameManager.Project.Entities) {
                foreach (var component in entity.Components) {
                    ComponentBlockEntity componentBlockEntity = component.Entity.FindComponent<ComponentBlockEntity>();
                    if (componentBlockEntity != null
                        && points.Contains(componentBlockEntity.Coordinates)
                        && component is not ComponentBlockEntity)
                        components.Add(new KeyValuePair<Component, Point3>(component, componentBlockEntity.Coordinates));
                }
            }
            return components.ToArray();
        }

        //private KeyValuePair<ASMElectricElement, Point3>[] FindElectricElementByPoints(Point3[] points) => m_subsystemASMElectricity.m_electricElementsByCellFace.Where((pair) => points.Contains(pair.Key.Point)).Select(pair => new KeyValuePair<ASMElectricElement,Point3>(pair.Value, pair.Key.Point)).ToArray();

        private KeyValuePair<ASMElectricElement, Point3>[] FindElectricElementByPoints(Point3[] points) {
            List<KeyValuePair<ASMElectricElement, Point3>> results = new List<KeyValuePair<ASMElectricElement, Point3>>();
            foreach (var electricElementByCell in m_subsystemASMElectricity.m_electricElementsByCellFace) {
                if (points.Contains(electricElementByCell.Key.Point)) {
                    results.Add(new KeyValuePair<ASMElectricElement, Point3>(electricElementByCell.Value, electricElementByCell.Key.Point));
                }
            }
            return results.ToArray();
        }



        public override void Save(ValuesDictionary valuesDictionary) {
            base.Save(valuesDictionary);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (KeyValuePair<IASMGizmo, Point3> gizmo in m_ignoredGizmos) {
                stringBuilder.Append(gizmo.Value.ToString());
                stringBuilder.Append('|');
            }
            valuesDictionary.SetValue("IgnoredGizmos", stringBuilder.ToString());
        }

        public void Update(float dt) {
            //因为load方法执行时Components还没有加载，所以component获取放在update里面
            if (m_ignoredPoints != null
                && m_ignoredPoints.Length > 0
                && m_ignoredPoints.Length != m_ignoredGizmos.Count) {
                try {
                    KeyValuePair<ASMElectricElement, Point3>[] elements = FindElectricElementByPoints(m_ignoredPoints);
                    var gizmos2 = elements.Where(pair => pair.Key is IASMGizmo).Select(pair => new KeyValuePair<IASMGizmo,Point3>(pair.Key as IASMGizmo, pair.Value)).ToArray();
                    foreach (var gizmo2 in gizmos2) {
                        if(!m_ignoredGizmos.ContainsKey(gizmo2.Key)) m_ignoredGizmos.Add(gizmo2.Key, gizmo2.Value);
                    }

                    KeyValuePair<Component, Point3>[] components = FindComponentByPoints(m_ignoredPoints);
                    var gizmos1 = components.Where(pair => pair.Key is IASMGizmo).Select(pair => new KeyValuePair<IASMGizmo, Point3>(pair.Key as IASMGizmo, pair.Value)).ToArray();
                    foreach (var gizmo1 in gizmos1) {
                        if(!m_ignoredGizmos.ContainsKey(gizmo1.Key)) m_ignoredGizmos.Add(gizmo1.Key, gizmo1.Value);
                    }
                    if(m_ignoredPoints.Length == m_ignoredGizmos.Count) m_ignoredPoints = null;
                }
                catch (Exception e) {
                    Log.Warning($"Gizmos管理器: 加载已忽略的Gizmos出错，原因: {e}");
                }
            }
            else if(m_ignoredPoints != null && m_ignoredPoints.Length == 0) {
                m_ignoredPoints = null;
            }
        }

        public UpdateOrder UpdateOrder => UpdateOrder.Default;
    }
}