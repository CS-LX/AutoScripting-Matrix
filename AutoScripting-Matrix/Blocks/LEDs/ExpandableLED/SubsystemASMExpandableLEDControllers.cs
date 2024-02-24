using GameEntitySystem;

namespace Game {
    public class SubsystemASMExpandableLEDControllers : Subsystem {
        public Dictionary<CellFace, ASMExpandableLEDController> m_positionToControllers = new();

        /// <summary>
        /// 名副其实
        /// </summary>
        /// <param name="cellFace"></param>
        /// <returns></returns>
        public bool IsThereAController(CellFace cellFace) => m_positionToControllers.ContainsKey(cellFace);

        /// <summary>
        /// 通过坐标获取储存在字典里的LED控制器，如果找不到，返回null
        /// </summary>
        /// <param name="cellFace">LED指定坐标</param>
        /// <returns>在这个位置对应的控制器</returns>
        public ASMExpandableLEDController? GetController(CellFace cellFace) => m_positionToControllers.ContainsKey(cellFace) ? m_positionToControllers[cellFace] : null;

        /// <summary>
        /// 获取使用同一个控制器实例的坐标
        /// </summary>
        /// <param name="controller">控制器实例</param>
        /// <returns>使用同一个控制器实例的坐标，如果没有则返回空列表</returns>
        public CellFace[] GetCellFaces(ASMExpandableLEDController controller) {
            List<CellFace> cellFaces = new();
            foreach (var cells in m_positionToControllers) {
                if(cells.Value == controller) cellFaces.Add(cells.Key);
            }
            return cellFaces.ToArray();
        }

        /// <summary>
        /// 获取使用同一个控制器实例的坐标（通过Linq查询）
        /// </summary>
        /// <param name="controller">控制器实例</param>
        /// <returns>使用同一个控制器实例的坐标，如果没有则返回空列表</returns>
        public CellFace[] GetCellFacesByLinq(ASMExpandableLEDController controller) {
            List<CellFace> cellFaces = new();
            cellFaces.AddRange(m_positionToControllers.Where(x => x.Value == controller).Select(x => x.Key));
            return cellFaces.ToArray();
        }

        /// <summary>
        /// 给一系列坐标的LED指定同一个控制器实例
        /// </summary>
        /// <param name="cellFaces">坐标集合（不会重复添加）</param>
        /// <param name="controller">对应控制器</param>
        public void AddControllerByPoints(CellFace[] cellFaces, ASMExpandableLEDController controller) {
            foreach (var cellFace in cellFaces) {
                if (m_positionToControllers.ContainsKey(cellFace)) continue;
                m_positionToControllers.Add(cellFace, controller);
            }
        }

        /// <summary>
        /// 删除指定坐标点LED与实例的对应关系
        /// </summary>
        /// <param name="cellFace"></param>
        /// <returns></returns>
        public bool RemovePoint(CellFace cellFace) => m_positionToControllers.Remove(cellFace);

        /// <summary>
        /// 删除一系列坐标点LED与实例的对应关系
        /// </summary>
        /// <param name="cellFaces"></param>
        public void RemovePoints(CellFace[] cellFaces) {
            foreach (var cellFace in cellFaces) {
                m_positionToControllers.Remove(cellFace);
            }
        }

        /// <summary>
        /// 删除某一个控制器，以及它与所有对应坐标点的对应关系
        /// </summary>
        /// <param name="controller"></param>
        public void RemoveController(ASMExpandableLEDController controller) {
            CellFace[] cellFaces = GetCellFaces(controller);
            foreach (var cellFace in cellFaces) {
                m_positionToControllers.Remove(cellFace);
            }
        }

        /// <summary>
        /// 获取储存的控制器实例数量（自动去重）
        /// </summary>
        /// <returns></returns>
        public int GetControllersCount() => m_positionToControllers.Values.Distinct().Count();
    }
}