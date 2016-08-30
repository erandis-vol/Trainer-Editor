using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lost
{
    public partial class MainForm
    {
        int trainerCount;

        string[] names;

        void LoadNames()
        {
            var firstTrainer = romInfo.GetInt32("trainers", "Data", 16);

            names = new string[trainerCount];
            for (int i = 0; i < trainerCount; i++)
            {
                rom.Seek(firstTrainer + i * 0x28 + 4);
                names[i] = rom.ReadText(12, CharacterEncoding.English);
            }
        }
    }
}
