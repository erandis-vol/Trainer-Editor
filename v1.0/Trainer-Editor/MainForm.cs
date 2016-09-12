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

        PictureBox[] partyPictureBoxes;//= new PictureBox[6] { p1, p2, p3, p4, p5, p6 };

        public MainForm()
        {
            InitializeComponent();

            partyPictureBoxes = new PictureBox[6] { p1, p2, p3, p4, p5, p6 };
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
                var item = new ListViewItem($"{i:D3}");
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
        }

        private void listTrainers_SelectedIndexChanged(object sender, EventArgs e)
        {
            var index = -1;
            foreach (int x in listTrainers.SelectedIndices)
                index = x;

            if (index == -1) return;

            LoadTrainer(index);

            txtName.Text = trainer.Name;
            rMale.Checked = trainer.Gender == 0;
            rFemale.Checked = trainer.Gender == 1;
            nSprite.Value = trainer.Sprite;
            cClass.SelectedIndex = trainer.Class;
            cItem1.SelectedIndex = trainer.Items[0];
            cItem2.SelectedIndex = trainer.Items[1];
            cItem3.SelectedIndex = trainer.Items[2];
            cItem4.SelectedIndex = trainer.Items[3];
        }

        bool OpenROM(string filename)
        {
            bool success = true;
            ROM temp = null;

            try
            {
                // create a new ROM
                temp = new ROM(filename);

                // check that it is valid
                if (!File.Exists($@"ROMs\{temp.Code}.ini"))
                    throw new Exception($"ROM type {temp.Code} is not supported!");

                // TODO: custom settings
                romInfo = Settings.FromFile($@"ROMs\{temp.Code}.ini", "ini");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
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
            classCount = romInfo.GetInt32("trainer_classes", "Count");

            // load all data needed
            LoadNames();
            LoadClasses();

            LoadPokemonNames();
            LoadAttacks();
            LoadItems();
        }
    }
}
