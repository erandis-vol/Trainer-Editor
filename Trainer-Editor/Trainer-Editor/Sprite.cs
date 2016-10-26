using System.Drawing;

namespace Lost
{
    public static class Sprites
    {
        public static Bitmap Draw16(byte[] sprite, int width, int height, Color[] palette)
        {
            var bmp = new Bitmap(width * 8, height * 8);

            int i = 0;
            for (int bY = 0; bY < height && i < sprite.Length; bY++)
            {
                for (int bX = 0; bX < width && i < sprite.Length; bX++)
                {
                    for (int y = 0; y < 8 && i < sprite.Length; y++)
                    {
                        for (int x = 0; x < 8 && i < sprite.Length; x += 2, i++)
                        {
                            var left = sprite[i] & 0xF;
                            var right = sprite[i] >> 4;

                            if (left > 0)
                                bmp.SetPixel(x + bX * 8, y + bY * 8, palette[left]);

                            if (right > 0)
                                bmp.SetPixel(x + 1 + bX * 8, y + bY * 8, palette[right]);
                        }
                    }
                }
            }
            
            return bmp;
        }
    }
}
