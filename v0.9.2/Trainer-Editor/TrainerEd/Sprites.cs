using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace HTE
{
    public static class Sprites
    {
        public static Bitmap DrawSprite16(byte[] spriteData, int width, int height, Color[] palette, bool drawColor0)
        {
            Bitmap bmp = new Bitmap(width * 8, height * 8);
            FastPixel fp = new FastPixel(bmp);
            fp.Lock();

            // draw
            int x = -1;
            int y = 0;
            int blockx = 0;
            int blocky = 0;
            for (int i = 0; i < spriteData.Length * 2; i++)
            {
                x++;
                if (x >= 8) // 8 pixels per block
                {
                    x = 0;
                    y++;
                }

                if (y >= 8) // 8 pixels per block
                {
                    y = 0;
                    blockx++;
                }

                if (blockx >= width) // width
                {
                    blockx = 0;
                    blocky++;
                }

                byte pal = spriteData[i / 2];
                if ((i & 1) == 0)
                    pal &= 0xF;
                else
                    pal = (byte)((pal & 0xF0) >> 4);

                try
                {
                    if (pal == 0 && !drawColor0)
                    { }
                    else
                    {
                        fp.SetPixel(x + blockx * 8, y + blocky * 8, palette[pal]);
                    }
                }
                catch (Exception ex) // Me no like.
                {

                }
            }

            fp.Unlock(true);
            return bmp;
        }
    }
}
