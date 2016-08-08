using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using HTE.GBA;

namespace HTE
{
    public partial class MainForm : Form
    {
        // TODO: move these two to ROM class
        FileSystemWatcher watcher;
        string romFile = string.Empty;

        ROM rom;
        Settings roms;

        Trainer[] trainers;
        
        int selectedTrainer = -1, selectedParty = -1;
        string[] classNames;
        string[] pokemonNames;
        Bitmap blankSprite = new Bitmap(64, 64);

        bool qxy = false;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                roms = Settings.FromFile("ROMs.ini", "ini");
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error with ROMs.ini:\n" + ex.Message, "Uh-oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }

            groupBox2.Enabled = false;
            groupBox8.Enabled = false;
            exportToolStripMenuItem.Enabled = false;
            importToolStripMenuItem.Enabled = false;
            massExportToolStripMenuItem.Enabled = false;
            simpleRandomizeToolStripMenuItem.Enabled = false;

            saveToolStripMenuItem.Enabled = false;
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            blankSprite.Dispose();
        }

        #region Menu

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Open ROM";
            openFileDialog1.Filter = "GBA ROMs|*.gba";
            openFileDialog1.FileName = "";

            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;

            // do the initial loading
            qxy = true;

            // load a new ROM file
            var temp = new ROM(openFileDialog1.FileName);
            var code = temp.Code;

            // check for valid ROM types
            //if (!ini.GetSections().Contains(temp.Code))
            if (!roms.ContainsSection(code))
            {
                MessageBox.Show($"{code} is not a recognized ROM!", "Uh-oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Disable form
                if (romFile != string.Empty)
                {
                    goto fail;
                }
                return;
            }

            // valid ROM loaded, begin working with this one
            romFile = openFileDialog1.FileName;
            watcher = new FileSystemWatcher(Path.GetDirectoryName(romFile), $"*.{Path.GetExtension(romFile)}");
            rom = temp;

            // next, load file data

            // 1. load all Trainer data
            rom.Seek(roms.GetInt32(code, "TrainerData"));   // read pointer to trainer data
            int firstTrainer = rom.ReadPointer();

            int trainerCount = roms.GetInt32(code, "NumberOfTrainers"); // number of trainers from ini
            trainers = new Trainer[trainerCount];

            for (int i = 0; i < trainerCount; i++)
                trainers[i] = rom.ReadTrainer();

            // set a few properties for trainers
            txtFindID.MaxValue = (uint)trainerCount - 1;
            groupBox8.Enabled = true;

            listTrainers.Items.Clear(); // populate listview with trainer names
            for (int i = 0; i < trainerCount; i++)
            {
                ListViewItem item = new ListViewItem(i.ToString("X3"));
                item.SubItems.Add(trainers[i].Name);
                item.Tag = i;
                listTrainers.Items.Add(item);
            }

            // 2 . load trainer classses
            int classCount = roms.GetInt32(code, "NumberOfClassNames");
            rom.Seek(roms.GetInt32(code, "ClassNames"));
            rom.Seek(rom.ReadPointer());
            classNames = rom.ReadTextTable(13, classCount);

            cClass.Items.Clear(); cClass2.Items.Clear();
            for (int i = 0; i < classCount; i++)
            {
                cClass.Items.Add(i.ToString("X2"));
                cClass2.Items.Add(classNames[i]);
            }

            // this:
            nSprite.Maximum = roms.GetInt32(code, "NumberOfTrainerSprites");

            // 3. load item names
            string[] items = LoadItemNames(br);
            cItem1.Items.Clear();
            cItem2.Items.Clear();
            cItem3.Items.Clear();
            cItem4.Items.Clear();
            cHeld.Items.Clear();
            cItem1.Items.AddRange(items);
            cItem2.Items.AddRange(items);
            cItem3.Items.AddRange(items);
            cItem4.Items.AddRange(items);
            cHeld.Items.AddRange(items);

            string[] attacks = LoadAttackNames(br);
            cAtk1.Items.Clear();
            cAtk2.Items.Clear();
            cAtk3.Items.Clear();
            cAtk4.Items.Clear();
            cAtk1.Items.AddRange(attacks);
            cAtk2.Items.AddRange(attacks);
            cAtk3.Items.AddRange(attacks);
            cAtk4.Items.AddRange(attacks);

            pokemonNames = LoadPokemonNames(br);
            cSpecies.Items.Clear(); cSpeciesN.Items.Clear();
            //cSpecies.Items.AddRange(pokemonNames);
            for (int i = 0; i < pokemonNames.Length; i++)
            {
                cSpeciesN.Items.Add(i.ToString("X2"));
                cSpecies.Items.Add(pokemonNames[i]);
            }


            massExportToolStripMenuItem.Enabled = true;
            simpleRandomizeToolStripMenuItem.Enabled = true;
            qxy = false;

            return;

            // call at any point a failed loading happens
            fail:
            listTrainers.Items.Clear();
            lblROM.Text = "";
            groupBox2.Enabled = false;
            groupBox8.Enabled = false;
            exportToolStripMenuItem.Enabled = false;
            importToolStripMenuItem.Enabled = false;
            massExportToolStripMenuItem.Enabled = false;
            simpleRandomizeToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedTrainer > -1)
            {
                SaveTrainer(trainer, selectedTrainer);

                // Update name & party offset
                qxy = true;
                listTrainers.Items[selectedTrainer].SubItems[1].Text = trainer.Name;
                trainerNames[selectedTrainer] = trainer.Name;
                groupBox6.Text = "Pokémon @ 0x" + trainer.PartyOffset.ToString("X");
                qxy = false;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedTrainer > -1)
            {
                openFileDialog1.Title = "Import Trainer 0x" + selectedTrainer.ToString("X");
                openFileDialog1.Filter = "Trainer Files|*.trainer";
                openFileDialog1.InitialDirectory = Path.GetDirectoryName(rom.File);
                openFileDialog1.FileName = "";

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // this should correct importing issues..
                        uint partyOffset = trainer.PartyOffset;

                        trainer = ImportTrainer(openFileDialog1.FileName);
                        trainer.PartyOffset = partyOffset;

                        FillDisplay();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedTrainer > -1)
            {
                // dialog setup
                saveFileDialog1.Title = "Export Trainer 0x" + selectedTrainer.ToString("X");
                saveFileDialog1.Filter = "Trainer Files|*.trainer";
                saveFileDialog1.InitialDirectory = Path.GetDirectoryName(rom.File);
                if (trainer.Name.Length > 0)
                    saveFileDialog1.FileName = trainer.Name + ".trainer";
                else
                    saveFileDialog1.FileName = selectedTrainer.ToString("X3") + ".trainer";

                // show dialog, then export
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    ExportTrainer(saveFileDialog1.FileName, trainer);
                }
            }
        }

        private void massExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            Application.DoEvents();

            MassExportDialog med = new MassExportDialog(listTrainers.Items);
            if (med.ShowDialog() == DialogResult.OK)
            {
                int[] ids = med.Result;
                if (ids.Length == 0) return;

                folderBrowserDialog1.ShowNewFolderButton = true;
                folderBrowserDialog1.SelectedPath = Path.GetDirectoryName(rom.File);
                if (folderBrowserDialog1.ShowDialog() != DialogResult.OK) return;

                using (GBABinaryReader br = new GBABinaryReader(File.OpenRead(rom.File)))
                {
                    foreach (int id in ids)
                    {
                        Trainer tr = LoadTrainer(id, br);
                        ExportTrainer(folderBrowserDialog1.SelectedPath + "\\" + id.ToString("000") + " - " + tr.Name + ".trainer", tr);
                    }
                }

                MessageBox.Show("Mass export was a success!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            this.Cursor = Cursors.Default;
        }

        private void simpleRandomizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will randomize all the trainers in the game, without any regard for what the Pokémon actually are (apart from preventing the illegal ones between Celebi and Treecko).\n\nDo you actually wanna do this?", "Simple Randomize?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            Random rndm = new Random();
            int trainerCount = ini.GetInt32(rom.Code, "NumberOfTrainers");
            int pkmnCount = ini.GetInt32(rom.Code, "NumberOfPokemon");

            for (int tr = 0; tr < trainerCount; tr++)
            {
                // Load the trainer
                Trainer trainer;
                using (GBABinaryReader br = new GBABinaryReader(rom))
                {
                    trainer = LoadTrainer(tr, br);
                }

                // Randomize the Pokémon
                // Preserve the original levels and stuff
                // JUST RANDOMIZE THE SPECIES
                for (int pkmn = 0; pkmn < trainer.Party.Count; pkmn++)
                {
                    // Get random, legal species
                    int species = rndm.Next(1, pkmnCount);
                    while (species > 251 && species < 277)
                    {
                        species = rndm.Next(pkmnCount);
                    }

                    // Set it and go
                    trainer.Party[pkmn].Species = (ushort)species;
                }

                // And save the trainer
                // There shouldn't be any repointing needed
                // TODO: quick save function
                SaveTrainer(trainer, tr);
            }

            MessageBox.Show("All trainers randomized!");
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // show about form
            using (AboutDialog ad = new AboutDialog())
            {
                ad.ShowDialog();
            }
            // it will auto-destroy
        }

        #endregion

        #region Loading

        /*
        private Trainer LoadTrainer(int id, GBABinaryReader br)
        {
            // setup
            Trainer trainer = new Trainer(); trainer.Party.Clear(); // ready~
            br.BaseStream.Seek(Convert.ToUInt32(ini[rom.Code, "TrainerData"], 16), SeekOrigin.Begin);
            uint offset = br.ReadPointer();
            //this.Text = "itari's Trainer Editor -- " + id.ToString("000") + " @ 0x" + (offset + id * 40).ToString("X");
            groupBox2.Text = "Trainer 0x" + id.ToString("X") + " @ 0x" + (offset + id * 40).ToString("X");
            groupBox6.Text = "Pokémon";

            // load trainer
            br.BaseStream.Seek(offset + id * 40, SeekOrigin.Begin);
            byte flags = br.ReadByte();
            trainer.HasCustomAttacks = ((flags & 1) == 1);
            trainer.HasHeldItems = ((flags & 2) == 2);
            trainer.Class = br.ReadByte();
            byte genderMusic = br.ReadByte();
            trainer.Gender = (byte)((genderMusic & 128) >> 7);
            trainer.Music = (byte)(genderMusic & 127);
            trainer.Sprite = br.ReadByte();
            trainer.Name = TextTable.GetEnglishString(br.ReadBytes(12));
            for (int i = 0; i < 4; i++) // If HasHeldItems == false, should all be 0
                trainer.HeldItems[i] = br.ReadUInt16();
            trainer.DoubleBattle = (br.ReadByte() == 1);
            br.BaseStream.Seek(3L, SeekOrigin.Current);
            trainer.AI = br.ReadUInt32();
            //br.BaseStream.Seek(3L, SeekOrigin.Current);
            int partyCount = br.ReadByte();
            if (partyCount == 0) return trainer;
            br.BaseStream.Seek(3L, SeekOrigin.Current);
            trainer.PartyOffset = br.ReadUInt32();
            if (trainer.PartyOffset == 0) return trainer;
            else trainer.PartyOffset -= 0x08000000;
            groupBox6.Text += " @ 0x" + trainer.PartyOffset.ToString("X");

            // party
            br.BaseStream.Seek(trainer.PartyOffset, SeekOrigin.Begin);
            for (int p = 0; p < partyCount; p++)
            {
                Pokemon pkmn = new Pokemon();
                pkmn.IVs = br.ReadUInt16();
                pkmn.Level = br.ReadUInt16();
                pkmn.Species = br.ReadUInt16();
                if (trainer.HasHeldItems)
                {
                    pkmn.HeldItem = br.ReadUInt16(); // there regardless of flags
                }

                if (trainer.HasCustomAttacks) // ;)
                {
                    for (int a = 0; a < 4; a++) pkmn.Attacks[a] = br.ReadUInt16();
                }

                if (!trainer.HasHeldItems) br.ReadUInt16();

                trainer.Party.Add(pkmn);
            }

            // done
            return trainer;
        }*/

        private string[] LoadAttackNames(GBABinaryReader br)
        {
            string[] names = new string[Convert.ToInt32(ini[rom.Code, "NumberOfAttacks"])];
            br.BaseStream.Seek(Convert.ToUInt32(ini[rom.Code, "AttackNames"], 16), SeekOrigin.Begin);
            uint offset = br.ReadPointer();

            //var charSet = (ini[rom.Code, "CharSet"] == "E" ? TextTable.GBACharSet.English : TextTable.GBACharSet.Japanese);

            br.BaseStream.Seek(offset, SeekOrigin.Begin);
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = TextTable.GetEnglishString(br.ReadBytes(13));
            }
            return names;
        }

        private string[] LoadPokemonNames(GBABinaryReader br)
        {
            string[] names = new string[Convert.ToInt32(ini[rom.Code, "NumberOfPokemon"])];

            br.BaseStream.Seek(Convert.ToUInt32(ini[rom.Code, "PokemonNames"], 16), SeekOrigin.Begin);
            uint offset = br.ReadPointer();

            //var charSet = (ini[rom.Code, "CharSet"] == "E" ? TextTable.GBACharSet.English : TextTable.GBACharSet.Japanese);

            br.BaseStream.Seek(offset, SeekOrigin.Begin);
            for (int i = 0; i < names.Length; i++)
            {
                names[i] = TextTable.GetEnglishString(br.ReadBytes(11));
            }
            return names;
        }

        private string[] LoadItemNames(GBABinaryReader br)
        {
            string[] names = new string[Convert.ToInt32(ini[rom.Code, "NumberOfItems"])];
            br.BaseStream.Seek(Convert.ToUInt32(ini[rom.Code, "ItemData"], 16), SeekOrigin.Begin);
            uint offset = br.ReadPointer();

            //var charSet = (ini[rom.Code, "CharSet"] == "E" ? TextTable.GBACharSet.English : TextTable.GBACharSet.Japanese);

            // read
            for (int i = 0; i < names.Length; i++)
            {
                br.BaseStream.Seek(offset + i * 44, SeekOrigin.Begin);
                names[i] = TextTable.GetEnglishString(br.ReadBytes(14));
                // there is other crap we disregard
            }
            return names;
        }

        private Bitmap LoadTrainerSprite(int id)
        {
            try
            {
                byte[] sprite; Color[] palette;
                using (GBABinaryReader br = new GBABinaryReader(File.OpenRead(rom.File)))
                {
                    // sprite
                    br.BaseStream.Seek(Convert.ToUInt32(ini[rom.Code, "TrainerSprites"], 16), SeekOrigin.Begin);
                    uint offset = br.ReadPointer();

                    br.BaseStream.Seek(offset + id * 8, SeekOrigin.Begin);
                    uint spriteOffset = br.ReadPointer();

                    br.BaseStream.Seek(spriteOffset, SeekOrigin.Begin);
                    sprite = br.ReadLZ77Bytes();

                    // palette
                    br.BaseStream.Seek(Convert.ToUInt32(ini[rom.Code, "TrainerPalettes"], 16), SeekOrigin.Begin);
                    offset = br.ReadPointer();

                    br.BaseStream.Seek(offset + id * 8, SeekOrigin.Begin);
                    uint paletteOffset = br.ReadPointer();

                    br.BaseStream.Seek(paletteOffset, SeekOrigin.Begin);
                    try
                    {
                        palette = br.ReadLZ77Palette();
                    }
                    catch (Exception ex)
                    {
                        br.BaseStream.Seek(paletteOffset, SeekOrigin.Begin);
                        palette = br.ReadPalette();
                    }
                    //byte[] pal = br.ReadLZ77Bytes();
                    //byte[] pal = br.ReadBytes(32);
                }

                int width = 8;
                int height = sprite.Length / (width * 32);
                return Sprites.DrawSprite16(sprite, width, height, palette, false);
            }
            catch (Exception ex)
            {
                return Properties.Resources.invalid_sprite;
            }
            //return new Bitmap(64, 64);
        }

        private Bitmap LoadPokemonSprite(int id)
        {
            try
            {
                byte[] sprite; Color[] palette;
                using (GBABinaryReader br = new GBABinaryReader(File.OpenRead(rom.File)))
                {
                    // sprite
                    br.BaseStream.Seek(Convert.ToUInt32(ini[rom.Code, "PokemonFrontSprites"], 16), SeekOrigin.Begin);
                    uint offset = br.ReadPointer();

                    br.BaseStream.Seek(offset + id * 8, SeekOrigin.Begin);
                    uint spriteOffset = br.ReadPointer();

                    br.BaseStream.Seek(spriteOffset, SeekOrigin.Begin);
                    sprite = br.ReadLZ77Bytes();

                    // palette
                    br.BaseStream.Seek(Convert.ToUInt32(ini[rom.Code, "PokemonNormalPalettes"], 16), SeekOrigin.Begin);
                    offset = br.ReadPointer();

                    br.BaseStream.Seek(offset + id * 8, SeekOrigin.Begin);
                    uint paletteOffset = br.ReadPointer();

                    try
                    {
                        br.BaseStream.Seek(paletteOffset, SeekOrigin.Begin);
                        palette = br.ReadLZ77Palette();
                    }
                    catch (Exception ex)
                    {
                        br.BaseStream.Seek(paletteOffset, SeekOrigin.Begin);
                        palette = br.ReadPalette();
                    }
                    //byte[] pal = br.ReadLZ77Bytes();
                    //byte[] pal = br.ReadBytes(32);
                }

                int width = 8;
                int height = sprite.Length / (width * 32);
                return Sprites.DrawSprite16(sprite, width, height, palette, false);
            }
            catch (Exception ex)
            {
                return Properties.Resources.invalid_sprite;
            }
        }

        private int LoadClassMoneyRate(int id)
        {
            using (GBABinaryReader br = new GBABinaryReader(rom))
            {
                br.BaseStream.Seek(Convert.ToUInt32(ini[rom.Code, "ClassMoneyData"], 16), SeekOrigin.Begin);
                uint offset = br.ReadPointer();

                br.BaseStream.Seek(offset, SeekOrigin.Begin);
                while (true)
                {
                    byte b = br.ReadByte();
                    if (b == 0xFF) break;
                    else if (b == id)
                    {
                        return br.ReadByte();
                    }
                    else
                    {
                        br.BaseStream.Seek(3L, SeekOrigin.Current);
                    }

                    // TODO: don't let this crash with bad data
                }
                return 0;
            }
        }

        #endregion

        #region Saving

        public void SaveClassNames()
        {
            // language
            //var charSet = (ini[rom.Code, "CharSet"] == "E" ? TextTable.GBACharSet.English : TextTable.GBACharSet.Japanese);

            // read location
            uint offset;
            using (GBABinaryReader br = new GBABinaryReader(File.OpenRead(rom.File)))
            {
                br.BaseStream.Seek(Convert.ToUInt32(ini[rom.Code, "ClassNames"], 16), SeekOrigin.Begin);
                offset = br.ReadPointer();
            }

            // then goto @ write
            using (GBABinaryWriter bw = new GBABinaryWriter(File.OpenWrite(rom.File)))
            {
                bw.BaseStream.Seek(offset, SeekOrigin.Begin);
                for (int i = 0; i < classNames.Length; i++)
                {
                    // get bytes
                    byte[] name = TextTable.GetBytes(classNames[i], CharacterEncoding.English);

                    // add terminator (if length == 12)
                    Array.Resize(ref name, 13);
                    name[12] = 0xFF;

                    // write
                    bw.Write(name);
                }
            }
        }

        public void SaveTrainer(Trainer trainer, int id)
        {
            // read relevant crap
            uint trainerOffset; int originalPkmnSize;
            using (GBABinaryReader br = new GBABinaryReader(File.OpenRead(rom.File)))
            {
                // read trainer offset
                br.BaseStream.Seek(Convert.ToUInt32(ini[rom.Code, "TrainerData"], 16), SeekOrigin.Begin);
                trainerOffset = br.ReadPointer();

                // calc size
                br.BaseStream.Seek(trainerOffset + id * 40, SeekOrigin.Begin);
                byte flags = br.ReadByte();
                br.BaseStream.Seek(0x1FL, SeekOrigin.Current);
                byte count = br.ReadByte();
                originalPkmnSize = (count * ((flags & 1) == 1 ? 16 : 8));
            }

            // check size & repoint if necessary
            int neededPkmnSize = trainer.Party.Count * (trainer.HasCustomAttacks ? 16 : 8);
            if (neededPkmnSize > originalPkmnSize)
            {
                // get free space
                FreeSpaceFinderDialog fsf = new FreeSpaceFinderDialog(rom.File, (uint)neededPkmnSize);
                fsf.Text = "Repoint Party Data";

                if (fsf.ShowDialog() != DialogResult.OK) return;

                // show
                //MessageBox.Show("Repointing to...\n0x" + fsf.FreeSpaceOffset.ToString("X"));

                // overwrite with FF
                using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(rom.File)))
                {
                    bw.BaseStream.Seek(trainer.PartyOffset, SeekOrigin.Begin);
                    for (int i = 0; i < originalPkmnSize; i++) bw.Write(byte.MaxValue);
                }

                // now, hold onto it
                trainer.PartyOffset = fsf.FreeSpaceOffset;
            }

            // write data to rom
            using (GBABinaryWriter bw = new GBABinaryWriter(File.OpenWrite(rom.File)))
            {
                bw.BaseStream.Seek(trainerOffset + id * 40, SeekOrigin.Begin);
                //byte flags = (byte)((trainer.HasCustomAttacks ? 1 : 0) + (trainer.HasHeldItems ? 2 : 0));
                bw.Write((byte)((trainer.HasCustomAttacks ? 1 : 0) + (trainer.HasHeldItems ? 2 : 0)));
                bw.Write(trainer.Class);
                bw.Write((byte)((trainer.Gender * 0x80) + trainer.Music));
                bw.Write(trainer.Sprite);

                byte[] name = TextTable.GetBytes(trainer.Name, CharacterEncoding.English);
                Array.Resize(ref name, 12);
                name[11] = 0xFF;
                bw.Write(name);

                for (int i = 0; i < 4; i++) bw.Write(trainer.HeldItems[i]);

                bw.Write((byte)(trainer.DoubleBattle ? 1 : 0));
                bw.BaseStream.Seek(3, SeekOrigin.Current);
                bw.Write(trainer.AI);
                //bw.BaseStream.Seek(3, SeekOrigin.Current);
                bw.Write((byte)trainer.Party.Count);
                bw.BaseStream.Seek(3, SeekOrigin.Current);
                if (trainer.Party.Count <= 0) return;

                bw.WritePointer(trainer.PartyOffset); // pointer~
                if (trainer.PartyOffset == 0) return;

                // pokemon
                bw.BaseStream.Seek(trainer.PartyOffset, SeekOrigin.Begin);
                for (int i = 0; i < trainer.Party.Count; i++)
                {
                    Pokemon pkmn = trainer.Party[i];
                    bw.Write(pkmn.IVs);
                    bw.Write(pkmn.Level);
                    bw.Write(pkmn.Species);
                    if (trainer.HasHeldItems) bw.Write(pkmn.HeldItem);
                    //else bw.Write((ushort)0);
                    if (trainer.HasCustomAttacks)
                    {
                        for (int k = 0; k < 4; k++) bw.Write(pkmn.Attacks[k]);
                    }

                    if (!trainer.HasHeldItems) bw.Write((ushort)0);
                }
            }
        }

        public void SaveClassMoneyRate(int id, byte value)
        {
            long offset = 0;
            using (GBABinaryReader br = new GBABinaryReader(rom))
            {
                br.BaseStream.Seek(Convert.ToUInt32(ini[rom.Code, "ClassMoneyData"], 16), SeekOrigin.Begin);
                uint table = br.ReadPointer();

                br.BaseStream.Seek(table, SeekOrigin.Begin);
                while (true)
                {
                    byte b = br.ReadByte();
                    if (b == 0xFF) break;
                    else if (b == id)
                    {
                        offset = br.BaseStream.Position;
                        break;
                    }
                    else
                    {
                        br.BaseStream.Seek(3L, SeekOrigin.Current);
                    }
                }
            }

            // not found
            if (offset == 0) return;

            using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(rom.File)))
            {
                bw.BaseStream.Seek(offset, SeekOrigin.Begin);
                bw.Write(value);
            }
        }

        #endregion

        #region Import/Export

        private void ExportTrainer(string writeTo, Trainer trainer)
        {
            // lets go~
            using (BinaryWriter bw = new BinaryWriter(File.Create(writeTo)))
            {
                // Store trainer data
                bw.Write(0x524E5254u); // TRNR
                bw.Write(trainer.Name);
                bw.Write(trainer.Class);
                bw.Write(trainer.Gender);
                bw.Write(trainer.Sprite);
                bw.Write(trainer.Music);

                for (int i = 0; i < 4; i++) bw.Write(trainer.HeldItems[i]);

                bw.Write(trainer.AI);

                bw.Write(trainer.HasHeldItems);
                bw.Write(trainer.HasCustomAttacks);
                bw.Write(trainer.DoubleBattle);

                // Store party data
                bw.Write(0x4E4D4B50u); // PKMN
                bw.Write((byte)trainer.Party.Count);

                foreach (Pokemon pkmn in trainer.Party)
                {
                    bw.Write(pkmn.Species);
                    bw.Write(pkmn.Level);
                    bw.Write(pkmn.IVs);

                    if (trainer.HasHeldItems)
                    {
                        bw.Write(pkmn.HeldItem);
                    }

                    if (trainer.HasCustomAttacks)
                    {
                        for (int a = 0; a < 4; a++) bw.Write(pkmn.Attacks[a]);
                    }
                }

                // Stare game data
                bw.Write(0x454D4147u); // GAME
                bw.Write(ini.GetUInt32(rom.Code, "NumberOfPokemon"));
                bw.Write(ini.GetUInt32(rom.Code, "NumberOfAttacks"));
                bw.Write(ini.GetUInt32(rom.Code, "NumberOfItems"));
            }
        }

        private Trainer ImportTrainer(string loadFrom)
        {
            Trainer tr = new Trainer();
            using (BinaryReader br = new BinaryReader(File.OpenRead(loadFrom)))
            {
                // Load trainer data
                if (br.ReadUInt32() != 0x524E5254u) throw new Exception("This isn't a trainer file!");
                tr.Name = br.ReadString();
                tr.Class = br.ReadByte();
                tr.Gender = br.ReadByte();
                tr.Sprite = br.ReadByte();
                tr.Music = br.ReadByte();

                for (int i = 0; i < 4; i++) tr.HeldItems[i] = br.ReadUInt16();

                tr.AI = br.ReadUInt32();

                tr.HasHeldItems = br.ReadBoolean();
                tr.HasCustomAttacks = br.ReadBoolean();
                tr.DoubleBattle = br.ReadBoolean();

                // Load party data
                if (br.ReadUInt32() != 0x4E4D4B50u) throw new Exception("Corrupted Pokémon data!");
                byte count = br.ReadByte();

                tr.Party.Clear();
                for (int i = 0; i < count; i++)
                {
                    var pkmn = new Pokemon();
                    pkmn.Species = br.ReadUInt16();
                    pkmn.Level = br.ReadUInt16();
                    pkmn.IVs = br.ReadUInt16();

                    if (tr.HasHeldItems)
                    {
                        pkmn.HeldItem = br.ReadUInt16();
                    }

                    if (tr.HasCustomAttacks)
                    {
                        for (int a = 0; a < 4; a++) pkmn.Attacks[a] = br.ReadUInt16();
                    }

                    tr.Party.Add(pkmn);
                }

                // Verify game data
                if (br.ReadUInt32() != 0x454D4147u) throw new Exception("Corrupted game data!");

                if (br.ReadUInt32() != ini.GetUInt32(rom.Code, "NumberOfPokemon"))
                    throw new Exception("This trainer is from a game with a differing number of Pokémon!");
                if (br.ReadUInt32() != ini.GetUInt32(rom.Code, "NumberOfAttacks"))
                    throw new Exception("This trainer is from a game with a differing number of attacks!");
                if (br.ReadUInt32() != ini.GetUInt32(rom.Code, "NumberOfItems"))
                    throw new Exception("This trainer is from a game with a differing number of items!");
            }
            return tr;
        }

        #endregion

        private void FillDisplay()
        {
            qxy = true;

            // trainer
            txtName.Text = trainer.Name;
            rMale.Checked = trainer.Gender == 0;
            rFemale.Checked = trainer.Gender == 1;

            cClass.SelectedIndex = trainer.Class;
            cClass2.SelectedIndex = trainer.Class;
            txtClass.Text = classNames[trainer.Class];

            txtMoneyRate.Value = (uint)LoadClassMoneyRate(trainer.Class);
            FillPrizeMoney();

            nSprite.Value = trainer.Sprite;
            pSprite.Image = LoadTrainerSprite(trainer.Sprite);
            pSprite.Invalidate();

            txtMusic.Value = trainer.Music;
            txtAI.Value = trainer.AI;

            cItem1.SelectedIndex = trainer.HeldItems[0];
            cItem2.SelectedIndex = trainer.HeldItems[1];
            cItem3.SelectedIndex = trainer.HeldItems[2];
            cItem4.SelectedIndex = trainer.HeldItems[3];

            chkDouble.Checked = trainer.DoubleBattle;

            // party
            chkHeld.Checked = trainer.HasHeldItems;
            chkAttacks.Checked = trainer.HasCustomAttacks;
            //cHeld.Enabled = trainer.HasHeldItems;
            //groupBox7.Enabled = trainer.HasCustomAttacks;

            listParty.Items.Clear();
            for (int i = 0; i < trainer.Party.Count; i++)
            {
                ListViewItem item = new ListViewItem(pokemonNames[trainer.Party[i].Species]);
                item.SubItems.Add(trainer.Party[i].Level.ToString());
                listParty.Items.Add(item);
            }

            cSpecies.Enabled = false;
            cSpeciesN.Enabled = false;
            txtLevel.Enabled = false;
            txtEVs.Enabled = false;
            cHeld.Enabled = false;
            groupBox7.Enabled = false;
            pSpecies.Image = blankSprite;

            qxy = false;
        }

        private void FillPrizeMoney()
        {
            // TODO: this is clearly not it :')
            if (selectedTrainer > -1 && trainer.Party.Count > 0)
            {
                int rate = 4 * trainer.Party[trainer.Party.Count - 1].Level;
                txtMoney.Text = "$" + (rate * (int)txtMoneyRate.Value);
                //txtMoney.Text = "$" + ((trainer.Party.Count * (trainer.HasCustomAttacks ? 16 : 8) + (trainer.Party[trainer.Party.Count - 1].Level * (trainer.DoubleBattle ? 2 : 1))) * (int)nMoneyRate.Value);
            }
            else
            {
                txtMoney.Text = "$0";
            }
        }

        #region Controls

        private void listTrainers_SelectedIndexChanged(object sender, EventArgs e)
        {
            // get list selection
            int index = -1;
            foreach (int i in listTrainers.SelectedIndices) index = i;
            if (index < 0) return;

            // figure out which trainer it is
            selectedTrainer = (int)listTrainers.Items[index].Tag;

            try
            {
                // load it~
                using (GBABinaryReader br = new GBABinaryReader(File.OpenRead(rom.File)))
                {
                    trainer = LoadTrainer(selectedTrainer, br);
                }

                // display
                FillDisplay();
                groupBox2.Enabled = true;
                saveToolStripMenuItem.Enabled = true;
                importToolStripMenuItem.Enabled = true;
                exportToolStripMenuItem.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Uh-oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            if (qxy) return;

            if (selectedTrainer == -1) return;

            // try to set the text
            try
            {
                txtName.BackColor = SystemColors.Window;

                string actual = TextTable.GetEnglishString(TextTable.GetEnglishBytes(txtName.Text));
                if (actual.Length > 0)
                    trainer.Name = actual;
            }
            catch (Exception ex)
            {
                txtName.BackColor = Color.PaleVioletRed;
            }

        }

        private void txtName_KeyPress(object sender, KeyPressEventArgs e)
        {
            // check text length
            // stop names longer than 11 characters
            /*if (TextTable.GetEnglishBytes(txtName.Text).Length >= 11)
            {
                if (e.KeyChar != '\b')
                {
                    e.Handled = true;
                    System.Media.SystemSounds.Beep.Play();
                }
            }*/
        }

        private void nSprite_ValueChanged(object sender, EventArgs e)
        {
            if (qxy) return;

            if (selectedTrainer > -1)
            {
                // set
                trainer.Sprite = (byte)nSprite.Value;

                // draw
                pSprite.Image = LoadTrainerSprite(trainer.Sprite);
                pSprite.Invalidate();
            }
        }

        private void rMale_CheckedChanged(object sender, EventArgs e)
        {
            if (qxy) return;

            if (selectedTrainer > -1 && rMale.Checked)
            {
                trainer.Gender = 0;
            }
        }

        private void rFemale_CheckedChanged(object sender, EventArgs e)
        {
            if (qxy) return;

            if (selectedTrainer > -1 && rFemale.Checked)
            {
                trainer.Gender = 1;
            }
        }

        private void cClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (qxy) return;

            if (selectedTrainer > -1)
            {
                trainer.Class = (byte)cClass.SelectedIndex;
            }

            qxy = true;
            cClass2.SelectedIndex = cClass.SelectedIndex;
            txtClass.Text = classNames[cClass.SelectedIndex];

            txtMoneyRate.Value = (uint)LoadClassMoneyRate(trainer.Class);
            FillPrizeMoney();
            qxy = false;
        }

        private void cClass2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (qxy) return;

            if (selectedTrainer > -1)
            {
                trainer.Class = (byte)cClass2.SelectedIndex;
            }

            qxy = true;
            cClass.SelectedIndex = cClass2.SelectedIndex;
            txtClass.Text = classNames[cClass2.SelectedIndex];

            txtMoneyRate.Value = (uint)LoadClassMoneyRate(trainer.Class);
            FillPrizeMoney();
            qxy = false;
        }

        private void bChangeClass_Click(object sender, EventArgs e)
        {
            qxy = true;
            if (txtClass.TextLength == 0)
                return;

            try
            {
                // get text as actual string
                // takes care of pesky things like raw values
                string actual = TextTable.GetEnglishString(TextTable.GetEnglishBytes(txtClass.Text));

                // set data
                int id = cClass2.SelectedIndex;
                classNames[id] = actual;

                // update ui
                cClass2.Items[id] = actual;

                // save in rom
                SaveClassNames();

                txtClass.BackColor = SystemColors.Window;
            }
            catch
            {
                txtClass.BackColor = Color.PaleVioletRed;
            }
            qxy = false;
        }

        private void txtClass_KeyPress(object sender, KeyPressEventArgs e)
        {
            /*if (TextTable.GetEnglishBytes(txtClass.Text).Length >= 12)
            {
                if (e.KeyChar != '\b')
                {
                    e.Handled = true;
                    System.Media.SystemSounds.Beep.Play();
                }
            }*/
        }

        private void txtMusic_TextChanged(object sender, EventArgs e)
        {
            if (qxy) return;

            if (selectedTrainer > -1)
            {
                trainer.Music = (byte)txtMusic.Value;
            }
        }

        private void txtAI_TextChanged(object sender, EventArgs e)
        {
            if (qxy) return;

            if (selectedTrainer > -1)
            {
                trainer.AI = txtAI.Value;
            }
        }

        private void chkDouble_CheckedChanged(object sender, EventArgs e)
        {
            if (qxy) return;

            if (selectedTrainer > -1)
            {
                trainer.DoubleBattle = chkDouble.Checked;
            }
        }

        private void chkHeld_CheckedChanged(object sender, EventArgs e)
        {
            if (qxy) return;

            if (selectedTrainer > -1)
            {
                trainer.HasHeldItems = chkHeld.Checked;
                if (selectedParty > -1) cHeld.Enabled = chkHeld.Checked;
            }
        }

        private void chkAttacks_CheckedChanged(object sender, EventArgs e)
        {
            if (qxy) return;

            if (selectedTrainer > -1)
            {
                trainer.HasCustomAttacks = chkAttacks.Checked;
                if (selectedParty > -1) groupBox7.Enabled = chkAttacks.Checked;
            }
        }

        private void cItem1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (qxy) return;

            if (selectedTrainer > -1)
            {
                trainer.HeldItems[0] = (ushort)cItem1.SelectedIndex;
            }
        }

        private void cItem3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (qxy) return;

            if (selectedTrainer > -1)
            {
                trainer.HeldItems[2] = (ushort)cItem3.SelectedIndex;
            }
        }

        private void cItem2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (qxy) return;

            if (selectedTrainer > -1)
            {
                trainer.HeldItems[1] = (ushort)cItem2.SelectedIndex;
            }
        }

        private void cItem4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (qxy) return;

            if (selectedTrainer > -1)
            {
                trainer.HeldItems[3] = (ushort)cItem4.SelectedIndex;
            }
        }

        private void txtMoneyRate_TextChanged(object sender, EventArgs e)
        {
            if (qxy) return;

            if (selectedTrainer > -1)
            {
                FillPrizeMoney();
                SaveClassMoneyRate(cClass2.SelectedIndex, (byte)txtMoneyRate.Value);
            }
        }

        private void listParty_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (qxy) return;

            if (selectedTrainer > -1 && trainer.Party.Count > 0)
            {
                selectedParty = -1;
                foreach (int i in listParty.SelectedIndices) selectedParty = i;
                if (selectedParty < 0) return;
                //selectedParty = listParty.SelectedIndices[listParty.SelectedIndices.Count - 1];

                // display crap
                qxy = true;
                cSpecies.SelectedIndex = trainer.Party[selectedParty].Species;
                cSpeciesN.SelectedIndex = trainer.Party[selectedParty].Species;
                txtLevel.Value = trainer.Party[selectedParty].Level;
                txtEVs.Value = trainer.Party[selectedParty].IVs;
                cSpecies.Enabled = true;
                cSpeciesN.Enabled = true;
                txtLevel.Enabled = true;
                txtEVs.Enabled = true;

                pSpecies.Image = LoadPokemonSprite(trainer.Party[selectedParty].Species);
                pSpecies.Invalidate();

                if (trainer.HasHeldItems)
                {
                    cHeld.SelectedIndex = trainer.Party[selectedParty].HeldItem;
                    cHeld.Enabled = true;
                }
                else
                {
                    cHeld.SelectedIndex = 0;
                    cHeld.Enabled = false;
                }

                if (trainer.HasCustomAttacks)
                {
                    cAtk1.SelectedIndex = trainer.Party[selectedParty].Attacks[0];
                    cAtk2.SelectedIndex = trainer.Party[selectedParty].Attacks[1];
                    cAtk3.SelectedIndex = trainer.Party[selectedParty].Attacks[2];
                    cAtk4.SelectedIndex = trainer.Party[selectedParty].Attacks[3];
                    groupBox7.Enabled = true;
                }
                else
                {
                    cAtk1.SelectedIndex = 0;
                    cAtk2.SelectedIndex = 0;
                    cAtk3.SelectedIndex = 0;
                    cAtk4.SelectedIndex = 0;
                    groupBox7.Enabled = false;
                }
                qxy = false;
            }
        }

        private void bAdd_Click(object sender, EventArgs e)
        {
            if (selectedTrainer > -1 && trainer.Party.Count < 6)
            {
                qxy = true;
                trainer.Party.Add(new Pokemon());
                ListViewItem item = new ListViewItem(pokemonNames[0]);
                item.SubItems.Add("0");
                listParty.Items.Add(item);
                qxy = false;
            }
        }

        private void bRemove_Click(object sender, EventArgs e)
        {
            if (selectedTrainer > -1 && trainer.Party.Count > 0 && selectedParty > -1)
            {
                qxy = true;
                trainer.Party.RemoveAt(selectedParty);
                listParty.Items.RemoveAt(selectedParty);
                selectedParty = -1;
                qxy = false;
            }
        }

        private void cSpecies_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (qxy) return;

            if (selectedTrainer > -1 && selectedParty > -1)
            {
                trainer.Party[selectedParty].Species = (ushort)cSpecies.SelectedIndex;

                qxy = true;
                cSpeciesN.SelectedIndex = cSpecies.SelectedIndex;
                listParty.Items[selectedParty].Text = pokemonNames[cSpecies.SelectedIndex];
                pSpecies.Image = LoadPokemonSprite(cSpecies.SelectedIndex);
                pSpecies.Invalidate();
                qxy = false;
            }
        }

        private void cSpeciesN_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (qxy) return;

            if (selectedTrainer > -1 && selectedParty > -1)
            {
                trainer.Party[selectedParty].Species = (ushort)cSpeciesN.SelectedIndex;

                qxy = true;
                cSpecies.SelectedIndex = cSpeciesN.SelectedIndex;
                listParty.Items[selectedParty].Text = pokemonNames[cSpeciesN.SelectedIndex];
                pSpecies.Image = LoadPokemonSprite(cSpeciesN.SelectedIndex);
                pSpecies.Invalidate();
                qxy = false;
            }
        }

        private void txtLevel_TextChanged(object sender, EventArgs e)
        {
            if (qxy) return;

            if (selectedTrainer > -1 && selectedParty > -1)
            {
                trainer.Party[selectedParty].Level = (ushort)txtLevel.Value;

                qxy = true;
                listParty.Items[selectedParty].SubItems[1].Text = txtLevel.Value.ToString();
                FillPrizeMoney();
                qxy = false;
            }
        }

        private void txtEVs_TextChanged(object sender, EventArgs e)
        {
            if (qxy) return;

            if (selectedTrainer > -1 && selectedParty > -1)
            {
                trainer.Party[selectedParty].IVs = (ushort)txtEVs.Value;
            }
        }

        private void cHeld_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (qxy) return;

            if (selectedTrainer > -1 && selectedParty > -1)
            {
                trainer.Party[selectedParty].HeldItem = (ushort)cHeld.SelectedIndex;
            }
        }

        private void cAtk1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (qxy) return;

            if (selectedTrainer > -1 && selectedParty > -1)
            {
                trainer.Party[selectedParty].Attacks[0] = (ushort)cAtk1.SelectedIndex;
            }
        }

        private void cAtk2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (qxy) return;

            if (selectedTrainer > -1 && selectedParty > -1)
            {
                trainer.Party[selectedParty].Attacks[1] = (ushort)cAtk2.SelectedIndex;
            }
        }

        private void cAtk3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (qxy) return;

            if (selectedTrainer > -1 && selectedParty > -1)
            {
                trainer.Party[selectedParty].Attacks[2] = (ushort)cAtk3.SelectedIndex;
            }
        }

        private void cAtk4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (qxy) return;

            if (selectedTrainer > -1 && selectedParty > -1)
            {
                trainer.Party[selectedParty].Attacks[3] = (ushort)cAtk4.SelectedIndex;
            }
        }

        #endregion

        #region Search

        private void txtFindID_TextChanged(object sender, EventArgs e)
        {
            //uint value = txtFindID.Value;
            //groupBox8.Text = "$" + txtFindID.Value.ToString("X");
            //listTrainers.TopItem = listTrainers.Items[(int)txtFindID.Value];
        }

        private void txtFindName_TextChanged(object sender, EventArgs e)
        {
            // Searches for a trainer name
            /*string search = txtFindName.Text.ToLower();
            if (search.Length == 0) return;

            int id = -1; bool found = false;
            foreach (string name in trainerNames)
            {
                id++;
                if (name.ToLower().StartsWith(search))
                {
                    found = true;
                    break;
                }
            }

            // Scrolls to it, but does not select it
            if (found) listTrainers.TopItem = listTrainers.Items[id];
            else listTrainers.TopItem = listTrainers.Items[0];*/
        }

        private void txtFindName_KeyUp(object sender, KeyEventArgs e)
        {
            
        }

        private void txtFindName_KeyDown(object sender, KeyEventArgs e)
        {
            // if enter
            if (e.KeyCode != Keys.Enter)
            {
                return;
            }
            else
            {
                e.Handled = e.SuppressKeyPress = true;
            }
            //MessageBox.Show("Search!");

            // search away
            // use the text table for enhanced searching
            //string search = TextTable.GetEnglishString(TextTable.GetEnglishBytes(txtFindName.Text));
            
            if (txtFindName.TextLength == 0) return;
            string search = txtFindName.Text.ToLower();

            // get starting position
            /*int start = 0;
            foreach (int i in listTrainers.SelectedIndices) start = i;

            if (start > 0) start++;*/

            // search from current position
            int start = listTrainers.TopItem.Index;
            if (start > 0) start++;

            // and search
            int id = -1;
            for (int i = start; i < trainerNames.Length; i++)
            {
                string trainer = trainerNames[i].ToLower();

                if (trainer.IndexOf(search) >= 0)
                {
                    id = i;
                    break;
                }
            }

            // display, if one was found
            if (id >= 0)
            {
                listTrainers.TopItem = listTrainers.Items[id];
                listTrainers.TopItem.Selected = true;
            }
            else
            {
                MessageBox.Show(string.Format("A trainer with '{0}' in its name could not be found!\n(From the current spot onward!)", txtFindName.Text));
            }
        }

        private void txtFindID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                listTrainers.TopItem = listTrainers.Items[(int)txtFindID.Value];
                listTrainers.TopItem.Selected = true;
                
                e.Handled = e.SuppressKeyPress = true;
            }
        }

        #endregion




    }
}
