using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GBAHL.IO;

namespace Hopeless
{
    public partial class FreeSpaceDialog : Form
    {
        ROM rom;
        int[] results;
        int selection = -1;

        public FreeSpaceDialog(ROM rom, int neededBytes, int searchStart)
        {
            InitializeComponent();
            this.rom = rom;

            txtNeeded.Value = neededBytes;
            txtSearchStart.Value = searchStart;
            txtNeeded.ReadOnly = true;

            Search();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void txtRepointTo_TextChanged(object sender, EventArgs e)
        {
            button1.Enabled = txtRepointTo.Value > 0;
        }

        private void Search()
        {
            txtRepointTo.Value = rom.Find(0xFF, txtNeeded.Value, txtSearchStart.Value);
            button1.Enabled = txtRepointTo.Value > 0;
        }

        public int Offset
        {
            get { return txtRepointTo.Value; }
        }

        public int SearchStart
        {
            get { return txtSearchStart.Value; }
        }
    }
}
