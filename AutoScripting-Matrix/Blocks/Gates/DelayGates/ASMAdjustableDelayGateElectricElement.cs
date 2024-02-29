namespace Game {
    public class ASMAdjustableDelayGateElectricElement : ASMDelayGateBaseElectricElement {

        public int m_delaySteps;
        public SubsystemASMDelayGateBlockBehavior m_delayGateBehavior;


        public override int DelaySteps => m_delaySteps;

        public ASMAdjustableDelayGateElectricElement(SubsystemASMElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace) {
            m_delayGateBehavior = subsystemElectricity.Project.FindSubsystem<SubsystemASMDelayGateBlockBehavior>(true);
            AppendDelay();
        }

        public void AppendDelay() {
            if (m_delayGateBehavior.GetBlockData(CellFaces[0].Point) != null) {
                m_delaySteps = m_delayGateBehavior.GetBlockData(CellFaces[0].Point).Data;
            }
            else {
                m_delaySteps = 0;
            }
        }
    }
}