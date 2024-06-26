using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game {
    public class SubsystemASMGizmosControllerBehavior : SubsystemBlockBehavior {
        public override int[] HandledBlocks => [ASMGizmosControllerBlock.Index];

        public SubsystemASMElectricity m_subsystemASMElectricity;

        public SubsystemASMGizmos m_subsystemGizmos;

        public override bool OnUse(Ray3 ray, ComponentMiner componentMiner) {
            object obj = componentMiner.Raycast(ray, RaycastMode.Interaction);
            if (obj is TerrainRaycastResult) {
                CellFace cellFace = ((TerrainRaycastResult)obj).CellFace;
                if (m_subsystemASMElectricity.m_electricElementsByCellFace.ContainsKey(cellFace)) {
                    if (m_subsystemASMElectricity.m_electricElementsByCellFace[cellFace] is IASMGizmo gizmos) {
                        if (m_subsystemGizmos.IsIgnore(gizmos)) {
                            m_subsystemGizmos.UnignoreGizmos(gizmos);
                            componentMiner.ComponentPlayer.ComponentGui.DisplaySmallMessage($"已显示位于{cellFace}的Gizmos", Color.White, false, false);
                        }
                        else {
                            m_subsystemGizmos.IgnoreGizmos(gizmos, cellFace.Point);
                            componentMiner.ComponentPlayer.ComponentGui.DisplaySmallMessage($"已隐藏位于{cellFace}的Gizmos", Color.White, false, false);
                        }
                        return true;
                    }
                }
                Component component = FindComponent(cellFace.Point);
                if (component is IASMGizmo componentGizmos) {
                    if (m_subsystemGizmos.IsIgnore(componentGizmos)) {
                        m_subsystemGizmos.UnignoreGizmos(componentGizmos);
                        componentMiner.ComponentPlayer.ComponentGui.DisplaySmallMessage($"已显示位于{cellFace}的Gizmos", Color.White, false, false);
                    }
                    else {
                        m_subsystemGizmos.IgnoreGizmos(componentGizmos, cellFace.Point);
                        componentMiner.ComponentPlayer.ComponentGui.DisplaySmallMessage($"已隐藏位于{cellFace}的Gizmos", Color.White, false, false);
                    }
                    return true;
                }
            }
            return false;
        }

        public override bool OnEditInventoryItem(IInventory inventory, int slotIndex, ComponentPlayer componentPlayer) {
            DialogsManager.ShowDialog(componentPlayer.GuiWidget, new EditASMGizmosDialog(Project.FindSubsystem<SubsystemASMGizmos>(true)));
            return true;
        }

        public Component FindComponent(Point3 point) {
            foreach (var entity in GameManager.Project.Entities) {
                foreach (var component in entity.Components) {
                    ComponentBlockEntity componentBlockEntity = component.Entity.FindComponent<ComponentBlockEntity>();
                    if (componentBlockEntity != null
                        && componentBlockEntity.Coordinates == point
                        && component is not ComponentBlockEntity)
                        return component;
                }
            }
            return null;
        }

        public override void Load(ValuesDictionary valuesDictionary) {
            base.Load(valuesDictionary);
            m_subsystemASMElectricity = Project.FindSubsystem<SubsystemASMElectricity>(true);
            m_subsystemGizmos = Project.FindSubsystem<SubsystemASMGizmos>(true);
        }
    }
}