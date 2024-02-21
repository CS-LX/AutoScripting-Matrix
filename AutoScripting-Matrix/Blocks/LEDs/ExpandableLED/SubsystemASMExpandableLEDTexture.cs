using Engine;
using Engine.Graphics;
using Engine.Media;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game {
    public class SubsystemASMExpandableLEDTexture : Subsystem {
        List<ASMELEDFacialData> m_facialData = new List<ASMELEDFacialData>();

        public Dictionary<ASMELEDFacialData, int> m_facialDataToSlot = new Dictionary<ASMELEDFacialData, int>();

        public static Texture2D m_ledTexture = null;

        public override void Load(ValuesDictionary valuesDictionary) {
            GenerateFacialData();
            Image rawImage = ContentManager.Get<Image>("Textures/ASMExpandableLED");
            for (int i = 0; i < m_facialData.Count; i++) {
                //添加数据入字典，方便查询格子
                ASMELEDFacialData facialData = m_facialData[i];
                m_facialDataToSlot.Add(facialData, i);

                //获取该在哪个格子绘制
                int slotX = i % 8;
                int slotY = (int)MathUtils.Floor(i / 8f);

                //画点
                if(facialData.m_points[0]) DrawRectangle(rawImage, slotX * 32, slotY * 32, 2, 2);
                if(facialData.m_points[1]) DrawRectangle(rawImage, slotX * 32 + 30, slotY * 32, 2, 2);
                if(facialData.m_points[2]) DrawRectangle(rawImage, slotX * 32 + 30, slotY * 32 + 30, 2, 2);
                if(facialData.m_points[3]) DrawRectangle(rawImage, slotX * 32, slotY * 32 + 30, 2, 2);
                //画线
                if(facialData.m_sides[0]) DrawRectangle(rawImage, slotX * 32 + 2, slotY * 32, 28, 2);
                if(facialData.m_sides[1]) DrawRectangle(rawImage, slotX * 32 + 30, slotY * 32 + 2, 2, 28);
                if(facialData.m_sides[2]) DrawRectangle(rawImage, slotX * 32 + 2, slotY * 32 + 30, 28, 2);
                if(facialData.m_sides[3]) DrawRectangle(rawImage, slotX * 32, slotY * 32 + 2, 2, 28);
            }
            using (FileStream stream = new FileStream(@"D:\AAA.png", FileMode.OpenOrCreate)) {
                Image.Save(rawImage, stream, ImageFileFormat.Png, true);
            }
        }

        private void GenerateFacialData() {
            bool[] points = [false, false, false, false];
            bool[] sides = [false, false, false, false];
            //1点
            for (int i = 0; i < 4; i++) {
                bool[] p = [false, false, false, false];
                p[i] = true;
                m_facialData.Add(new ASMELEDFacialData(p, sides));
            }
            //2点
            for (int i = 0; i < 4; i++) {
                for (int j = 1; j < 4 - i; j++) {
                    bool[] p = [false, false, false, false];
                    p[i] = true;
                    p[j + i] = true;
                    m_facialData.Add(new ASMELEDFacialData(p, sides));
                }
            }
            //3点
            for (int i = 0; i < 4; i++) {
                bool[] p = [true, true, true, true];
                p[i] = false;
                m_facialData.Add(new ASMELEDFacialData(p, sides));
            }
            //4点
            m_facialData.Add(new ASMELEDFacialData([true, true, true, true], sides));
            //2点夹1线
            for (int i = 0; i < 4; i++) {
                bool[] l = [false, false, false, false];
                bool[] p = [false, false, false, false];

                l[i] = true;
                p[i] = true;
                p[(i + 1) % 4] = true;

                m_facialData.Add(new ASMELEDFacialData(p, l));
            }
            //3点夹2线
            for (int i = 0; i < 4; i++) {
                bool[] p = [true, true, true, true];
                bool[] l = [true, true, true, true];

                p[i] = false;
                l[i] = false;
                l[(i + 3) % 4] = false;

                m_facialData.Add(new ASMELEDFacialData(p, l));
            }
            //只开1线口
            for (int i = 0; i < 4; i++) {
                bool[] p = [true, true, true, true];
                bool[] l = [true, true, true, true];

                l[i] = false;

                m_facialData.Add(new ASMELEDFacialData(p, l));
            }
            //全满
            m_facialData.Add(new ASMELEDFacialData([true, true, true, true], [true, true, true, true]));
        }

        private void DrawRectangle(Image image, int x, int y, int w, int h) {
            for (int i = 0; i < w; i++) {
                for (int j = 0; j < h; j++) {
                    int nx = x + i;
                    int ny = y + j;
                    Color baseColor = image.GetPixel(nx, ny);
                    image.SetPixel(nx, ny, new Color(baseColor.R / 2, baseColor.G / 2, baseColor.B / 2));
                }
            }
        }

    }
}