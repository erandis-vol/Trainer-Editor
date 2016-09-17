using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Lost
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
        }

        private void txtSearchStart_TextChanged(object sender, EventArgs e)
        {
            Search();
        }

        private void listResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (results != null && results.Length > 0)
            {
                selection = listResults.SelectedIndex;
                button1.Enabled = true;
            }
        }

        void Search()
        {
            var start = txtSearchStart.Value;
            var search = new List<int>();

            // search for up to 8 offsets
            for (int i = 0; i < 8; i++)
            {
                var result = rom.FindFreeSpace(txtNeeded.Value, 0xFF, start, 4);
                if (result == -1)
                    break;
                else
                    search.Add(result);

                start = result + txtNeeded.Value;
            }

            listResults.Items.Clear();
            if (search.Count > 0)
            {
                // add results to listbox
                foreach (var r in search)
                    listResults.Items.Add($"{r:X7}");

                // ---
                results = search.ToArray();

                // select first option
                listResults.SelectedIndex = 0;
                selection = 0;
            }
            else
            {
                selection = -1;
            }

            button1.Enabled = search.Count > 0;
        }

        public int Offset
        {
            get
            {
                return selection == -1 ? -1 : results[selection];
            }
        }
    }
}
