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

		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value) => "矩阵可拓展数码LED";

		public override string GetDescription(int value) => "一个更加人性化的矩阵显示器，将矩阵通过4x4表格与16个数字显示，更加符合人们的阅读习惯，并且可拓展。\r\n详见https://github.com/CS-LX/AutoScripting-Matrix";

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
			if(environmentData.DrawBlockMode != DrawBlockMode.UI || !environmentData.ViewProjectionMatrix.HasValue) return;
			Vector3 translation = matrix.Translation;
			Vector3 vector2;
			Vector3 vector3;
			vector2 = matrix.Forward;
			vector3 = matrix.Up;
			size *= 14f / 16f;
			Vector3 zero = translation + (size * (-vector2 - vector3));
			Vector3 u = translation + (size * (vector2 - vector3));
			Vector3 ur = translation + (size * (vector2 + vector3));
			Vector3 r = translation + (size * (-vector2 + vector3));

			zero -= 0.09f * matrix.Right;
			u -= 0.09f * matrix.Right;
			ur -= 0.09f * matrix.Right;
			r -= 0.09f * matrix.Right;

			FlatBatch3D flatBatch3D = primitivesRenderer.FlatBatch();
			Matrix m = environmentData.ViewProjectionMatrix.Value;

			Vector3.Transform(ref zero, ref m, out zero);
			Vector3.Transform(ref u, ref m, out u);
			Vector3.Transform(ref ur, ref m, out ur);
			Vector3.Transform(ref r, ref m, out r);

			Vector3 up = u - zero;
			Vector3 right = r - zero;

			for (int i = 0; i <= 4; i++) {
				float height = i / 4f;
				flatBatch3D.QueueQuad(
					zero + up * (height - 0.02f / 2),
					r + up * (height - 0.02f / 2),
					r + up * (height + 0.02f / 2),
					zero + up * (height + 0.02f / 2),
					new Color(110, 110, 110)
				);
			}

			for (int i = 0; i <= 4; i++) {
				float width = i / 4f;
				flatBatch3D.QueueQuad(
					zero + right * (width - 0.02f / 2),
					zero + right * (width + 0.02f / 2),
					u + right * (width + 0.02f / 2),
					u + right * (width - 0.02f / 2),
					new Color(110, 110, 110)
				);
			}
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

		public override ASMElectricElement CreateElectricElement(SubsystemASMElectricity subsystemElectricity, int value, int x, int y, int z) => new ASMExpandableLEDElectricElement(subsystemElectricity, new CellFace(x, y, z, GetFace(value)));

		public override ASMElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z) {
			int face2 = GetFace(value);
			if (face == face2 && SubsystemElectricity.GetConnectorDirection(face2, 0, connectorFace).HasValue)
			{
				return ASMElectricConnectorType.Input;
			}
			return null;
		}

		public ASMELEDFacialData GetFacialData(int face, Point3 point, SubsystemTerrain terrain) {
			bool[] p = [true, true, true, true];
			bool[] s = [true, true, true, true];
			//默认为地面上的值
			Point3[] axes = [-Point3.UnitZ, Point3.UnitX, Point3.UnitZ, -Point3.UnitX];
			Point3[] conners = [new Point3(-1, 0, -1), new Point3(1, 0, -1), new Point3(1, 0, 1), new Point3(-1, 0, 1)];

			ASMUtils.FaceToAxesAndConner(face, out axes, out conners);

			for (int i = 0; i < 4; i++) {
				Point3 pos = point + axes[i];//获取四个邻面的绝对坐标
				Point3 posN = point + axes[(i + 3) % 4];//获取四个邻面的绝对坐标
				s[i] = CheckBlock(terrain, point, pos);
				Point3 posC = point + conners[i];//获取四个角绝对坐标
				p[i] = CheckBlock(terrain, point, posC) || CheckBlock(terrain, point, pos) || CheckBlock(terrain, point, posN);
			}
			return new ASMELEDFacialData(p, s);
		}

		private bool CheckBlock(SubsystemTerrain terrain, Point3 position, Point3 neighborPosition) {
			if (Terrain.ExtractContents(terrain.Terrain.GetCellValueFast(neighborPosition.X, neighborPosition.Y, neighborPosition.Z)) == Index
				&& GetFace(terrain.Terrain.GetCellValueFast(position.X, position.Y, position.Z)) == GetFace(terrain.Terrain.GetCellValueFast(neighborPosition.X, neighborPosition.Y, neighborPosition.Z))) {
				return false;
			}
			return true;
		}
	}
}