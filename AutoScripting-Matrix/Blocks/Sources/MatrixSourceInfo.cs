namespace Game {
    public struct MatrixSourceInfo(string displayName, string description, string texture, ASMElectricConnectorDirection[] inputDirections, ASMElectricConnectorDirection[] outputDirections, bool isLarger = false) {
        public string DisplayName = displayName;
        public string Description = description;
        public string Texture = texture;
        public ASMElectricConnectorDirection[] InputDirections = inputDirections;
        public ASMElectricConnectorDirection[] OutputDirections = outputDirections;
        public bool IsLarger = isLarger;
    }
}