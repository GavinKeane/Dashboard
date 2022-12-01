using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WIE_Dashboard_NEW.Properties;

namespace WIE_Dashboard_NEW
#pragma warning disable CS8601
#pragma warning disable CS8604
{
    public partial class Form3 : Form
    {
        private readonly Form1 _form1;

        // CONSTRUCTOR HAS A Form1 OBJECT TO BE ABLE TO USE THE PUBLIC MEMBERS
        public Form3(Form1 form1) {
            _form1 = form1;
            InitializeComponent();
        }

        private void Form3_Shown(object sender, EventArgs e) {

            // POPULATING ALL OF THE LISTBOXES FROM THE DATA CONTAINED IN ALL OF THE PUBLIC
            // MEMBERS OF _form1.
            for (int i = 0; i < _form1.officesIncluded.Count; i++) {
                listBox1.Items.Add(_form1.officesIncluded[i]);
            }
            for (int i = 0; i < _form1.offices.Count; i++) {
                if (!_form1.officesIncluded.Contains(_form1.offices[i])) {
                    listBox2.Items.Add(_form1.offices[i]);
                }
            }
            foreach (KeyValuePair<String, Dictionary<String, Dictionary<String, String>>> k in _form1.history) {
                listBox3.Items.Add(k.Key);
            }

            // LOADING SCORE SETTINGS SECTION
            String treeText = Properties.Settings.Default.treeText;
            if (treeText.Length > 0) {
                String[] parents = treeText.Split(';');
                for (int i = 0; i < parents.Length - 1; i++) {
                    String[] indiv = parents[i].Split(',');
                    for (int j = 0; j < indiv.Length; j++) {
                        if (j == 0) {
                            treeView1.Nodes.Add(indiv[0]);
                        } else {
                            treeView1.Nodes[i].Nodes.Add(indiv[j]);
                        }
                    }
                }
            }

            // JUST TO BE SURE IDK
            listBox3.Sorted = true;

            // SAME DEAL EXCEPT JUST A CheckBox
            checkBox1.Checked = _form1.record;
        }

        private void Button1_Click(object sender, EventArgs e) {

            // REMOVE BUTTON (RIGHT)
            try {
                _form1.officesIncluded.Remove(listBox1.SelectedItem.ToString());
                listBox2.Items.Add(listBox1.SelectedItem);
                listBox1.Items.Remove(listBox1.SelectedItem);
            } catch { }
        }

        private void Button2_Click(object sender, EventArgs e) {

            // ADD BUTTON (LEFT)
            try {
                _form1.officesIncluded.Add(listBox2.SelectedItem.ToString());
                listBox1.Items.Add(listBox2.SelectedItem);
                listBox2.Items.Remove(listBox2.SelectedItem);
            } catch { }
        }

        private void Button3_Click(object sender, EventArgs e) {
            String treeText = "";
            for (int i = 0; i < treeView1.Nodes.Count; i++) {
                treeText += treeView1.Nodes[i].Text;
                for (int j = 0; j < treeView1.Nodes[i].Nodes.Count; j++) {
                    treeText += "," + treeView1.Nodes[i].Nodes[j].Text;
                }
                treeText += ";";
            }
            Properties.Settings.Default.treeText = treeText;
            Properties.Settings.Default.Save();
            this.Hide();
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e) {
            _form1.record = checkBox1.Checked;
        }

        private void ListBox3_SelectedIndexChanged(object sender, EventArgs e) {
            try { _form1.selection = listBox3.SelectedItem.ToString(); } catch { }
        }

        private void Button4_Click(object sender, EventArgs e) {
            try {
                var result = MessageBox.Show("Are you sure you want to delete history of: " + listBox3.SelectedItem + "? This cannot be undone after the next report is generated.", "Confirm", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes) {
                    _form1.history.Remove(listBox3.SelectedItem.ToString());
                    listBox3.Items.Remove(listBox3.SelectedItem);
                }
                try {
                    listBox3.SelectedIndex = 0;
                    _form1.selection = listBox3.SelectedItem.ToString();
                } catch { }
            } catch { }
        }

        private void Button5_Click(object sender, EventArgs e) {
            treeView1.Nodes.Add(textBox1.Text);
        }

        private void Button6_Click(object sender, EventArgs e) {
            if (treeView1.SelectedNode == null) {
                label6.Text = "Select a root node";
                timer1.Start();
            } else {
                if (treeView1.SelectedNode.Parent == null) {
                    treeView1.SelectedNode.Nodes.Add(textBox2.Text);
                } else {
                    label6.Text = "Select a root node";
                    timer1.Start();
                }
            }
        }

        private void Timer1_Tick(object sender, EventArgs e) {
            label6.Text = "";
            timer1.Stop();
        }


        private void Button7_Click(object sender, EventArgs e) {
            try {
                treeView1.Nodes.Remove(treeView1.SelectedNode);
            } catch {
                MessageBox.Show("Nothing selected");
            }
        }

        private void Form3_Load(object sender, EventArgs e) {

        }
    }
}
