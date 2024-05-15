namespace Game {
    public class ASMPlayerCameraSetterData : IEditableItemData {
        public string Name = DefaultName;

        public static string DefaultName => "矩阵摄像机";

        public IEditableItemData Copy() => new ASMPlayerCameraSetterData { Name = this.Name };

        public void LoadString(string data) => Name = data;

        public string SaveString() => Name;
    }
}