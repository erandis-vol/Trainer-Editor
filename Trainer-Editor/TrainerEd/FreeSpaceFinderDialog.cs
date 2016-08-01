using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace HTE
{
    public partial class FreeSpaceFinderDialog : Form
    {
        private string romFilePath;

        private int[] searchResults;
        private int searchSelection;

        private bool success, thingy = true;

        public FreeSpaceFinderDialog(string rom)
        {
            InitializeComponent();

            romFilePath = rom;
            txtNeeded.Value = 0;
            bOK.Visible = false;
            bCancel.Text = "Done";
        }

        public FreeSpaceFinderDialog(string rom, uint neededBytes)
        {
            InitializeComponent();

            romFilePath = rom;
            txtNeeded.Value = neededBytes;
            txtNeeded.ReadOnly = true;
        }

        private void FreeSpaceFinderDialog_Load(object sender, EventArgs e)
        {
            success = false;
            searchResults = null;
            searchSelection = -1;

            cFSByte.SelectedIndex = 1; // FF
            PerformSearch();
            thingy = false;
        }

        private void FreeSpaceFinderDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            // The only success measured is whether we chose OK or not
            if (success && searchResults != null && searchSelection >= 0) DialogResult = DialogResult.OK;
            else DialogResult = DialogResult.Cancel;
        }

        private void bOK_Click(object sender, EventArgs e)
        {
            success = true;
            Close();
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            success = false;
            Close();
        }

        private void PerformSearch()
        {
            int needed = (int)txtNeeded.Value;
            uint start = txtSearchStart.Value;
            byte fs = (byte)(cFSByte.SelectedIndex == 1 ? 0xFF : 0x00);

            if (needed <= 0)
            {
                listResults.Items.Clear(); // Take no prisoners
                bOK.Enabled = false;
                searchSelection = -1;

                return; // Yup
            }

            // Let's go~
            byte[] romData = File.ReadAllBytes(romFilePath);
            List<int> offsets = new List<int>();
            for (int i = 0; i < 10; i++)
            {
                // Try to find an offset to use
                int offset = Tasks.FindFreeSpace(romData, needed, start, fs);
                if (offset != -1)
                {
                    offsets.Add(offset);
                    start = (uint)(offset + needed);
                }
                else break;
            }

            // Now share it.
            thingy = true;
            listResults.Items.Clear();
            if (offsets.Count == 0)
            {
                MessageBox.Show("Oh no! It seems there is no free space left in the ROM!",
                    "Uh-oh", MessageBoxButtons.OK, MessageBoxIcon.Error);

                bOK.Enabled = false;
                searchSelection = -1;
            }
            else
            {
                // Share it.
                bOK.Enabled = false;
                searchSelection = -1;
                searchResults = offsets.ToArray();

                foreach (int i in offsets)
                {
                    listResults.Items.Add("0x" + i.ToString("X"));
                }
            }
            thingy = false;
        }

        private void listResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (thingy) return;

            if (searchResults == null) return;

            searchSelection = listResults.SelectedIndex;
            bOK.Enabled = true;
        }

        private void txtNeeded_TextChanged(object sender, EventArgs e)
        {
            if (thingy) return;

            PerformSearch();
        }

        private void cFSByte_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (thingy) return;

            PerformSearch();
        }

        private void txtSearchStart_TextChanged(object sender, EventArgs e)
        {
            if (thingy) return;

            PerformSearch();
        }

        public uint FreeSpaceOffset
        {
            get { return (uint)searchResults[searchSelection]; }
        }
    }
}
