using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game {
    /// <summary>
    /// 用于处理一些杂项
    /// </summary>
    public class SubsystemASMManager : Subsystem, IUpdateable {
        public static string CategoryName => "[智械] 矩阵";

        public SubsystemPlayers m_subsystemPlayers;

        public bool m_isCreativeMode;

        public override void Load(ValuesDictionary valuesDictionary) {
            base.Load(valuesDictionary);
            m_isCreativeMode = Project.FindSubsystem<SubsystemGameInfo>(true).WorldSettings.GameMode == GameMode.Creative;
            if (m_isCreativeMode) {
                m_subsystemPlayers = Project.FindSubsystem<SubsystemPlayers>(true);
            }
        }

        public void Update(float dt) {
            try {
                if (m_isCreativeMode) {
                    foreach (ComponentPlayer componentPlayer in m_subsystemPlayers.ComponentPlayers) {
                        if (componentPlayer.ComponentGui.ModalPanelWidget is CreativeInventoryWidget widget) {
                            foreach (CreativeInventoryWidget.Category c in widget.m_categories) {
                                if (c.Name == CategoryName) {
                                    c.Color = new Color(137, 255, 128);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        public UpdateOrder UpdateOrder => UpdateOrder.Default;
    }
}