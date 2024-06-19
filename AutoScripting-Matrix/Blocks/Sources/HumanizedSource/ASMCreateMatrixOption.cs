using Engine;
using TemplatesDatabase;

namespace Game {
    public struct ASMCreateMatrixOption : IASMItemData<ASMCreateMatrixOption> {
        public enum Type {
            Translate,
            AxisAngle,
            Quaternion,
            YawPitchRoll,
            LookAt,
            Orthographic,
            OrthographicOffCenter,
            Perspective,
            PerspectiveFieldOfView,
            PerspectiveOffCenter,
            RotationX,
            RotationY,
            RotationZ,
            Scale
        }

        public float[] m_parameters;

        public int m_parametersCount;

        public Type m_type;

        public ASMCreateMatrixOption(int parametersCount) {
            m_parameters = new float[parametersCount];
            m_parametersCount = parametersCount;
        }

        public ASMCreateMatrixOption(float[] parameters) {
            m_parameters = parameters;
            m_parametersCount = parameters.Length;
        }

        public void SetParameters(float[] parameters) {
            for (int i = 0; i < m_parametersCount; i++) m_parameters[i] = parameters[i];
        }

        public ValuesDictionary Save() {
            ValuesDictionary result = new();
            result.SetValue("Type", m_type);
            for (int i = 0; i < m_parametersCount; i++) {
                result.SetValue($"Parameter {i}", m_parameters[i]);
            }
            return result;
        }

        public ASMCreateMatrixOption Load(ValuesDictionary valuesDictionary) {
            m_type = valuesDictionary.GetValue<Type>("Type");
            int paramCount = 0;
            foreach (var kv in valuesDictionary) {
                if (kv.Key.Contains("Parameter")) {
                    m_parameters[paramCount++] = (float)kv.Value;
                }
            }
            m_parametersCount = paramCount;
            return this;
        }

        public ASMCreateMatrixOption Copy(ASMCreateMatrixOption value) => value;
    }
}