using iTextSharp.text.pdf;
using PdfSharp.Drawing;
using PdfSharp.Pdf.IO;
using System.Text.Json;
using WIE_Dashboard_NEW.Properties;


namespace WIE_Dashboard_NEW
{
#pragma warning disable IDE0044
#pragma warning disable CS8600
#pragma warning disable CS8601
#pragma warning disable CS8602
    public partial class Form1 : Form
    {
        // THIS IS FOR COMPARISON FROM DASHBOARD TO DASHBOARD. THE COGNITIVE LOAD OF THIS STRUCTURE
        // IS MASSIVE TO BE HONEST. IT IS STRUCTURED AS FOLLOWS:
        // < "d MMMM yy\, h:mm:sst" ,
        //     < poPages[i] ,
        //         < Field Name , Field Value > > >
        public String entry = "";
        public Dictionary<String, Dictionary<String, Dictionary<String, String>>> history = new();
        public bool headerSet = false;
        public bool record = true;
        public String selection = "";
        public List<String> warnings = new();


        // DISGUSTING
        public String CSV = "";

        // TRACKING FIELDS THAT HAVE CHANGED
        // < poPage FILE NAME ONLY, FIELD NAME > 
        public List<String[]> changes = new();

        // PUBLIC LIST OF ALL DIFFERENT OFFICE SYMBOLS FOR USE IN FORM 3
        public List<String> offices = new();
        public List<String> officesIncluded = new();

        // THIS IS FOR KEEPING TRACK OF THE SELECTIONS MADE IN FORM 3
        private List<String> poPagesBlacklist = new();

        // FILE PATHS FOR PO PAGES.
        private List<String> poPages = new();

        // THIS IS <FILE NAME, TIME THAT FILE WAS LAST MODIFIED> TIME IS FORMATTED AS: "d MMMM yy"
        private Dictionary<String, String> lastModifieds = new();

        // THESE ARE FOR THE SNAPSHOT. THESE THINGS ARE COLLECTED ALONG THE WAY.
        private Dictionary<String, String> fileProgram = new();
        private Dictionary<String, String> fileOffice = new();
        private Dictionary<String, double> fileScore = new();

        public Form1() {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e) {
            // LOADING APPLICATION LEVEL SETTINGS.
            folderSelector.SelectedPath = Settings.Default.folder;
            //Properties.Settings.Default.cover = Application.StartupPath + "\\cover.pdf";
            Settings.Default.cover = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Dashboard Files\\cover.pdf";
            //Properties.Settings.Default.snapshot = Application.StartupPath + "\\snapshot.pdf";
            Settings.Default.snapshot = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Dashboard Files\\snapshot.pdf";
            Settings.Default.Save();

            // LOAD history FROM history.txt
            string mydocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            history = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(json: File.ReadAllText(mydocs + "\\Dashboard Files\\history.txt"));
        }

        private void Generate_Click(object sender, EventArgs e) {
            generate.Enabled = false;
            options.Enabled = false;

            // FOR COMPARISONS
            if (record) {
                entry = DateTime.Now.ToString("d MMMM yy, h:mm:sst");
                history.Add(entry, new Dictionary<String, Dictionary<String, String>>());
            }

            // THE "MAIN METHOD".
            // THIS IS THE MAIN PIPELINE.
            progress.Value = 0;

            // ... DOING EVERYTHING
            DeleteOldFiles();
            GetLastModifieds();
            BackupOriginals();
            DropInColors();
            DateCover();

            // history[entry] IS FULLY FLESHED OUT HERE INSIDE DatePoPages 
            // FOR EFFICIENCY'S SAKE ALTHOUGH A LOT LESS CLEAN.
            // changes IS ALSO POPULATED HERE
            // PO PAGES ARE FLATTENED HERE AS WELL
            DatePoPages();

            // ... CONTINUE DOING EVERYTHING
            FillSnapshot();
            //ColorSnapshot();

            // MAKING THE DASHBOARD
            Concatenate();

            // PUTTING EVERYTHING BACK
            RestoreOriginals();
        }

        private void SelectFolder_Click(object sender, EventArgs e) {
            DialogResult result = folderSelector.ShowDialog();
            if (result == DialogResult.OK) {
                string folderName = folderSelector.SelectedPath;
                Settings.Default.folder = folderName;
                Settings.Default.Save();

                CollectFiles();

                options.Enabled = true;
                generate.Enabled = true;
            }
        }

        private void CollectFiles() {

            // CLEAR IN CASE A FOLDER IS SELECTED TWICE
            listBox1.Items.Clear();
            poPages.Clear();

            // POPULATES THE LISTBOX WITH PDFS WITH NAMES ONLY.
            // POPULATES GLOBAL VARIABLE poPages WITH FULL PATHS.
            String[] files = Directory.GetFiles(Settings.Default.folder, "*.pdf");
            for (int i = 0; i < files.Length; i++) {
                String name = files[i].Split('\\')[^1];
                if (!(name.Equals("Cover.pdf") || name.Equals("Dashboard.pdf") || name.Equals("Snapshot.pdf") || name.Equals("Cover1.pdf") || name.Equals("tmp.pdf") || name.Equals("Snapshot.pdf") || name.Equals("Snapshot1.pdf"))) {
                    listBox1.Items.Add(name);
                    poPages.Add(files[i]);
                }
            }

            // BEST TO GET THIS OUT OF THE WAY EARLY SO I CAN USE offices AND officesInluced
            // WHENEVER I WANT
            GetUniqueOfficeSymbols();

            // PROGRESS BAR
            // THERE IS A +1 TO ADD WHEN THE ENTIRE PROCESS IS COMPLETELY DONE
            // THE VALUE IS INCREMENTED AT THE END OF dropInColors
            // THE FINAL +1 IS ADDDED WHEN THE ORIGINALS ARE RESTORED IN restoreOriginals
            progress.Maximum = poPages.Count + 1;
        }

        private static void DateCover() {

            // COPY THE COVER FILE INTO THE WORKING FOLDER
            
            String folderPath = Settings.Default.folder;
            String coverPath = folderPath + "\\Cover.pdf";
            String cover1Path = folderPath + "\\Cover1.pdf";
            File.Copy(Settings.Default.cover, coverPath, true);

            // OPEN A PDF STAMPER (iTextSharp) TO WRITE IN DATE OF COVER INSIDE WORKING FOLDER
            iTextSharp.text.pdf.PdfReader reader = new(coverPath);
            PdfStamper stamper = new(reader, new FileStream(cover1Path, FileMode.Create));

            // FILL THE FORM WITH TODAY'S DATE
            String date = DateTime.Now.ToString("d MMMM yy");
            stamper.AcroFields.SetField("coverdate", date);

            // FLATTEN
            stamper.FormFlattening = true;
            stamper.AcroFields.GenerateAppearances = true;

            // CLOSING STREAMS
            stamper.Close();
            reader.Close();

            // REPLACING COVER WITH COVER1 THEN DELETING COVER1
            File.Copy(cover1Path, coverPath, true);
            File.Delete(cover1Path);
        }

        private void RecordHistory(iTextSharp.text.pdf.PdfReader reader, String name) {
            history[entry].Add(name, new Dictionary<String, String>());
            if (!headerSet) {
                foreach (KeyValuePair<string, AcroFields.Item> k in reader.AcroFields.Fields) {
                    if (!k.Key.Contains("Color") && !k.Key.Contains("score")) {
                        CSV += "\"" + k.Key + "\",";
                    }
                }
                CSV += "\n";
                headerSet = true;
            }
            foreach (KeyValuePair<string, AcroFields.Item> k in reader.AcroFields.Fields) {
                String fieldName = k.Key;
                String fieldValue = reader.AcroFields.GetField(fieldName);
                history[entry][name].Add(fieldName, fieldValue);
                if (fieldValue.Length > 0 && fieldValue[0] == '-') {
                    fieldValue = " -" + fieldValue[1..];
                }
                if (!fieldName.Contains("Color") && !k.Key.Contains("score")) {
                    CSV += "\"" + fieldValue + "\",";
                }
            }
            CSV += "\n";
            File.WriteAllText(Settings.Default.folder + "\\Dashboard.csv", CSV);
        }

        private void FillSnapshot() {

            // COPY THE SNAPSHOT FILE INTO THE WORKING FOLDER
            String folderPath = Settings.Default.folder;
            String snapshotPath = folderPath + "\\Snapshot.pdf";
            String snapshot1Path = folderPath + "\\Snapshot1.pdf";
            File.Copy(Settings.Default.snapshot, snapshotPath, true);

            // OPEN A PDF STAMPER (iTextSharp) TO WRITE IN DATA TO SNAPSHOT INSIDE WORKING FOLDER
            iTextSharp.text.pdf.PdfReader reader = new(snapshotPath);
            PdfStamper stamper = new(reader, new FileStream(snapshot1Path, FileMode.Create));

            int skipped = 0;
            for (int i = 0; i < poPages.Count; i++) {
                if (!poPagesBlacklist.Contains(poPages[i])) {

                    // COLLECTING SOME DATA
                    String office = fileOffice[poPages[i]];
                    String program = fileProgram[poPages[i]];

                    // FILLING IN OFFICE AND PROGRAM
                    stamper.AcroFields.SetField("office" + (i + 1 - skipped), office);
                    stamper.AcroFields.SetField("program" + (i + 1 - skipped), program);

                    // COLORING THE BUTTONS ON THE SNAPSHOT
                    PushbuttonField button = stamper.AcroFields.GetNewPushbuttonFromField("score" + (i + 1 - skipped));
                    XColor color = ScoredColor(fileScore[poPages[i]]);
                    button.BackgroundColor = new iTextSharp.text.BaseColor(color.R, color.G, color.B, 255);
                    stamper.AcroFields.ReplacePushbuttonField("score" + (i + 1 - skipped), button.Field);

                    // INCREMENTING VALUE ON THE PROGRESS BAR OCCURS HERE
                    progress.Value++;
                    progress.Refresh();
                } else {
                    skipped++;
                }
            }

            // FLATTEN
            stamper.FormFlattening = true;
            stamper.AcroFields.GenerateAppearances = true;

            // CLOSING STREAMS
            stamper.Close();
            reader.Close();

            // REPLACING SNAPSHOT WITH SNAPSHOT1 THEN DELETING SNAPSHOT1
            File.Copy(snapshot1Path, snapshotPath, true);
            File.Delete(snapshot1Path);
        }

        private void Concatenate() {

            // THIS METHOD IS RESPONSBILE FOR STITCHING TOGETHER ALL OF THE
            // PAGES THAT WILL BE INCLUDED IN THE FINAL REPORTS
            String coverPath = Settings.Default.folder + "\\Cover.pdf";
            String snapshotPath = Settings.Default.folder + "\\Snapshot.pdf";
            String dashPath = Settings.Default.folder + "\\Dashboard.pdf";

            // WILL BECOME THE MAIN DOCUMENT
            PdfSharp.Pdf.PdfDocument dash = new();

            // THESE ARE THE BOILERPLATE COVER AND SNAPSHOT SLIDES
            PdfSharp.Pdf.PdfDocument cover = PdfSharp.Pdf.IO.PdfReader.Open(coverPath, PdfDocumentOpenMode.Import);
            PdfSharp.Pdf.PdfDocument snapshot = PdfSharp.Pdf.IO.PdfReader.Open(snapshotPath, PdfDocumentOpenMode.Import);
            dash.AddPage(cover.Pages[0]);
            dash.AddPage(snapshot.Pages[0]);

            // LOOPING THROUGH THE poPages
            for (int i = 0; i < poPages.Count; i++) {
                if (!poPagesBlacklist.Contains(poPages[i])) {
                    PdfSharp.Pdf.PdfDocument input = PdfSharp.Pdf.IO.PdfReader.Open(poPages[i], PdfDocumentOpenMode.Import);

                    // THIS LOOP IS FOR COMPLETENESS SAKE. THIS COVERS THE SITUATION THAT A poPage IS
                    // MORE THAN ONE PAGE FOR SOME REASON
                    for (int j = 0; j < input.PageCount; j++) {
                        PdfSharp.Pdf.PdfPage page = input.Pages[j];
                        dash.AddPage(page);
                    }
                    input.Close();
                }
            }

            // CLOSINGS
            dash.Close();
            dash.Save(dashPath);
            cover.Close();
            snapshot.Close();
        }

        private void DatePoPages() {
            for (int i = 0; i < poPages.Count; i++) {
                String path = poPages[i];
                String name = poPages[i].Split('\\')[^1];
                String tmp = Settings.Default.folder + "\\tmp.pdf";
                File.Copy(path, tmp, true);

                // OPEN A PDF STAMPER (iTextSharp) TO WRITE IN DATE OF COVER INSIDE WORKING FOLDER
                iTextSharp.text.pdf.PdfReader reader = new(path);

                // FOR COMPARISON
                if (record) {
                    RecordHistory(reader, name);
                }

                // TRACKING changes FROM COMPARISON SELECTED
                // ITERATE THROUGH ALL OF THE FIELDS
                foreach (KeyValuePair<string, AcroFields.Item> k in reader.AcroFields.Fields) {
                    String key = k.Key;
                    String value = reader.AcroFields.GetField(key);
                    // TRY/CATCHED IN THE CASE THAT NO COMPARISON IS SELECTED
                    try {
                        if (!value.Equals(history[selection][name][key])) {
                            String[] change = new string[2];
                            change[0] = poPages[i];
                            change[1] = key;
                            changes.Add(change);
                        }
                    } catch { }
                }

                PdfStamper stamper = new(reader, new FileStream(tmp, FileMode.Create));

                // ADD TO fileOffice and fileProgram OCCURS HERE. THIS IS A GOOD PLACE TO DO IT.
                String office = reader.AcroFields.GetField("office");
                String program = reader.AcroFields.GetField("title");
                fileOffice.Add(path, office);
                fileProgram.Add(path, program);

                // TRACKING BLACKLIST FROM FORM 3 SELECTION
                if (!officesIncluded.Contains(office) && !(office == null)) {
                    poPagesBlacklist.Add(poPages[i]);
                }

                // FILL IN THE FORM WITH TODAY'S DATE
                _ = DateTime.Now.ToString("d MMMM yy");

                // DATE IS NOTHING
                // HOPEFULLY A TEMPORARY LETDOWN
                stamper.AcroFields.SetField("date", " ");

                // FLATTENING
                stamper.AcroFields.GenerateAppearances = true;
                stamper.FormFlattening = true;


                // CLOSING STREAMS
                stamper.Close();
                reader.Close();

                // REPLACING BACK
                File.Copy(tmp, path, true);
                File.Delete(tmp);
            }
            SaveHistory();
            progress.Maximum -= poPagesBlacklist.Count;
        }

        private void SaveHistory() {
            File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Dashboard Files\\history.txt", JsonSerializer.Serialize(history));
        }

        private static void DeleteOldFiles() {

            // CLEANING UP OLDER REPORTS TO AVOID FUNNY BUSINESS.
            try {
                File.Delete(Settings.Default.folder + "\\cover.pdf");
                File.Delete(Settings.Default.folder + "\\Dashboard.pdf");
            } catch { }
        }

        private void GetLastModifieds() {
            for (int i = 0; i < poPages.Count; i++) {

                // ADDES <FILENAME, TIME LAST MODIFIED> TO lastModifieds
                String key = poPages[i];
                String value = File.GetLastWriteTime(poPages[i]).ToString("d MMMM yy");
                lastModifieds.Add(key, value);
            }
        }

        private void BackupOriginals() {

            // CREATES A NEW FOLDER CALLED Orginals AT THE LOCATION
            // OF THE FOLDER SELECTOR. ALL OF poPages ARE COPIED INTO
            // THIS FOLDER BECAUSE I DON'T TRUST LIKE THAT.
            String folderPath = Settings.Default.folder;
            bool needToCreateFolder = !Directory.Exists(folderPath + "\\Originals");
            if (needToCreateFolder) {
                Directory.CreateDirectory(folderPath + "\\Originals");
            }

            for (int i = 0; i < poPages.Count; i++) {
                String name = poPages[i].Split('\\')[^1];
                File.Copy(poPages[i], folderPath + "\\Originals\\" + name, true);
            }
        }

        private void DropInColors() {
            for (int i = 0; i < poPages.Count; i++) {
                String file = poPages[i];

                // LOADING SCORE SETTINGS
                TreeView tv1 = new();
                String treeText = Settings.Default.treeText;
                List<String> allIndivs = new();
                if (treeText.Length > 0) {
                    String[] parents = treeText.Split(';');
                    for (int j = 0; j < parents.Length - 1; j++) {
                        String[] indiv = parents[j].Split(',');
                        for (int k = 0; k < indiv.Length; k++) {
                            if (k == 0) {
                                tv1.Nodes.Add(indiv[0]);
                            } else {
                                tv1.Nodes[j].Nodes.Add(indiv[k]);

                                // AGGREGATING THE TOTAL LIST FROM ALL OF THE TREEVIEW tv1 CHILDREN
                                allIndivs.Add(indiv[k]);
                            }

                        }
                    }
                }

                // CONVERTING LIST allIndivs INTO A PLAIN STRING ARRAY
                String[] indivsArray = new string[allIndivs.Count];
                for (int j = 0; j < allIndivs.Count; j++) {
                    indivsArray[j] = allIndivs[j];
                }


                List<double> scores = new();

                // TOTAL SCORE IS THE FIRST ADDED TO THE LIST scores
                scores.Add(CalculateScore(file, indivsArray));


                // CALCULATING THE SCORES FOR EACH OF THE LARGER BOXES ON THE poPage[i].

                // THIS MESS CREATES STRING ARRAYS OF THE CHILDREN NODES AND COMPUTES THE SCORE OF THE BUTTONS
                // WITH THE SAME NAMES AS THE ONES THAT APPEAR IN THE ARRAY AND ADDS THOSE SCORES TO scores
                for (int j = 0; j < tv1.Nodes.Count; j++) {
                    List<String> items = new();
                    for (int k = 0; k < tv1.Nodes[j].Nodes.Count; k++) {
                        items.Add(tv1.Nodes[j].Nodes[k].Text);
                    }
                    String[] itemsArray = new String[items.Count];
                    for (int k = 0; k < items.Count; k++) {
                        itemsArray[k] = items[k];
                    }
                    scores.Add(CalculateScore(file, itemsArray));
                }

                // CREATING A COPY OF THE poPage AND ATTACHING READERS AND STAMPERS TO THESE
                iTextSharp.text.pdf.PdfReader templateReader = new(poPages[i]);
                String tempName = poPages[i][..^4] + "temp.pdf";
                File.Copy(poPages[i], tempName);
                PdfStamper stamper = new(templateReader, new FileStream(tempName, FileMode.Create));

                // CREATING iText BUTTON OBJECTS TO SWAP INTO THE poPage AFTER THE COLOR IS SET CORRECTLY
                List<PushbuttonField> buttons = new();
                for (int j = 0; j < tv1.Nodes.Count; j++) {
                    buttons.Add(stamper.AcroFields.GetNewPushbuttonFromField(tv1.Nodes[j].Text));
                }

                // EITHER RED, YELLOW OR GREEN COLOR BASED ON THE SCORE
                List<XColor> colors = new();
                // ITS j = 1 BECAUSE THE TOTAL SCORE IS SAVED AT THE FRONT 
                for (int j = 1; j < scores.Count; j++) {
                    colors.Add(ScoredColor(scores[j]));
                }

                // SETTING THE BACKCOLOR OF THE BUTTON OBJECTS
                for (int j = 0; j < buttons.Count; j++) {
                    try {
                        buttons[j].BackgroundColor = new iTextSharp.text.BaseColor(colors[j].R, colors[j].G, colors[j].B, 255);
                    } catch {
                        MessageBox.Show("Warning: no such element \"" + tv1.Nodes[j].Text + "\" exists in file \"" + poPages[i].Split('\\')[^1] + "\".");
                    }
                }

                // SWAPPING THE COLORED BUTTONS INTO THE STAMPER
                for (int j = 0; j < buttons.Count; j++) {
                    try {
                        stamper.AcroFields.ReplacePushbuttonField(tv1.Nodes[j].Text, buttons[j].Field);
                    } catch { }
                }

                // CLEANING THINGS UP
                stamper.Close();
                templateReader.Close();
                File.Copy(tempName, poPages[i], true);
                File.Delete(tempName);

                // TOTAL IS ADDED TO fileScore BECAUSE IT MAKES SENSE
                fileScore.Add(file, scores[0]);
            }
        }

        private double CalculateScore(String file, String[] radios) {
            iTextSharp.text.pdf.PdfReader reader = new(file);

            double score = 0;
            int numberGood = 0;
            int numberOk = 0;
            int numberBad = 0;
            for (int i = 0; i < radios.Length; i++) {
                try {
                    String s = reader.AcroFields.GetNewPushbuttonFromField(radios[i]).BackgroundColor.ToString().Split('[')[1];
                    s = s[..^1];
                    // FULL OPAQUE GREEN
                    if (s.Equals("FF00FF00")) {
                        numberGood++;
                        // FULL OPAQUE YELLOW
                    } else if (s.Equals("FFFFFF00")) {
                        numberOk++;
                        // FULL OPAQUE RED
                    } else if (s.Equals("FFFF0000")) {
                        numberBad++;
                    }
                } catch (NullReferenceException) {
                    String text = "Warning: no such element \"" + radios[i] + "\" exists in file \"" + file.Split('\\')[^1] + "\".";
                    bool show = true;
                    for (int j = 0; j < warnings.Count; j++) {
                        if (warnings[j].Equals(text)) {
                            show = false;
                        }
                    }
                    if (show) {
                        warnings.Add(text);
                        MessageBox.Show(text);
                    }
                    numberBad++;
                }
            }

            // THIS IS HARD-FAST AS PER THE NEW COLORING SYSTEM
            if (numberBad == 0 & numberOk == 0) {
                // FULL GREEN
                score = 1.0;
            }

            if (numberOk > 0) {
                // FULL YELLOW
                score = 0.5;
            }

            if (numberBad > 0) {
                // FULL RED
                score = 0.0;
            }

            reader.Close();
            return score;
        }

        private void RestoreOriginals() {
            String folder = Settings.Default.folder;

            // COPYING ALL OF THE ORIGINAL FILES BACK TO folder
            String[] files = Directory.GetFiles(folder + "\\Originals", "*.pdf");
            for (int i = 0; i < files.Length; i++) {
                String name = files[i].Split('\\')[^1];
                try {
                    File.Copy(files[i], folder + "\\" + name, true);
                } catch { }
            }

            // DELETING COVER AND SNAPSHOT THAT WERE COPIED IN EARLIER
            // WE DON'T NEED THEM ANYMORE
            try {
                File.Delete(folder + "\\Cover.pdf");
            } catch { }
            try {
                File.Delete(folder + "\\Snapshot.pdf");
            } catch { }

            // DONE!
            try { progress.Value++; } catch { }
        }

        public static XColor ScoredColor(double score) {

            // THIS RETURNS A COLOR ON A GREEN-YELLOW-RED GRADIENT
            // THE COLOR IS DETERMINED BY THE score WHICH MUST BE [0.0, 1.0]
            // 0.0 IS THE MOST RED AND 1.0 IS THE MOST GREEN
            // BLUE ALWAYS ZERO ON THIS GRADIENT
            XColor color = new() { B = 0 };

            // FULL RED MIXED WITH RATIO OF GREEN
            if (score < 0.5) {
                color.R = 254;
                color.G = (byte)Math.Round(score / 0.5 * 250);
            }

            //FULL GREEN MIXED WITH RATIO OF RED
            else {
                color.G = 254;
                color.R = (byte)(254 - (int)(Math.Round((score - 0.5) / 0.5 * 254)));
            }

            return color;
        }

        private void Contact_Click(object sender, EventArgs e) {

            // LITERALLY ME
            String line = Environment.NewLine;
            MessageBox.Show("Contact for questions" + line + line + "Gavin Keane" + line + "gavin.keane@us.af.mil" + line + "Cell: 440-714-7002", "POC");
        }

        private void Migrate_Click(object sender, EventArgs e) {
            Form2 form2 = new();
            form2.Show();
        }

        private void Button1_Click(object sender, EventArgs e) {
            Form3 form3 = new(this);
            form3.Show();
        }

        private void GetUniqueOfficeSymbols() {

            // THIS METHOD POPULATES THE office LIST. THIS IS USED FOR THE DIRECTORATE
            // SELECTION ListBox ON Form3
            officesIncluded.Clear();
            for (int i = 0; i < poPages.Count; i++) {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                PdfSharp.Pdf.PdfDocument pdf = PdfSharp.Pdf.IO.PdfReader.Open(poPages[i], PdfDocumentOpenMode.ReadOnly);
                String officeSymbol = ".AF/ABC*.";
                try {
                    officeSymbol = pdf.AcroForm.Fields["office"].Value.ToString();
                } catch {
                    _ = MessageBox.Show("Warning: no such element \"" + "office" + "\" exists in file \"" + poPages[i].Split('\\')[^1] + "\".");
                }


                officeSymbol = officeSymbol[1..^1];
                if (!offices.Contains(officeSymbol)) {
                    offices.Add(officeSymbol);
                    officesIncluded.Add(officeSymbol);
                }
                pdf.Close();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) {
            try {
                RestoreOriginals();
            } catch { }
        }

        private void Button1_Click_1(object sender, EventArgs e) {
            Form3 form3 = new(this);
            form3.Show();
        }
    }
}
