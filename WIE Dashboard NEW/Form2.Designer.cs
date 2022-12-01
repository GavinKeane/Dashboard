namespace WIE_Dashboard_NEW
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            this.folder = new System.Windows.Forms.Button();
            this.template = new System.Windows.Forms.Button();
            this.migrate = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.progress = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // folder
            // 
            this.folder.Location = new System.Drawing.Point(8, 12);
            this.folder.Name = "folder";
            this.folder.Size = new System.Drawing.Size(242, 40);
            this.folder.TabIndex = 0;
            this.folder.Text = "Select Forms to Migrate";
            this.folder.UseVisualStyleBackColor = true;
            this.folder.Click += new System.EventHandler(this.Folder_Click);
            // 
            // template
            // 
            this.template.Location = new System.Drawing.Point(8, 58);
            this.template.Name = "template";
            this.template.Size = new System.Drawing.Size(242, 40);
            this.template.TabIndex = 1;
            this.template.Text = "Select Template Form";
            this.template.UseVisualStyleBackColor = true;
            this.template.Click += new System.EventHandler(this.Template_Click);
            // 
            // migrate
            // 
            this.migrate.Enabled = false;
            this.migrate.Location = new System.Drawing.Point(8, 104);
            this.migrate.Name = "migrate";
            this.migrate.Size = new System.Drawing.Size(242, 40);
            this.migrate.TabIndex = 2;
            this.migrate.Text = "Migrate";
            this.migrate.UseVisualStyleBackColor = true;
            this.migrate.Click += new System.EventHandler(this.Migrate_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(8, 150);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(242, 290);
            this.listBox1.TabIndex = 3;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // progress
            // 
            this.progress.Location = new System.Drawing.Point(8, 446);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(242, 23);
            this.progress.TabIndex = 4;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(257, 476);
            this.Controls.Add(this.progress);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.migrate);
            this.Controls.Add(this.template);
            this.Controls.Add(this.folder);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form2";
            this.Text = "Migrate";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button folder;
        private System.Windows.Forms.Button template;
        private System.Windows.Forms.Button migrate;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ProgressBar progress;
    }
}