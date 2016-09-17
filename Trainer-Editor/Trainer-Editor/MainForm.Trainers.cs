using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Lost
{
    public partial class MainForm
    {
        Trainer trainer;
        Pokemon member = null;

        int trainerCount;
        int trainerSpriteCount;
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
                var p = new Pokemon(i);
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

        void SaveTrainer()
        {
            // repoint party
            if (trainer.RequiresRepoint)
            {
                int newOffset = -1;

                // find freespace
                if (repointAutomaticallyToolStripMenuItem.Checked)
                {
                    newOffset = rom.FindFreeSpace(trainer.PartySize, 0xFF, 0x720000, 4);
                }
                else
                {
                    using (var fs = new FreeSpaceDialog(rom, trainer.PartySize, 0x720000))
                    {
                        fs.Text = "Repoint Party";

                        if (fs.ShowDialog() == DialogResult.OK)
                            newOffset = fs.Offset;
                    }
                }

                if (newOffset == -1)
                {
                    MessageBox.Show("Unable to repoint party!\nIt was not saved.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }

                // overwrite old party with freespace
                rom.Seek(trainer.PartyOffset);
                for (int i = 0; i < trainer.OriginalPartySize; i++)
                    rom.WriteByte(0xFF);

                // set new offset for party
                trainer.PartyOffset = newOffset;
            }

            rom.Seek(romInfo.GetInt32("trainers", "Data", 16) + trainer.Index * 40);

            // write trainer data
            rom.WriteByte((byte)((trainer.HasCustomAttacks ? 1 : 0) + (trainer.HasHeldItems ? 2 : 0)));
            rom.WriteByte(trainer.Class);
            rom.WriteByte((byte)((trainer.Gender << 7) + trainer.Music));
            rom.WriteByte(trainer.Sprite);
            rom.WriteText(trainer.Name, 12, CharacterEncoding.English);
            for (int i = 0; i < 4; i++)
                rom.WriteUInt16(trainer.Items[i]);
            rom.WriteByte((byte)(trainer.DoubleBattle ? 1 : 0));
            rom.Skip(3);
            rom.WriteUInt32(trainer.AI);
            rom.WriteByte((byte)trainer.Party.Count);
            rom.Skip(3);
            rom.WritePointer(trainer.PartyOffset);

            if (trainer.Party.Count == 0)
                return;

            // write party
            rom.Seek(trainer.PartyOffset);

            foreach (var p in trainer.Party)
            {
                rom.WriteUInt16(p.EVs);
                rom.WriteUInt16(p.Level);
                rom.WriteUInt16(p.Species);

                if (trainer.HasHeldItems)
                    rom.WriteUInt16(p.HeldItem);

                if (trainer.HasCustomAttacks)
                    for (int i = 0; i < 4; i++)
                        rom.WriteUInt16(p.Attacks[i]);

                if (!trainer.HasHeldItems)
                    rom.WriteUInt16(0);
            }

            // finally, update original party size
            trainer.OriginalPartySize = trainer.PartySize;
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

        void SaveClasses()
        {
            var table = romInfo.GetInt32("trainer_classes", "Names", 16);

            rom.Seek(table);
            rom.WriteTextTable(classes, 13, CharacterEncoding.English);
        }

        Bitmap LoadTrainerSprite(int id)
        {
            try
            {
                // ------------------------------
                // read compressed sprite
                rom.Seek(romInfo.GetInt32("trainer_sprites", "Data", 16) + id * 8);
                var spriteOffset = rom.ReadPointer();

                rom.Seek(spriteOffset);
                var sprite = rom.ReadCompressedBytes();

                // ------------------------------
                // read compressed palette
                rom.Seek(romInfo.GetInt32("trainer_sprites", "Palettes", 16) + id * 8);
                var paletteOffset = rom.ReadPointer();

                rom.Seek(paletteOffset);
                var palette = rom.ReadCompressedPalette();

                // ------------------------------
                return Sprites.Draw16(sprite, 8, 8, palette);
            }
            catch
            {
                return invisible;
            }
        }

        void ExportTrainer(string filename)
        {
            // dump a trainer to a file
            if (trainer == null)
                return;

            using (var fs = File.Create(filename))
            using (var bw = new BinaryWriter(fs))
            {
                // write trainer data
                bw.Write(trainer.Name);
                bw.Write(trainer.Class);
                bw.Write(trainer.Gender);
                bw.Write(trainer.Sprite);
                bw.Write(trainer.Music);
                for (int i = 0; i < 4; i++)
                    bw.Write(trainer.Items[i]);

                bw.Write(trainer.DoubleBattle);
                bw.Write(trainer.HasCustomAttacks);
                bw.Write(trainer.HasHeldItems);

                // write party
                bw.Write(trainer.Party.Count);
                foreach (var p in trainer.Party)
                {
                    bw.Write(p.Species);
                    bw.Write(p.Level);
                    bw.Write(p.EVs);
                    bw.Write(p.HeldItem);
                    for (int i = 0; i < 4; i++)
                        bw.Write(p.Attacks[i]);
                }

                // write game information
                // # of attacks, etc
                //bw.Write(pokemonCount);
                //bw.Write(attackCount);
            }
        }

        void ImportTrainer(string filename)
        {
            if (trainer == null)
                return;

            using (var fs = File.OpenRead(filename))
            using (var br = new BinaryReader(fs))
            {
                trainer.Name = br.ReadString();
                trainer.Class = br.ReadByte();
                trainer.Gender = br.ReadByte();
                trainer.Sprite = br.ReadByte();
                trainer.Music = br.ReadByte();
                for (int i = 0; i < 4; i++)
                    trainer.Items[i] = br.ReadUInt16();

                trainer.Party.Clear();
                var count = br.ReadInt32();

                for (int i = 0; i < count; i++)
                {
                    var p = new Pokemon(i);
                    p.Species = br.ReadUInt16();
                    p.Level = br.ReadUInt16();
                    p.EVs = br.ReadUInt16();
                    p.HeldItem = br.ReadUInt16();
                    for (int j = 0; j < 4; j++)
                        p.Attacks[j] = br.ReadUInt16();

                    trainer.Party.Add(p);
                }

                // TODO: check that values <= game max for safe importing
            }
        }
    }
}
