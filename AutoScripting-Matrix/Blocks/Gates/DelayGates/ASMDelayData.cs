namespace Game {
    public class ASMDelayData : IEditableItemData {
        public int Data = 0;

        public IEditableItemData Copy() => new ASMDelayData{Data = this.Data};

        public void LoadString(string data) => Data = int.Parse(data);

        public string SaveString() => Data.ToString();
    }
}