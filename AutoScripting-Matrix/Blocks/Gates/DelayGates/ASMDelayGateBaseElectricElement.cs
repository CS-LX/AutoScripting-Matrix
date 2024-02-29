using System.Collections.Generic;
using Engine;

namespace Game {
    public abstract class ASMDelayGateBaseElectricElement : ASMRotateableElectricElement {
        public Matrix m_voltage;

        public Matrix m_lastStoredVoltage;

        public Dictionary<int, Matrix> m_voltagesHistory = new Dictionary<int, Matrix>();

        public abstract int DelaySteps { get; }

        public ASMDelayGateBaseElectricElement(SubsystemASMElectricity subsystemElectricity, CellFace cellFace) : base(subsystemElectricity, cellFace) { }

        public override Matrix GetOutputVoltage(int face) {
            return m_voltage;
        }

        public override bool Simulate() {
            Matrix voltage = m_voltage;
            int delaySteps = DelaySteps;
            Matrix num = Matrix.Zero;
            foreach (ASMElectricConnection connection in Connections) {
                if (connection.ConnectorType != ASMElectricConnectorType.Output
                    && connection.NeighborConnectorType != 0) {
                    num = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                    break;
                }
            }
            if (delaySteps > 0) {
                if (m_voltagesHistory.TryGetValue(SubsystemElectricity.CircuitStep, out Matrix value)) {
                    m_voltage = value;
                    m_voltagesHistory.Remove(SubsystemElectricity.CircuitStep);
                }
                if (num != m_lastStoredVoltage) {
                    m_lastStoredVoltage = num;
                    if (m_voltagesHistory.Count < 300) {
                        m_voltagesHistory[SubsystemElectricity.CircuitStep + DelaySteps] = num;
                        SubsystemElectricity.QueueElectricElementForSimulation(this, SubsystemElectricity.CircuitStep + DelaySteps);
                    }
                }
            }
            else {
                m_voltage = num;
            }
            return m_voltage != voltage;
        }
    }
}