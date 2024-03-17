using System.Xml.Serialization;
using Engine;
using SerializableKeyValuePair = Game.ASMSerializableDictionary<string, object>.SerializableKeyValuePair;

namespace Game {
    public static class ASMSettingsManager {
        public static string ExtPath => VersionsManager.Platform == Platform.Ios || VersionsManager.Platform == Platform.Android ? "config:" : "app:";

        public static string SettingsPath = ExtPath + "/ASMSettings.xml";

        public static ASMSerializableDictionary<string, object> Settings;

        public static SerializableKeyValuePair[] SettingsItems = [
            new SerializableKeyValuePair("DisplayUpperLeft", false),
            new SerializableKeyValuePair("DisplayBackRect", true),

            new SerializableKeyValuePair("DisplayConnectorMatrix", false),
            new SerializableKeyValuePair("DisplayConnectorMatrix.DisplayIn", false),
            new SerializableKeyValuePair("DisplayConnectorMatrix.Thickness", 1),

            new SerializableKeyValuePair("TPPlateVolume", 1),
        ];

        public static void Init() {
            if (Storage.FileExists(SettingsPath)) {//已有settings
                using (Stream asmSettingsFile = Storage.OpenFile(SettingsPath, OpenFileMode.CreateOrOpen)) {
                    try {
                        XmlSerializer serializer = new (typeof(ASMSerializableDictionary<string, object>));
                        Settings = (ASMSerializableDictionary<string, object>)serializer.Deserialize(asmSettingsFile);

                        //检测序列化后的字典是否包含所有设置项
                        foreach (var pair in SettingsItems) {
                            if(Settings.ContainsKey(pair.Key)) continue;
                            Settings.Add(pair);
                            Log.Warning($"[智械-矩阵:设置管理器] 检测到载入的设置未包含项\"{pair.Key}\"，已自动补充缺失的项。");
                        }
                    }
                    catch (Exception e) {
                        Log.Warning($"[智械-矩阵:设置管理器] 设置文件非法。");
                        CreateDefaultSettings();
                    }

                }
            }
            else {//不存在，则创建settings并且赋默认值
                CreateDefaultSettings();
            }
        }

        private static void CreateDefaultSettings() {
            Settings = new ASMSerializableDictionary<string, object>();
            foreach (var pair in SettingsItems) {
                Settings.Add(pair);
            }
        }

        public static void Save() {
            Task.Run(
                () => {
                    using (Stream stream = Storage.OpenFile(SettingsPath, OpenFileMode.CreateOrOpen)) {
                        XmlSerializer serializer = new(typeof(ASMSerializableDictionary<string, object>));
                        serializer.Serialize(stream, Settings);
                    }
                    Log.Information($"[智械-矩阵:设置管理器] 保存设置。");
                }
            );
        }

        public static object Get(string key) => Settings[key];

        public static void Set(string key, object value) => Settings[key] = value;
    }
}