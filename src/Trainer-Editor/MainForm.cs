using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using GBAHL;
using GBAHL.IO;

namespace Hopeless
{
    public partial class MainForm : Form
    {
        Settings settings;

        ROM rom;
        Settings romInfo;
        int lastSearch = 0x720000;

        Bitmap invisible = new Bitmap(64, 64);
        PictureBox[] partyPictureBoxes;

        bool ignore = false;

        public MainForm()
        {
            InitializeComponent();

            partyPictureBoxes = new PictureBox[6] { p1, p2, p3, p4, p5, p6 };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!File.Exists("Settings.ini"))
                return;

            settings = Settings.FromFile("Settings.ini", Settings.Format.INI);

            repointAutomaticallyToolStripMenuItem.Checked = settings.GetBoolean("Settings", "RepointAutomatically");
            cleanRepointedTrainersToolStripMenuItem.Checked = settings.GetBoolean("Settings", "CleanRepointed");

            grpTrainer.Enabled = false;
            bRandomize.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
            importToolStripMenuItem.Enabled = false;
            exportToolStripMenuItem.Enabled = false;
            changePartyOffsetToolStripMenuItem.Enabled = false;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            invisible.Dispose();
            rom?.Dispose();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "GBA ROMs|*.gba";
            openFileDialog1.Title = "Open ROM";

            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;

            if (!OpenROM(openFileDialog1.FileName))
                return;

            LoadAll();

            // populate form
            listTrainers.Items.Clear();
            for (int i = 0; i < trainerCount; i++)
            {
                var item = new ListViewItem($"{i:X3}");
                item.SubItems.Add(names[i]);
                listTrainers.Items.Add(item);
            }
            txtSearchId.MaximumValue = trainerCount - 1;

            grpTrainer.Enabled = true;
            bRandomize.Enabled = true;
            saveToolStripMenuItem.Enabled = true;
            importToolStripMenuItem.Enabled = true;
            exportToolStripMenuItem.Enabled = true;
            changePartyOffsetToolStripMenuItem.Enabled = true;

            cClass.Items.Clear();
            cClass.Items.AddRange(classes);

            cItem1.Items.Clear();
            cItem1.Items.AddRange(items);
            cItem2.Items.Clear();
            cItem2.Items.AddRange(items);
            cItem3.Items.Clear();
            cItem3.Items.AddRange(items);
            cItem4.Items.Clear();
            cItem4.Items.AddRange(items);

            cHeld.Items.Clear();
            cHeld.Items.AddRange(items);

            cSpecies.Items.Clear();
            cSpecies.Items.AddRange(pokemon);

            cAttack1.Items.Clear();
            cAttack1.Items.AddRange(attacks);
            cAttack2.Items.Clear();
            cAttack2.Items.AddRange(attacks);
            cAttack3.Items.Clear();
            cAttack3.Items.AddRange(attacks);
            cAttack4.Items.Clear();
            cAttack4.Items.AddRange(attacks);

            DisplayTrainer();
            DisplayPartyMember();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (trainer == null)
                return;

            SaveTrainer();
            SaveClasses();

            grpParty.Text = $"Party (0x{trainer.PartyOffset:X7})";

            listTrainers.Items[trainer.Index].SubItems[1].Text = trainer.Name;
            names[trainer.Index] = trainer.Name;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (trainer == null)
                return;

            openFileDialog1.Title = $"Import Trainer {trainer.Index}";
            openFileDialog1.Filter = "Trainer Files|*.trainer";
            openFileDialog1.FileName = "";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ImportTrainer(openFileDialog1.FileName);

                DisplayTrainer();
                member = null;

                for (int i = 0; i < 6; i++)
                {
                    Image sprite = invisible;
                    if (i < trainer.Party.Count)
                    {
                        sprite = LoadFrontSprite(trainer.Party[i].Species);
                    }
                    partyPictureBoxes[i].Image = sprite;
                }
                pSprite.Image = LoadTrainerSprite(trainer.Sprite);  
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (trainer == null)
                return;

            saveFileDialog1.Title = $"Export Trainer {trainer.Index}";
            saveFileDialog1.Filter = "Trainer Files|*.trainer";
            saveFileDialog1.FileName = $"{trainer.Index:X3} {trainer.Name}";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                ExportTrainer(saveFileDialog1.FileName);
        }

        private void repointAutomaticallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            settings.Set("Settings", "RepointAutomatically", repointAutomaticallyToolStripMenuItem.Checked);
            settings.Save("Settings.ini", Settings.Format.INI);
        }

        private void cleanRepointedTrainersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            settings.Set("Settings", "CleanRepointed", cleanRepointedTrainersToolStripMenuItem.Checked);
            settings.Save("Settings.ini", Settings.Format.INI);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var a = new AboutDialog())
                a.ShowDialog();
        }

        private void listTrainers_SelectedIndexChanged(object sender, EventArgs e)
        {
            var index = -1;
            foreach (int x in listTrainers.SelectedIndices)
                index = x;

            if (index == -1)
                return;

            // ------------------------------
            LoadTrainer(index);
            member = null;

            for (int i = 0; i < 6; i++)
            {
                Image sprite = invisible;
                if (i < trainer.Party.Count)
                {
                    sprite = LoadFrontSprite(trainer.Party[i].Species);
                }
                partyPictureBoxes[i].Image = sprite;
            }
            pSprite.Image = LoadTrainerSprite(trainer.Sprite);

            // ------------------------------
            DisplayTrainer();
            DisplayPartyMember();
        }

        private void listParty_SelectedIndexChanged(object sender, EventArgs e)
        {
            var index = -1;
            foreach (int x in listParty.SelectedIndices)
                index = x;

            if (index == -1)
                return;

            // ------------------------------
            member = trainer.Party[index];
            DisplayPartyMember();
        }

        bool OpenROM(string filename)
        {
            ROM temp = null;

            //var custom = Path.ChangeExtension(filename, ".hte");
            try
            {
                // open ROM file
                temp = new ROM(filename);

                // check for settings file
                if (!File.Exists($@"ROMs\{temp.Code}.ini")) {
                    MessageBox.Show("ROM code {temp.Code} is not supported!",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    temp?.Dispose();
                    return false;
                }

                // open settings file
                romInfo = Settings.FromFile($@"ROMs\{temp.Code}.ini", Settings.Format.INI);
            }
            catch (Exception ex)
            {
#if DEBUG
                MessageBox.Show($"{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
#else
                MessageBox.Show($"There was an error opening the ROM.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif

                temp?.Dispose();
                return false;
            }

            // update title
            Text = $"Hopeless Trainer Editor v1.0 - [{Path.GetFileName(openFileDialog1.FileName)}]";

            // set new open ROM and report success
            rom?.Dispose();
            rom = temp;
            return true;
        }

        void LoadAll()
        {
            if (rom == null) return;

            // get limits from .ini
            pokemonCount = romInfo.GetInt32("pokemon", "Count");
            itemCount = romInfo.GetInt32("items", "Count");
            attackCount = romInfo.GetInt32("attacks", "Count");

            trainerCount = romInfo.GetInt32("trainers", "Count");
            trainerSpriteCount = romInfo.GetInt32("trainer_sprites", "Count");
            classCount = romInfo.GetInt32("trainer_classes", "Count");

            // load all data needed
            LoadNames();
            LoadClasses();
            LoadPokemonNames();
            LoadAttacks();
            LoadItems();

            // settings for editor
            txtSpecies.MaximumValue = pokemonCount - 1;
            txtClassID.MaximumValue = classCount - 1;
            nSprite.Maximum = trainerSpriteCount - 1;
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            if (ignore)
                return;

            trainer.Name = txtName.Text;
        }

        private void rMale_CheckedChanged(object sender, EventArgs e)
        {
            if (ignore)
                return;

            if (rMale.Checked)
                trainer.Gender = 0;
        }

        private void rFemale_CheckedChanged(object sender, EventArgs e)
        {
            if (ignore)
                return;

            if (rFemale.Checked)
                trainer.Gender = 1;
        }

        private void nSprite_ValueChanged(object sender, EventArgs e)
        {
            if (ignore)
                return;

            trainer.Sprite = (byte)nSprite.Value;
            pSprite.Image = LoadTrainerSprite(trainer.Sprite);
        }

        private void txtClassID_TextChanged(object sender, EventArgs e)
        {
            if (ignore || txtClassID.Value >= classCount)
                return;

            trainer.Class = (byte)txtClassID.Value;

            ignore = true;
            cClass.SelectedIndex = txtClassID.Value;
            txtClass.Text = classes[trainer.Class];
            ignore = false;
        }

        private void cClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ignore)
                return;

            trainer.Class = (byte)cClass.SelectedIndex;

            ignore = true;
            txtClassID.Value = cClass.SelectedIndex;
            txtClass.Text = classes[trainer.Class];
            ignore = false;
        }

        private void txtClass_TextChanged(object sender, EventArgs e)
        {
            if (ignore)
                return;

            classes[trainer.Class] = txtClass.Text;
            cClass.Items[trainer.Class] = txtClass.Text;
        }

        private void cItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ignore)
                return;

            trainer.Items[0] = (ushort)cItem1.SelectedIndex;
            trainer.Items[1] = (ushort)cItem2.SelectedIndex;
            trainer.Items[2] = (ushort)cItem3.SelectedIndex;
            trainer.Items[3] = (ushort)cItem4.SelectedIndex;
        }

        private void txtMusic_TextChanged(object sender, EventArgs e)
        {
            if (ignore)
                return;

            trainer.Music = (byte)txtMusic.Value;
        }

        private void txtAI_TextChanged(object sender, EventArgs e)
        {
            if (ignore)
                return;

            trainer.AI = (trainer.AI & ~0x1FFu) | (uint)(txtAI.Value & 0x1FF);
        }

        private void chkDoubleBattle_CheckedChanged(object sender, EventArgs e)
        {
            if (ignore)
                return;

            trainer.DoubleBattle = chkDoubleBattle.Checked;
        }

        private void chkHeldItems_CheckedChanged(object sender, EventArgs e)
        {
            if (ignore)
                return;

            trainer.HasHeldItems = chkHeldItems.Checked;

            ignore = true;
            cHeld.Enabled = trainer.HasHeldItems;

            if (!trainer.HasHeldItems || member == null)
            {
                cHeld.SelectedIndex = 0;
            }
            else
            {
                cHeld.SelectedIndex = member.HeldItem;
            }
            ignore = false;
        }

        private void chkMovesets_CheckedChanged(object sender, EventArgs e)
        {
            if (ignore)
                return;

            trainer.HasCustomAttacks = chkMovesets.Checked;

            ignore = true;
            cAttack1.Enabled = trainer.HasCustomAttacks;
            cAttack2.Enabled = trainer.HasCustomAttacks;
            cAttack3.Enabled = trainer.HasCustomAttacks;
            cAttack4.Enabled = trainer.HasCustomAttacks;

            if (!trainer.HasCustomAttacks || member == null)
            {
                cAttack1.SelectedIndex = 0;
                cAttack2.SelectedIndex = 0;
                cAttack3.SelectedIndex = 0;
                cAttack4.SelectedIndex = 0;
            }
            else
            {
                cAttack1.SelectedIndex = member.Attacks[0];
                cAttack2.SelectedIndex = member.Attacks[1];
                cAttack3.SelectedIndex = member.Attacks[2];
                cAttack4.SelectedIndex = member.Attacks[3];
            }
            ignore = false;
        }

        private void txtSpecies_TextChanged(object sender, EventArgs e)
        {
            if (ignore || member == null || txtSpecies.Value >= pokemonCount)
                return;

            member.Species = (ushort)txtSpecies.Value;

            ignore = true;
            cSpecies.SelectedIndex = member.Species;

            partyPictureBoxes[member.Index].Image = LoadFrontSprite(member.Species);
            listParty.Items[member.Index].SubItems[0].Text = pokemon[member.Species];
            ignore = false;
        }

        private void cSpecies_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ignore || member == null)
                return;

            member.Species = (ushort)cSpecies.SelectedIndex;

            ignore = true;
            txtSpecies.Value = member.Species;

            partyPictureBoxes[member.Index].Image = LoadFrontSprite(member.Species);
            listParty.Items[member.Index].SubItems[0].Text = pokemon[member.Species];
            ignore = false;
        }

        private void txtLevel_TextChanged(object sender, EventArgs e)
        {
            if (ignore || member == null)
                return;

            member.Level = (ushort)((member.Level & 0xFF00) | (byte)txtLevel.Value);

            ignore = true;
            listParty.Items[member.Index].SubItems[1].Text = member.Level.ToString();
            ignore = false;
        }

        private void txtEVs_TextChanged(object sender, EventArgs e)
        {
            if (ignore || member == null)
                return;

            member.EVs = (ushort)((member.EVs & 0xFF00) | (byte)txtEVs.Value);
        }

        private void cHeld_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ignore || member == null)
                return;

            member.HeldItem = (ushort)cHeld.SelectedIndex;
        }

        private void cAttack_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ignore || member == null)
                return;

            member.Attacks[0] = (ushort)cAttack1.SelectedIndex;
            member.Attacks[1] = (ushort)cAttack2.SelectedIndex;
            member.Attacks[2] = (ushort)cAttack3.SelectedIndex;
            member.Attacks[3] = (ushort)cAttack4.SelectedIndex;
        }

        private void bPartyAdd_Click(object sender, EventArgs e)
        {
            if (trainer == null || trainer.Party.Count >= 6)
                return;

            var p = new Pokemon(trainer.Party.Count);
            trainer.Party.Add(p);

            ignore = true;
            var i = new ListViewItem(pokemon[0]);
            i.SubItems.Add("0");
            listParty.Items.Add(i);

            grpParty.Text = $"Party (0x{trainer.PartyOffset:X7})";
            if (trainer.RequiresRepoint) grpParty.Text += "*";

            ShiftParty();
            ignore = false;
        }

        private void bPartyRemove_Click(object sender, EventArgs e)
        {
            if (member == null || trainer.Party.Count <= 1)
                return;

            trainer.Party.RemoveAt(member.Index);

            ignore = true;
            listParty.Items.RemoveAt(member.Index);
            member = null;

            txtSpecies.Value = 0;
            txtLevel.Value = 0;
            txtEVs.Value = 0;
            cSpecies.SelectedIndex = 0;
            cHeld.SelectedIndex = 0;
            cAttack1.SelectedIndex = 0;
            cAttack2.SelectedIndex = 0;
            cAttack3.SelectedIndex = 0;
            cAttack4.SelectedIndex = 0;

            grpParty.Text = $"Party (0x{trainer.PartyOffset:X7})";
            if (trainer.RequiresRepoint) grpParty.Text += "*";

            ShiftParty();

            ignore = false;
        }

        void ShiftParty()
        {
            for (int i = 0; i < trainer.Party.Count; i++)
                trainer.Party[i].Index = i;

            for (int i = 0; i < 6; i++)
            {
                Image sprite = invisible;
                if (i < trainer.Party.Count)
                    sprite = LoadFrontSprite(trainer.Party[i].Species);

                partyPictureBoxes[i].Image = sprite;
            }
        }

        private void txtSearchId_KeyUp(object sender, KeyEventArgs e)
        {
            if (rom == null)
                return;

            if (e.KeyCode == Keys.Enter /*&& txtSearchId.Value <= trainerCount*/)
            {
                listTrainers.TopItem = listTrainers.Items[txtSearchId.Value];
                listTrainers.TopItem.Selected = true;

                e.Handled = e.SuppressKeyPress = true;
            }
        }

        private void txtSearchName_KeyUp(object sender, KeyEventArgs e)
        {
            if (rom == null || e.KeyCode != Keys.Enter || txtSearchName.TextLength <= 0)
                return;

            // get text to search for
            var search = txtSearchName.Text.ToLower();

            // get initial position to search from
            var start = listTrainers.TopItem.Index;
            if (start > 0)
                start++;

            var result = -1;

        search:
            // search names for text
            for (int i = start; i < trainerCount; i++)
            {
                var name = names[i].ToLower();
                if (name.IndexOf(search) >= 0)
                {
                    result = i;
                    break;
                }
            }

            if (result == -1 && start > 0)
            {
                // first time not found, search from top
                start = 0;
                goto search;
            }
            else if (result >= 0)
            {
                // result found
                listTrainers.TopItem = listTrainers.Items[result];
                listTrainers.TopItem.Selected = true;
            }

            e.Handled = e.SuppressKeyPress = true;
        }

        private void bRandomize_Click(object sender, EventArgs e)
        {
            if (rom == null)
                return;

            // confirm that the user actually wants to randomize
            if (MessageBox.Show("This will randomize every single trainer's party.\n" +
                "Are you sure you want to do this?",
                "Randomize?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            // (simple) randomize all trainers
            RandomizeTrainers();

            // reload current trainer
            if (trainer != null)
            {
                LoadTrainer(trainer.Index);

                if (member != null)
                    member = trainer.Party[member.Index];

                DisplayTrainer();
                DisplayPartyMember();

                for (int i = 0; i < 6; i++)
                {
                    Image sprite = invisible;
                    if (i < trainer.Party.Count)
                    {
                        sprite = LoadFrontSprite(trainer.Party[i].Species);
                    }
                    partyPictureBoxes[i].Image = sprite;
                }
            }
        }

        private void changePartyOffsetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var d = new OffsetDialog(trainer.PartyOffset, trainer.Party.Count))
            {
                if (d.ShowDialog() == DialogResult.OK)
                {
                    // --
                    if (trainer.PartyOffset == d.Offset)
                        return;

                    // Load a fresh party
                    trainer.PartyOffset = d.Offset;
                    LoadParty(d.Count);

                    // Adjust party
                    member = null;

                    for (int i = 0; i < 6; i++) {
                        Image sprite = invisible;
                        if (i < trainer.Party.Count) {
                            sprite = LoadFrontSprite(trainer.Party[i].Species);
                        }
                        partyPictureBoxes[i].Image = sprite;
                    }
                    pSprite.Image = LoadTrainerSprite(trainer.Sprite);

                    // ------------------------------
                    DisplayTrainer();
                    DisplayPartyMember();
                }
#if DEBUG
                else
                {
                    Console.WriteLine("Change offset canceled.");
                }
#endif
            }
        }
    }
}
