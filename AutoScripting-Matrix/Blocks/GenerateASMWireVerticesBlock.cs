using Engine;

namespace Game {
    public abstract class GenerateASMWireVerticesBlock : Block
    {
        public readonly DynamicArray<ASMElectricConnectionPath> m_ASMtmpConnectionPaths = [];

        public virtual void GenerateASMWireVertices(BlockGeometryGenerator generator, int value, int x, int y, int z, int mountingFace, float centerBoxSize, Vector2 centerOffset, TerrainGeometrySubset subset)
		{
			SubsystemASMElectricity SubsystemElectricity = generator.SubsystemTerrain.Project.FindSubsystem<SubsystemASMElectricity>(true);
			if (SubsystemElectricity == null)
			{
				return;
			}

			Color color = ASMWireBlock.WireColor;
			int num = Terrain.ExtractContents(value);
			if (num == ASMWireBlock.Index)
			{
				int? color2 = ASMWireBlock.GetColor(Terrain.ExtractData(value));
				if (color2.HasValue)
				{
					color = SubsystemPalette.GetColor(generator, color2);
				}
			}
			int num2 = Terrain.ExtractLight(value);
			float num3 = LightingManager.LightIntensityByLightValue[num2];
			Vector3 v = new Vector3((float)x + 0.5f, (float)y + 0.5f, (float)z + 0.5f) - (0.5f * CellFace.FaceToVector3(mountingFace));
			Vector3 vector = CellFace.FaceToVector3(mountingFace);
			Vector2 v2 = new Vector2(0.9376f, 0.0001f);
			Vector2 v3 = new Vector2(0.03125f, 0.00550781237f);
			Point3 point = CellFace.FaceToPoint3(mountingFace);
			int cellContents = generator.Terrain.GetCellContents(x - point.X, y - point.Y, z - point.Z);
			bool flag = cellContents == 2 || cellContents == 7 || cellContents == 8 || cellContents == 6 || cellContents == 62 || cellContents == 72;
			Vector3 v4 = CellFace.FaceToVector3(SubsystemASMElectricity.GetConnectorFace(mountingFace, ASMElectricConnectorDirection.Top));
			Vector3 vector2 = (CellFace.FaceToVector3(SubsystemASMElectricity.GetConnectorFace(mountingFace, ASMElectricConnectorDirection.Left)) * centerOffset.X) + (v4 * centerOffset.Y);
			int num4 = 0;
			m_ASMtmpConnectionPaths.Clear();
			SubsystemElectricity.GetAllConnectedNeighbors(x, y, z, mountingFace, m_ASMtmpConnectionPaths);
			foreach (ASMElectricConnectionPath tmpConnectionPath in m_ASMtmpConnectionPaths)
			{
				if ((num4 & (1 << tmpConnectionPath.ConnectorFace)) == 0)
				{
					ASMElectricConnectorDirection? connectorDirection = SubsystemASMElectricity.GetConnectorDirection(mountingFace, 0, tmpConnectionPath.ConnectorFace);
					if (!(centerOffset == Vector2.Zero) || connectorDirection != ASMElectricConnectorDirection.In)
					{
						num4 |= 1 << tmpConnectionPath.ConnectorFace;
						Color color3 = color;
						if (num != ASMWireBlock.Index)
						{
							int cellValue = generator.Terrain.GetCellValue(x + tmpConnectionPath.NeighborOffsetX, y + tmpConnectionPath.NeighborOffsetY, z + tmpConnectionPath.NeighborOffsetZ);
							if (Terrain.ExtractContents(cellValue) == ASMWireBlock.Index)
							{
								int? color4 = ASMWireBlock.GetColor(Terrain.ExtractData(cellValue));
								if (color4.HasValue)
								{
									color3 = SubsystemPalette.GetColor(generator, color4);
								}
							}
						}
						Vector3 vector3 = (connectorDirection != ASMElectricConnectorDirection.In) ? CellFace.FaceToVector3(tmpConnectionPath.ConnectorFace) : (-Vector3.Normalize(vector2));
						Vector3 vector4 = Vector3.Cross(vector, vector3);
						float s = (centerBoxSize >= 0f) ? MathUtils.Max(0.03125f, centerBoxSize / 2f) : (centerBoxSize / 2f);
						float num5 = (connectorDirection == ASMElectricConnectorDirection.In) ? 0.03125f : 0.5f;
						float num6 = (connectorDirection == ASMElectricConnectorDirection.In) ? 0f : ((tmpConnectionPath.ConnectorFace == tmpConnectionPath.NeighborFace) ? (num5 + 0.03125f) : ((tmpConnectionPath.ConnectorFace != CellFace.OppositeFace(tmpConnectionPath.NeighborFace)) ? num5 : (num5 - 0.03125f)));
						Vector3 v5 = v - (vector4 * 0.03125f) + (vector3 * s) + vector2;
						Vector3 vector5 = v - (vector4 * 0.03125f) + (vector3 * num5);
						Vector3 vector6 = v + (vector4 * 0.03125f) + (vector3 * num5);
						Vector3 v6 = v + (vector4 * 0.03125f) + (vector3 * s) + vector2;
						Vector3 vector7 = v + (vector * 0.03125f) + (vector3 * (centerBoxSize / 2f)) + vector2;
						Vector3 vector8 = v + (vector * 0.03125f) + (vector3 * num6);
						Vector2 vector10 = v2 + (v3 * new Vector2(MathUtils.Max(0.0625f, centerBoxSize), 0f));
						Vector2 vector11 = v2 + (v3 * new Vector2(num5 * 2f, 0f));
						Vector2 vector12 = v2 + (v3 * new Vector2(num5 * 2f, 1f));
						Vector2 vector13 = v2 + (v3 * new Vector2(MathUtils.Max(0.0625f, centerBoxSize), 1f));
						Vector2 vector14 = v2 + (v3 * new Vector2(centerBoxSize, 0.5f));
						Vector2 vector15 = v2 + (v3 * new Vector2(num6 * 2f, 0.5f));
						int num7 = Terrain.ExtractLight(generator.Terrain.GetCellValue(x + tmpConnectionPath.NeighborOffsetX, y + tmpConnectionPath.NeighborOffsetY, z + tmpConnectionPath.NeighborOffsetZ));
						float num8 = LightingManager.LightIntensityByLightValue[num7];
						float num9 = 0.5f * (num3 + num8);
						float num10 = LightingManager.CalculateLighting(-vector4);
						float num11 = LightingManager.CalculateLighting(vector4);
						float num12 = LightingManager.CalculateLighting(vector);
						float num13 = num10 * num3;
						float num14 = num10 * num9;
						float num15 = num11 * num9;
						float num16 = num11 * num3;
						float num17 = num12 * num3;
						float num18 = num12 * num9;
						Color color5 = new Color((byte)((float)(int)color3.R * num13), (byte)((float)(int)color3.G * num13), (byte)((float)(int)color3.B * num13));
						Color color6 = new Color((byte)((float)(int)color3.R * num14), (byte)((float)(int)color3.G * num14), (byte)((float)(int)color3.B * num14));
						Color color7 = new Color((byte)((float)(int)color3.R * num15), (byte)((float)(int)color3.G * num15), (byte)((float)(int)color3.B * num15));
						Color color8 = new Color((byte)((float)(int)color3.R * num16), (byte)((float)(int)color3.G * num16), (byte)((float)(int)color3.B * num16));
						Color color9 = new Color((byte)((float)(int)color3.R * num17), (byte)((float)(int)color3.G * num17), (byte)((float)(int)color3.B * num17));
						Color color10 = new Color((byte)((float)(int)color3.R * num18), (byte)((float)(int)color3.G * num18), (byte)((float)(int)color3.B * num18));
						int count = subset.Vertices.Count;
						subset.Vertices.Count += 6;
						TerrainVertex[] array = subset.Vertices.Array;
						BlockGeometryGenerator.SetupVertex(v5.X, v5.Y, v5.Z, color5, vector10.X, vector10.Y, ref array[count]);
						BlockGeometryGenerator.SetupVertex(vector5.X, vector5.Y, vector5.Z, color6, vector11.X, vector11.Y, ref array[count + 1]);
						BlockGeometryGenerator.SetupVertex(vector6.X, vector6.Y, vector6.Z, color7, vector12.X, vector12.Y, ref array[count + 2]);
						BlockGeometryGenerator.SetupVertex(v6.X, v6.Y, v6.Z, color8, vector13.X, vector13.Y, ref array[count + 3]);
						BlockGeometryGenerator.SetupVertex(vector7.X, vector7.Y, vector7.Z, color9, vector14.X, vector14.Y, ref array[count + 4]);
						BlockGeometryGenerator.SetupVertex(vector8.X, vector8.Y, vector8.Z, color10, vector15.X, vector15.Y, ref array[count + 5]);
						int count2 = subset.Indices.Count;
						subset.Indices.Count += (connectorDirection == ASMElectricConnectorDirection.In) ? 15 : 12;
						var array2 = subset.Indices.Array;
						array2[count2] = count;
						array2[count2 + 1] = count + 5;
						array2[count2 + 2] = count + 1;
						array2[count2 + 3] = count + 5;
						array2[count2 + 4] = count;
						array2[count2 + 5] = count + 4;
						array2[count2 + 6] = count + 4;
						array2[count2 + 7] = count + 2;
						array2[count2 + 8] = count + 5;
						array2[count2 + 9] = count + 2;
						array2[count2 + 10] = count + 4;
						array2[count2 + 11] = count + 3;
						if (connectorDirection == ASMElectricConnectorDirection.In)
						{
							array2[count2 + 12] = count + 2;
							array2[count2 + 13] = count + 1;
							array2[count2 + 14] = count + 5;
						}
					}
				}
			}
			if (centerBoxSize != 0f || (num4 == 0 && num != ASMWireBlock.Index))
			{
				return;
			}
			for (int i = 0; i < 6; i++)
			{
				if (i != mountingFace && i != CellFace.OppositeFace(mountingFace) && (num4 & (1 << i)) == 0)
				{
					Vector3 vector16 = CellFace.FaceToVector3(i);
					Vector3 v7 = Vector3.Cross(vector, vector16);
					Vector3 v8 = v - (v7 * 0.03125f) + (vector16 * 0.03125f);
					Vector3 v9 = v + (v7 * 0.03125f) + (vector16 * 0.03125f);
					Vector3 vector17 = v + (vector * 0.03125f);
					Vector2 vector19 = v2 + (v3 * new Vector2(0.0625f, 0f));
					Vector2 vector20 = v2 + (v3 * new Vector2(0.0625f, 1f));
					Vector2 vector21 = v2 + (v3 * new Vector2(0f, 0.5f));
					float num19 = LightingManager.CalculateLighting(vector16) * num3;
					float num20 = LightingManager.CalculateLighting(vector) * num3;
					Color color11 = new Color((byte)((float)(int)color.R * num19), (byte)((float)(int)color.G * num19), (byte)((float)(int)color.B * num19));
					Color color12 = new Color((byte)((float)(int)color.R * num20), (byte)((float)(int)color.G * num20), (byte)((float)(int)color.B * num20));
					int count3 = subset.Vertices.Count;
					subset.Vertices.Count += 3;
					TerrainVertex[] array3 = subset.Vertices.Array;
					BlockGeometryGenerator.SetupVertex(v8.X, v8.Y, v8.Z, color11, vector19.X, vector19.Y, ref array3[count3]);
					BlockGeometryGenerator.SetupVertex(v9.X, v9.Y, v9.Z, color11, vector20.X, vector20.Y, ref array3[count3 + 1]);
					BlockGeometryGenerator.SetupVertex(vector17.X, vector17.Y, vector17.Z, color12, vector21.X, vector21.Y, ref array3[count3 + 2]);
					int count4 = subset.Indices.Count;
					subset.Indices.Count += 3;
					var array4 = subset.Indices.Array;
					array4[count4] = count3;
					array4[count4 + 1] = count3 + 2;
					array4[count4 + 2] = count3 + 1;
				}
			}
		}

    }
}