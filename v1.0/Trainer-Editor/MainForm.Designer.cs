namespace Lost
{
    partial class MainForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.listTrainers = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.grpTrainer = new System.Windows.Forms.GroupBox();
            this.grpSprite = new System.Windows.Forms.GroupBox();
            this.nSprite = new System.Windows.Forms.NumericUpDown();
            this.pSprite = new System.Windows.Forms.PictureBox();
            this.rFemale = new System.Windows.Forms.RadioButton();
            this.rMale = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grpClass = new System.Windows.Forms.GroupBox();
            this.bClassEdit = new System.Windows.Forms.Button();
            this.txtClass = new System.Windows.Forms.TextBox();
            this.cClass = new System.Windows.Forms.ComboBox();
            this.grpItems = new System.Windows.Forms.GroupBox();
            this.cItem1 = new System.Windows.Forms.ComboBox();
            this.cItem2 = new System.Windows.Forms.ComboBox();
            this.cItem4 = new System.Windows.Forms.ComboBox();
            this.cItem3 = new System.Windows.Forms.ComboBox();
            this.grpParty = new System.Windows.Forms.GroupBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cSpecies = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cHeld = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.comboBox4 = new System.Windows.Forms.ComboBox();
            this.comboBox5 = new System.Windows.Forms.ComboBox();
            this.p1 = new System.Windows.Forms.PictureBox();
            this.p2 = new System.Windows.Forms.PictureBox();
            this.p3 = new System.Windows.Forms.PictureBox();
            this.p4 = new System.Windows.Forms.PictureBox();
            this.p5 = new System.Windows.Forms.PictureBox();
            this.p6 = new System.Windows.Forms.PictureBox();
            this.menuStrip1.SuspendLayout();
            this.grpTrainer.SuspendLayout();
            this.grpSprite.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nSprite)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pSprite)).BeginInit();
            this.grpClass.SuspendLayout();
            this.grpItems.SuspendLayout();
            this.grpParty.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.p1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.p2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.p3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.p4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.p5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.p6)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(608, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = global::Lost.Properties.Resources.OpenFolder_16x;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // listTrainers
            // 
            this.listTrainers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listTrainers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listTrainers.FullRowSelect = true;
            this.listTrainers.GridLines = true;
            this.listTrainers.Location = new System.Drawing.Point(12, 27);
            this.listTrainers.MultiSelect = false;
            this.listTrainers.Name = "listTrainers";
            this.listTrainers.Size = new System.Drawing.Size(141, 608);
            this.listTrainers.TabIndex = 1;
            this.listTrainers.UseCompatibleStateImageBehavior = false;
            this.listTrainers.View = System.Windows.Forms.View.Details;
            this.listTrainers.SelectedIndexChanged += new System.EventHandler(this.listTrainers_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "#";
            this.columnHeader1.Width = 32;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Trainer";
            this.columnHeader2.Width = 80;
            // 
            // grpTrainer
            // 
            this.grpTrainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpTrainer.Controls.Add(this.grpParty);
            this.grpTrainer.Controls.Add(this.grpItems);
            this.grpTrainer.Controls.Add(this.grpSprite);
            this.grpTrainer.Controls.Add(this.rFemale);
            this.grpTrainer.Controls.Add(this.rMale);
            this.grpTrainer.Controls.Add(this.label2);
            this.grpTrainer.Controls.Add(this.txtName);
            this.grpTrainer.Controls.Add(this.label1);
            this.grpTrainer.Controls.Add(this.grpClass);
            this.grpTrainer.Location = new System.Drawing.Point(159, 27);
            this.grpTrainer.Name = "grpTrainer";
            this.grpTrainer.Size = new System.Drawing.Size(437, 608);
            this.grpTrainer.TabIndex = 2;
            this.grpTrainer.TabStop = false;
            this.grpTrainer.Text = "Trainer";
            // 
            // grpSprite
            // 
            this.grpSprite.Controls.Add(this.nSprite);
            this.grpSprite.Controls.Add(this.pSprite);
            this.grpSprite.Location = new System.Drawing.Point(6, 58);
            this.grpSprite.Name = "grpSprite";
            this.grpSprite.Size = new System.Drawing.Size(76, 115);
            this.grpSprite.TabIndex = 6;
            this.grpSprite.TabStop = false;
            this.grpSprite.Text = "Sprite";
            // 
            // nSprite
            // 
            this.nSprite.Location = new System.Drawing.Point(6, 89);
            this.nSprite.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.nSprite.Name = "nSprite";
            this.nSprite.Size = new System.Drawing.Size(64, 20);
            this.nSprite.TabIndex = 1;
            // 
            // pSprite
            // 
            this.pSprite.Location = new System.Drawing.Point(6, 19);
            this.pSprite.Name = "pSprite";
            this.pSprite.Size = new System.Drawing.Size(64, 64);
            this.pSprite.TabIndex = 0;
            this.pSprite.TabStop = false;
            // 
            // rFemale
            // 
            this.rFemale.AutoSize = true;
            this.rFemale.ForeColor = System.Drawing.Color.HotPink;
            this.rFemale.Location = new System.Drawing.Point(215, 33);
            this.rFemale.Name = "rFemale";
            this.rFemale.Size = new System.Drawing.Size(59, 17);
            this.rFemale.TabIndex = 5;
            this.rFemale.Text = "Female";
            this.rFemale.UseVisualStyleBackColor = true;
            // 
            // rMale
            // 
            this.rMale.AutoSize = true;
            this.rMale.Checked = true;
            this.rMale.ForeColor = System.Drawing.Color.RoyalBlue;
            this.rMale.Location = new System.Drawing.Point(161, 33);
            this.rMale.Name = "rMale";
            this.rMale.Size = new System.Drawing.Size(48, 17);
            this.rMale.TabIndex = 4;
            this.rMale.TabStop = true;
            this.rMale.Text = "Male";
            this.rMale.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(158, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Gender:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(6, 32);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(146, 20);
            this.txtName.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Name:";
            // 
            // grpClass
            // 
            this.grpClass.Controls.Add(this.bClassEdit);
            this.grpClass.Controls.Add(this.txtClass);
            this.grpClass.Controls.Add(this.cClass);
            this.grpClass.Location = new System.Drawing.Point(231, 56);
            this.grpClass.Name = "grpClass";
            this.grpClass.Size = new System.Drawing.Size(200, 72);
            this.grpClass.TabIndex = 0;
            this.grpClass.TabStop = false;
            this.grpClass.Text = "Class";
            // 
            // bClassEdit
            // 
            this.bClassEdit.Image = global::Lost.Properties.Resources.Edit_16x;
            this.bClassEdit.Location = new System.Drawing.Point(145, 44);
            this.bClassEdit.Name = "bClassEdit";
            this.bClassEdit.Size = new System.Drawing.Size(49, 23);
            this.bClassEdit.TabIndex = 2;
            this.bClassEdit.Text = "Edit";
            this.bClassEdit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.bClassEdit.UseVisualStyleBackColor = true;
            // 
            // txtClass
            // 
            this.txtClass.Location = new System.Drawing.Point(6, 46);
            this.txtClass.Name = "txtClass";
            this.txtClass.Size = new System.Drawing.Size(133, 20);
            this.txtClass.TabIndex = 1;
            // 
            // cClass
            // 
            this.cClass.FormattingEnabled = true;
            this.cClass.Location = new System.Drawing.Point(6, 19);
            this.cClass.Name = "cClass";
            this.cClass.Size = new System.Drawing.Size(133, 21);
            this.cClass.TabIndex = 0;
            // 
            // grpItems
            // 
            this.grpItems.Controls.Add(this.cItem4);
            this.grpItems.Controls.Add(this.cItem3);
            this.grpItems.Controls.Add(this.cItem2);
            this.grpItems.Controls.Add(this.cItem1);
            this.grpItems.Location = new System.Drawing.Point(40, 179);
            this.grpItems.Name = "grpItems";
            this.grpItems.Size = new System.Drawing.Size(354, 73);
            this.grpItems.TabIndex = 7;
            this.grpItems.TabStop = false;
            this.grpItems.Text = "Items";
            // 
            // cItem1
            // 
            this.cItem1.FormattingEnabled = true;
            this.cItem1.Location = new System.Drawing.Point(6, 19);
            this.cItem1.Name = "cItem1";
            this.cItem1.Size = new System.Drawing.Size(168, 21);
            this.cItem1.TabIndex = 0;
            // 
            // cItem2
            // 
            this.cItem2.FormattingEnabled = true;
            this.cItem2.Location = new System.Drawing.Point(180, 19);
            this.cItem2.Name = "cItem2";
            this.cItem2.Size = new System.Drawing.Size(168, 21);
            this.cItem2.TabIndex = 1;
            // 
            // cItem4
            // 
            this.cItem4.FormattingEnabled = true;
            this.cItem4.Location = new System.Drawing.Point(180, 46);
            this.cItem4.Name = "cItem4";
            this.cItem4.Size = new System.Drawing.Size(168, 21);
            this.cItem4.TabIndex = 3;
            // 
            // cItem3
            // 
            this.cItem3.FormattingEnabled = true;
            this.cItem3.Location = new System.Drawing.Point(6, 46);
            this.cItem3.Name = "cItem3";
            this.cItem3.Size = new System.Drawing.Size(168, 21);
            this.cItem3.TabIndex = 2;
            // 
            // grpParty
            // 
            this.grpParty.Controls.Add(this.p6);
            this.grpParty.Controls.Add(this.p5);
            this.grpParty.Controls.Add(this.p4);
            this.grpParty.Controls.Add(this.p3);
            this.grpParty.Controls.Add(this.p2);
            this.grpParty.Controls.Add(this.p1);
            this.grpParty.Controls.Add(this.comboBox5);
            this.grpParty.Controls.Add(this.comboBox4);
            this.grpParty.Controls.Add(this.comboBox3);
            this.grpParty.Controls.Add(this.label7);
            this.grpParty.Controls.Add(this.comboBox2);
            this.grpParty.Controls.Add(this.label6);
            this.grpParty.Controls.Add(this.cHeld);
            this.grpParty.Controls.Add(this.textBox2);
            this.grpParty.Controls.Add(this.label5);
            this.grpParty.Controls.Add(this.textBox1);
            this.grpParty.Controls.Add(this.label4);
            this.grpParty.Controls.Add(this.label3);
            this.grpParty.Controls.Add(this.cSpecies);
            this.grpParty.Controls.Add(this.listView1);
            this.grpParty.Location = new System.Drawing.Point(6, 258);
            this.grpParty.Name = "grpParty";
            this.grpParty.Size = new System.Drawing.Size(425, 329);
            this.grpParty.TabIndex = 8;
            this.grpParty.TabStop = false;
            this.grpParty.Text = "Party";
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(6, 86);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(240, 237);
            this.listView1.TabIndex = 2;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "#";
            this.columnHeader3.Width = 24;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Species";
            this.columnHeader4.Width = 80;
            // 
            // cSpecies
            // 
            this.cSpecies.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cSpecies.FormattingEnabled = true;
            this.cSpecies.Location = new System.Drawing.Point(251, 102);
            this.cSpecies.Name = "cSpecies";
            this.cSpecies.Size = new System.Drawing.Size(168, 21);
            this.cSpecies.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(248, 86);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Species:";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(248, 126);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Level:";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(251, 142);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(47, 20);
            this.textBox1.TabIndex = 6;
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox2.Location = new System.Drawing.Point(304, 142);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(47, 20);
            this.textBox2.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(301, 126);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "EVs:";
            // 
            // cHeld
            // 
            this.cHeld.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cHeld.FormattingEnabled = true;
            this.cHeld.Location = new System.Drawing.Point(251, 181);
            this.cHeld.Name = "cHeld";
            this.cHeld.Size = new System.Drawing.Size(168, 21);
            this.cHeld.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(248, 165);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Held Item:";
            // 
            // comboBox2
            // 
            this.comboBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(251, 221);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(168, 21);
            this.comboBox2.TabIndex = 11;
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(248, 205);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(46, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Attacks:";
            // 
            // comboBox3
            // 
            this.comboBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Location = new System.Drawing.Point(251, 248);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(168, 21);
            this.comboBox3.TabIndex = 13;
            // 
            // comboBox4
            // 
            this.comboBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox4.FormattingEnabled = true;
            this.comboBox4.Location = new System.Drawing.Point(251, 275);
            this.comboBox4.Name = "comboBox4";
            this.comboBox4.Size = new System.Drawing.Size(168, 21);
            this.comboBox4.TabIndex = 14;
            // 
            // comboBox5
            // 
            this.comboBox5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox5.FormattingEnabled = true;
            this.comboBox5.Location = new System.Drawing.Point(251, 302);
            this.comboBox5.Name = "comboBox5";
            this.comboBox5.Size = new System.Drawing.Size(168, 21);
            this.comboBox5.TabIndex = 15;
            // 
            // p1
            // 
            this.p1.Location = new System.Drawing.Point(6, 19);
            this.p1.Name = "p1";
            this.p1.Size = new System.Drawing.Size(64, 64);
            this.p1.TabIndex = 16;
            this.p1.TabStop = false;
            // 
            // p2
            // 
            this.p2.Location = new System.Drawing.Point(76, 19);
            this.p2.Name = "p2";
            this.p2.Size = new System.Drawing.Size(64, 64);
            this.p2.TabIndex = 17;
            this.p2.TabStop = false;
            // 
            // p3
            // 
            this.p3.Location = new System.Drawing.Point(146, 19);
            this.p3.Name = "p3";
            this.p3.Size = new System.Drawing.Size(64, 64);
            this.p3.TabIndex = 18;
            this.p3.TabStop = false;
            // 
            // p4
            // 
            this.p4.Location = new System.Drawing.Point(216, 19);
            this.p4.Name = "p4";
            this.p4.Size = new System.Drawing.Size(64, 64);
            this.p4.TabIndex = 19;
            this.p4.TabStop = false;
            // 
            // p5
            // 
            this.p5.Location = new System.Drawing.Point(286, 19);
            this.p5.Name = "p5";
            this.p5.Size = new System.Drawing.Size(64, 64);
            this.p5.TabIndex = 20;
            this.p5.TabStop = false;
            // 
            // p6
            // 
            this.p6.Location = new System.Drawing.Point(356, 19);
            this.p6.Name = "p6";
            this.p6.Size = new System.Drawing.Size(64, 64);
            this.p6.TabIndex = 21;
            this.p6.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(608, 647);
            this.Controls.Add(this.grpTrainer);
            this.Controls.Add(this.listTrainers);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Trainer Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.grpTrainer.ResumeLayout(false);
            this.grpTrainer.PerformLayout();
            this.grpSprite.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nSprite)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pSprite)).EndInit();
            this.grpClass.ResumeLayout(false);
            this.grpClass.PerformLayout();
            this.grpItems.ResumeLayout(false);
            this.grpParty.ResumeLayout(false);
            this.grpParty.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.p1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.p2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.p3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.p4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.p5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.p6)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ListView listTrainers;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.GroupBox grpTrainer;
        private System.Windows.Forms.GroupBox grpClass;
        private System.Windows.Forms.ComboBox cClass;
        private System.Windows.Forms.TextBox txtClass;
        private System.Windows.Forms.Button bClassEdit;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton rFemale;
        private System.Windows.Forms.RadioButton rMale;
        private System.Windows.Forms.GroupBox grpSprite;
        private System.Windows.Forms.PictureBox pSprite;
        private System.Windows.Forms.NumericUpDown nSprite;
        private System.Windows.Forms.GroupBox grpItems;
        private System.Windows.Forms.ComboBox cItem2;
        private System.Windows.Forms.ComboBox cItem1;
        private System.Windows.Forms.ComboBox cItem4;
        private System.Windows.Forms.ComboBox cItem3;
        private System.Windows.Forms.GroupBox grpParty;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ComboBox cSpecies;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cHeld;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBox5;
        private System.Windows.Forms.ComboBox comboBox4;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.PictureBox p6;
        private System.Windows.Forms.PictureBox p5;
        private System.Windows.Forms.PictureBox p4;
        private System.Windows.Forms.PictureBox p3;
        private System.Windows.Forms.PictureBox p2;
        private System.Windows.Forms.PictureBox p1;
    }
}

