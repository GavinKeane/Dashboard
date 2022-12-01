namespace WIE_Dashboard_NEW
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.selectFolder = new System.Windows.Forms.Button();
            this.folderSelector = new System.Windows.Forms.FolderBrowserDialog();
            this.generate = new System.Windows.Forms.Button();
            this.progress = new System.Windows.Forms.ProgressBar();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.migrate = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.contact = new System.Windows.Forms.Button();
            this.options = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // selectFolder
            // 
            this.selectFolder.Location = new System.Drawing.Point(8, 12);
            this.selectFolder.Name = "selectFolder";
            this.selectFolder.Size = new System.Drawing.Size(242, 40);
            this.selectFolder.TabIndex = 0;
            this.selectFolder.Text = "Select Folder";
            this.selectFolder.UseVisualStyleBackColor = true;
            this.selectFolder.Click += new System.EventHandler(this.SelectFolder_Click);
            // 
            // generate
            // 
            this.generate.Enabled = false;
            this.generate.Location = new System.Drawing.Point(8, 58);
            this.generate.Name = "generate";
            this.generate.Size = new System.Drawing.Size(192, 40);
            this.generate.TabIndex = 1;
            this.generate.Text = "Generate Dashboard";
            this.generate.UseVisualStyleBackColor = true;
            this.generate.Click += new System.EventHandler(this.Generate_Click);
            // 
            // progress
            // 
            this.progress.Location = new System.Drawing.Point(8, 400);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(242, 23);
            this.progress.TabIndex = 2;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(8, 104);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(242, 290);
            this.listBox1.TabIndex = 3;
            // 
            // migrate
            // 
            this.migrate.Location = new System.Drawing.Point(5, 23);
            this.migrate.Name = "migrate";
            this.migrate.Size = new System.Drawing.Size(231, 25);
            this.migrate.TabIndex = 4;
            this.migrate.Text = "Migrate Form";
            this.migrate.UseVisualStyleBackColor = true;
            this.migrate.Click += new System.EventHandler(this.Migrate_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Other tools";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.contact);
            this.panel1.Controls.Add(this.migrate);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(8, 429);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(242, 89);
            this.panel1.TabIndex = 6;
            // 
            // contact
            // 
            this.contact.Location = new System.Drawing.Point(5, 54);
            this.contact.Name = "contact";
            this.contact.Size = new System.Drawing.Size(231, 25);
            this.contact.TabIndex = 6;
            this.contact.Text = "Contact";
            this.contact.UseVisualStyleBackColor = true;
            this.contact.Click += new System.EventHandler(this.Contact_Click);
            // 
            // options
            // 
            this.options.Enabled = false;
            this.options.Image = ((System.Drawing.Image)(resources.GetObject("options.Image")));
            this.options.Location = new System.Drawing.Point(206, 58);
            this.options.Name = "options";
            this.options.Size = new System.Drawing.Size(44, 40);
            this.options.TabIndex = 7;
            this.options.UseVisualStyleBackColor = true;
            this.options.Click += new System.EventHandler(this.Button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(257, 524);
            this.Controls.Add(this.options);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.progress);
            this.Controls.Add(this.generate);
            this.Controls.Add(this.selectFolder);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Dashboard";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button selectFolder;
        private System.Windows.Forms.FolderBrowserDialog folderSelector;
        private System.Windows.Forms.ProgressBar progress;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button migrate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button contact;
        private System.Windows.Forms.Button options;
        public System.Windows.Forms.Button generate;
    }
}

