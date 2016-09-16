using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lost
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
            var nameTable = romInfo.GetInt32("pokemon", "Names", 16);

            rom.Seek(nameTable);
            pokemon = rom.ReadTextTable(11, pokemonCount, CharacterEncoding.English);
        }

        void LoadAttacks()
        {
            var table = romInfo.GetInt32("attacks", "Names", 16);

            rom.Seek(table);
            attacks = rom.ReadTextTable(13, attackCount, CharacterEncoding.English);
        }

        void LoadItems()
        {
            var firstItem = romInfo.GetInt32("items", "Data", 16);

            items = new string[itemCount];
            for (int i = 0; i < itemCount; i++)
            {
                rom.Seek(firstItem + i * 44);
                items[i] = rom.ReadText(14, CharacterEncoding.English);
            }
        }

        Bitmap LoadFrontSprite(int id)
        {
            try
            {
                // ------------------------------
                // read compressed sprite
                rom.Seek(romInfo.GetInt32("pokemon_sprites", "FrontData", 16) + id * 8);
                var spriteOffset = rom.ReadPointer();

                rom.Seek(spriteOffset);
                var sprite = rom.ReadCompressedBytes();

                // ------------------------------
                // read compressed palette
                rom.Seek(romInfo.GetInt32("pokemon_sprites", "RegularPalettes", 16) + id * 8);
                var paletteOffset = rom.ReadPointer();

                rom.Seek(paletteOffset);
                var palette = rom.ReadCompressedPalette();

                // ------------------------------
                return Sprites.Draw16(sprite, 8, 8, palette, false);
            }
            catch
            {
                return invisible;
            }
        }
    }
}
