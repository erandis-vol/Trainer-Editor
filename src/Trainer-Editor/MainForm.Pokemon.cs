using System;
using System.Drawing;
using GBAHL.Drawing;
using GBAHL.IO;
using GBAHL.Text;

namespace Hopeless
{
    public partial class MainForm
    {
        int pokemonCount;
        int itemCount;
        int attackCount;

        string[] pokemon;
        string[] items;
        string[] attacks;

        void LoadPokemonNames()
        {
            var nameTablePtr = romInfo.GetInt32("pokemon", "Names", 16);
            rom.Seek(nameTablePtr);
            rom.ReadPointerAndSeek();
            pokemon = rom.ReadTextTable(11, pokemonCount, Table.Encoding.English);
        }

        void LoadAttacks()
        {
            var tablePtr = romInfo.GetInt32("attacks", "Names", 16);
            rom.Seek(tablePtr);
            rom.ReadPointerAndSeek();
            attacks = rom.ReadTextTable(13, attackCount, Table.Encoding.English);
        }

        void LoadItems()
        {
            var firstItemPtr = romInfo.GetInt32("items", "Data", 16);
            rom.Seek(firstItemPtr);
            var firstItem = rom.ReadPointer();

            items = new string[itemCount];
            for (int i = 0; i < itemCount; i++)
            {
                rom.Seek(firstItem + i * 44);
                items[i] = rom.ReadText(14, Table.Encoding.English);
            }
        }

        Image LoadFrontSprite(int id)
        {
            Sprite sprite;
            Palette palette;

            try
            {
                // ------------------------------
                // read compressed sprite
                rom.Seek(romInfo.GetInt32("pokemon_sprites", "FrontData", 16));
                rom.ReadPointerAndSeek();
                rom.Skip(id * 8);
                rom.ReadPointerAndSeek();
                sprite = rom.ReadCompressedSprite4();

                // ------------------------------
                // read compressed palette
                rom.Seek(romInfo.GetInt32("pokemon_sprites", "RegularPalettes", 16));
                rom.ReadPointerAndSeek();
                rom.Skip(id * 8);
                rom.ReadPointerAndSeek();
                if (rom.PeekCompressed())
                    palette = rom.ReadCompressedPalette();
                else
                    palette = rom.ReadPalette(16);

                // ------------------------------
                return sprite.ToImage(8, 8, palette, false);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
#endif
                return invisible;
            }
        }
    }
}
