using Engine;

namespace Game {
    public static class ASMPlayerCameraSetterManager {
        public static (string, Point3, ASMComplexPerspectiveCamera)[] m_cameras = new (string, Point3, ASMComplexPerspectiveCamera)[4];
        public static bool[] m_isCameraActive = [false, false, false, false];
    }
}