using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lost
{
    public partial class MainForm
    {
        Trainer trainer;

        int trainerCount;
        int classCount;

        string[] names;
        string[] classes;

        void LoadTrainer(int index)
        {
            trainer = new Trainer(index);
            rom.Seek(romInfo.GetInt32("trainers", "Data", 16) + index * 40);

            // read trainer data
            var flags = rom.ReadByte();
                trainer.HasCustomAttacks = (flags & 1) == 1;
                trainer.HasHeldItems = (flags & 2) == 2;
            trainer.Class = rom.ReadByte();
            var genderMusic = rom.ReadByte();
                trainer.Gender = (byte)((genderMusic & 128) >> 7);
                trainer.Music = (byte)(genderMusic & 127);
            trainer.Sprite = rom.ReadByte();
            trainer.Name = rom.ReadText(12, CharacterEncoding.English);
            for (int i = 0; i < 4; i++)
                trainer.Items[i] = rom.ReadUInt16();
            trainer.DoubleBattle = rom.ReadByte() == 1;
            rom.Skip(3);
            trainer.AI = rom.ReadUInt32();
            var partyCount = rom.ReadByte();
            rom.Skip(3);
            var partyStart = rom.ReadPointer();
            // todo: there's more padding ?

            // read party data
            if (partyCount == 0) return;

            trainer.PartyOffset = partyStart;
            rom.Seek(partyStart);

            for (int i = 0; i < partyCount; i++)
            {
                var p = new Pokemon();
                p.EVs = rom.ReadUInt16();
                p.Level = rom.ReadUInt16();
                p.Species = rom.ReadUInt16();

                if (trainer.HasHeldItems)
                    p.HeldItem = rom.ReadUInt16();

                if (trainer.HasCustomAttacks)
                    for (int j = 0; j < 4; j++)
                        p.Attacks[j] = rom.ReadUInt16();

                if (!trainer.HasHeldItems) // TODO: would be neat to edit this
                    rom.Skip(2);

                trainer.Party.Add(p);
            }

            trainer.OriginalPartySize = rom.Position - partyStart;
        }

        void LoadNames()
        {
            var firstTrainer = romInfo.GetInt32("trainers", "Data", 16);

            names = new string[trainerCount];
            for (int i = 0; i < trainerCount; i++)
            {
                rom.Seek(firstTrainer + i * 40 + 4);
                names[i] = rom.ReadText(12, CharacterEncoding.English);
            }
        }

        void LoadClasses()
        {
            var table = romInfo.GetInt32("trainer_classes", "Names", 16);

            rom.Seek(table);
            classes = rom.ReadTextTable(13, classCount, CharacterEncoding.English);
        }
    }
}
