using Engine;
using Engine.Graphics;
using System;
using System.Collections.Generic;

namespace Game
{
	public class ASMTranslatePlateBlock : ASMMountedElectricElementBlock
	{
		public const int Index = 610;

		public BlockMesh m_standaloneBlockMesh;

		public BlockMesh[] m_blockMeshesByData = new BlockMesh[16];

		public BoundingBox[][] m_collisionBoxesByData = new BoundingBox[16][];

		public Texture2D texture;

		public override void Initialize() {
			Model model = ContentManager.Get<Model>("Models/ASMTransportPlate");
			texture = ContentManager.Get<Texture2D>("Textures/ASMTranslatePlate");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("TransportPlate").ParentBone);
			for (int j = 0; j < 6; j++) {
				int num2 = SetMountingFace(0, j);
				Matrix matrix = (j >= 4) ? ((j != 4) ? (Matrix.CreateRotationX((float)Math.PI) * Matrix.CreateTranslation(0.5f, 1f, 0.5f)) : Matrix.CreateTranslation(0.5f, 0f, 0.5f)) : (Matrix.CreateRotationX((float)Math.PI / 2f) * Matrix.CreateTranslation(0f, 0f, -0.5f) * Matrix.CreateRotationY(j * (float)Math.PI / 2f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f));
				m_blockMeshesByData[num2] = new BlockMesh();
				m_blockMeshesByData[num2]
				.AppendModelMeshPart(
					model.FindMesh("TransportPlate").MeshParts[0],
					boneAbsoluteTransform * matrix,
					makeEmissive: false,
					flipWindingOrder: false,
					doubleSided: false,
					flipNormals: false,
					Color.White
				);
				var vector = Vector3.Transform(new Vector3(-0.5f, 0f, -0.5f), matrix);
				var vector2 = Vector3.Transform(new Vector3(0.5f, 0.0625f, 0.5f), matrix);
				vector.X = MathUtils.Round(vector.X * 100f) / 100f;
				vector.Y = MathUtils.Round(vector.Y * 100f) / 100f;
				vector.Z = MathUtils.Round(vector.Z * 100f) / 100f;
				vector2.X = MathUtils.Round(vector2.X * 100f) / 100f;
				vector2.Y = MathUtils.Round(vector2.Y * 100f) / 100f;
				vector2.Z = MathUtils.Round(vector2.Z * 100f) / 100f;
				m_collisionBoxesByData[num2] = new BoundingBox[1] { new BoundingBox(new Vector3(MathUtils.Min(vector.X, vector2.X), MathUtils.Min(vector.Y, vector2.Y), MathUtils.Min(vector.Z, vector2.Z)), new Vector3(MathUtils.Max(vector.X, vector2.X), MathUtils.Max(vector.Y, vector2.Y), MathUtils.Max(vector.Z, vector2.Z))) };
			}
			Matrix identity = Matrix.Identity;
			m_standaloneBlockMesh = new BlockMesh();
			m_standaloneBlockMesh.AppendModelMeshPart(
				model.FindMesh("TransportPlate").MeshParts[0],
				boneAbsoluteTransform * identity,
				makeEmissive: false,
				flipWindingOrder: false,
				doubleSided: false,
				flipNormals: false,
				Color.White
			);
		}

		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value) => "实体变换板";

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			int data = SetMountingFace(Terrain.ExtractData(value), raycastResult.CellFace.Face);
			int value2 = Terrain.ReplaceData(value, data);
			BlockPlacementData result = default;
			result.Value = value2;
			result.CellFace = raycastResult.CellFace;
			return result;
		}

		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			int num = Terrain.ExtractData(value);
			if (num >= m_collisionBoxesByData.Length)
			{
				return null;
			}
			return m_collisionBoxesByData[num];
		}

		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			return face != CellFace.OppositeFace(GetFace(value));
		}

		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value);
			if (num < m_blockMeshesByData.Length)
			{
				generator.GenerateMeshVertices(this, x, y, z, m_blockMeshesByData[num], Color.White, null, geometry.GetGeometry(texture).SubsetOpaque);
				GenerateASMWireVertices(generator, value, x, y, z, GetFace(value), 0.8125f, Vector2.Zero, geometry.SubsetOpaque);
			}
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, texture, color, 2f * size, ref matrix, environmentData);
		}

		public override ASMElectricElement CreateElectricElement(SubsystemASMElectricity subsystemElectricity, int value, int x, int y, int z) => new ASMTranslatePlateElectricElement(subsystemElectricity, new CellFace(x, y, z, GetFace(value)));

		public override ASMElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int face2 = GetFace(value);
			if (face == face2 && SubsystemASMElectricity.GetConnectorDirection(face2, 0, connectorFace).HasValue) {
				ASMElectricConnectorDirection connection = SubsystemASMElectricity.GetConnectorDirection(face2, 0, connectorFace)!.Value;
				if (connection == ASMElectricConnectorDirection.Left
					|| connection == ASMElectricConnectorDirection.Right)
					return ASMElectricConnectorType.Input;
			}
			return null;
		}

		public static int GetMountingFace(int data)
		{
			return (data >> 1) & 7;
		}

		public static int SetMountingFace(int data, int face)
		{
			return (data & -15) | ((face & 7) << 1);
		}

		public override int GetFace(int value)
		{
			return GetMountingFace(Terrain.ExtractData(value));
		}
	}
}
