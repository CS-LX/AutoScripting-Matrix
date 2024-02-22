using Engine;
using Engine.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Game {
    public class ASMExpandableLEDBlock  : ASMRotateableMountedElectricElementBlock
	{
		public const int Index = 615;

		public Texture2D texture;

		public Texture2D facialDefault;

		public ASMExpandableLEDBlock() : base("Models/ASMExpandableLED", "ASMLed", 0.9f, "Face") { }

		public override void Initialize()
		{
			base.Initialize();
			texture = ContentManager.Get<Texture2D>("Textures/ASMLed");
			facialDefault = ContentManager.Get<Texture2D>("Textures/ASMExpandableLEDDefault");
		}

		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value) => "矩阵可拓展LED";

		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value) & 0x1F;
			SubsystemASMExpandableLEDTexture subsystemAsmExpandableLedTexture = generator.SubsystemTerrain.Project.FindSubsystem<SubsystemASMExpandableLEDTexture>(true);
			ASMELEDFacialData asmeledFacialData = GetFacialData(GetFace(value), new Point3(x, y, z), generator.SubsystemTerrain);
			Texture2D facialTexture = subsystemAsmExpandableLedTexture.m_facialDataToSlot.ContainsKey(asmeledFacialData) ? subsystemAsmExpandableLedTexture.m_facialDataToSlot[asmeledFacialData] : facialDefault;

			generator.GenerateMeshVertices(this, x, y, z, m_blockMeshes[num], Color.White, null, geometry.GetGeometry(texture).SubsetOpaque);
			generator.GenerateMeshVertices(this, x, y, z, m_blockMeshes2[num], Color.White, null, geometry.GetGeometry(facialTexture).SubsetOpaque);
			GenerateASMWireVertices(generator, value, x, y, z, GetFace(value), m_centerBoxSize, Vector2.Zero, geometry.SubsetOpaque);
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, texture, color, 2f * size, ref matrix, environmentData);
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh2, facialDefault, color, 2f * size, ref matrix, environmentData);
		}

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			int num5 = Terrain.ExtractData(value);
			num5 &= -29;
			num5 |= raycastResult.CellFace.Face << 2;
			BlockPlacementData result = default;
			result.Value = Terrain.MakeBlockValue(BlockIndex, 0, SetRotation(num5, 0));
			result.CellFace = raycastResult.CellFace;
			return result;
		}

		public override ASMElectricElement CreateElectricElement(SubsystemASMElectricity subsystemElectricity, int value, int x, int y, int z) => new ASMBlockDisplayerElectricElement(subsystemElectricity, new CellFace(x, y, z, GetFace(value)), value);

		public override ASMElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			if (GetFace(value) == face)
			{
				ASMElectricConnectorDirection? connectorDirection = SubsystemASMElectricity.GetConnectorDirection(GetFace(value), GetRotation(data), connectorFace);
				if (connectorDirection == ASMElectricConnectorDirection.Right || connectorDirection == ASMElectricConnectorDirection.Left)
				{
					return ASMElectricConnectorType.Input;
				}
			}
			return null;
		}

		public ASMELEDFacialData GetFacialData(int face, Point3 point, SubsystemTerrain terrain) {
			bool[] p = [true, true, true, true];
			bool[] s = [true, true, true, true];
			//默认为地面上的值
			Point3[] axes = [-Point3.UnitZ, Point3.UnitX, Point3.UnitZ, -Point3.UnitX];
			Point3[] conners = [new Point3(-1, 0, -1), new Point3(1, 0, -1), new Point3(1, 0, 1), new Point3(-1, 0, 1)];

			switch (face) {
				case 0:
					axes = [Point3.UnitY, Point3.UnitX, -Point3.UnitY, -Point3.UnitX];
					conners = [new Point3(-1, 1, 0), new Point3(1, 1, 0), new Point3(1, -1, 0), new Point3(-1, -1, 0)];
					break;
				case 1:
					axes = [Point3.UnitY, -Point3.UnitZ, -Point3.UnitY, Point3.UnitZ];
					conners = [new Point3(0, 1, 1), new Point3(0, 1, -1), new Point3(0, -1, -1), new Point3(0, -1, 1)];
					break;
				case 2:
					axes = [Point3.UnitY, -Point3.UnitX, -Point3.UnitY, Point3.UnitX];
					conners = [new Point3(1, 1, 0), new Point3(-1, 1, 0), new Point3(-1, -1, 0), new Point3(1, -1, 0)];
					break;
				case 3:
					axes = [Point3.UnitY, Point3.UnitZ, -Point3.UnitY, -Point3.UnitZ];
					conners = [new Point3(0, 1, -1), new Point3(0, 1, 1), new Point3(0, -1, 1), new Point3(0, -1, -1)];
					break;
				case 4:
					axes = [-Point3.UnitZ, Point3.UnitX, Point3.UnitZ, -Point3.UnitX];
					conners = [new Point3(-1, 0, -1), new Point3(1, 0, -1), new Point3(1, 0, 1), new Point3(-1, 0, 1)];
					break;
				case 5:
					axes = [Point3.UnitZ, Point3.UnitX, -Point3.UnitZ, -Point3.UnitX];
					conners = [new Point3(-1, 0, 1), new Point3(1, 0, 1), new Point3(1, 0, -1), new Point3(-1, 0, -1)];
					break;
			}
			for (int i = 0; i < 4; i++) {
				Point3 pos = point + axes[i];//获取四个邻面的绝对坐标
				Point3 posN = point + axes[(i + 3) % 4];//获取四个邻面的绝对坐标
				s[i] = Terrain.ExtractContents(terrain.Terrain.GetCellValueFast(pos.X, pos.Y, pos.Z)) != Index;
				Point3 posC = point + conners[i];//获取四个角绝对坐标
				p[i] = Terrain.ExtractContents(terrain.Terrain.GetCellValueFast(posC.X, posC.Y, posC.Z)) != Index || Terrain.ExtractContents(terrain.Terrain.GetCellValueFast(pos.X, pos.Y, pos.Z)) != Index || Terrain.ExtractContents(terrain.Terrain.GetCellValueFast(posN.X, posN.Y, posN.Z)) != Index;
			}
			return new ASMELEDFacialData(p, s);
		}
	}
}