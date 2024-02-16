using Engine;

namespace Game {
    public class ASMatrixSourcesElectricElement : ASMRotateableElectricElement {
        public Matrix m_voltage;
        public int m_type;
        public SubsystemPlayers m_subsystemPlayers;

        public ASMatrixSourcesElectricElement(SubsystemASMElectricity subsystemElectricity, CellFace cellFace, int value) : base(subsystemElectricity, cellFace) {
            m_type = ASMatrixSourcesBlock.GetType(Terrain.ExtractData(value));
            m_subsystemPlayers = SubsystemElectricity.Project.FindSubsystem<SubsystemPlayers>(true);
        }

        public override Matrix GetOutputVoltage(int face) {
            return m_voltage;
        }

        public override bool Simulate() {
            Matrix voltage = m_voltage;
            switch (m_type) {
                case 0://世界坐标
                    m_voltage = Matrix.CreateTranslation(new Vector3(CellFaces[0].Point));
                    break;
                case 1:
                    SubsystemElectricity.QueueElectricElementForSimulation(this, SubsystemElectricity.CircuitStep + 1);
                    ComponentPlayer? player = m_subsystemPlayers.FindNearestPlayer(new Vector3(CellFaces[0].Point));
                    if (player != null) m_voltage = player.ComponentBody.Matrix;
                    break;
            }
            return m_voltage != voltage;
        }
    }
}