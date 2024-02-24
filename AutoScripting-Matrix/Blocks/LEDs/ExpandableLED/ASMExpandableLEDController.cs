using Engine;

namespace Game {
    public class ASMExpandableLEDController {
        /// <summary>
        /// 表示在内存中的位置（唯一ID）
        /// </summary>
        public int ID => GetHashCode();

        /// <summary>
        /// 所有控制的元件的数量
        /// </summary>
        public int ControlledCount => m_ledElements.Count;

        /// <summary>
        /// 所控制的所有LED元件以及坐标
        /// </summary>
        public Dictionary<CellFace, ASMExpandableLEDElectricElement> m_ledElements = new Dictionary<CellFace, ASMExpandableLEDElectricElement>();

        public Matrix DisplayMatrix;

        /// <summary>
        /// 通过指定一个控制器所管辖的LED来实例化一个控制器（实例化中控制器会执行CollectCellsMatrix）
        /// </summary>
        /// <param name="ledElectricElements"></param>
        public ASMExpandableLEDController(Dictionary<CellFace, ASMExpandableLEDElectricElement> ledElectricElements) {
            m_ledElements = ledElectricElements;
            CollectCellsMatrix();
        }

        /// <summary>
        /// 重新获取管辖的所有LED方块的显示电压，并且加在一起作为最终的显示电压
        /// </summary>
        public void CollectCellsMatrix() {
            DisplayMatrix = Matrix.Zero;
            foreach (var ledElement in m_ledElements.Values) {
                if(ledElement == null) continue;
                DisplayMatrix += ledElement.OutputMatrixToController();
            }
        }
    }
}