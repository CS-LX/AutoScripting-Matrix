using Engine;

namespace Game {
    public class ASMatrixSourcesElectricElement : ASMRotateableElectricElement {
        public Matrix m_voltage_top;
        public Matrix m_voltage_left;
        public Matrix m_voltage_right;
        public Matrix m_voltage_in;
        public Matrix m_voltage_bottom;
        public int m_type;
        public SubsystemPlayers m_subsystemPlayers;
        public SubsystemGameWidgets m_subsystemViews;
        public static Random s_random = new Random();

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
                    if (viewVoltage.HasValue)
                        m_voltage_top = viewVoltage.Value;
                    else
                        GetPlayerViewMatrix(out m_voltage_top, out _, out _, out _);
                    break;
                case 9:
                    Matrix? randomVoltage = SubsystemElectricity.ReadPersistentVoltage(CellFaces[0].Point); //读取存着的电压
                    m_voltage_top = randomVoltage.HasValue ? randomVoltage.Value : s_random.Float(0, 1).ToCMatrix();
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
                    case ASMElectricConnectorDirection.In: return m_voltage_in;
                    case ASMElectricConnectorDirection.Bottom: return m_voltage_bottom;
                }
            }
            return m_voltage_top;
        }

        public override bool Simulate() {
            Matrix voltage_top = m_voltage_top;
            Matrix voltage_left = m_voltage_left;
            Matrix voltage_right = m_voltage_right;
            Matrix voltage_in = m_voltage_in;
            Matrix voltage_bottom = m_voltage_bottom;
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
                case 2: //玩家视图
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
                case 3: //投影视图
                    GetPlayerViewMatrix(out _, out m_voltage_left, out m_voltage_right, out _);
                    SubsystemElectricity.QueueElectricElementForSimulation(this, SubsystemElectricity.CircuitStep + 1);
                    break;
                case 4: //创建位移
                    GetInputs(
                        Rotation,
                        out Matrix leftInput,
                        out Matrix rightInput,
                        out Matrix bottomInput,
                        out _,
                        out Matrix inInput
                    );
                    m_voltage_top = Matrix.CreateTranslation(leftInput.ToFloat() + inInput.M11, bottomInput.ToFloat() + inInput.M12, rightInput.ToFloat() + inInput.M13);
                    break;
                case 5: //创建旋转
                    GetInputs(
                        Rotation,
                        out Matrix leftInput2,
                        out Matrix rightInput2,
                        out Matrix bottomInput2,
                        out _,
                        out Matrix inInput2
                    );
                    m_voltage_top = Matrix.CreateFromYawPitchRoll(leftInput2.ToFloat() + inInput2.M11, bottomInput2.ToFloat() + inInput2.M12, rightInput2.ToFloat() + inInput2.M13);
                    break;
                case 6: //创建缩放
                    GetInputs(
                        Rotation,
                        out Matrix _,
                        out Matrix _,
                        out Matrix scale,
                        out _,
                        out _
                    );
                    m_voltage_top = Matrix.CreateScale(scale.ToFloat());
                    break;
                case 7: //创建三轴缩放
                    GetInputs(
                        Rotation,
                        out Matrix scaleX,
                        out Matrix scaleZ,
                        out Matrix scaleY,
                        out _,
                        out Matrix scaleXYZ
                    );
                    m_voltage_top = Matrix.CreateScale(scaleX.ToFloat() + scaleXYZ.M11, scaleY.ToFloat() + scaleXYZ.M12, scaleZ.ToFloat() + scaleXYZ.M13);
                    break;
                case 8: //创建观察矩阵
                    GetInputs(
                        Rotation,
                        out Matrix position,
                        out Matrix up,
                        out Matrix target,
                        out _,
                        out _
                    );
                    m_voltage_top = Matrix.CreateLookAt(position.ToVector3T(), target.ToVector3T(), up.ToVector3T());
                    break;
                case 9: //随机数
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
                        if (needClockOutput) m_voltage_top = s_random.Float(0, 1).ToCMatrix();
                    }
                    else {
                        m_voltage_top = s_random.Float(0, 1).ToCMatrix();
                        SubsystemElectricity.QueueElectricElementForSimulation(this, SubsystemElectricity.CircuitStep + 1);
                    }
                    if (m_voltage_top != voltage_top) {
                        SubsystemElectricity.WritePersistentVoltage(CellFaces[0].Point, m_voltage_top);
                    }
                    break;
                case 10: //二阶方阵转矩阵
                    GetInputs(
                        Rotation,
                        out Matrix left,
                        out Matrix right,
                        out Matrix bottom,
                        out Matrix top,
                        out _
                    );
                    m_voltage_in = new Matrix(
                        top.M11,
                        top.M12,
                        right.M11,
                        right.M12,
                        top.M21,
                        top.M22,
                        right.M21,
                        right.M22,
                        bottom.M11,
                        bottom.M12,
                        left.M11,
                        left.M12,
                        bottom.M21,
                        bottom.M22,
                        left.M21,
                        left.M22
                    );
                    break;
                case 11: //四维横向量转矩阵
                    GetInputs(
                        Rotation,
                        out Matrix left2,
                        out Matrix right2,
                        out Matrix bottom2,
                        out Matrix top2,
                        out _
                    );
                    m_voltage_in = new Matrix(
                        top2.M11,
                        top2.M12,
                        top2.M13,
                        top2.M14,
                        right2.M11,
                        right2.M12,
                        right2.M13,
                        right2.M14,
                        bottom2.M11,
                        bottom2.M12,
                        bottom2.M13,
                        bottom2.M14,
                        left2.M11,
                        left2.M12,
                        left2.M13,
                        left2.M14
                    );
                    break;
                case 12: //浮点转四维横向量
                    GetInputs(
                        Rotation,
                        out Matrix left3,
                        out Matrix right3,
                        out Matrix bottom3,
                        out Matrix top3,
                        out _
                    );
                    m_voltage_in = new Matrix(
                        top3.ToFloat(),
                        right3.ToFloat(),
                        bottom3.ToFloat(),
                        left3.ToFloat(),
                        0,
                        0,
                        0,
                        0,
                        0,
                        0,
                        0,
                        0,
                        0,
                        0,
                        0,
                        0
                    );
                    break;
                case 13: //正交
                    GetInputs(
                        Rotation,
                        out Matrix farPlane,
                        out Matrix height,
                        out Matrix nearPlane,
                        out Matrix width,
                        out _
                    );
                    m_voltage_in = Matrix.CreateOrthographic(width.ToFloat(), height.ToFloat(), nearPlane.ToFloat(), farPlane.ToFloat());
                    break;
                case 14: //透视
                    GetInputs(
                        Rotation,
                        out Matrix farPlane2,
                        out Matrix aspectRatio,
                        out Matrix nearPlane2,
                        out Matrix fieldOfViewY,
                        out _
                    );
                    m_voltage_in = Matrix.CreateOrthographic(fieldOfViewY.ToFloat(), aspectRatio.ToFloat(), nearPlane2.ToFloat(), farPlane2.ToFloat());
                    break;
                case 15: //实时钟
                    GetInputs(Rotation, out _, out _, out _, out _, out Matrix clockControlMatrix);
                    switch (MathUtils.Floor(clockControlMatrix.M11)) {
                        case 0:
                            m_voltage_top = DateTime.Now.Millisecond.ToCMatrix();
                            m_voltage_right = DateTime.Now.Second.ToCMatrix();
                            m_voltage_bottom = DateTime.Now.Minute.ToCMatrix();
                            m_voltage_left = DateTime.Now.Hour.ToCMatrix();
                            break;
                        case 1:
                            m_voltage_top = ((int)DateTime.Now.DayOfWeek).ToCMatrix();
                            m_voltage_right = DateTime.Now.Day.ToCMatrix();
                            m_voltage_bottom = DateTime.Now.Month.ToCMatrix();
                            m_voltage_left = DateTime.Now.Year.ToCMatrix();
                            break;
                    }
                    SubsystemElectricity.QueueElectricElementForSimulation(this, SubsystemElectricity.CircuitStep + 1);
                    break;
            }
            return m_voltage_top != voltage_top || m_voltage_left != voltage_left || m_voltage_right != voltage_right || m_voltage_in != voltage_in || m_voltage_bottom != voltage_bottom;
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

        private void GetInputs(int rotation, out Matrix leftInput, out Matrix rightInput, out Matrix bottomInput, out Matrix topInput, out Matrix inInput) {
            leftInput = rightInput = bottomInput = topInput = inInput = Matrix.Zero;
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
                        else if (connectorDirection == ASMElectricConnectorDirection.Top) {
                            topInput = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                        }
                        else if (connectorDirection == ASMElectricConnectorDirection.In) {
                            inInput = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
                        }
                    }
                }
            }
        }
    }
}