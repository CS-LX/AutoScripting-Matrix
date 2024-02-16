using Engine;
using Engine.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Game {
    public class ASMBlockDisplayerBlock  : ASMRotateableMountedElectricElementBlock
	{
		public const int Index = 607;

		public Texture2D texture;

		public ASMBlockDisplayerBlock() : base("Models/ASMLed", "ASMLed", 0.9f) { }

		public override void Initialize()
		{
			base.Initialize();
			texture = ContentManager.Get<Texture2D>("Textures/ASMBlockDisplayer");
		}

		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value) => "方块展示板";

		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value) & 0x1F;
			generator.GenerateMeshVertices(this, x, y, z, m_blockMeshes[num], Color.White, null, geometry.GetGeometry(texture).SubsetOpaque);
			GenerateASMWireVertices(generator, value, x, y, z, GetFace(value), m_centerBoxSize, Vector2.Zero, geometry.SubsetOpaque);
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, texture, color, 2f * size, ref matrix, environmentData);
			if (environmentData.DrawBlockMode == DrawBlockMode.UI) //在格子内就画草方块效果
			{
				Matrix matrix2 = Matrix.CreateTranslation(new Vector3(-0.65f, 0.33f, 0.2f));
				BlocksManager.DrawCubeBlock(
					primitivesRenderer,
					GrassBlock.Index,
					new Vector3(0.4f),
					ref matrix2,
					Color.White,
					new Color(4281917775),
					environmentData
				);
			}
		}

		public override ASMElectricElement CreateElectricElement(SubsystemASMElectricity subsystemElectricity, int value, int x, int y, int z) => new ASMBlockDisplayerElectricElement(subsystemElectricity, new CellFace(x, y, z, GetFace(value)), value);

		public override ASMElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			if (GetFace(value) == face)
			{
				ElectricConnectorDirection? connectorDirection = SubsystemElectricity.GetConnectorDirection(GetFace(value), GetRotation(data), connectorFace);
				if (connectorDirection == ElectricConnectorDirection.Right || connectorDirection == ElectricConnectorDirection.Left)
				{
					return ASMElectricConnectorType.Input;
				}
			}
			return null;
		}

		public override string GetDescription(int value) => "方块展示板，可以通过矩阵控制展示方块的形状。"
			+ "它有两个输入端(左端: 控制矩阵; 右端: 变换矩阵。"
			+ "控制矩阵第一个元素: 方块ID; "
			+ "控制矩阵第二个元素: 方块受环境因素影响(0: 否; 其他: 是);"
			+ "控制矩阵第三个元素: 使用相对坐标(0: 否; 其他: 是);";
	}
}