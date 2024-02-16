namespace Game {
    public struct MatrixSourceInfo(string displayName, string description, string texture, ElectricConnectorDirection[] inputDirections, ElectricConnectorDirection[] outputDirections) {
        public string DisplayName = displayName;
        public string Description = description;
        public string Texture = texture;
        public ElectricConnectorDirection[] InputDirections = inputDirections;
        public ElectricConnectorDirection[] OutputDirections = outputDirections;
    }
}