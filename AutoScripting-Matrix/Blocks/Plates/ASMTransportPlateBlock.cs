using Engine;
using Engine.Graphics;
using System;
using System.Collections.Generic;

namespace Game
{
	public class ASMTransportPlateBlock : ASMRotateableMountedElectricElementBlock
	{
		public const int Index = 609;
		public ASMTransportPlateBlock() : base("Models/ASMTransportPlate", "TransportPlate", 0.9f) { }

		public Texture2D texture;

		public override void Initialize() {
			base.Initialize();
			texture = ContentManager.Get<Texture2D>("Textures/ASMTransportPlate");
		}

		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value) => "实体转移板";

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

		public override ASMElectricElement CreateElectricElement(SubsystemASMElectricity subsystemElectricity, int value, int x, int y, int z) => new ASMTransportPlateElectricElement(subsystemElectricity, new CellFace(x, y, z, GetFace(value)));

		public override ASMElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z)
		{
			int face2 = GetFace(value);
			if (face == face2 && SubsystemASMElectricity.GetConnectorDirection(face2, 0, connectorFace).HasValue)
			{
				return ASMElectricConnectorType.Input;
			}
			return null;
		}
	}
}
