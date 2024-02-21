namespace Game {
    public struct ASMDecomposerInfo(string displayName, string description, string texture, ASMElectricConnectorDirection[] inputDirections, ASMElectricConnectorDirection[] outputDirections, bool isDisplay = true) {
        public string DisplayName = displayName;
        public string Description = description;
        public string Texture = texture;
        public ASMElectricConnectorDirection[] InputDirections = inputDirections;
        public ASMElectricConnectorDirection[] OutputDirections = outputDirections;
        public bool IsDisplay = isDisplay;
    }
}