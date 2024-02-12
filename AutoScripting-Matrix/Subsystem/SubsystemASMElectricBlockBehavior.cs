using TemplatesDatabase;

namespace Game
{
    public class SubsystemASMElectricBlockBehavior : SubsystemBlockBehavior
    {
		public SubsystemASMElectricity m_subsystemElectricity;

		public override int[] HandledBlocks => new int[]
		{

		};

		public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
		{
			m_subsystemElectricity.OnElectricElementBlockGenerated(x, y, z);
		}

		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			m_subsystemElectricity.OnElectricElementBlockAdded(x, y, z);
		}

		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			m_subsystemElectricity.OnElectricElementBlockRemoved(x, y, z);
		}

		public override void OnBlockModified(int value, int oldValue, int x, int y, int z)
		{
			m_subsystemElectricity.OnElectricElementBlockModified(x, y, z);
		}

		public override void OnChunkDiscarding(TerrainChunk chunk)
		{
			m_subsystemElectricity.OnChunkDiscarding(chunk);
		}

		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			for (int i = 0; i < 6; i++)
			{
				m_subsystemElectricity.GetElectricElement(x, y, z, i)?.OnNeighborBlockChanged(new CellFace(x, y, z, i), neighborX, neighborY, neighborZ);
			}
		}

		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			int x = raycastResult.CellFace.X;
			int y = raycastResult.CellFace.Y;
			int z = raycastResult.CellFace.Z;
			for (int i = 0; i < 6; i++)
			{
				ASMElectricElement electricElement = m_subsystemElectricity.GetElectricElement(x, y, z, i);
				if (electricElement != null)
				{
					return electricElement.OnInteract(raycastResult, componentMiner);
				}
			}
			return false;
		}

		public override void OnCollide(CellFace cellFace, float velocity, ComponentBody componentBody)
		{
			int x = cellFace.X;
			int y = cellFace.Y;
			int z = cellFace.Z;
			int num = 0;
			ASMElectricElement electricElement;
			while (true)
			{
				if (num < 6)
				{
					electricElement = m_subsystemElectricity.GetElectricElement(x, y, z, num);
					if (electricElement != null)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			electricElement.OnCollide(cellFace, velocity, componentBody);
		}

		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			int x = cellFace.X;
			int y = cellFace.Y;
			int z = cellFace.Z;
			int num = 0;
			ASMElectricElement electricElement;
			while (true)
			{
				if (num < 6)
				{
					electricElement = m_subsystemElectricity.GetElectricElement(x, y, z, num);
					if (electricElement != null)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			electricElement.OnHitByProjectile(cellFace, worldItem);
		}

		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			m_subsystemElectricity = Project.FindSubsystem<SubsystemASMElectricity>(throwOnError: true);
		}
    }
}