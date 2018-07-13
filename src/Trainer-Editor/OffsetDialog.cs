using System;
using System.Windows.Forms;

namespace Hopeless
{
    public partial class OffsetDialog : Form
    {
        public OffsetDialog(int offset, int count)
        {
            InitializeComponent();

            Offset = offset;
            Count = count;
        }

        public int Offset
        {
            get { return txtOffset.Value; }
            private set { txtOffset.Value = value; }
        }

        public int Count
        {
            get { return txtSize.Value; }
            set { txtSize.Value = value; }
        }
    }
}
