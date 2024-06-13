namespace Game {
    public struct CalcGateInfo(string displayName, string description, string texture, bool isPointToPoint = false, bool isDisplay = true, string extraDesc = "加加减减数数算算一三五七九，小小数字，变化万千") {
        public string DisplayName = displayName;
        public string Description = description;
        public string Texture = texture;
        public bool IsPointToPoint = isPointToPoint;
        public bool IsDisplay = isDisplay;
        public string ExtraDescription = extraDesc;
    }
}