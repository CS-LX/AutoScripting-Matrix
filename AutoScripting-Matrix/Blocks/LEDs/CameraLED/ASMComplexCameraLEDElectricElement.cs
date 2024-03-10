using Engine;

namespace Game
{
	public class ASMComplexCameraLEDElectricElement : ASMRotateableElectricElement
	{
		public Matrix m_voltage_top;
		public Matrix m_voltage_bottom;
		public Matrix m_voltage_left;
		public Matrix m_voltage_right;
		public Matrix m_voltage_in;

		public int m_blockValue;

		public ASMComplexCameraLEDElectricElement(SubsystemASMElectricity subsystemElectricity, CellFace cellFace, int blockValue)
			: base(subsystemElectricity, cellFace) {
			m_blockValue = blockValue;
		}

		public Matrix GetInputMatrix() => m_voltage_top;

		public Matrix GetProjectionMatrix() => m_voltage_right;

		public Matrix GetDisplayTransformMatrix() => m_voltage_bottom;

		public Matrix GetControlMatrix() => m_voltage_left;//控制矩阵，M11: 是否自适应裁剪画面

		public override bool Simulate() {
			m_voltage_bottom = m_voltage_in = m_voltage_right = m_voltage_top = m_voltage_left = Matrix.Zero;
			foreach (ASMElectricConnection connection in Connections) {
				if (connection.ConnectorType != ASMElectricConnectorType.Output
					&& connection.NeighborConnectorType != 0) {
					ASMElectricConnectorDirection? connectorDirection = SubsystemASMElectricity.GetConnectorDirection(CellFaces[0].Face, Rotation, connection.ConnectorFace);
					if (connectorDirection.HasValue) {
						switch (connectorDirection) {
							case ASMElectricConnectorDirection.Bottom:
								m_voltage_bottom = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
								break;
							case ASMElectricConnectorDirection.In:
								m_voltage_in = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
								break;
							case ASMElectricConnectorDirection.Left:
								m_voltage_left = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
								break;
							case ASMElectricConnectorDirection.Right:
								m_voltage_right = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
								break;
							case ASMElectricConnectorDirection.Top:
								m_voltage_top = connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
								break;
						}
					}
				}
			}
			return false;
		}
	}
}
