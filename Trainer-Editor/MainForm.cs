using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrainerEditor
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void menuItemOpen_Click(object sender, EventArgs e)
        {
            if (Program.IsProjectOpen())
            {
                listBox1.Items.Clear();
                listBox1.Items.AddRange(Trainer.LoadIdentifiers().ToArray());
            }
        }

        private void menuItemSave_Click(object sender, EventArgs e)
        {

        }

        private void menuItemExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void menuItemImport_Click(object sender, EventArgs e)
        {

        }

        private void menuItemExport_Click(object sender, EventArgs e)
        {

        }

        private void menuItemAbout_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var index = listBox1.SelectedIndex;
            if (index < 0)
                return;

            Trainer.Load(listBox1.Items[index].ToString());
        }
    }
}
