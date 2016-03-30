using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HTE
{
    public partial class NumericTextBox : TextBox
    {
        private NumberStyles numberStyle;
        private uint maxValue, minValue;

        public NumericTextBox()
        {
            numberStyle = NumberStyles.Decimal;
            maxValue = uint.MaxValue - 1;
            minValue = 0;
            //Value = 0;
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            // Limit Key presses
            /*if (numberStyle == NumberStyles.Decimal)
            {
                if (char.IsDigit(e.KeyChar)) { }
                else if (e.KeyChar == '\b') { }
                else e.Handled = true;
            }
            else if (numberStyle == NumberStyles.Hexadecimal)
            {
                if (char.IsDigit(e.KeyChar)) { }
                else if (e.KeyChar == '\b') { }
                else if (e.KeyChar >= 'a' && e.KeyChar <= 'f') { }
                else if (e.KeyChar >= 'A' && e.KeyChar <= 'F') { }
                else if (e.KeyChar == 'x' && Text.StartsWith("0") && !Text.StartsWith("0x")/* && TextLength == 1*) { }
                else e.Handled = true;
            }*/

            base.OnKeyPress(e);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            uint? val = ThisToUInt32();
            if (val == null)
            {
                this.BackColor = Color.PaleVioletRed;
            }
            else
            {
                if (this.BackColor == Color.PaleVioletRed) this.BackColor = SystemColors.Window;

                if ((uint)val < minValue) Value = minValue;
                else if ((uint)val > maxValue) Value = maxValue;
            }

            base.OnTextChanged(e);
        }

        [Description("Gets or sets the default number base used by the TextBox."), DefaultValue(NumberStyles.Decimal)]
        public NumberStyles NumberStyle
        {
            get { return numberStyle; }
            set
            {
                uint val = Value;
                numberStyle = value;
                Value = val; // Yeah.
            }
        }

        public uint Value
        {
            get
            {
                uint? v = ThisToUInt32();
                if (v == null) return minValue;
                else return (uint)v;
            }
            set
            {
                if (numberStyle == NumberStyles.Binary) Text = Convert.ToString(value, 2);
                else if (numberStyle == NumberStyles.Decimal) Text = value.ToString();
                else if (numberStyle == NumberStyles.Hexadecimal) Text = "0x" + value.ToString("X");
            }
        }

        [Description("Gets or sets the maximum value allowed by the TextBox."), DefaultValue(uint.MaxValue - 1)]
        public uint MaxValue
        {
            get { return maxValue; }
            set { maxValue = value; }
        }

        [Description("Gets or sets the minimum value allowed by the TextBox."), DefaultValue(uint.MaxValue - 1)]
        public uint MinValue
        {
            get { return minValue; }
            set { minValue = value; }
        }

        public enum NumberStyles : int
        {
            Binary = 2, Decimal = 10, Hexadecimal = 16
        }

        private uint? ThisToUInt32()
        {
            try
            {
                // three formats:
                // binary, decimal, hexadecimal
                if (Text.StartsWith("0b") || Text.StartsWith("0B"))
                {
                    return Convert.ToUInt32(Text.Substring(2), 2);
                }
                else if (Text.StartsWith("0x") || Text.StartsWith("0X"))
                {
                    return Convert.ToUInt32(Text.Substring(2), 16);
                }
                else
                {
                    return Convert.ToUInt32(Text, 10);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
