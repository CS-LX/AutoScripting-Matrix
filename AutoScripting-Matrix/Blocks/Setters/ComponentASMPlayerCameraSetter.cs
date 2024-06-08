using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game {
    public class ComponentASMPlayerCameraSetter : Component, IUpdateable, IASMGizmo {
        public UpdateOrder UpdateOrder => UpdateOrder.Default;

        public ComponentBlockEntity m_componentBlockEntity;
        public ASMPlayerCameraSetterElectricElement m_electricElement;
        public SubsystemASMElectricity m_subsystemAsmElectricity;
        public SubsystemTerrain m_subsystemTerrain;

        public int m_playerIndex = 3;

        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap) {
            base.Load(valuesDictionary, idToEntityMap);
            m_componentBlockEntity = Entity.FindComponent<ComponentBlockEntity>(true);
            m_subsystemAsmElectricity = Project.FindSubsystem<SubsystemASMElectricity>(true);
            m_subsystemTerrain = Project.FindSubsystem<SubsystemTerrain>(true);
        }

        public void Update(float dt) {
            if (m_electricElement == null) {
                FindLed();
                return;
            }
            m_playerIndex = m_electricElement.PlayerIndex;

            ASMComplexPerspectiveCamera asmComplexPerspectiveCamera = ASMPlayerCameraSetterManager.m_cameras[m_playerIndex].Item3;
            if (asmComplexPerspectiveCamera != null) {
                Matrix lastViewMatrix = asmComplexPerspectiveCamera.ViewMatrix.Invert();
                Matrix inputMatrix = m_electricElement.ViewMatrix;
                Matrix currentViewMatrix =  ASMStaticMethods.Lerp(lastViewMatrix, inputMatrix, 0.5f);
                asmComplexPerspectiveCamera.SetViewMatrix(currentViewMatrix);

                asmComplexPerspectiveCamera.m_projectionMatrix = m_electricElement.ProjectionMatrix;
            }
        }

        public void GizmosDraw(FlatBatch3D flatBatch) {
        }

        public void TopMostGizmosDraw(FlatBatch3D flatBatch) {
        }

        public void FindLed() {
            Point3 coordinates = Entity.FindComponent<ComponentBlockEntity>(true).Coordinates;
            int blockValue = m_subsystemTerrain.Terrain.GetCellValueFast(coordinates.X, coordinates.Y, coordinates.Z);
            int face = ASMRotateableMountedElectricElementBlock.GetFace_Static(blockValue);
            m_electricElement = m_subsystemAsmElectricity.GetElectricElement(coordinates.X, coordinates.Y, coordinates.Z, face) as ASMPlayerCameraSetterElectricElement;
        }
    }
}