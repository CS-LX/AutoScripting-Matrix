using Engine;

namespace Game {
    public class ASMatrixSourcesElectricElement : ASMRotateableElectricElement {
        public Matrix m_voltage_top;
        public Matrix m_voltage_left;
        public Matrix m_voltage_right;
        public int m_type;
        public SubsystemPlayers m_subsystemPlayers;
        public SubsystemGameWidgets m_subsystemViews;

        public bool m_clockAllowed;

        public ASMatrixSourcesElectricElement(SubsystemASMElectricity subsystemElectricity, CellFace cellFace, int value) : base(subsystemElectricity, cellFace) {
            m_type = ASMatrixSourcesBlock.GetType(Terrain.ExtractData(value));
            m_subsystemPlayers = SubsystemElectricity.Project.FindSubsystem<SubsystemPlayers>(true);
            m_subsystemViews = SubsystemElectricity.Project.FindSubsystem<SubsystemGameWidgets>(true);
            switch (m_type) {
                //玩家变换
                case 1:
                    Matrix? voltage = SubsystemElectricity.ReadPersistentVoltage(CellFaces[0].Point); //读取存着的电压
                    m_voltage_top = voltage.HasValue ? voltage.Value : GetPlayerTransform();
                    break;
                case 2:
                    Matrix? viewVoltage = SubsystemElectricity.ReadPersistentVoltage(CellFaces[0].Point); //读取存着的电压
                    if (viewVoltage.HasValue) m_voltage_top = viewVoltage.Value;
                    else GetPlayerViewMatrix(out m_voltage_top, out _, out _, out _);
                    break;
            }
        }

        public override Matrix GetOutputVoltage(int face) {
            ASMElectricConnectorDirection? connectorDirection = SubsystemASMElectricity.GetConnectorDirection(CellFaces[0].Face, Rotation, face);
            if (connectorDirection.HasValue) {
                switch (connectorDirection) {
                    case ASMElectricConnectorDirection.Left: return m_voltage_left;
                    case ASMElectricConnectorDirection.Right: return m_voltage_right;
                    case ASMElectricConnectorDirection.Top: return m_voltage_top;
                }
            }
            return m_voltage_top;
        }

        public override bool Simulate() {
            Matrix voltage_top = m_voltage_top;
            Matrix voltage_left = m_voltage_left;
            Matrix voltage_right = m_voltage_right;
            bool hasClockConnection = false;
            bool needClockOutput = false;
            switch (m_type) {
                case 0: //世界坐标
                    m_voltage_top = Matrix.CreateTranslation(new Vector3(CellFaces[0].Point));
                    break;
                case 1: //玩家坐标
                    foreach (ASMElectricConnection connection in Connections) {
                        if (connection.ConnectorType != ASMElectricConnectorType.Output
                            && connection.NeighborConnectorType != 0) //有时钟端输入
                        {
                            if (connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace).ToFloat() > 0) {
                                if (m_clockAllowed) {
                                    needClockOutput = true;
                                    m_clockAllowed = false;
                                }
                            }
                            else {
                                m_clockAllowed = true;
                            }
                            hasClockConnection = true;
                        }
                    }

                    if (hasClockConnection) {
                        if (needClockOutput) m_voltage_top = GetPlayerTransform();
                    }
                    else {
                        m_voltage_top = GetPlayerTransform();
                        SubsystemElectricity.QueueElectricElementForSimulation(this, SubsystemElectricity.CircuitStep + 1);
                    }
                    if (m_voltage_top != voltage_top) {
                        SubsystemElectricity.WritePersistentVoltage(CellFaces[0].Point, m_voltage_top);
                    }
                    break;
                case 2://玩家视图
                    foreach (ASMElectricConnection connection in Connections) {
                        if (connection.ConnectorType != ASMElectricConnectorType.Output
                            && connection.NeighborConnectorType != 0) //有时钟端输入
                        {
                            if (connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace).ToFloat() > 0) {
                                if (m_clockAllowed) {
                                    needClockOutput = true;
                                    m_clockAllowed = false;
                                }
                            }
                            else {
                                m_clockAllowed = true;
                            }
                            hasClockConnection = true;
                        }
                    }

                    if (hasClockConnection) {
                        if (needClockOutput) GetPlayerViewMatrix(out m_voltage_top, out _, out _, out _);
                    }
                    else {
                        GetPlayerViewMatrix(out m_voltage_top, out _, out _, out _);
                        SubsystemElectricity.QueueElectricElementForSimulation(this, SubsystemElectricity.CircuitStep + 1);
                    }
                    if (m_voltage_top != voltage_top) {
                        SubsystemElectricity.WritePersistentVoltage(CellFaces[0].Point, m_voltage_top);
                    }
                    break;
                case 3://投影视图
                    GetPlayerViewMatrix(out _, out m_voltage_left, out m_voltage_right, out _);
                    SubsystemElectricity.QueueElectricElementForSimulation(this, SubsystemElectricity.CircuitStep + 1);
                    break;
                case 4://创建位移
                    GetInputs(Rotation, out Matrix leftInput, out Matrix rightInput, out Matrix bottomInput);
                    m_voltage_top = Matrix.CreateTranslation(leftInput.ToFloat(), bottomInput.ToFloat(), rightInput.ToFloat());
                    break;
                case 5://创建旋转
                    GetInputs(Rotation, out Matrix leftInput2, out Matrix rightInput2, out Matrix bottomInput2);
                    m_voltage_top = Matrix.CreateFromYawPitchRoll(leftInput2.ToFloat(), bottomInput2.ToFloat(), rightInput2.ToFloat());
                    break;
                case 6://创建缩放
                    GetInputs(Rotation, out Matrix _, out Matrix _, out Matrix scale);
                    m_voltage_top = Matrix.CreateScale(scale.ToFloat());
                    break;
                case 7://创建三轴缩放
                    GetInputs(Rotation, out Matrix scaleX, out Matrix scaleZ, out Matrix scaleY);
                    m_voltage_top = Matrix.CreateScale(scaleX.ToFloat(), scaleY.ToFloat(), scaleZ.ToFloat());
                    break;
                case 8://创建观察矩阵
                    GetInputs(Rotation, out Matrix position, out Matrix up, out Matrix target);
                    m_voltage_top = Matrix.CreateLookAt(position.ToVector3T(), target.ToVector3T(), up.ToVector3T());
                    break;
            }
            return m_voltage_top != voltage_top || m_voltage_left != voltage_left || m_voltage_right != voltage_right;
        }

        private Matrix GetPlayerTransform() {
            Matrix transform = Matrix.Zero;
            ComponentPlayer? player = m_subsystemPlayers.FindNearestPlayer(new Vector3(CellFaces[0].Point));
            if (player != null) transform = player.ComponentBody.Matrix;
            return transform;
        }

        private void GetPlayerViewMatrix(out Matrix viewMatrix, out Matrix projectionMatrix, out Matrix screenProjectionMatrix, out Matrix viewportMatrix) {
            viewMatrix = Matrix.Zero;
            projectionMatrix = Matrix.Zero;
            screenProjectionMatrix = Matrix.Zero;
            viewportMatrix = Matrix.Zero;
            ComponentPlayer? player = m_subsystemPlayers.FindNearestPlayer(new Vector3(CellFaces[0].Point));
            if (player != null) {
                GameWidget viewGameWidget = m_subsystemViews.GameWidgets[m_subsystemPlayers.ComponentPlayers.IndexOf(player)];
                Camera viewCamera = viewGameWidget.ActiveCamera;
                if (viewCamera is BasePerspectiveCamera) {
                    BasePerspectiveCamera viewPCamera = viewCamera as BasePerspectiveCamera;
                    viewMatrix = viewPCamera.ViewMatrix;
                    screenProjectionMatrix = viewPCamera.ScreenProjectionMatrix;
                    projectionMatrix = viewPCamera.ProjectionMatrix;
                    viewportMatrix = viewPCamera.ViewportMatrix;
                }
            }
        }

        private void GetInputs(int rotation, out Matrix leftInput, out Matrix rightInput, out Matrix bottomInput) {
            leftInput = rightInput = bottomInput = Matrix.Zero;
            foreach (ASMElectricConnection connection in Connections) {
                if (connection.ConnectorType != ASMElectricConnectorType.Output
                    && connection.NeighborConnectorType != 0) {
                    ASMElectricConnectorDirection? connectorDirection = SubsystemASMElectricity.GetConnectorDirection(CellFaces[0].Face, rotation, connection.ConnectorFace);
                    if (connectorDirection.HasValue) {
                        if (connectorDirection == ASMElectricConnectorDirection.Right) {
                            rightInput = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                        }
                        else if (connectorDirection == ASMElectricConnectorDirection.Left) {
                            leftInput = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                        }
                        else if (connectorDirection == ASMElectricConnectorDirection.Bottom) {
                            bottomInput = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                        }
                    }
                }
            }
        }
    }
}