using Engine;
using Engine.Graphics;
using System.Collections.Generic;


namespace Game
{
    public class ASMWireBlock : GenerateASMWireVerticesBlock, IASMElectricWireElementBlock, IASMElectricElementBlock, IPaintableBlock
    {
		public const int Index = 600;

		public BlockMesh m_standaloneBlockMesh = new BlockMesh();

		public BoundingBox[] m_collisionBoxesByFace = new BoundingBox[6];

		public static readonly Color WireColor = new Color(79, 36, 21);

		public override void Initialize()
		{
			Model model = ContentManager.Get<Model>("Models/Wire");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Wire").ParentBone);
			m_standaloneBlockMesh.AppendModelMeshPart(model.FindMesh("Wire").MeshParts[0], boneAbsoluteTransform * Matrix.CreateTranslation(0f, -0.5f, 0f), makeEmissive: false, flipWindingOrder: false, doubleSided: false, flipNormals: false, Color.White);
			m_standaloneBlockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(0.9375f, 0f, 0f));
			for (int i = 0; i < 6; i++)
			{
				Vector3 v = CellFace.FaceToVector3(i);
				Vector3 v2 = new Vector3(0.5f, 0.5f, 0.5f) - (0.5f * v);
				Vector3 v3;
				Vector3 v4;
				if (v.X != 0f)
				{
					v3 = new Vector3(0f, 1f, 0f);
					v4 = new Vector3(0f, 0f, 1f);
				}
				else if (v.Y != 0f)
				{
					v3 = new Vector3(1f, 0f, 0f);
					v4 = new Vector3(0f, 0f, 1f);
				}
				else
				{
					v3 = new Vector3(1f, 0f, 0f);
					v4 = new Vector3(0f, 1f, 0f);
				}
				Vector3 v5 = v2 - (0.5f * v3) - (0.5f * v4);
				Vector3 v6 = v2 + (0.5f * v3) + (0.5f * v4) + (0.05f * v);
				m_collisionBoxesByFace[i] = new BoundingBox(Vector3.Min(v5, v6), Vector3.Max(v5, v6));
			}
		}

		public ASMElectricElement CreateElectricElement(SubsystemASMElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return null;
		}

		public ASMElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			if (!WireExistsOnFace(value, face))
			{
				return null;
			}
			return ASMElectricConnectorType.InputOutput;
		}

		public int GetConnectionMask(int value)
		{
			int? color = GetColor(Terrain.ExtractData(value));
			if (!color.HasValue)
			{
				return int.MaxValue;
			}
			return 1 << color.Value;
		}

		public int GetConnectedWireFacesMask(int value, int face)
		{
			int num = 0;
			if (WireExistsOnFace(value, face))
			{
				int num2 = CellFace.OppositeFace(face);
				bool flag = false;
				for (int i = 0; i < 6; i++)
				{
					if (i == face)
					{
						num |= 1 << i;
					}
					else if (i != num2 && WireExistsOnFace(value, i))
					{
						num |= 1 << i;
						flag = true;
					}
				}
				if (flag && WireExistsOnFace(value, num2))
				{
					num |= 1 << num2;
				}
			}
			return num;
		}

		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			var array = new BoundingBox[6];
			for (int i = 0; i < 6; i++)
			{
				array[i] = WireExistsOnFace(value, i) ? m_collisionBoxesByFace[i] : default;
			}
			return array;
		}

		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			for (int i = 0; i < 6; i++)
			{
				if (WireExistsOnFace(value, i))
				{
					GenerateASMWireVertices(generator, value, x, y, z, i, 0f, Vector2.Zero, geometry.SubsetOpaque);
				}
			}
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int? paintColor = GetPaintColor(value);
			Color color2 = paintColor.HasValue ? (color * SubsystemPalette.GetColor(environmentData, paintColor)) : (1.25f * WireColor * color);
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color2, 2f * size, ref matrix, environmentData);
		}

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			Point3 point = CellFace.FaceToPoint3(raycastResult.CellFace.Face);
			int cellValue = subsystemTerrain.Terrain.GetCellValue(raycastResult.CellFace.X + point.X, raycastResult.CellFace.Y + point.Y, raycastResult.CellFace.Z + point.Z);
			int num = Terrain.ExtractContents(cellValue);
			Block block = BlocksManager.Blocks[num];
			int wireFacesBitmask = GetWireFacesBitmask(cellValue);
			int num2 = wireFacesBitmask | (1 << raycastResult.CellFace.Face);
			BlockPlacementData result;
			if (num2 != wireFacesBitmask || !(block is ASMWireBlock))
			{
				result = default;
				result.Value = SetWireFacesBitmask(value, num2);
				result.CellFace = raycastResult.CellFace;
				return result;
			}
			result = default;
			return result;
		}

		public override BlockPlacementData GetDigValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, int toolValue, TerrainRaycastResult raycastResult)
		{
			int wireFacesBitmask = GetWireFacesBitmask(value);
			wireFacesBitmask &= ~(1 << raycastResult.CollisionBoxIndex);
			BlockPlacementData result = default;
			result.Value = SetWireFacesBitmask(value, wireFacesBitmask);
			result.CellFace = raycastResult.CellFace;
			return result;
		}

		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			int? paintColor = GetPaintColor(oldValue);
			for (int i = 0; i < 6; i++)
			{
				if (WireExistsOnFace(oldValue, i) && !WireExistsOnFace(newValue, i))
				{
					dropValues.Add(new BlockDropValue
					{
						Value = Terrain.MakeBlockValue(ASMWireBlock.Index, 0, SetColor(0, paintColor)),
						Count = 1
					});
				}
			}
			showDebris = dropValues.Count > 0;
		}

		public override IEnumerable<int> GetCreativeValues()
		{
			yield return Terrain.MakeBlockValue(Index);
		}

		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int? paintColor = GetPaintColor(value);
			return SubsystemPalette.GetName(subsystemTerrain, paintColor, "矩阵导线");
		}

		public int? GetPaintColor(int value)
		{
			return GetColor(Terrain.ExtractData(value));
		}

		public int Paint(SubsystemTerrain subsystemTerrain, int value, int? color)
		{
			int data = Terrain.ExtractData(value);
			return Terrain.ReplaceData(value, SetColor(data, color));
		}

		public static bool WireExistsOnFace(int value, int face)
		{
			return (GetWireFacesBitmask(value) & (1 << face)) != 0;
		}

		public static int GetWireFacesBitmask(int value)
		{
			if (Terrain.ExtractContents(value) == ASMWireBlock.Index)
			{
				return Terrain.ExtractData(value) & 0x3F;
			}
			return 0;
		}

		public static int SetWireFacesBitmask(int value, int bitmask)
		{
			int num = Terrain.ExtractData(value);
			num &= -64;
			num |= bitmask & 0x3F;
			return Terrain.ReplaceData(Terrain.ReplaceContents(value, Index), num);
		}

		public static int? GetColor(int data)
		{
			if ((data & 0x40) != 0)
			{
				return (data >> 7) & 0xF;
			}
			return null;
		}

		public static int SetColor(int data, int? color)
		{
			if (color.HasValue)
			{
				return (data & -1985) | 0x40 | ((color.Value & 0xF) << 7);
			}
			return data & -1985;
		}
    }
}