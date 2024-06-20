using Engine;
using TemplatesDatabase;

namespace Game {
    public class ASMHumanizedSourceOptions: IASMItemData<ASMHumanizedSourceOptions> {
        public List<ASMCreateMatrixOption> m_options;

        public Matrix GetMatrix() {
            Matrix result = Matrix.Identity;
            foreach (var option in m_options) {
                result *= MatchOptions(option.m_type, option.m_parameters);
            }
            return result;
        }


        public ValuesDictionary Save() {
            ValuesDictionary result = new();
            int index = 0;
            foreach (var option in m_options) {
                ValuesDictionary optionSaveDic = option.Save();
                result.SetValue($"Option {index++}", optionSaveDic);
            }
            return result;
        }

        public ASMHumanizedSourceOptions Load(ValuesDictionary valuesDictionary) {
            m_options.Clear();
            foreach (var valuePair in valuesDictionary) {
                if (valuePair.Key.Contains("Option") && valuePair.Value is ValuesDictionary optionValue) {
                    ASMCreateMatrixOption option = new ASMCreateMatrixOption().Load(optionValue);
                    m_options.Add(option);
                }
            }
            return this;
        }

        public ASMHumanizedSourceOptions Copy(ASMHumanizedSourceOptions value) => new() { m_options = new List<ASMCreateMatrixOption>(this.m_options) };

        static Matrix MatchOptions(ASMCreateMatrixOption.Type type, float[] parameters) {
            switch (type) {
                case ASMCreateMatrixOption.Type.Orthographic: return Matrix.CreateOrthographic(parameters[0], parameters[1], parameters[2], parameters[3]);
                case ASMCreateMatrixOption.Type.Perspective: return Matrix.CreatePerspective(parameters[0], parameters[1], parameters[2], parameters[3]);
                case ASMCreateMatrixOption.Type.Quaternion: return Matrix.CreateFromQuaternion(new Quaternion(parameters[0], parameters[1], parameters[2], parameters[3]));
                case ASMCreateMatrixOption.Type.Scale: return Matrix.CreateScale(parameters[0], parameters[1], parameters[2]);
                case ASMCreateMatrixOption.Type.Translate: return Matrix.CreateTranslation(parameters[0], parameters[1], parameters[2]);
                case ASMCreateMatrixOption.Type.AxisAngle: return Matrix.CreateFromAxisAngle(new Vector3(parameters[0], parameters[1], parameters[2]), parameters[3]);
                case ASMCreateMatrixOption.Type.LookAt: return Matrix.CreateLookAt(new Vector3(parameters[0], parameters[1], parameters[2]), new Vector3(parameters[3], parameters[4], parameters[5]), new Vector3(parameters[6], parameters[7], parameters[8]));
                case ASMCreateMatrixOption.Type.RotationX: return Matrix.CreateRotationX(parameters[0]);
                case ASMCreateMatrixOption.Type.RotationY: return Matrix.CreateRotationY(parameters[0]);
                case ASMCreateMatrixOption.Type.RotationZ: return Matrix.CreateRotationZ(parameters[0]);
                case ASMCreateMatrixOption.Type.OrthographicOffCenter:
                    return Matrix.CreateOrthographicOffCenter(
                        parameters[0],
                        parameters[1],
                        parameters[2],
                        parameters[3],
                        parameters[4],
                        parameters[5]
                    );
                case ASMCreateMatrixOption.Type.PerspectiveOffCenter:
                    return Matrix.CreatePerspectiveOffCenter(
                        parameters[0],
                        parameters[1],
                        parameters[2],
                        parameters[3],
                        parameters[4],
                        parameters[5]
                    );
                case ASMCreateMatrixOption.Type.YawPitchRoll: return Matrix.CreateFromYawPitchRoll(parameters[0], parameters[1], parameters[2]);
                case ASMCreateMatrixOption.Type.PerspectiveFieldOfView: return Matrix.CreatePerspectiveFieldOfView(parameters[0], parameters[1], parameters[2], parameters[3]);
                default: return Matrix.Zero;
            }
        }

        static int GetParamsCount(ASMCreateMatrixOption.Type type) {
            switch (type) {
                case ASMCreateMatrixOption.Type.Orthographic: return 4;
                case ASMCreateMatrixOption.Type.Perspective: return 4;
                case ASMCreateMatrixOption.Type.Quaternion: return 4;
                case ASMCreateMatrixOption.Type.Scale: return 3;
                case ASMCreateMatrixOption.Type.Translate: return 3;
                case ASMCreateMatrixOption.Type.AxisAngle: return 4;
                case ASMCreateMatrixOption.Type.LookAt: return 9;
                case ASMCreateMatrixOption.Type.RotationX: return 1;
                case ASMCreateMatrixOption.Type.RotationY: return 1;
                case ASMCreateMatrixOption.Type.RotationZ: return 1;
                case ASMCreateMatrixOption.Type.OrthographicOffCenter: return 6;
                case ASMCreateMatrixOption.Type.PerspectiveOffCenter: return 6;
                case ASMCreateMatrixOption.Type.YawPitchRoll: return 3;
                case ASMCreateMatrixOption.Type.PerspectiveFieldOfView: return 4;
                default: return 0;
            }
        }
    }
}