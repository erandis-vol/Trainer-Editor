using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lost
{
    public partial class MainForm
    {
        int pokemonCount;
        int typeCount;
        int itemCount;
        int attackCount;

        string[] pokemon;
        string[] items;
        string[] types;
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
            attacks = rom.ReadTextTable(13, 0, CharacterEncoding.English);
        }

        void LoadTypes()
        {
            var nameTable = romInfo.GetInt32("types", "Names", 16);
            var typeChart = romInfo.GetInt32("types", "Data", 16);

            // first, load the type chart to find the number of types
            var lastType = 0;

            rom.Seek(typeChart);

            switch (romInfo.GetString("types", "Format"))
            {
                case "full-chart":
                    // format where for every type, data is stored
                    // (used by the EM battle engine upgrade)
                    // the idea is to analyze the data by reading until a value of
                    // 00, 05, 10, or 20 is not found again
                    throw new NotImplementedException("This type chart format \"full-chart\" is not implemented yet!");

                case "vanilla":
                // type chart as stored in a vanilla game
                // 3 bytes per entry, [attacker] [defender] [effectiveness]
                // terminated by ff fe 00
                // second chart follows for foresight, terminated by ff ff 00
                default:
                    // normal type data
                    while (rom.PeekByte() != 0xFF)
                    {
                        var attacker = rom.ReadByte();
                        var defender = rom.ReadByte();
                        var effectiveness = rom.ReadByte();

                        if (lastType < attacker)
                            lastType = attacker;
                        if (lastType < defender)
                            lastType = defender;

                        if (rom.PeekByte() == 0xFE)
                        {
                            rom.Skip(3);
                            break;
                        }
                    }

                    // foresight type data
                    while (rom.PeekByte() != 0xFF)
                    {
                        var attacker = rom.ReadByte();
                        var defender = rom.ReadByte();
                        var effectiveness = rom.ReadByte();

                        if (lastType < attacker)
                            lastType = attacker;
                        if (lastType < defender)
                            lastType = defender;
                    }
                    break;
            }

            // get typeCount
            typeCount = lastType + 1;

            // load type names now
            rom.Seek(nameTable);
            types = rom.ReadTextTable(7, typeCount, CharacterEncoding.English);
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
    }
}
