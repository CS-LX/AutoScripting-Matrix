using Engine;
using Engine.Serialization;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game {
    public class SubsystemASMExpandableLEDController : Subsystem, IUpdateable {

        public UpdateOrder UpdateOrder => UpdateOrder.Default;

        public Dictionary<Point3, ASMELEDController> m_controllers = new Dictionary<Point3, ASMELEDController>();

        public void Update(float dt) {
        }

        public ASMELEDController AddController(Point3 point) {//加入控制器
            if (m_controllers.Keys.Contains(point)) throw new Exception($"在此位置({point})已经存在控制器!");
            ASMELEDController controller = new ASMELEDController();
            controller.Position = point;
            m_controllers.Add(point, controller);
            return controller;
        }

        public void RemoveController(Point3 point) {//移除控制器
            m_controllers.Remove(point);
        }

        public ASMELEDController FindController(Point3 point) {
            m_controllers.TryGetValue(point, out ASMELEDController controller);
            return controller;
        }

        public Point3 FindPoint(ASMELEDController controller) => m_controllers.FirstOrDefault(p => p.Value == controller).Key;

        public override void Save(ValuesDictionary valuesDictionary) {
            base.Save(valuesDictionary);
            var valuesDictionary2 = new ValuesDictionary();
            valuesDictionary.SetValue("Blocks", valuesDictionary2);
            foreach (KeyValuePair<Point3, ASMELEDController> blocksDatum in m_controllers)
            {
                valuesDictionary2.SetValue(HumanReadableConverter.ConvertToString(blocksDatum.Key), blocksDatum.Value.SaveToString());
            }
        }

        public override void Load(ValuesDictionary valuesDictionary) {
            base.Load(valuesDictionary);
            foreach (KeyValuePair<string, object> item in valuesDictionary.GetValue<ValuesDictionary>("Blocks"))
            {
                Point3 key = HumanReadableConverter.ConvertFromString<Point3>(item.Key);
                m_controllers[key] = ASMELEDController.CreateByString(item.Value.ToString());
                m_controllers[key].Position = key;
            }
        }
    }
}