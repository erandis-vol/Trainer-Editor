namespace HTE
{
    partial class FreeSpaceFinderDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FreeSpaceFinderDialog));
            this.bOK = new System.Windows.Forms.Button();
            this.bCancel = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.listResults = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.cFSByte = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSearchStart = new HTE.NumericTextBox();
            this.txtNeeded = new HTE.NumericTextBox();
            this.SuspendLayout();
            // 
            // bOK
            // 
            this.bOK.Location = new System.Drawing.Point(53, 282);
            this.bOK.Name = "bOK";
            this.bOK.Size = new System.Drawing.Size(75, 23);
            this.bOK.TabIndex = 20;
            this.bOK.Text = "OK";
            this.bOK.UseVisualStyleBackColor = true;
            this.bOK.Click += new System.EventHandler(this.bOK_Click);
            // 
            // bCancel
            // 
            this.bCancel.Location = new System.Drawing.Point(134, 282);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(75, 23);
            this.bCancel.TabIndex = 19;
            this.bCancel.Text = "Cancel";
            this.bCancel.UseVisualStyleBackColor = true;
            this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel2.Location = new System.Drawing.Point(12, 275);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(197, 1);
            this.panel2.TabIndex = 18;
            // 
            // listResults
            // 
            this.listResults.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listResults.FormattingEnabled = true;
            this.listResults.Location = new System.Drawing.Point(12, 96);
            this.listResults.Name = "listResults";
            this.listResults.Size = new System.Drawing.Size(197, 173);
            this.listResults.TabIndex = 17;
            this.listResults.SelectedIndexChanged += new System.EventHandler(this.listResults_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Search From:";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel1.Location = new System.Drawing.Point(12, 36);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(197, 1);
            this.panel1.TabIndex = 15;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Needed Bytes:";
            // 
            // cFSByte
            // 
            this.cFSByte.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cFSByte.FormattingEnabled = true;
            this.cFSByte.Items.AddRange(new object[] {
            "00",
            "FF"});
            this.cFSByte.Location = new System.Drawing.Point(107, 43);
            this.cFSByte.Name = "cFSByte";
            this.cFSByte.Size = new System.Drawing.Size(41, 21);
            this.cFSByte.TabIndex = 13;
            this.cFSByte.SelectedIndexChanged += new System.EventHandler(this.cFSByte_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Free Space Byte:";
            // 
            // txtSearchStart
            // 
            this.txtSearchStart.Location = new System.Drawing.Point(107, 70);
            this.txtSearchStart.MaxValue = ((uint)(4294967294u));
            this.txtSearchStart.Name = "txtSearchStart";
            this.txtSearchStart.NumberStyle = HTE.NumericTextBox.NumberStyles.Hexadecimal;
            this.txtSearchStart.Size = new System.Drawing.Size(102, 20);
            this.txtSearchStart.TabIndex = 22;
            this.txtSearchStart.Text = "0x6B0000";
            this.txtSearchStart.Value = ((uint)(7012352u));
            this.txtSearchStart.TextChanged += new System.EventHandler(this.txtSearchStart_TextChanged);
            // 
            // txtNeeded
            // 
            this.txtNeeded.Location = new System.Drawing.Point(107, 10);
            this.txtNeeded.MaxValue = ((uint)(4294967294u));
            this.txtNeeded.Name = "txtNeeded";
            this.txtNeeded.Size = new System.Drawing.Size(102, 20);
            this.txtNeeded.TabIndex = 21;
            this.txtNeeded.Text = "0";
            this.txtNeeded.Value = ((uint)(0u));
            this.txtNeeded.TextChanged += new System.EventHandler(this.txtNeeded_TextChanged);
            // 
            // FreeSpaceFinderDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(221, 319);
            this.Controls.Add(this.txtSearchStart);
            this.Controls.Add(this.txtNeeded);
            this.Controls.Add(this.bOK);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.listResults);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cFSByte);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FreeSpaceFinderDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Find Free Space";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FreeSpaceFinderDialog_FormClosed);
            this.Load += new System.EventHandler(this.FreeSpaceFinderDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bOK;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ListBox listResults;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cFSByte;
        private System.Windows.Forms.Label label1;
        private HTE.NumericTextBox txtNeeded;
        private HTE.NumericTextBox txtSearchStart;
    }
}