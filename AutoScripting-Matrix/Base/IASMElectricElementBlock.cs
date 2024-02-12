namespace Game
{
    public interface IASMElectricElementBlock
    {
        ASMElectricElement CreateElectricElement(SubsystemASMElectricity subsystemElectricity, int value, int x, int y, int z);

        ASMElectricConnectorType? GetConnectorType(SubsystemTerrain terrain, int value, int face, int connectorFace, int x, int y, int z);

        int GetConnectionMask(int value);
    }
}