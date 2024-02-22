using Engine;
using Engine.Graphics;
using Engine.Media;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game {
    public class SubsystemASMExpandableLEDTexture : Subsystem {

        List<ASMELEDFacialData> m_facialData = new List<ASMELEDFacialData>();

        public Dictionary<ASMELEDFacialData, Texture2D> m_facialDataToSlot = new Dictionary<ASMELEDFacialData, Texture2D>();

        public override void Load(ValuesDictionary valuesDictionary) {
            GenerateFacialData();
            Image rawImage = ContentManager.Get<Image>("Textures/ASMGeBlock");
            for (int i = 0; i < m_facialData.Count; i++) {
                //添加数据入字典，方便查询格子
                ASMELEDFacialData facialData = m_facialData[i];

                Image slotImage = new (rawImage);

                //画点
                if(facialData.m_points[0]) DrawRectangle(slotImage, 0, 0, 2, 2);
                if(facialData.m_points[1]) DrawRectangle(slotImage, 30, 0, 2, 2);
                if(facialData.m_points[2]) DrawRectangle(slotImage, 30, 30, 2, 2);
                if(facialData.m_points[3]) DrawRectangle(slotImage, 0, 30, 2, 2);
                //画线
                if(facialData.m_sides[0]) DrawRectangle(slotImage, 2, 0, 28, 2);
                if(facialData.m_sides[1]) DrawRectangle(slotImage, 30, 2, 2, 28);
                if(facialData.m_sides[2]) DrawRectangle(slotImage, 2, 30, 28, 2);
                if(facialData.m_sides[3]) DrawRectangle(slotImage, 0, 2, 2, 28);

                m_facialDataToSlot.Add(facialData, Texture2D.Load(slotImage));

                #if false
                using (FileStream fileStream = new FileStream(@"D:\Slots\AAA" + i.ToString() + ".png", FileMode.OpenOrCreate)) {
                    Image.Save(slotImage, fileStream, ImageFileFormat.Png, true);
                }
                #endif
            }
        }

        private void GenerateFacialData() {
            bool[] points = [false, false, false, false];
            bool[] sides = [false, false, false, false];
            //无
            m_facialData.Add(new ASMELEDFacialData(points, sides));
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
            //3点带1线2
            for (int j = 0; j < 4; j++) {
                for (int i = 0; i < 4; i++) {
                    bool[] p = [true, true, true, true];
                    bool[] l = [false, false, false, false];

                    p[i] = false;
                    l[(i + j) % 4] = true;

                    m_facialData.Add(new ASMELEDFacialData(p, l));
                }
            }
            //只开1线口
            for (int i = 0; i < 4; i++) {
                bool[] p = [true, true, true, true];
                bool[] l = [true, true, true, true];

                l[i] = false;

                m_facialData.Add(new ASMELEDFacialData(p, l));
            }
            //只开2线口
            for (int i = 0; i < 4; i++) {
                for (int j = 1; j < 4 - i; j++) {
                    bool[] p = [true, true, true, true];
                    bool[] l = [true, true, true, true];
                    l[i] = false;
                    l[j + i] = false;
                    m_facialData.Add(new ASMELEDFacialData(p, l));
                }
            }
            //只开3线口
            for (int i = 0; i < 4; i++) {
                bool[] p = [true, true, true, true];
                bool[] l = [false, false, false, false];

                l[i] = true;

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