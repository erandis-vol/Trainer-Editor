using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GBAHL.Drawing;
using GBAHL.Text;

namespace Hopeless
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

            // seek trainer offset
            rom.Seek(romInfo.GetInt32("trainers", "Data", 16));
            rom.ReadPointerAndSeek();
            rom.Skip(index * 40);

            // read trainer data
            var flags = rom.ReadByte();
                trainer.HasCustomAttacks = (flags & 1) == 1;
                trainer.HasHeldItems = (flags & 2) == 2;
            trainer.Class = rom.ReadByte();
            var genderMusic = rom.ReadByte();
                trainer.Gender = (byte)((genderMusic & 128) >> 7);
                trainer.Music = (byte)(genderMusic & 127);
            trainer.Sprite = rom.ReadByte();
            trainer.Name = rom.ReadText(12, Table.Encoding.English);
            for (int i = 0; i < 4; i++)
                trainer.Items[i] = rom.ReadUInt16();
            trainer.DoubleBattle = rom.ReadByte() == 1;
            rom.Skip(3);
            trainer.AI = rom.ReadUInt32();
            var partyCount = rom.ReadByte();
            rom.Skip(3);
            trainer.PartyOffset = rom.ReadPointer();
            // todo: there's more padding ?

            // read party data
            LoadParty(partyCount);
        }

        void LoadParty(int partyCount)
        {
            if (partyCount <= 0 || trainer.PartyOffset <= 0)
                return;

            // --
            rom.Seek(trainer.PartyOffset);

            // read pokemon data
            trainer.Party.Clear();
            for (int i = 0; i < partyCount; i++) {
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

            trainer.OriginalPartySize = rom.Position - trainer.PartyOffset;
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
                    newOffset = rom.Find(0xFF, trainer.PartySize, 0x720000);
                }
                else
                {
                    using (var fs = new FreeSpaceDialog(rom, trainer.PartySize, lastSearch))
                    {
                        fs.Text = "Repoint Party";

                        if (fs.ShowDialog() == DialogResult.OK)
                        {
                            newOffset = fs.Offset;
                            lastSearch = fs.SearchStart;
                        }
                    }
                }

                if (newOffset == -1)
                {
                    MessageBox.Show("Unable to repoint party!\nIt was not saved.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }

                // overwrite old party with freespace
                if (cleanRepointedTrainersToolStripMenuItem.Checked)
                {
                    rom.Seek(trainer.PartyOffset);
                    for (int i = 0; i < trainer.OriginalPartySize; i++)
                        rom.WriteByte(0xFF);
                }

                // set new offset for party
                trainer.PartyOffset = newOffset;
            }

            //rom.Seek(romInfo.GetInt32("trainers", "Data", 16) + trainer.Index * 40);
            rom.Seek(romInfo.GetInt32("trainers", "Data", 16));
            rom.ReadPointerAndSeek();
            rom.Skip(trainer.Index * 40);

            // write trainer data
            rom.WriteByte((byte)((trainer.HasCustomAttacks ? 1 : 0) + (trainer.HasHeldItems ? 2 : 0)));
            rom.WriteByte(trainer.Class);
            rom.WriteByte((byte)((trainer.Gender << 7) + trainer.Music));
            rom.WriteByte(trainer.Sprite);
            rom.WriteText(trainer.Name, 12, Table.Encoding.English);
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

        void ResizeTrainers(int newCount)
        {
            // repoint the trainer table

            // original information
            var originalOffset = romInfo.GetInt32("trainers", "Data", 16);
            var originalSize = trainerCount * 40;

            var newOffset = -1;
            var newSize = newCount * 40;

            var buffer = new int[newCount];

            // copy old table for moving
            rom.Seek(originalOffset);
            for (int i = 0; i < Math.Min(trainerCount, newCount); i++)
                buffer[i] = rom.ReadPointer();

            // overwrite old table with free space
            rom.Seek(originalOffset);
            for (int i = 0; i < trainerCount; i++)
                rom.WriteUInt32(0xFFFFFFFFu);

            // find space for the new table
            if (newCount > trainerCount)
            {
                if (repointAutomaticallyToolStripMenuItem.Checked)
                {
                    newOffset = rom.Find(0xFF, newSize, 0x720000);
                }
                else
                {
                    using (var fs = new FreeSpaceDialog(rom, newSize, lastSearch))
                    {
                        fs.Text = "Repoint Trainer Table";

                        if (fs.ShowDialog() == DialogResult.OK) {
                            newOffset = fs.Offset;
                            lastSearch = fs.SearchStart;
                        }
                    }
                }
            }
            else
            {
                newOffset = originalOffset;
            }

            if (newOffset == -1)
            {
                MessageBox.Show("Unable to resize trainer table!\nAborted.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            // write new table
            rom.Seek(newOffset);
            for (int i = 0; i < newCount; i++)
                rom.WritePointer(buffer[i]);
            
            // TODO: trainers with an offset of 0 result in error, allow for 'blank' trainer
            //       find and replace all pointers to table
            //       update ini
        }

        void RandomizeTrainers()
        {
            // preserve current trainer
            var t = trainer;
            var m = member;
            var r = new Random();

            // for every trainer, randomize just the species
            // of every pokemon in their party
            for (int i = 0; i < trainerCount; i++)
            {
                LoadTrainer(i);

                foreach (var member in trainer.Party)
                {
                    var s = r.Next(0, pokemonCount);
                    while ((s > 251 && s < 277) || (s > 411 && s < 440)) // unsafe indexes
                        s = r.Next(0, pokemonCount);

                    member.Species = (ushort)s;
                }

                SaveTrainer();
            }

            trainer = t;
            member = m;
        }

        void LoadNames()
        {
            // Read offset of names
            var firstTrainerPtr = romInfo.GetInt32("trainers", "Data", 16);
            rom.Seek(firstTrainerPtr);
            var firstTrainer = rom.ReadPointer();

            // Read names
            names = new string[trainerCount];
            for (int i = 0; i < trainerCount; i++)
            {
                rom.Seek(firstTrainer + i * 40 + 4);
                names[i] = rom.ReadText(12, Table.Encoding.English);
            }
        }

        void LoadClasses()
        {
            var tablePtr = romInfo.GetInt32("trainer_classes", "Names", 16);
            rom.Seek(tablePtr);

            rom.ReadPointerAndSeek();
            classes = rom.ReadTextTable(13, classCount, Table.Encoding.English);
        }

        void SaveClasses()
        {
            var tablePtr = romInfo.GetInt32("trainer_classes", "Names", 16);
            rom.Seek(tablePtr);

            rom.ReadPointerAndSeek();
            rom.WriteTextTable(classes, 13, Table.Encoding.English);
        }

        Image LoadTrainerSprite(int id)
        {
            Sprite sprite;
            Palette palette;

            try
            {
                // ------------------------------
                // read compressed sprite
                rom.Seek(romInfo.GetInt32("trainer_sprites", "Data", 16));
                rom.ReadPointerAndSeek();
                rom.Skip(id * 8);
                rom.ReadPointerAndSeek();
                sprite = rom.ReadCompressedSprite4();

                // ------------------------------
                // read compressed palette
                rom.Seek(romInfo.GetInt32("trainer_sprites", "Palettes", 16));
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

                trainer.DoubleBattle = br.ReadBoolean();
                trainer.HasCustomAttacks = br.ReadBoolean();
                trainer.HasHeldItems = br.ReadBoolean();

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

        void DisplayTrainer()
        {
            ignore = true;
            if (trainer == null)
            {
                txtName.Text = string.Empty; ;
                rMale.Checked = true;
                rFemale.Checked = false;

                nSprite.Value = 0;

                cClass.SelectedIndex = 0;
                txtClassID.Value = 0;
                txtClass.Text = classes[0];

                cItem1.SelectedIndex = 0;
                cItem2.SelectedIndex = 0;
                cItem3.SelectedIndex = 0;
                cItem4.SelectedIndex = 0;

                txtMusic.Value = 0;
                txtAI.Value = 0;

                chkDoubleBattle.Checked = false;
                chkHeldItems.Checked = false;
                chkMovesets.Checked = false;

                listParty.Items.Clear();
                cHeld.Enabled = false;
                cAttack1.Enabled = false;
                cAttack2.Enabled = false;
                cAttack3.Enabled = false;
                cAttack4.Enabled = false;

                pSprite.Image = invisible;
                for (int i = 0; i < 6; i++)
                    partyPictureBoxes[i].Image = invisible;

                grpTrainer.Text = "Trainer";
                grpParty.Text = "Party";
            }
            else
            {
                txtName.Text = trainer.Name;
                rMale.Checked = trainer.Gender == 0;
                rFemale.Checked = trainer.Gender == 1;

                nSprite.Value = trainer.Sprite;

                cClass.SelectedIndex = trainer.Class;
                txtClassID.Value = trainer.Class;
                txtClass.Text = classes[trainer.Class];

                cItem1.SelectedIndex = trainer.Items[0];
                cItem2.SelectedIndex = trainer.Items[1];
                cItem3.SelectedIndex = trainer.Items[2];
                cItem4.SelectedIndex = trainer.Items[3];

                txtMusic.Value = trainer.Music;
                txtAI.Value = (int)(trainer.AI & 0x1FF); // AI is the first 9 bits

                chkDoubleBattle.Checked = trainer.DoubleBattle;
                chkHeldItems.Checked = trainer.HasHeldItems;
                chkMovesets.Checked = trainer.HasCustomAttacks;

                listParty.Items.Clear();
                foreach (var pk in trainer.Party)
                {
                    var i = new ListViewItem(pokemon[pk.Species]);
                    i.SubItems.Add($"{pk.Level}");

                    listParty.Items.Add(i);
                }

                cHeld.Enabled = trainer.HasHeldItems;

                cAttack1.Enabled = trainer.HasCustomAttacks;
                cAttack2.Enabled = trainer.HasCustomAttacks;
                cAttack3.Enabled = trainer.HasCustomAttacks;
                cAttack4.Enabled = trainer.HasCustomAttacks;

                grpTrainer.Text = $"Trainer (0x{trainer.Index:X3})";
                grpParty.Text = $"Party (0x{trainer.PartyOffset:X7})";
                if (trainer.RequiresRepoint) grpParty.Text += "*";
            }
            ignore = false;
        }

        void DisplayPartyMember()
        {
            ignore = true;

            if (member == null)
            {
                txtSpecies.Value = 0;
                txtLevel.Value = 0;
                txtEVs.Value = 0;
                cSpecies.SelectedIndex = 0;
                cHeld.SelectedIndex = 0;
                cAttack1.SelectedIndex = 0;
                cAttack2.SelectedIndex = 0;
                cAttack3.SelectedIndex = 0;
                cAttack4.SelectedIndex = 0;
            }
            else
            {
                txtSpecies.Value = member.Species;
                cSpecies.SelectedIndex = member.Species;
                txtLevel.Value = member.Level;
                txtEVs.Value = member.EVs;

                if (trainer.HasHeldItems)
                    cHeld.SelectedIndex = member.HeldItem;

                if (trainer.HasCustomAttacks)
                {
                    cAttack1.SelectedIndex = member.Attacks[0];
                    cAttack2.SelectedIndex = member.Attacks[1];
                    cAttack3.SelectedIndex = member.Attacks[2];
                    cAttack4.SelectedIndex = member.Attacks[3];
                }
            }

            ignore = false;
        }
    }
}
