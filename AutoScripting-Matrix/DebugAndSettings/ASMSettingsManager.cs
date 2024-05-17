using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Engine;
using TemplatesDatabase;
using XmlUtilities;
using SerializableKeyValuePair = Game.ASMSerializableDictionary<string, object>.SerializableKeyValuePair;

namespace Game {
    public static class ASMSettingsManager {
        public static string ExtPath => VersionsManager.Platform == Platform.Ios || VersionsManager.Platform == Platform.Android ? "config:" : "app:";

        public static string SettingsPath = ExtPath + "/ASMSettings.xml";

        public static ValuesDictionary Settings = new ValuesDictionary();

        public static SerializableKeyValuePair[] SettingsItems = [
            new SerializableKeyValuePair("DisplayUpperLeft", false),
            new SerializableKeyValuePair("DisplayBackRect", true),
            new SerializableKeyValuePair("GizmosEnable", false),

            new SerializableKeyValuePair("DisplayConnectorMatrix", false),
            new SerializableKeyValuePair("DisplayConnectorMatrix.DisplayIn", false),
            new SerializableKeyValuePair("DisplayConnectorMatrix.Thickness", 1f),

            new SerializableKeyValuePair("TPPlateVolume", 1f),
        ];

        public static void Init() {
            if (Storage.FileExists(SettingsPath)) {//已有settings
                using (Stream asmSettingsFile = Storage.OpenFile(SettingsPath, OpenFileMode.CreateOrOpen)) {
                    try {
                        XElement xElement = XmlUtils.LoadXmlFromStream(asmSettingsFile, null, throwOnError: true);
                        Settings.ApplyOverrides(xElement);

                        //检测序列化后的字典是否包含所有设置项
                        foreach (var pair in SettingsItems) {
                            if(Settings.ContainsKey(pair.Key)) continue;
                            Settings.Add(pair.Key, pair.Value);
                            Log.Warning($"[智械-矩阵:设置管理器] 检测到载入的设置未包含项\"{pair.Key}\"，已自动补充缺失的项");
                        }
                    }
                    catch (Exception e) {
                        Log.Warning($"[智械-矩阵:设置管理器] 设置文件非法\r\n原因: {e.Message}");
                        CreateDefaultSettings();
                    }

                }
            }
            else {//不存在，则创建settings并且赋默认值
                CreateDefaultSettings();
            }
        }

        private static void CreateDefaultSettings() {
            Settings = new ValuesDictionary();
            foreach (var pair in SettingsItems) {
                Settings.Add(pair.Key, pair.Value);
            }
        }

        public static void Save() {
            Task.Run(
                () => {
                    using (Stream stream = Storage.OpenFile(SettingsPath, OpenFileMode.Create)) {
                        XElement xElement = new XElement("ASMSettings");
                        Settings.Save(xElement);
                        XmlUtils.SaveXmlToStream(xElement, stream, null, throwOnError: true);
                    }
                    Log.Information($"[智械-矩阵:设置管理器] 保存设置");
                }
            );
        }

        private static object Get(string key) => Settings[key];

        public static T Get<T>(string key) => Settings.GetValue<T>(key);

        public static void Set(string key, object value) => Settings[key] = value;
    }
}