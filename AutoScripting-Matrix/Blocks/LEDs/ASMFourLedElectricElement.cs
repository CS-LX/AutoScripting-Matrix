using Engine;

namespace Game
{
	public class ASMFourLedElectricElement : ASMMountedElectricElement
	{
		public SubsystemGlow m_subsystemGlow;

		public Matrix m_voltage = Matrix.Zero;

		public Color m_color;

		public GlowPoint[] m_glowPoints = new GlowPoint[16];

		public ASMFourLedElectricElement(SubsystemASMElectricity subsystemElectricity, CellFace cellFace)
			: base(subsystemElectricity, cellFace)
		{
			m_subsystemGlow = subsystemElectricity.Project.FindSubsystem<SubsystemGlow>(throwOnError: true);
		}

		public override void OnAdded()
		{
			CellFace cellFace = CellFaces[0];
			int data = Terrain.ExtractData(SubsystemElectricity.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z));
			int mountingFace = ASMFourLedBlock.GetMountingFace(data);
			m_color = LedBlock.LedColors[ASMFourLedBlock.GetColor(data)];
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++) {
					int index = i * 4 + j;
					var v = new Vector3(cellFace.X + 0.5f, cellFace.Y + 0.5f, cellFace.Z + 0.5f);
					Vector3 vector = CellFace.FaceToVector3(mountingFace);
					Vector3 unitX = (mountingFace < 4) ? Vector3.UnitY : Vector3.UnitX;
					var unitY = Vector3.Cross(vector, unitX);
					m_glowPoints[index] = m_subsystemGlow.AddGlowPoint();
					m_glowPoints[index].Position = v - (0.4375f * CellFace.FaceToVector3(mountingFace)) - unitX * 0.375f - unitY * 0.375f + unitX * 0.25f * i + unitY * 0.25f * j;
					m_glowPoints[index].Forward = vector;
					m_glowPoints[index].Up = unitX;
					m_glowPoints[index].Right = unitY;
					m_glowPoints[index].Color = Color.Transparent;
					m_glowPoints[index].Size = 0.13f;
					m_glowPoints[index].FarSize = 0.13f;
					m_glowPoints[index].FarDistance = 1f;
					m_glowPoints[index].Type = GlowPointType.Square;
				}
			}
		}

		public override void OnRemoved()
		{
			for (int i = 0; i < 16; i++)
			{
				m_subsystemGlow.RemoveGlowPoint(m_glowPoints[i]);
			}
		}

		public override bool Simulate()
		{
			Matrix voltage = m_voltage;
			m_voltage = Matrix.Identity;
			foreach (ASMElectricConnection connection in Connections)
			{
				if (connection.ConnectorType != ASMElectricConnectorType.Output && connection.NeighborConnectorType != 0)
				{
					m_voltage += connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace);
				}
			}
			if (m_voltage != voltage) {
				for (int i = 0; i < 16; i++) {
					float element = m_voltage.GetElement(i);
					m_glowPoints[i].FarSize = 0.13f * MathUtils.Clamp(element, 0, 1);
					m_glowPoints[i].Size = 0.13f * MathUtils.Clamp(element, 0, 1);
					m_glowPoints[i].Color = element > 0 ? Color.White : Color.Transparent;
				}
			}
			return false;
		}
	}
}
