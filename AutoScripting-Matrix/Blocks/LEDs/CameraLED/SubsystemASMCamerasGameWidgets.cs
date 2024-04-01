using GameEntitySystem;
using TemplatesDatabase;

namespace Game {
    public class SubsystemASMCamerasGameWidgets : Subsystem {
        public SubsystemGameWidgets m_subsystemGameWidgets;
        public readonly HashSet<GameWidget> m_gameWidgets = new();

        public override void Load(ValuesDictionary valuesDictionary) {
            base.Load(valuesDictionary);
            m_subsystemGameWidgets = Project.FindSubsystem<SubsystemGameWidgets>(true);
            int index = Project.m_subsystems.IndexOf(m_subsystemGameWidgets);
            Project.m_subsystems.Remove(this);
            Project.m_subsystems.Insert(index - 1, this);
            //退出存档时，SubsystemGameWidgets会从界面移除所有GameWidget，而我加的GameWidget不在界面中，因此而报错
            //所以我需要在SubsystemGameWidgets移除所有GameWidget前，把我加的GameWidget移除
        }

        public override void Dispose() {
            foreach (GameWidget widget in m_gameWidgets) {
                m_subsystemGameWidgets.m_gameWidgets.Remove(widget);
            }
        }
    }
}