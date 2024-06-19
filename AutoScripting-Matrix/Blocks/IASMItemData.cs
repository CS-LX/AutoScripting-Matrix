using TemplatesDatabase;

namespace Game {
    public interface IASMItemData<T> {
        public ValuesDictionary Save();
        public T Load(ValuesDictionary valuesDictionary);

        public T Copy(T value);
    }
}