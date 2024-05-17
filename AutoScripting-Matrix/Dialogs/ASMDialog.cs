using System.Xml.Linq;

namespace Game {
    public class ASMDialog : Dialog {
        public ASMDialog(string dialogPath) {
            XElement node = ContentManager.Get<XElement>(dialogPath);
            LoadContents(this, node);
            RectangleWidget backRect = Children.Find<RectangleWidget>("BackRect");
            if (backRect != null) {
                backRect.IsVisible = ASMSettingsManager.Get<bool>("DisplayBackRect");
            }
        }
    }
}