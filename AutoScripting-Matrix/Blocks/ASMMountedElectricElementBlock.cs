namespace Game {
    public abstract class ASMMountedElectricElementBlock : GenerateASMWireVerticesBlock, IASMElectricElementBlock {
        public abstract int GetFace(int value);

        public abstract ASMElectricElement CreateElectricElement(SubsystemASMElectricity subsystemASMElectricity, int value, int x, int y, int z);

        public abstract ASMElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z);

        public virtual int GetConnectionMask(int value)
        {
            return int.MaxValue;
        }
    }
}