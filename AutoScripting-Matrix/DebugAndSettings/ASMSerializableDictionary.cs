using System.Collections;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml.Schema;
using System.Xml;
using Engine;

namespace Game {
    public class ASMSerializableDictionary;

    [Serializable]
    public class ASMSerializableDictionary<TKey, TValue> : ASMSerializableDictionary, IDictionary<TKey, TValue>, IXmlSerializable {
        private List<SerializableKeyValuePair> list = new List<SerializableKeyValuePair>();

        [Serializable]
        public struct SerializableKeyValuePair {
            public TKey Key;
            public TValue Value;

            public SerializableKeyValuePair(TKey key, TValue value) {
                Key = key;
                Value = value;
            }
        }

        private Type[] supportTypes = new Type[] {
            typeof(Color),
            typeof(Vector3),
            typeof(Vector4),
            typeof(Quaternion),
            typeof(Vector2),
        };

        private Dictionary<TKey, int> KeyPositions => _keyPositions.Value;
        private Lazy<Dictionary<TKey, int>> _keyPositions;

        public ASMSerializableDictionary() {
            _keyPositions = new Lazy<Dictionary<TKey, int>>(MakeKeyPositions);
        }

        private Dictionary<TKey, int> MakeKeyPositions() {
            var dictionary = new Dictionary<TKey, int>(list.Count);
            for (var i = 0; i < list.Count; i++) {
                dictionary[list[i].Key] = i;
            }
            return dictionary;
        }

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize() {
            _keyPositions = new Lazy<Dictionary<TKey, int>>(MakeKeyPositions);
        }

        /// <summary>
        /// 从对象的 XML 表示形式生成该对象
        /// </summary>
        /// <param name="reader"></param>
        public void ReadXml(XmlReader reader) {
            var keySerializer = new XmlSerializer(typeof(TKey), supportTypes);
            var valueSerializer = new XmlSerializer(typeof(TValue), supportTypes);
            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();
            if (wasEmpty) return;
            while (reader.NodeType != XmlNodeType.EndElement) {
                reader.ReadStartElement("item");
                reader.ReadStartElement("key");
                var key = (TKey)keySerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadStartElement("value");
                var value = (TValue)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();
                Add(key, value);
                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        /// <summary>
        /// 将对象转换为其 XML 表示形式
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(XmlWriter writer) {
            var keySerializer = new XmlSerializer(typeof(TKey), supportTypes);
            var valueSerializer = new XmlSerializer(typeof(TValue), supportTypes);
            foreach (TKey key in Keys) {
                writer.WriteStartElement("item");
                writer.WriteStartElement("key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();
                writer.WriteStartElement("value");
                TValue value = this[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }

        public XmlSchema GetSchema() {
            return null;
        }

        #region IDictionary<TKey, TValue>

        public TValue this[TKey key] {
            get => list[KeyPositions[key]].Value;
            set {
                var pair = new SerializableKeyValuePair(key, value);
                if (KeyPositions.ContainsKey(key)) {
                    list[KeyPositions[key]] = pair;
                }
                else {
                    KeyPositions[key] = list.Count;
                    list.Add(pair);
                }
            }
        }

        public ICollection<TKey> Keys => list.Select(tuple => tuple.Key).ToArray();
        public ICollection<TValue> Values => list.Select(tuple => tuple.Value).ToArray();

        public void Add(TKey key, TValue value) {
            if (KeyPositions.ContainsKey(key))
                throw new ArgumentException("An element with the same key already exists in the dictionary.");
            else {
                KeyPositions[key] = list.Count;
                list.Add(new SerializableKeyValuePair(key, value));
            }
        }

        public void Add(SerializableKeyValuePair pair) => Add(pair.Key, pair.Value);

        public bool ContainsKey(TKey key) => KeyPositions.ContainsKey(key);

        public bool Remove(TKey key) {
            if (KeyPositions.TryGetValue(key, out var index)) {
                KeyPositions.Remove(key);
                list.RemoveAt(index);
                for (var i = index; i < list.Count; i++) KeyPositions[list[i].Key] = i;
                return true;
            }
            else
                return false;
        }

        public bool TryGetValue(TKey key, out TValue value) {
            if (KeyPositions.TryGetValue(key, out var index)) {
                value = list[index].Value;
                return true;
            }
            else {
                value = default;
                return false;
            }
        }

        #endregion

        #region ICollection <KeyValuePair<TKey, TValue>>

        public int Count => list.Count;
        public bool IsReadOnly => false;

        public void Add(KeyValuePair<TKey, TValue> kvp) => Add(kvp.Key, kvp.Value);

        public void Clear() => list.Clear();
        public bool Contains(KeyValuePair<TKey, TValue> kvp) => KeyPositions.ContainsKey(kvp.Key);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            var numKeys = list.Count;
            if (array.Length - arrayIndex < numKeys) throw new ArgumentException("arrayIndex");
            for (var i = 0; i < numKeys; i++, arrayIndex++) {
                var entry = list[i];
                array[arrayIndex] = new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> kvp) => Remove(kvp.Key);

        #endregion

        #region IEnumerable <KeyValuePair<TKey, TValue>>

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            return list.Select(ToKeyValuePair).GetEnumerator();

            static KeyValuePair<TKey, TValue> ToKeyValuePair(SerializableKeyValuePair skvp) {
                return new KeyValuePair<TKey, TValue>(skvp.Key, skvp.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }


    [XmlRoot("dictionary"), Serializable]
    public class XmlSerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable {
        public XmlSerializableDictionary() { }

        public XmlSerializableDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary) { }

        public XmlSerializableDictionary(IEqualityComparer<TKey> comparer) : base(comparer) { }


        public XmlSerializableDictionary(int capacity) : base(capacity) { }

        public XmlSerializableDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer) { }

        protected XmlSerializableDictionary(SerializationInfo info, StreamingContext context) : base(info, context) { }

        #region IXmlSerializable Members

        public XmlSchema GetSchema() {
            return null;
        }

        /// <summary>
        /// 从对象的 XML 表示形式生成该对象
        /// </summary>
        /// <param name="reader"></param>
        public void ReadXml(XmlReader reader) {
            var keySerializer = new XmlSerializer(typeof(TKey));
            var valueSerializer = new XmlSerializer(typeof(TValue));
            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();
            if (wasEmpty) return;
            while (reader.NodeType != XmlNodeType.EndElement) {
                reader.ReadStartElement("item");
                reader.ReadStartElement("key");
                var key = (TKey)keySerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadStartElement("value");
                var value = (TValue)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();
                Add(key, value);
                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        /// <summary>
        /// 将对象转换为其 XML 表示形式
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(XmlWriter writer) {
            var keySerializer = new XmlSerializer(typeof(TKey));
            var valueSerializer = new XmlSerializer(typeof(TValue));
            foreach (TKey key in Keys) {
                writer.WriteStartElement("item");
                writer.WriteStartElement("key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();
                writer.WriteStartElement("value");
                TValue value = this[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }

        #endregion
    }
}