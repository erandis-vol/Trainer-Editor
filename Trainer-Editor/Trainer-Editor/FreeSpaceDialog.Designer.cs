namespace Lost
{
    partial class FreeSpaceDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtNeeded = new Lost.DecimalBox();
            this.txtSearchStart = new Lost.HexBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.listResults = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Search Start:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Needed Bytes:";
            // 
            // txtNeeded
            // 
            this.txtNeeded.Location = new System.Drawing.Point(95, 12);
            this.txtNeeded.MaximumValue = 33554431;
            this.txtNeeded.MinimumValue = 0;
            this.txtNeeded.Name = "txtNeeded";
            this.txtNeeded.Size = new System.Drawing.Size(114, 20);
            this.txtNeeded.TabIndex = 2;
            this.txtNeeded.Value = 0;
            // 
            // txtSearchStart
            // 
            this.txtSearchStart.Location = new System.Drawing.Point(110, 38);
            this.txtSearchStart.MaximumValue = 33554431;
            this.txtSearchStart.MinimumValue = 0;
            this.txtSearchStart.Name = "txtSearchStart";
            this.txtSearchStart.Size = new System.Drawing.Size(99, 20);
            this.txtSearchStart.TabIndex = 3;
            this.txtSearchStart.Value = 0;
            this.txtSearchStart.TextChanged += new System.EventHandler(this.txtSearchStart_TextChanged);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(95, 38);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(17, 20);
            this.textBox1.TabIndex = 4;
            this.textBox1.Text = "0x";
            // 
            // listResults
            // 
            this.listResults.FormattingEnabled = true;
            this.listResults.Location = new System.Drawing.Point(12, 77);
            this.listResults.Name = "listResults";
            this.listResults.Size = new System.Drawing.Size(197, 147);
            this.listResults.TabIndex = 5;
            this.listResults.SelectedIndexChanged += new System.EventHandler(this.listResults_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Results:";
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(53, 230);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "Repoint";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(134, 230);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 8;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // FreeSpaceDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(221, 265);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.listResults);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.txtSearchStart);
            this.Controls.Add(this.txtNeeded);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FreeSpaceDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Find Free Space";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private DecimalBox txtNeeded;
        private HexBox txtSearchStart;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox listResults;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}