using Engine;
using GameEntitySystem;
using System.Globalization;
using System.Text;
using TemplatesDatabase;

namespace Game
{
    public class SubsystemASMElectricity : Subsystem, IUpdateable
    {
		public static ASMElectricConnectionPath[] m_connectionPathsTable = new ASMElectricConnectionPath[120]
		{
			new ASMElectricConnectionPath(0, 1, -1, 4, 4, 0),
			new ASMElectricConnectionPath(0, 1, 0, 0, 4, 5),
			new ASMElectricConnectionPath(0, 1, -1, 2, 4, 5),
			new ASMElectricConnectionPath(0, 0, 0, 5, 4, 2),
			new ASMElectricConnectionPath(-1, 0, -1, 3, 3, 0),
			new ASMElectricConnectionPath(-1, 0, 0, 0, 3, 1),
			new ASMElectricConnectionPath(-1, 0, -1, 2, 3, 1),
			new ASMElectricConnectionPath(0, 0, 0, 1, 3, 2),
			new ASMElectricConnectionPath(0, -1, -1, 5, 5, 0),
			new ASMElectricConnectionPath(0, -1, 0, 0, 5, 4),
			new ASMElectricConnectionPath(0, -1, -1, 2, 5, 4),
			new ASMElectricConnectionPath(0, 0, 0, 4, 5, 2),
			new ASMElectricConnectionPath(1, 0, -1, 1, 1, 0),
			new ASMElectricConnectionPath(1, 0, 0, 0, 1, 3),
			new ASMElectricConnectionPath(1, 0, -1, 2, 1, 3),
			new ASMElectricConnectionPath(0, 0, 0, 3, 1, 2),
			new ASMElectricConnectionPath(0, 0, -1, 2, 2, 0),
			null,
			null,
			null,
			new ASMElectricConnectionPath(-1, 1, 0, 4, 4, 1),
			new ASMElectricConnectionPath(0, 1, 0, 1, 4, 5),
			new ASMElectricConnectionPath(-1, 1, 0, 3, 4, 5),
			new ASMElectricConnectionPath(0, 0, 0, 5, 4, 3),
			new ASMElectricConnectionPath(-1, 0, 1, 0, 0, 1),
			new ASMElectricConnectionPath(0, 0, 1, 1, 0, 2),
			new ASMElectricConnectionPath(-1, 0, 1, 3, 0, 2),
			new ASMElectricConnectionPath(0, 0, 0, 2, 0, 3),
			new ASMElectricConnectionPath(-1, -1, 0, 5, 5, 1),
			new ASMElectricConnectionPath(0, -1, 0, 1, 5, 4),
			new ASMElectricConnectionPath(-1, -1, 0, 3, 5, 4),
			new ASMElectricConnectionPath(0, 0, 0, 4, 5, 3),
			new ASMElectricConnectionPath(-1, 0, -1, 2, 2, 1),
			new ASMElectricConnectionPath(0, 0, -1, 1, 2, 0),
			new ASMElectricConnectionPath(-1, 0, -1, 3, 2, 0),
			new ASMElectricConnectionPath(0, 0, 0, 0, 2, 3),
			new ASMElectricConnectionPath(-1, 0, 0, 3, 3, 1),
			null,
			null,
			null,
			new ASMElectricConnectionPath(0, 1, 1, 4, 4, 2),
			new ASMElectricConnectionPath(0, 1, 0, 2, 4, 5),
			new ASMElectricConnectionPath(0, 1, 1, 0, 4, 5),
			new ASMElectricConnectionPath(0, 0, 0, 5, 4, 0),
			new ASMElectricConnectionPath(1, 0, 1, 1, 1, 2),
			new ASMElectricConnectionPath(1, 0, 0, 2, 1, 3),
			new ASMElectricConnectionPath(1, 0, 1, 0, 1, 3),
			new ASMElectricConnectionPath(0, 0, 0, 3, 1, 0),
			new ASMElectricConnectionPath(0, -1, 1, 5, 5, 2),
			new ASMElectricConnectionPath(0, -1, 0, 2, 5, 4),
			new ASMElectricConnectionPath(0, -1, 1, 0, 5, 4),
			new ASMElectricConnectionPath(0, 0, 0, 4, 5, 0),
			new ASMElectricConnectionPath(-1, 0, 1, 3, 3, 2),
			new ASMElectricConnectionPath(-1, 0, 0, 2, 3, 1),
			new ASMElectricConnectionPath(-1, 0, 1, 0, 3, 1),
			new ASMElectricConnectionPath(0, 0, 0, 1, 3, 0),
			new ASMElectricConnectionPath(0, 0, 1, 0, 0, 2),
			null,
			null,
			null,
			new ASMElectricConnectionPath(1, 1, 0, 4, 4, 3),
			new ASMElectricConnectionPath(0, 1, 0, 3, 4, 5),
			new ASMElectricConnectionPath(1, 1, 0, 1, 4, 5),
			new ASMElectricConnectionPath(0, 0, 0, 5, 4, 1),
			new ASMElectricConnectionPath(1, 0, -1, 2, 2, 3),
			new ASMElectricConnectionPath(0, 0, -1, 3, 2, 0),
			new ASMElectricConnectionPath(1, 0, -1, 1, 2, 0),
			new ASMElectricConnectionPath(0, 0, 0, 0, 2, 1),
			new ASMElectricConnectionPath(1, -1, 0, 5, 5, 3),
			new ASMElectricConnectionPath(0, -1, 0, 3, 5, 4),
			new ASMElectricConnectionPath(1, -1, 0, 1, 5, 4),
			new ASMElectricConnectionPath(0, 0, 0, 4, 5, 1),
			new ASMElectricConnectionPath(1, 0, 1, 0, 0, 3),
			new ASMElectricConnectionPath(0, 0, 1, 3, 0, 2),
			new ASMElectricConnectionPath(1, 0, 1, 1, 0, 2),
			new ASMElectricConnectionPath(0, 0, 0, 2, 0, 1),
			new ASMElectricConnectionPath(1, 0, 0, 1, 1, 3),
			null,
			null,
			null,
			new ASMElectricConnectionPath(0, -1, -1, 2, 2, 4),
			new ASMElectricConnectionPath(0, 0, -1, 4, 2, 0),
			new ASMElectricConnectionPath(0, -1, -1, 5, 2, 0),
			new ASMElectricConnectionPath(0, 0, 0, 0, 2, 5),
			new ASMElectricConnectionPath(-1, -1, 0, 3, 3, 4),
			new ASMElectricConnectionPath(-1, 0, 0, 4, 3, 1),
			new ASMElectricConnectionPath(-1, -1, 0, 5, 3, 1),
			new ASMElectricConnectionPath(0, 0, 0, 1, 3, 5),
			new ASMElectricConnectionPath(0, -1, 1, 0, 0, 4),
			new ASMElectricConnectionPath(0, 0, 1, 4, 0, 2),
			new ASMElectricConnectionPath(0, -1, 1, 5, 0, 2),
			new ASMElectricConnectionPath(0, 0, 0, 2, 0, 5),
			new ASMElectricConnectionPath(1, -1, 0, 1, 1, 4),
			new ASMElectricConnectionPath(1, 0, 0, 4, 1, 3),
			new ASMElectricConnectionPath(1, -1, 0, 5, 1, 3),
			new ASMElectricConnectionPath(0, 0, 0, 3, 1, 5),
			new ASMElectricConnectionPath(0, -1, 0, 5, 5, 4),
			null,
			null,
			null,
			new ASMElectricConnectionPath(0, 1, -1, 2, 2, 5),
			new ASMElectricConnectionPath(0, 0, -1, 5, 2, 0),
			new ASMElectricConnectionPath(0, 1, -1, 4, 2, 0),
			new ASMElectricConnectionPath(0, 0, 0, 0, 2, 4),
			new ASMElectricConnectionPath(1, 1, 0, 1, 1, 5),
			new ASMElectricConnectionPath(1, 0, 0, 5, 1, 3),
			new ASMElectricConnectionPath(1, 1, 0, 4, 1, 3),
			new ASMElectricConnectionPath(0, 0, 0, 3, 1, 4),
			new ASMElectricConnectionPath(0, 1, 1, 0, 0, 5),
			new ASMElectricConnectionPath(0, 0, 1, 5, 0, 2),
			new ASMElectricConnectionPath(0, 1, 1, 4, 0, 2),
			new ASMElectricConnectionPath(0, 0, 0, 2, 0, 4),
			new ASMElectricConnectionPath(-1, 1, 0, 3, 3, 5),
			new ASMElectricConnectionPath(-1, 0, 0, 5, 3, 1),
			new ASMElectricConnectionPath(-1, 1, 0, 4, 3, 1),
			new ASMElectricConnectionPath(0, 0, 0, 1, 3, 4),
			new ASMElectricConnectionPath(0, 1, 0, 4, 4, 5),
			null,
			null,
			null
		};

		public static ASMElectricConnectorDirection?[] m_connectorDirectionsTable = new ASMElectricConnectorDirection?[36]
		{
			null,
			ASMElectricConnectorDirection.Right,
			ASMElectricConnectorDirection.In,
			ASMElectricConnectorDirection.Left,
			ASMElectricConnectorDirection.Top,
			ASMElectricConnectorDirection.Bottom,
			ASMElectricConnectorDirection.Left,
			null,
			ASMElectricConnectorDirection.Right,
			ASMElectricConnectorDirection.In,
			ASMElectricConnectorDirection.Top,
			ASMElectricConnectorDirection.Bottom,
			ASMElectricConnectorDirection.In,
			ASMElectricConnectorDirection.Left,
			null,
			ASMElectricConnectorDirection.Right,
			ASMElectricConnectorDirection.Top,
			ASMElectricConnectorDirection.Bottom,
			ASMElectricConnectorDirection.Right,
			ASMElectricConnectorDirection.In,
			ASMElectricConnectorDirection.Left,
			null,
			ASMElectricConnectorDirection.Top,
			ASMElectricConnectorDirection.Bottom,
			ASMElectricConnectorDirection.Bottom,
			ASMElectricConnectorDirection.Right,
			ASMElectricConnectorDirection.Top,
			ASMElectricConnectorDirection.Left,
			null,
			ASMElectricConnectorDirection.In,
			ASMElectricConnectorDirection.Top,
			ASMElectricConnectorDirection.Right,
			ASMElectricConnectorDirection.Bottom,
			ASMElectricConnectorDirection.Left,
			ASMElectricConnectorDirection.In,
			null
		};

		public static int[] m_connectorFacesTable = new int[30]
		{
			4,
			3,
			5,
			1,
			2,
			4,
			0,
			5,
			2,
			3,
			4,
			1,
			5,
			3,
			0,
			4,
			2,
			5,
			0,
			1,
			2,
			1,
			0,
			3,
			5,
			0,
			1,
			2,
			3,
			4
		};

		public float m_remainingSimulationTime;

		public Dictionary<Point3, Matrix> m_persistentElementsVoltages = new Dictionary<Point3, Matrix>();

		public Dictionary<ASMElectricElement, bool> m_electricElements = new Dictionary<ASMElectricElement, bool>();

		public Dictionary<CellFace, ASMElectricElement> m_electricElementsByCellFace = new Dictionary<CellFace, ASMElectricElement>();

		public Dictionary<Point3, bool> m_pointsToUpdate = new Dictionary<Point3, bool>();

		public Dictionary<Point3, ASMElectricElement> m_electricElementsToAdd = new Dictionary<Point3, ASMElectricElement>();

		public Dictionary<ASMElectricElement, bool> m_electricElementsToRemove = new Dictionary<ASMElectricElement, bool>();

		public Dictionary<Point3, bool> m_wiresToUpdate = new Dictionary<Point3, bool>();

		public List<Dictionary<ASMElectricElement, bool>> m_listsCache = new List<Dictionary<ASMElectricElement, bool>>();

		public Dictionary<int, Dictionary<ASMElectricElement, bool>> m_futureSimulateLists = new Dictionary<int, Dictionary<ASMElectricElement, bool>>();

		public Dictionary<ASMElectricElement, bool> m_nextStepSimulateList;

		public DynamicArray<ASMElectricConnectionPath> m_tmpConnectionPaths = new DynamicArray<ASMElectricConnectionPath>();

		public Dictionary<CellFace, bool> m_tmpVisited = new Dictionary<CellFace, bool>();

		public Dictionary<CellFace, bool> m_tmpResult = new Dictionary<CellFace, bool>();

		public static bool DebugDrawElectrics = false;

		public static int SimulatedElectricElements;

		public const float CircuitStepDuration = 0.01f;

		public SubsystemTime SubsystemTime
		{
			get;
			set;
		}

		public SubsystemTerrain SubsystemTerrain
		{
			get;
			set;
		}

		public SubsystemAudio SubsystemAudio
		{
			get;
			set;
		}

		public int FrameStartCircuitStep
		{
			get;
			set;
		}

		public int CircuitStep
		{
			get;
			set;
		}

		public UpdateOrder UpdateOrder => UpdateOrder.Default;

		public void OnElectricElementBlockGenerated(int x, int y, int z)
		{
			m_pointsToUpdate[new Point3(x, y, z)] = false;
		}

		public void OnElectricElementBlockAdded(int x, int y, int z)
		{
			m_pointsToUpdate[new Point3(x, y, z)] = true;
		}

		public void OnElectricElementBlockRemoved(int x, int y, int z)
		{
			m_pointsToUpdate[new Point3(x, y, z)] = true;
		}

		public void OnElectricElementBlockModified(int x, int y, int z)
		{
			m_pointsToUpdate[new Point3(x, y, z)] = true;
		}

		public void OnChunkDiscarding(TerrainChunk chunk)
		{
			foreach (CellFace key in m_electricElementsByCellFace.Keys)
			{
				if (key.X >= chunk.Origin.X && key.X < chunk.Origin.X + 16 && key.Z >= chunk.Origin.Y && key.Z < chunk.Origin.Y + 16)
				{
					m_pointsToUpdate[new Point3(key.X, key.Y, key.Z)] = false;
				}
			}
		}

		public static ASMElectricConnectorDirection? GetConnectorDirection(int mountingFace, int rotation, int connectorFace)
		{
			ASMElectricConnectorDirection? result = m_connectorDirectionsTable[(6 * mountingFace) + connectorFace];
			if (result.HasValue)
			{
				if (result.Value < ASMElectricConnectorDirection.In)
				{
					return (ASMElectricConnectorDirection)((int)(result.Value + rotation) % 4);
				}
				return result;
			}
			return null;
		}

		public static int GetConnectorFace(int mountingFace, ASMElectricConnectorDirection connectionDirection)
		{
			return m_connectorFacesTable[(int)((5 * mountingFace) + connectionDirection)];
		}

		public void GetAllConnectedNeighbors(int x, int y, int z, int mountingFace, DynamicArray<ASMElectricConnectionPath> list)
		{
			int cellValue = SubsystemTerrain.Terrain.GetCellValue(x, y, z);
			var electricElementBlock = BlocksManager.Blocks[Terrain.ExtractContents(cellValue)] as IASMElectricElementBlock;
			if (electricElementBlock == null)
			{
				return;
			}
			for (ASMElectricConnectorDirection electricConnectorDirection = ASMElectricConnectorDirection.Top; electricConnectorDirection < (ASMElectricConnectorDirection)5; electricConnectorDirection++)
			{
				for (int i = 0; i < 4; i++)
				{
					ASMElectricConnectionPath electricConnectionPath = m_connectionPathsTable[(20 * mountingFace) + (4 * (int)electricConnectorDirection) + i];
					if (electricConnectionPath == null)
					{
						break;
					}
					ASMElectricConnectorType? connectorType = electricElementBlock.GetConnectorType(SubsystemTerrain, cellValue, mountingFace, electricConnectionPath.ConnectorFace, x, y, z);
					if (!connectorType.HasValue)
					{
						break;
					}
					int x2 = x + electricConnectionPath.NeighborOffsetX;
					int y2 = y + electricConnectionPath.NeighborOffsetY;
					int z2 = z + electricConnectionPath.NeighborOffsetZ;
					int cellValue2 = SubsystemTerrain.Terrain.GetCellValue(x2, y2, z2);
					var electricElementBlock2 = BlocksManager.Blocks[Terrain.ExtractContents(cellValue2)] as IASMElectricElementBlock;
					if (electricElementBlock2 == null)
					{
						continue;
					}
					ASMElectricConnectorType? connectorType2 = electricElementBlock2.GetConnectorType(SubsystemTerrain, cellValue2, electricConnectionPath.NeighborFace, electricConnectionPath.NeighborConnectorFace, x2, y2, z2);
					if (connectorType2.HasValue && ((connectorType.Value != 0 && connectorType2.Value != ASMElectricConnectorType.Output) || (connectorType.Value != ASMElectricConnectorType.Output && connectorType2.Value != 0)))
					{
						int connectionMask = electricElementBlock.GetConnectionMask(cellValue);
						int connectionMask2 = electricElementBlock2.GetConnectionMask(cellValue2);
						if ((connectionMask & connectionMask2) != 0)
						{
							list.Add(electricConnectionPath);
						}
					}
				}
			}
		}

		public ASMElectricElement GetElectricElement(int x, int y, int z, int mountingFace)
		{
			m_electricElementsByCellFace.TryGetValue(new CellFace(x, y, z, mountingFace), out ASMElectricElement value);
			return value;
		}

		public void QueueElectricElementForSimulation(ASMElectricElement electricElement, int circuitStep)
		{
			if (circuitStep == CircuitStep + 1)
			{
				if (m_nextStepSimulateList == null && !m_futureSimulateLists.TryGetValue(CircuitStep + 1, out m_nextStepSimulateList))
				{
					m_nextStepSimulateList = GetListFromCache();
					m_futureSimulateLists.Add(CircuitStep + 1, m_nextStepSimulateList);
				}
				m_nextStepSimulateList[electricElement] = true;
			}
			else if (circuitStep > CircuitStep + 1)
			{
				if (!m_futureSimulateLists.TryGetValue(circuitStep, out Dictionary<ASMElectricElement, bool> value))
				{
					value = GetListFromCache();
					m_futureSimulateLists.Add(circuitStep, value);
				}
				value[electricElement] = true;
			}
		}

		public void QueueElectricElementConnectionsForSimulation(ASMElectricElement electricElement, int circuitStep)
		{
			foreach (ASMElectricConnection connection in electricElement.Connections)
			{
				if (connection.ConnectorType != 0 && connection.NeighborConnectorType != ASMElectricConnectorType.Output)
				{
					QueueElectricElementForSimulation(connection.NeighborElectricElement, circuitStep);
				}
			}
		}

		public Matrix? ReadPersistentVoltage(Point3 point)
		{
			if (m_persistentElementsVoltages.TryGetValue(point, out Matrix value))
			{
				return value;
			}
			return null;
		}

		public void WritePersistentVoltage(Point3 point, Matrix voltage)
		{
			m_persistentElementsVoltages[point] = voltage;
		}

		public void Update(float dt)
		{
			FrameStartCircuitStep = CircuitStep;
			SimulatedElectricElements = 0;
			m_remainingSimulationTime = MathUtils.Min(m_remainingSimulationTime + dt, 0.1f);
			while (m_remainingSimulationTime >= 0.01f)
			{
				UpdateElectricElements();
				int num = ++CircuitStep;
				m_remainingSimulationTime -= 0.01f;
				m_nextStepSimulateList = null;
				if (m_futureSimulateLists.TryGetValue(CircuitStep, out Dictionary<ASMElectricElement, bool> value))
				{
					m_futureSimulateLists.Remove(CircuitStep);
					SimulatedElectricElements += value.Count;
					foreach (ASMElectricElement key in value.Keys)
					{
						if (m_electricElements.ContainsKey(key))
						{
							SimulateElectricElement(key);
						}
					}
					ReturnListToCache(value);
				}
			}
			if (DebugDrawElectrics)
			{
				DebugDraw();
			}
		}

		public override void Load(ValuesDictionary valuesDictionary)
		{
			SubsystemTerrain = Project.FindSubsystem<SubsystemTerrain>(throwOnError: true);
			SubsystemTime = Project.FindSubsystem<SubsystemTime>(throwOnError: true);
			SubsystemAudio = Project.FindSubsystem<SubsystemAudio>(throwOnError: true);

			//Matrix m = valuesDictionary.GetValue<string>("ASMVoltagesByCell").ToMatrix();
#if true

			string[] array = valuesDictionary.GetValue<string>("ASMVoltagesByCell").Split(new char[1]
			{
				';'
			}, StringSplitOptions.RemoveEmptyEntries);
			int num = 0;
			while (true)
			{
				if (num < array.Length)
				{
					string[] array2 = array[num].Split(new string[] { "|" }, StringSplitOptions.None);
					if (array2.Length != 4)
					{
						break;
					}
					int x = int.Parse(array2[0], CultureInfo.InvariantCulture);
					int y = int.Parse(array2[1], CultureInfo.InvariantCulture);
					int z = int.Parse(array2[2], CultureInfo.InvariantCulture);
					Matrix value = array2[3].ToMatrix();
					m_persistentElementsVoltages[new Point3(x, y, z)] = value;
					num++;
					continue;
				}
				return;
			}
			throw new InvalidOperationException("Invalid number of tokens.");
#endif

		}

		public override void Save(ValuesDictionary valuesDictionary)
		{
			int num = 0;
			var stringBuilder = new StringBuilder();
			foreach (KeyValuePair<Point3, Matrix> persistentElementsVoltage in m_persistentElementsVoltages)
			{
				if (num > 500)
				{
					break;
				}
				stringBuilder.Append(persistentElementsVoltage.Key.X.ToString(CultureInfo.InvariantCulture));
				stringBuilder.Append('|');
				stringBuilder.Append(persistentElementsVoltage.Key.Y.ToString(CultureInfo.InvariantCulture));
				stringBuilder.Append('|');
				stringBuilder.Append(persistentElementsVoltage.Key.Z.ToString(CultureInfo.InvariantCulture));
				stringBuilder.Append('|');
				stringBuilder.Append(persistentElementsVoltage.Value.ToHexString());
				stringBuilder.Append(';');
				num++;
			}
			valuesDictionary.SetValue("ASMVoltagesByCell", stringBuilder.ToString());
		}

		public static ASMElectricConnectionPath GetConnectionPath(int mountingFace, ASMElectricConnectorDirection localConnector, int neighborIndex)
		{
			return m_connectionPathsTable[(16 * mountingFace) + (4 * (int)localConnector) + neighborIndex];
		}

		public void SimulateElectricElement(ASMElectricElement electricElement)
		{
			if (electricElement.Simulate())
			{
				QueueElectricElementConnectionsForSimulation(electricElement, CircuitStep + 1);
			}
		}

		public void AddElectricElement(ASMElectricElement electricElement)
		{
			m_electricElements.Add(electricElement, value: true);
			foreach (CellFace cellFace2 in electricElement.CellFaces)
			{
				m_electricElementsByCellFace.Add(cellFace2, electricElement);
				m_tmpConnectionPaths.Clear();
				GetAllConnectedNeighbors(cellFace2.X, cellFace2.Y, cellFace2.Z, cellFace2.Face, m_tmpConnectionPaths);
				foreach (ASMElectricConnectionPath tmpConnectionPath in m_tmpConnectionPaths)
				{
					var cellFace = new CellFace(cellFace2.X + tmpConnectionPath.NeighborOffsetX, cellFace2.Y + tmpConnectionPath.NeighborOffsetY, cellFace2.Z + tmpConnectionPath.NeighborOffsetZ, tmpConnectionPath.NeighborFace);
					if (m_electricElementsByCellFace.TryGetValue(cellFace, out ASMElectricElement value) && value != electricElement)
					{
						int cellValue = SubsystemTerrain.Terrain.GetCellValue(cellFace2.X, cellFace2.Y, cellFace2.Z);
						int num = Terrain.ExtractContents(cellValue);
						ASMElectricConnectorType value2 = ((IASMElectricElementBlock)BlocksManager.Blocks[num]).GetConnectorType(SubsystemTerrain, cellValue, cellFace2.Face, tmpConnectionPath.ConnectorFace, cellFace2.X, cellFace2.Y, cellFace2.Z).Value;
						int cellValue2 = SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z);
						int num2 = Terrain.ExtractContents(cellValue2);
						ASMElectricConnectorType value3 = ((IASMElectricElementBlock)BlocksManager.Blocks[num2]).GetConnectorType(SubsystemTerrain, cellValue2, cellFace.Face, tmpConnectionPath.NeighborConnectorFace, cellFace.X, cellFace.Y, cellFace.Z).Value;
						electricElement.Connections.Add(new ASMElectricConnection
						{
							CellFace = cellFace2,
							ConnectorFace = tmpConnectionPath.ConnectorFace,
							ConnectorType = value2,
							NeighborElectricElement = value,
							NeighborCellFace = cellFace,
							NeighborConnectorFace = tmpConnectionPath.NeighborConnectorFace,
							NeighborConnectorType = value3
						});
						value.Connections.Add(new ASMElectricConnection
						{
							CellFace = cellFace,
							ConnectorFace = tmpConnectionPath.NeighborConnectorFace,
							ConnectorType = value3,
							NeighborElectricElement = electricElement,
							NeighborCellFace = cellFace2,
							NeighborConnectorFace = tmpConnectionPath.ConnectorFace,
							NeighborConnectorType = value2
						});
					}
				}
			}
			QueueElectricElementForSimulation(electricElement, CircuitStep + 1);
			QueueElectricElementConnectionsForSimulation(electricElement, CircuitStep + 2);
			electricElement.OnAdded();
		}

		public void RemoveElectricElement(ASMElectricElement electricElement)
		{
			electricElement.OnRemoved();
			QueueElectricElementConnectionsForSimulation(electricElement, CircuitStep + 1);
			m_electricElements.Remove(electricElement);
			foreach (CellFace cellFace in electricElement.CellFaces)
			{
				m_electricElementsByCellFace.Remove(cellFace);
			}
			foreach (ASMElectricConnection connection in electricElement.Connections)
			{
				int num = connection.NeighborElectricElement.Connections.FirstIndex((ASMElectricConnection c) => c.NeighborElectricElement == electricElement);
				if (num >= 0)
				{
					connection.NeighborElectricElement.Connections.RemoveAt(num);
				}
			}
		}

		public void UpdateElectricElements()
		{
			foreach (KeyValuePair<Point3, bool> item in m_pointsToUpdate)
			{
				Point3 key = item.Key;
				int cellValue = SubsystemTerrain.Terrain.GetCellValue(key.X, key.Y, key.Z);
				for (int i = 0; i < 6; i++)
				{
					ASMElectricElement electricElement = GetElectricElement(key.X, key.Y, key.Z, i);
					if (electricElement != null)
					{
						if (electricElement is ASMWireDomainElectricElement)
						{
							m_wiresToUpdate[key] = true;
						}
						else
						{
							m_electricElementsToRemove[electricElement] = true;
						}
					}
				}
				if (item.Value)
				{
					m_persistentElementsVoltages.Remove(key);
				}
				int num = Terrain.ExtractContents(cellValue);
				if (BlocksManager.Blocks[num] is IASMElectricWireElementBlock)
				{
					m_wiresToUpdate[key] = true;
				}
				else
				{
					var electricElementBlock = BlocksManager.Blocks[num] as IASMElectricElementBlock;
					if (electricElementBlock != null)
					{
						ASMElectricElement electricElement2 = electricElementBlock.CreateElectricElement(this, cellValue, key.X, key.Y, key.Z);
						if (electricElement2 != null)
						{
							m_electricElementsToAdd[key] = electricElement2;
						}
					}
				}
			}
			RemoveWireDomains();
			foreach (KeyValuePair<ASMElectricElement, bool> item2 in m_electricElementsToRemove)
			{
				RemoveElectricElement(item2.Key);
			}
			AddWireDomains();
			foreach (ASMElectricElement value in m_electricElementsToAdd.Values)
			{
				AddElectricElement(value);
			}
			m_pointsToUpdate.Clear();
			m_wiresToUpdate.Clear();
			m_electricElementsToAdd.Clear();
			m_electricElementsToRemove.Clear();
		}

		public void AddWireDomains()
		{
			m_tmpVisited.Clear();
			foreach (Point3 key in m_wiresToUpdate.Keys)
			{
				for (int i = key.X - 1; i <= key.X + 1; i++)
				{
					for (int j = key.Y - 1; j <= key.Y + 1; j++)
					{
						for (int k = key.Z - 1; k <= key.Z + 1; k++)
						{
							for (int l = 0; l < 6; l++)
							{
								m_tmpResult.Clear();
								ScanWireDomain(new CellFace(i, j, k, l), m_tmpVisited, m_tmpResult);
								if (m_tmpResult.Count > 0)
								{
									var electricElement = new ASMWireDomainElectricElement(this, m_tmpResult.Keys);
									AddElectricElement(electricElement);
								}
							}
						}
					}
				}
			}
		}

		public void RemoveWireDomains()
		{
			foreach (Point3 key in m_wiresToUpdate.Keys)
			{
				for (int i = key.X - 1; i <= key.X + 1; i++)
				{
					for (int j = key.Y - 1; j <= key.Y + 1; j++)
					{
						for (int k = key.Z - 1; k <= key.Z + 1; k++)
						{
							for (int l = 0; l < 6; l++)
							{
								if (m_electricElementsByCellFace.TryGetValue(new CellFace(i, j, k, l), out ASMElectricElement value) && value is ASMWireDomainElectricElement)
								{
									RemoveElectricElement(value);
								}
							}
						}
					}
				}
			}
		}

		public void ScanWireDomain(CellFace startCellFace, Dictionary<CellFace, bool> visited, Dictionary<CellFace, bool> result)
		{
			var dynamicArray = new DynamicArray<CellFace>();
			dynamicArray.Add(startCellFace);
			while (dynamicArray.Count > 0)
			{
				CellFace key = dynamicArray.Array[--dynamicArray.Count];
				if (visited.ContainsKey(key))
				{
					continue;
				}
				TerrainChunk chunkAtCell = SubsystemTerrain.Terrain.GetChunkAtCell(key.X, key.Z);
				if (chunkAtCell == null || !chunkAtCell.AreBehaviorsNotified)
				{
					continue;
				}
				int cellValue = SubsystemTerrain.Terrain.GetCellValue(key.X, key.Y, key.Z);
				int num = Terrain.ExtractContents(cellValue);
				var electricWireElementBlock = BlocksManager.Blocks[num] as IASMElectricWireElementBlock;
				if (electricWireElementBlock == null)
				{
					continue;
				}
				int connectedWireFacesMask = electricWireElementBlock.GetConnectedWireFacesMask(cellValue, key.Face);
				if (connectedWireFacesMask == 0)
				{
					continue;
				}
				for (int i = 0; i < 6; i++)
				{
					if ((connectedWireFacesMask & (1 << i)) != 0)
					{
						var key2 = new CellFace(key.X, key.Y, key.Z, i);
						visited.Add(key2, value: true);
						result.Add(key2, value: true);
						m_tmpConnectionPaths.Clear();
						GetAllConnectedNeighbors(key2.X, key2.Y, key2.Z, key2.Face, m_tmpConnectionPaths);
						foreach (ASMElectricConnectionPath tmpConnectionPath in m_tmpConnectionPaths)
						{
							int x = key2.X + tmpConnectionPath.NeighborOffsetX;
							int y = key2.Y + tmpConnectionPath.NeighborOffsetY;
							int z = key2.Z + tmpConnectionPath.NeighborOffsetZ;
							dynamicArray.Add(new CellFace(x, y, z, tmpConnectionPath.NeighborFace));
						}
					}
				}
			}
		}

		public Dictionary<ASMElectricElement, bool> GetListFromCache()
		{
			if (m_listsCache.Count > 0)
			{
				Dictionary<ASMElectricElement, bool> result = m_listsCache[m_listsCache.Count - 1];
				m_listsCache.RemoveAt(m_listsCache.Count - 1);
				return result;
			}
			return new Dictionary<ASMElectricElement, bool>();
		}

		public void ReturnListToCache(Dictionary<ASMElectricElement, bool> list)
		{
			list.Clear();
			m_listsCache.Add(list);
		}

		public void DebugDraw()
		{
		}

    }
}