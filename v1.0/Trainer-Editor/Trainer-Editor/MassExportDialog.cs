using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HTE
{
    public partial class MassExportDialog : Form
    {
        ListView.ListViewItemCollection trainers;
        private int[] result;

        private bool ok = false;
        private bool q = false;

        public MassExportDialog(ListView.ListViewItemCollection trainerList)
        {
            InitializeComponent();
            trainers = trainerList;
        }

        private void MassExportDialog_Load(object sender, EventArgs e)
        {
            listTrainers.Items.Clear();
            for (int i = 0; i < trainers.Count; i++)
            {
                var item = new ListViewItem(trainers[i].Text);
                item.SubItems.Add(trainers[i].SubItems[1].Text);
                item.Checked = false;
                listTrainers.Items.Add(item);
            }

            PopulateThing();
        }

        private void MassExportDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (ok) this.DialogResult = DialogResult.OK;
            else this.DialogResult = DialogResult.Cancel;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ok = true;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ok = false;
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            q = true;
            for (int i = 0; i < listTrainers.Items.Count; i++)
            {
                listTrainers.Items[i].Checked = true;
            }
            q = false;

            PopulateThing();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            q = true;
            for (int i = 0; i < listTrainers.Items.Count; i++)
            {
                listTrainers.Items[i].Checked = false;
            }
            q = false;

            PopulateThing();
        }

        private void listTrainers_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (q) return;

            PopulateThing();
        }

        private void PopulateThing()
        {
            List<int> list = new List<int>();
            for (int i = 0; i < listTrainers.Items.Count; i++)
            {
                if (listTrainers.Items[i].Checked) list.Add(i);
            }
            result = list.ToArray();
        }

        public int[] Result
        {
            get { return result; }
        }
    }
}
