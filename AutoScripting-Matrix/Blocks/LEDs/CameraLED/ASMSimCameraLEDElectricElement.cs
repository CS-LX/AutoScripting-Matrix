using Engine;

namespace Game
{
	public class ASMSimCameraLEDElectricElement : ASMMountedElectricElement
	{
		public Matrix m_voltage = Matrix.Zero;

		public ASMSimCameraLEDElectricElement(SubsystemASMElectricity subsystemElectricity, CellFace cellFace)
			: base(subsystemElectricity, cellFace)
		{
		}

		public Matrix GetInputMatrix() => m_voltage;

		public override bool Simulate()
		{
			m_voltage = Matrix.Zero;
			foreach (ASMElectricConnection connection in Connections)
			{
				if (connection.ConnectorType != ASMElectricConnectorType.Output && connection.NeighborConnectorType != 0)
				{
					m_voltage += connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
				}
			}
			return false;
		}
	}
}
