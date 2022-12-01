using iTextSharp.text.pdf;
using PdfSharp.Pdf.IO;
using WIE_Dashboard_NEW.Properties;

namespace WIE_Dashboard_NEW {
#pragma warning disable CS8600
#pragma warning disable CS8602
    public partial class Form2 : Form {

        // FILE PATHS FOR PO PAGES.
        private List<String> poPages = new();

        // THIS KEEPS THE VALUES OF ALL THE RADIO BUTTONS FOR ALL THE DOCUMENTS
        // A BIT CONVOLUDED BUT WE SHALL SEE
        private Dictionary<String, Dictionary<String, String>> radioValues = new();

        // USED FOR ENABLING migrate BUTTON
        bool targetSelected = false;
        bool templateSelected = false;

        public Form2() {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e) {
            folderBrowserDialog1.SelectedPath = Settings.Default.target;
            openFileDialog1.FileName = Properties.Settings.Default.template;
        }

        private void Folder_Click(object sender, EventArgs e) {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK) {
                string folderName = folderBrowserDialog1.SelectedPath;
                Properties.Settings.Default.target = folderName;
                Properties.Settings.Default.Save();

                targetSelected = true;
                CollectFiles();
            } else {
                targetSelected = false;
            }
            CheckIfMigrateEnable();
        }

        private void Template_Click(object sender, EventArgs e) {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK) {
                string template = openFileDialog1.FileName;
                Properties.Settings.Default.template = template;
                Properties.Settings.Default.Save();

                templateSelected = true;
            } else {
                templateSelected = false;
            }
            CheckIfMigrateEnable();
        }

        private void CheckIfMigrateEnable() {
            migrate.Enabled = templateSelected && targetSelected;
        }

        private void CollectFiles() {

            // CLEAR IN CASE A FOLDER IS SELECTED TWICE
            listBox1.Items.Clear();
            poPages.Clear();

            // POPULATES THE LISTBOX WITH PDFS WITH NAMES ONLY.
            // POPULATES GLOBAL VARIABLE poPages WITH FULL PATHS.
            String[] files = Directory.GetFiles(Properties.Settings.Default.target, "*.pdf");
            for (int i = 0; i < files.Length; i++) {
                String name = files[i].Split('\\')[^1];
                if (!(name.Equals("Cover.pdf") || name.Equals("Dashboard.pdf") || name.Equals("Snapshot.pdf") || name.Equals("Cover1.pdf") || name.Equals("tmp.pdf") || name.Equals("Snapshot.pdf") || name.Equals("Snapshot1.pdf"))) {
                    listBox1.Items.Add(name);
                    poPages.Add(files[i]);
                }
            }

            // SETTING progess MAXIMUM VALUE (THIS IS A GOOD COMMENT)
            progress.Maximum = poPages.Count;
        }

        private void Migrate_Click(object sender, EventArgs e) {
            progress.Value = 0;

            String target = Properties.Settings.Default.target;
            String template = Properties.Settings.Default.template;

            // CREATES A FOLDER AT THE ROOT OF THE TARGET FOLDER CALLED Migration Results
            bool needToCreateFolder = !Directory.Exists(target + "\\Migration Results");
            if (needToCreateFolder) {
                Directory.CreateDirectory(target + "\\Migration Results");
            }

            // USING PdfSharp TO BUILD UP A DICTIONARY OF DICTIONARIES
            // THAT CONTAINS < FILE NAME, < FIELD NAME, VALUE > >
            // THIS for BLOCK IS USED TO CAPTURE THE VALUES OF ALL OF THE RADIO
            // BUTTON VALUES IN THE WHOLE STACK OF poPages
            for (int i = 0; i < poPages.Count; i++) {
                PdfSharp.Pdf.PdfDocument pdf = PdfSharp.Pdf.IO.PdfReader.Open(poPages[i], PdfDocumentOpenMode.ReadOnly);
                Dictionary<String, String> radios = new();
                for (int j = 0; j < pdf.AcroForm.Fields.Count; j++) {
                    String s = "";

                    // TRY CATCH BLOCK IN CASE THE VALUE OF THE FIELD IS NULL
                    try {
                        s = pdf.AcroForm.Fields[j].Value.ToString();
                    } catch { }

                    // CRUDE WAY OF DETERMINING IF THE FIELD IS A RADIO BUTTON.
                    // I UNDERSTAND THAT THIS COULD CAUSE A REALLY RARE BUG.
                    if (s.Equals("/0") || s.Equals("/1") || s.Equals("/2")) {

                        // TRY CATCH BLOCK HERE BECAUSE THERE ARE MULTIPLE RADIO BUTTONS WITH THE SAME
                        // NAME. NATURALLY, THIS VIOLATES THE Dictionary REQUIREMENT. I THINK.
                        try {
                            radios.Add(pdf.AcroForm.Fields[j].Name, s);
                        } catch { }
                    }
                }
                radioValues.Add(poPages[i], radios);
            }

            //iTextSharp PORTION
            for (int i = 0; i < poPages.Count; i++) {

                // poPages ELEMENT i WITHOUT FULL PATH. ONLY FILE NAME & EXTENSION
                String name = poPages[i].Split('\\')[^1];

                // NAMING NEW FILE TEMPLATE IN THE Migartion Results FOLDER
                String templateCopy = target + "\\Migration Results\\" + name;

                // CREATING READERS & WRITERS FOR poPages[i] & templateCopy
                // THESE ARE THE iTextSharp VERSIONS OF THESE OBJECTS
                iTextSharp.text.pdf.PdfReader targetReader = new(poPages[i]);
                iTextSharp.text.pdf.PdfReader templateReader = new(template);
                PdfStamper stamper = new(templateReader, new FileStream(templateCopy, FileMode.Create));

                // LOOPING THROUGH EVERY ACROFORM FIELD IN BOTH poPages[i] & template AND
                // MATCHING VALUES OF FIELDS WITH IDENTICAL NAMES
                foreach (KeyValuePair<string, AcroFields.Item> j in targetReader.AcroFields.Fields) {
                    foreach (KeyValuePair<string, AcroFields.Item> k in templateReader.AcroFields.Fields) {
                        if (j.Key.Equals(k.Key) ||
                            (j.Key.Equals("tb1") && k.Key.Equals("ato")) ||
                            (j.Key.Equals("tb2") && k.Key.Equals("atc")) ||
                            (j.Key.Equals("tb3") && k.Key.Equals("iatt")) ||
                            (j.Key.Equals("tb4") && k.Key.Equals("poam")) ||
                            (j.Key.Equals("tb5") && k.Key.Equals("aar")) ||
                            (j.Key.Equals("tb20") && k.Key.Equals("sar")) ||
                            (j.Key.Equals("tb21") && k.Key.Equals("rar")) ||
                            (j.Key.Equals("tb6") && k.Key.Equals("ssp")) ||
                            (j.Key.Equals("tb7") && k.Key.Equals("opcon")) ||
                            (j.Key.Equals("tb8") && k.Key.Equals("cca")) ||
                            (j.Key.Equals("tb9") && k.Key.Equals("itips")) ||
                            (j.Key.Equals("tb11") && k.Key.Equals("css")) ||
                            (j.Key.Equals("tb15") && k.Key.Equals("mods")) ||
                            (j.Key.Equals("tb16") && k.Key.Equals("sales")) ||
                            (j.Key.Equals("tb17") && k.Key.Equals("action")) ||
                            (j.Key.Equals("tb18") && k.Key.Equals("issues")) ||
                            (j.Key.Equals("tb19") && k.Key.Equals("requests")) ||
                            (j.Key.Equals("tb14") && k.Key.Equals("activities")) ||

                            (j.Key.Equals("r1") && k.Key.Equals("atoColor")) ||
                            (j.Key.Equals("r2") && k.Key.Equals("atcColor")) ||
                            (j.Key.Equals("r3") && k.Key.Equals("iattColor")) ||
                            (j.Key.Equals("r4") && k.Key.Equals("poamColor")) ||
                            (j.Key.Equals("r5") && k.Key.Equals("aarColor")) ||
                            (j.Key.Equals("r19") && k.Key.Equals("sarColor")) ||
                            (j.Key.Equals("r20") && k.Key.Equals("rarColor")) ||
                            (j.Key.Equals("r6") && k.Key.Equals("sspColor")) ||
                            (j.Key.Equals("r7") && k.Key.Equals("opconColor")) ||
                            (j.Key.Equals("r9") && k.Key.Equals("itipsColor")) ||
                            (j.Key.Equals("r11") && k.Key.Equals("cssColor")) ||
                            (j.Key.Equals("r14") && k.Key.Equals("modsColor")) ||
                            (j.Key.Equals("r15") && k.Key.Equals("salesColor")) ||
                            (j.Key.Equals("r16") && k.Key.Equals("actionColor")) ||
                            (j.Key.Equals("r17") && k.Key.Equals("issuesColor") ||
                            (j.Key.Equals("r18") && k.Key.Equals("requestsColor"))))

                             {

                            // THIS HAS TO BE TRIED IN THE CASE THAT THE THE NEW FORM REMOVES A
                            // ONCE PRESENT FIELD
                            try {
                                stamper.AcroFields.SetField(k.Key, targetReader.AcroFields.GetField(j.Key));
                            } catch { }

                            // THIS PART IS TO COPY OVER THE COLOR-CHANGING BUTTONS' VALUES
                            try {
                                PushbuttonField buttonToCopyFrom = targetReader.AcroFields.GetNewPushbuttonFromField(j.Key);
                                PushbuttonField buttonToCopyTo = stamper.AcroFields.GetNewPushbuttonFromField(k.Key);
                                buttonToCopyTo.BackgroundColor = buttonToCopyFrom.BackgroundColor;
                                stamper.AcroFields.ReplacePushbuttonField(k.Key, buttonToCopyTo.Field);
                            } catch { }
                        }
                    }
                }
                progress.Value++;

                // CLOSING STREAMS
                targetReader.Close();
                stamper.Close();
                templateReader.Close();
            }
        }
    }
}