using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lost
{
    public partial class MainForm : Form
    {
        ROM rom;
        Settings romInfo;

        Bitmap invisible = new Bitmap(64, 64);
        PictureBox[] partyPictureBoxes;

        bool ignore = false;

        public MainForm()
        {
            InitializeComponent();

            partyPictureBoxes = new PictureBox[6] { p1, p2, p3, p4, p5, p6 };
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            invisible.Dispose();
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
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (trainer == null)
                return;

            SaveTrainer();
            SaveClasses();

            rom.Save();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void listTrainers_SelectedIndexChanged(object sender, EventArgs e)
        {
            var index = -1;
            foreach (int x in listTrainers.SelectedIndices)
                index = x;

            if (index == -1)
                return;
            ignore = true;

            // ------------------------------
            LoadTrainer(index);
            member = null;

            for (int i = 0; i < 6; i++)
            {
                var sprite = invisible;
                if (i < trainer.Party.Count)
                {
                    sprite = LoadFrontSprite(trainer.Party[i].Species);
                }
                partyPictureBoxes[i].Image = sprite;
            }
            pSprite.Image = LoadTrainerSprite(trainer.Sprite);

            // ------------------------------
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

            txtSpecies.Value = 0;
            txtLevel.Value = 0;
            txtEVs.Value = 0;
            cSpecies.SelectedIndex = 0;
            cHeld.SelectedIndex = 0;
            cAttack1.SelectedIndex = 0;
            cAttack2.SelectedIndex = 0;
            cAttack3.SelectedIndex = 0;
            cAttack4.SelectedIndex = 0;

            cHeld.Enabled = trainer.HasHeldItems;

            cAttack1.Enabled = trainer.HasCustomAttacks;
            cAttack2.Enabled = trainer.HasCustomAttacks;
            cAttack3.Enabled = trainer.HasCustomAttacks;
            cAttack4.Enabled = trainer.HasCustomAttacks;

            grpTrainer.Text = $"Trainer (0x{trainer.Index:X3})";
            grpParty.Text = $"Party (0x{trainer.PartyOffset:X7})";

            ignore = false;
        }

        private void listParty_SelectedIndexChanged(object sender, EventArgs e)
        {
            var index = -1;
            foreach (int x in listParty.SelectedIndices)
                index = x;

            if (index == -1)
                return;
            ignore = true;

            // ------------------------------
            member = trainer.Party[index];

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

            ignore = false;
        }

        bool OpenROM(string filename)
        {
            bool success = true;
            ROM temp = null;

            var custom = Path.ChangeExtension(filename, ".h");

            try
            {
                // create a new ROM
                temp = new ROM(filename);

                // first check for custom settings
                if (File.Exists(custom))
                {
                    romInfo = Settings.FromFile(custom, "ini");
                }
                else
                {
                    // check that it is valid
                    if (!File.Exists($@"ROMs\{temp.Code}.ini"))
                        throw new Exception($"ROM type {temp.Code} is not supported!");

                    // load default settings
                    romInfo = Settings.FromFile($@"ROMs\{temp.Code}.ini", "ini");

                    // copy and save to custom settings
                    romInfo.Save(custom, "ini");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                success = false;
            }

            // finish
            if (success)
            {
                rom?.Dispose();
                rom = temp;
            }
            else
            {
                temp?.Dispose();
            }
            return success;
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

            txtSpecies.MaximumValue = pokemonCount - 1;
            txtClassID.MaximumValue = classCount - 1;
            nSprite.Maximum = trainerSpriteCount - 1;
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            if (ignore)
                return;

            trainer.Name = txtName.Text;
            listTrainers.Items[trainer.Index].SubItems[1].Text = txtName.Text;
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
            if (ignore || member == null)
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
                var sprite = invisible;
                if (i < trainer.Party.Count)
                    sprite = LoadFrontSprite(trainer.Party[i].Species);

                partyPictureBoxes[i].Image = sprite;
            }
        }
    }
}
