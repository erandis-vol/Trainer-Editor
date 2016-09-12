using System.Drawing;

namespace Lost
{
    public static class Sprites
    {
        public static Bitmap Draw16(byte[] sprite, int width, int height, Color[] palette, bool showColor0 = true)
        {
            var bmp = new Bitmap(width * 8, height * 8);

            int x = -1;
            int y = 0;
            int blockX = 0;
            int blockY = 0;

            for (int i = 0; i < sprite.Length * 2; i++)
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
                    blockX++;
                }

                if (blockX >= width) // width
                {
                    blockX = 0;
                    blockY++;
                }

                byte pal = sprite[i / 2];
                if ((i & 1) == 0)
                    pal &= 0xF;
                else
                    pal = (byte)((pal & 0xF0) >> 4);

                if (pal == 0 && !showColor0)
                    ;
                else
                    bmp.SetPixel(x + blockX * 8, y + blockY * 8, palette[pal]);
            }

            return bmp;
        }
    }
}
