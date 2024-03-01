using Engine;
using Engine.Graphics;

namespace Game {
    public class ASMWireThroughBlock : GenerateASMWireVerticesBlock, IASMElectricWireElementBlock, IASMElectricElementBlock {
        public const int Index = 614;

        public Texture2D texture;

        public WireTroughtInfo[] Infos = [
            new WireTroughtInfo("变体矩阵锗穿线快", "", 0, 21, true),
            new WireTroughtInfo("矩阵锗穿线快", "", 14, 35, true),
            new WireTroughtInfo("矩阵铁穿线快", "", 1, 22, true),
        ];

        public override void Initialize() {
            base.Initialize();
            texture = ContentManager.Get<Texture2D>("Textures/ASMWireThroughBlocks");
        }

        public override string GetCategory(int value) => "[智械]矩阵";

        public override string GetDescription(int value) => Infos[GetType(Terrain.ExtractData(value))].Description;

        public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value) => Infos[GetType(Terrain.ExtractData(value))].Name;

        public override IEnumerable<int> GetCreativeValues() {
            for (int i = 0; i < Infos.Length; i++) {
                if (!Infos[i].IsDisplay) continue;
                yield return Terrain.MakeBlockValue(Index, 0, SetWiredFaceAndType(0, 0, i));
            }
        }

        public override int GetFaceTextureSlot(int face, int value) {
            int type = GetType(Terrain.ExtractData(value));
            int wiredFace = GetWiredFace(Terrain.ExtractData(value));
            if (wiredFace == face || CellFace.OppositeFace(wiredFace) == face)
            {
                return Infos[type].WiredSlot;
            }
            return Infos[type].UnwiredSlot;
        }

        public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
        {
            Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
            float num = float.NegativeInfinity;
            int wiredFace = 0;
            for (int i = 0; i < 6; i++)
            {
                float num2 = Vector3.Dot(CellFace.FaceToVector3(i), forward);
                if (num2 > num)
                {
                    num = num2;
                    wiredFace = i;
                }
            }
            BlockPlacementData result = default;
            result.Value = Terrain.MakeBlockValue(Index, 0, SetWiredFaceAndType(0, wiredFace, GetType(Terrain.ExtractData(value))));
            result.CellFace = raycastResult.CellFace;
            return result;
        }

        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometry geometry, int value, int x, int y, int z)
        {
            generator.GenerateCubeVertices(this, value, x, y, z, Color.White, geometry.GetGeometry(texture).OpaqueSubsetsByFace);
        }

        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            BlocksManager.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, color, color, environmentData, texture);
        }

        public override int GetTextureSlotCount(int value) => 6;

        public int GetConnectedWireFacesMask(int value, int face) {
            int wiredFace = GetWiredFace(Terrain.ExtractData(value));
            if (wiredFace == face || CellFace.OppositeFace(wiredFace) == face)
            {
                return (1 << wiredFace) | (1 << CellFace.OppositeFace(wiredFace));
            }
            return 0;
        }

        public ASMElectricElement CreateElectricElement(SubsystemASMElectricity subsystemElectricity, int value, int x, int y, int z) => null;

        public ASMElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z) {
            {
                int wiredFace = GetWiredFace(Terrain.ExtractData(value));
                if ((face == wiredFace || face == CellFace.OppositeFace(wiredFace)) && connectorFace == CellFace.OppositeFace(face))
                {
                    return ASMElectricConnectorType.InputOutput;
                }
                return null;
            }
        }

        public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris) {
            int data = Terrain.ExtractData(oldValue);
            dropValues.Add(new BlockDropValue { Value = Terrain.MakeBlockValue(Index, 0, SetWiredFaceAndType(0, 0, GetType(data))), Count = 1 });
            showDebris = true;
        }

        public int GetConnectionMask(int value) => int.MaxValue;

        public static int GetWiredFace(int data)
        {
            if ((data & 3) == 0) {
                return 0;
            }
            if ((data & 3) == 1) {
                return 1;
            }
            return 4;
        }

        public static int GetType(int data) {
            return (data >> 3) & 0b1111;
        }


        public static int SetWiredFaceAndType(int data, int wiredFace, int type) {
            data &= -4;
            switch (wiredFace)
            {
                case 0:
                case 2:
                    return (data) | ((type & 0b1111) << 3);
                case 1:
                case 3:
                    return data | 1 | ((type & 0b1111) << 3);
                default:
                    return data | 2 | ((type & 0b1111) << 3);
            }
        }

        public struct WireTroughtInfo(string name, string description, int wiredSlot, int unwiredSlot, bool isDisplay) {
            public string Name = name;
            public string Description = description;
            public int WiredSlot = wiredSlot;
            public int UnwiredSlot = unwiredSlot;
            public bool IsDisplay = isDisplay;
        }
    }
}