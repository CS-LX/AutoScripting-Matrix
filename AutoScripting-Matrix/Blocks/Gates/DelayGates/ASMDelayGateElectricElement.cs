namespace Game {
    public class ASMDelayGateElectricElement : ASMDelayGateBaseElectricElement {
        public int? m_delaySteps;

        public int m_lastDelayCalculationStep;

        public static int[] m_delaysByPredecessorsCount = new int[3] { 20, 80, 400 };

        public override int DelaySteps {
            get {
                if (SubsystemElectricity.CircuitStep - m_lastDelayCalculationStep > 50) {
                    m_delaySteps = null;
                }
                if (!m_delaySteps.HasValue) {
                    int count = 0;
                    CountDelayPredecessors(this, ref count);
                    m_delaySteps = m_delaysByPredecessorsCount[count];
                    m_lastDelayCalculationStep = SubsystemElectricity.CircuitStep;
                }
                return m_delaySteps.Value;
            }
        }

        public ASMDelayGateElectricElement(SubsystemASMElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace) { }

        public static void CountDelayPredecessors(ASMDelayGateElectricElement delayGate, ref int count) {
            if (count < 2) {
                foreach (ASMElectricConnection connection in delayGate.Connections) {
                    if (connection.ConnectorType == ASMElectricConnectorType.Input) {
                        var delayGateElectricElement = connection.NeighborElectricElement as ASMDelayGateElectricElement;
                        if (delayGateElectricElement != null) {
                            count++;
                            CountDelayPredecessors(delayGateElectricElement, ref count);
                            break;
                        }
                    }
                }
            }
        }
    }
}