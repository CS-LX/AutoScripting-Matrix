using Engine;

namespace Game {
    public class ASMatrixSourcesElectricElement : ASMRotateableElectricElement {
        public Matrix m_voltage;
        public int m_type;
        public SubsystemPlayers m_subsystemPlayers;

        //玩家坐标
        public bool m_clockAllowed_value1;

        public ASMatrixSourcesElectricElement(SubsystemASMElectricity subsystemElectricity, CellFace cellFace, int value) : base(subsystemElectricity, cellFace) {
            m_type = ASMatrixSourcesBlock.GetType(Terrain.ExtractData(value));
            m_subsystemPlayers = SubsystemElectricity.Project.FindSubsystem<SubsystemPlayers>(true);
            switch (m_type) {
                //玩家变换
                case 1:
                    Matrix? voltage = SubsystemElectricity.ReadPersistentVoltage(CellFaces[0].Point); //读取存着的电压
                    m_voltage = voltage.HasValue ? voltage.Value : GetPlayerTransform();
                    break;
            }
        }

        public override Matrix GetOutputVoltage(int face) {
            return m_voltage;
        }

        public override bool Simulate() {
            Matrix voltage = m_voltage;
            switch (m_type) {
                case 0: //世界坐标
                    m_voltage = Matrix.CreateTranslation(new Vector3(CellFaces[0].Point));
                    break;
                case 1: //玩家坐标
                    bool hasClockConnection = false;
                    bool needClockOutput = false;
                    foreach (ASMElectricConnection connection in Connections) {
                        if (connection.ConnectorType != ASMElectricConnectorType.Output
                            && connection.NeighborConnectorType != 0) //有时钟端输入
                        {
                            if (connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace).ToFloat() > 0) {
                                if (m_clockAllowed_value1) {
                                    needClockOutput = true;
                                    m_clockAllowed_value1 = false;
                                }
                            }
                            else {
                                m_clockAllowed_value1 = true;
                            }
                            hasClockConnection = true;
                        }
                    }
                    //输出信号
                    if (hasClockConnection) {
                        if (needClockOutput) m_voltage = GetPlayerTransform();
                    }
                    else {
                        m_voltage = GetPlayerTransform();
                        SubsystemElectricity.QueueElectricElementForSimulation(this, SubsystemElectricity.CircuitStep + 1);
                    }
                    if (m_voltage != voltage) {
                        SubsystemElectricity.WritePersistentVoltage(CellFaces[0].Point, m_voltage);
                    }
                    break;
            }
            return m_voltage != voltage;
        }

        private Matrix GetPlayerTransform() {
            Matrix transform = Matrix.Zero;
            ComponentPlayer? player = m_subsystemPlayers.FindNearestPlayer(new Vector3(CellFaces[0].Point));
            if (player != null) transform = player.ComponentBody.Matrix;
            return transform;
        }
    }
}