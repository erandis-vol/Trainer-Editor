using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hopeless
{
    public partial class OffsetDialog : Form
    {
        public OffsetDialog(string title, string text, int initialOffset)
        {
            InitializeComponent();

            Title = title;
            Text = text;
            Offset = initialOffset;
        }

        public string Title
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        public new string Text
        {
            get { return lblDescription.Text; }
            set { lblDescription.Text = value; }
        }

        public int Offset
        {
            get { return txtOffset.Value; }
            private set { txtOffset.Value = value; }
        }
    }
}
