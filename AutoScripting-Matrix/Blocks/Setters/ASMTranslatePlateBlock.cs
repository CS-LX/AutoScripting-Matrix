using Engine;
using Engine.Graphics;
using System;
using System.Collections.Generic;

namespace Game
{
	public class ASMTranslatePlateBlock : ASMRotateableMountedElectricElementBlock
	{
		public const int Index = 610;

		public Texture2D texture;

		public ASMTranslatePlateBlock() : base("Models/ASMTransportPlate", "TransportPlate", 0.8f) { }

		public override void Initialize() {
			base.Initialize();
			texture = ContentManager.Get<Texture2D>("Textures/ASMTranslatePlate");
		}

		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value) => "实体变换板";

		public override string GetDescription(int value) => "将触碰到的实体变换按照左端输入矩阵进行映射并返回给实体，而实体的速度按照右端输入矩阵进行映射并返回给实体。掉落物同理。";

		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			return face != CellFace.OppositeFace(GetFace(value));
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData) {
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh_noRotation, texture, color, 2f * size, ref matrix, environmentData);
		}

		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
		{
			int num = Terrain.ExtractData(value) & 0x1F;
			generator.GenerateMeshVertices(this, x, y, z, m_blockMeshes[num], Color.White, null, geometry.GetGeometry(texture).SubsetOpaque);
			GenerateASMWireVertices(generator, value, x, y, z, GetFace(value), m_centerBoxSize, Vector2.Zero, geometry.SubsetOpaque);
		}

		public override ASMElectricElement CreateElectricElement(SubsystemASMElectricity subsystemElectricity, int value, int x, int y, int z) => new ASMTranslatePlateElectricElement(subsystemElectricity, new CellFace(x, y, z, GetFace(value)));

		public override ASMElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int face2 = GetFace(value);
			if (face == face2 && SubsystemASMElectricity.GetConnectorDirection(face2, GetRotation(Terrain.ExtractData(value)), connectorFace).HasValue) {
				ASMElectricConnectorDirection connection = SubsystemASMElectricity.GetConnectorDirection(face2, GetRotation(Terrain.ExtractData(value)), connectorFace)!.Value;
				if (connection == ASMElectricConnectorDirection.Left
					|| connection == ASMElectricConnectorDirection.Right)
					return ASMElectricConnectorType.Input;
			}
			return null;
		}
	}
}
