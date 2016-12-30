using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml;
using System.Collections;

namespace NMAT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool vulnID = false;
        public const string version_name = "Citadel";
        public const string version_num = "V3.1.1";
        public string NMATDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\NMAT_Temp\";
        public string NMATConfigDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\NMAT_Temp\Config\";
        Dictionary<string, string> dictionary_VulnID = new Dictionary<string, string>() { };
        List<VulnID> vulnids = new List<VulnID>();
        List<System.Windows.Controls.RadioButton> radioButtons = new List<System.Windows.Controls.RadioButton>();

        //-------------Initialize MainWindow-------------//
        public MainWindow()

        {
            InitializeComponent();
            version_label.Content = version_num;
            HideGroupBox();
            if (Directory.Exists(NMATConfigDir))
            {
                if (File.Exists(NMATConfigDir + "Import_VulnID.nmat"))
                {
                    vulnID = true;
                    XMLParser();
                }
                else
                {
                    vulnID = false;
                }
                if (vulnID)
                {
                    updateRadio();
                }
                else
                {
                    comboBox_VulnID.Items.Clear();
                    comboBox_VulnID.Items.Add("Import .nmat file...");
                }
                UpdateConfigInfo();
            }
            else
            {
                Directory.CreateDirectory(NMATDir);
                Directory.CreateDirectory(NMATConfigDir);
            };
        }

        private void DeleteFilesinDir()
        {
            Array.ForEach(Directory.GetFiles(NMATDir), File.Delete);
        }

        //-----------Search Field and Button------------//

        private void searchText_KeyPress(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                searchfunction();
                HideGroupBox();
            }
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            searchfunction();
            HideGroupBox();
        }

        //---------Search Field and Button End------------//

        //-----------VulnID ComboBox----------//

        private void comboBox_VulnID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VulnIDUpdate();
        }

        //--------VulnID ComboBox End----------//

        //------------Buttons---------------//

        //-----------Navigation Buttons--------------//

        private void Next_button_Click(object sender, RoutedEventArgs e)
                {
                    if (comboBox_VulnID.SelectedIndex <= comboBox_VulnID.Items.Count)
                    {
                        comboBox_VulnID.SelectedIndex += 1;
                    }
                }

                private void Previous_button_Click(object sender, RoutedEventArgs e)
                {
                    if (comboBox_VulnID.SelectedIndex >= 1)
                    {
                        comboBox_VulnID.SelectedIndex -= 1;
                    }
                }

                //-------------Navigation Buttons End-------------//

                //----------------Side Bar Buttons----------------//

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            LoadNewConfigs();
            updateRadio();
            UpdateConfigInfo();
            comboBox_VulnID.SelectedItem = 1;
            HideGroupBox();
        }

        private void update_vuln_button_Click(object sender, RoutedEventArgs e)
        {
            //update vulnID list
            //remove old list
            //copy new list to NMATConfigDir location
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "Select NMAT Configuration Files";
            fileDialog.InitialDirectory = "";
            fileDialog.Filter = "NMAT File | *.nmat";
            fileDialog.Multiselect = false;

            DialogResult myResult = fileDialog.ShowDialog();

            if (myResult == System.Windows.Forms.DialogResult.OK)
            {
                search_name.Text = "";
                searchText.Text = "";
                comboBox_VulnID.SelectedIndex = -1;
                string filedirectory = System.IO.Path.GetDirectoryName(fileDialog.FileName);
                string fullPath = String.Concat(filedirectory, "\\");
                string filePath = System.IO.Path.GetFullPath(fileDialog.FileName);
                string toPath = System.IO.Path.Combine(NMATConfigDir, "Import_VulnID.nmat");

                if (fullPath != NMATConfigDir)
                {
                    System.IO.File.Copy(filePath, toPath, true);
                    comboBox_VulnID.Items.Clear();
                    XMLParser();
                    System.Windows.Forms.MessageBox.Show("Import Successful", "Success!", MessageBoxButtons.OK, MessageBoxIcon.None);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Import Successful", "Success!", MessageBoxButtons.OK, MessageBoxIcon.None);
                }
            }
        }

        private void about_button_Click(object sender, RoutedEventArgs e)
        {
            foreach (System.Windows.Controls.RadioButton rb in radioButtons)
            {
                rb.IsChecked = false;
            }
            about f2 = new about();
            f2.ShowDialog();
        }

        private void help_button_Click(object sender, RoutedEventArgs e)
        {
            help f3 = new help();
            f3.ShowDialog();
        }

        private void exit_button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        //used in the event the user selects the "x" instead of the exit button
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            System.Windows.Application.Current.Shutdown();
        }
                //-------------Side Bar Buttons End--------------//
        //------------Buttons---------------//


        //-----------Functions--------------//

        public void XMLParser()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(NMATConfigDir + "Import_VulnID.nmat");

            XmlNodeList nodes = doc.DocumentElement.SelectNodes("/catalog/vulnid");

            foreach (XmlNode node in nodes)
            {
                VulnID vulnid = new VulnID();

                vulnid.title = node.SelectSingleNode("title").InnerText;
                vulnid.desc = node.SelectSingleNode("desc").InnerText;
                vulnid.example = node.SelectSingleNode("example").InnerText;
                vulnid.check1 = node.SelectSingleNode("check1").InnerText;
                vulnid.check2 = node.SelectSingleNode("check2").InnerText;
                vulnid.check3 = node.SelectSingleNode("check3").InnerText;
                vulnid.id = node.Attributes["id"].Value;

                vulnids.Add(vulnid);
            }
            comboBox_VulnID.Items.Clear();
            foreach (XmlNode node in doc.SelectNodes("//vulnid[@id]"))
            {
                comboBox_VulnID.Items.Add(node.Attributes["id"].Value);
            }
            comboBox_VulnID.SelectedIndex = -1;
            HideGroupBox();
        }

        private void LoadNewConfigs()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "Select Configuration Files";
            fileDialog.InitialDirectory = "";
            fileDialog.Filter = "Config Files | *.txt; *.cfg";
            fileDialog.Multiselect = true;

            DialogResult myResult = fileDialog.ShowDialog();
            if (myResult == System.Windows.Forms.DialogResult.OK)
            {
                DeleteFilesinDir();
                search_name.Text = "";
                searchText.Text = "";
                comboBox_VulnID.SelectedIndex = -1;

                foreach (String filePath in fileDialog.FileNames)
                {
                    string fileName = System.IO.Path.GetFileName(filePath);  // Just file name of the selected file
                    if (!fileName.Contains(".txt"))
                    {
                        string toPath = System.IO.Path.Combine(NMATDir, fileName + ".txt");   //  File path to the \NMAT_Temp\ folder with filename appended
                        System.IO.File.Copy(filePath, toPath, true);
                    }
                    else
                    {
                        string toPath = System.IO.Path.Combine(NMATDir, fileName);
                        System.IO.File.Copy(filePath, toPath, true);
                    }
                };
            }
            else
            {
                return;
            }
        }

        public void VulnIDUpdate()
        {
            if (comboBox_VulnID.SelectedIndex < 0) { }
            else
            {
                if (comboBox_VulnID.SelectedValue.ToString() == "Import .nmat file...")
                {
                    if (File.Exists(NMATConfigDir + "Import_VulnID.nmat"))
                    {
                        vulnID = true;
                        XMLParser();
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("No .nmat configuration file detected", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    string value = comboBox_VulnID.SelectedValue.ToString();
                    foreach (VulnID check in vulnids)
                    {
                        if (check.id == value)
                        {
                            string result = check.check1.ToString();
                            string description = check.desc.ToString();
                            string example = check.example.ToString();
                            string title = check.title.ToString();
                            title_name.Text = check.id + ": " + title;
                            title_name.Visibility = Visibility.Visible;
                            searchText.Text = result;
                            desc_name.Text = description;
                            if (example == "")
                            {
                                example_name.Text = "No example found";
                            }
                            else
                            {
                                example_name.Text = example;
                            }
                            search_name.Text = "";
                            search_name.Text = SearchFiles(result);
                            if (check.check2 != "none")
                            {
                                System.Windows.Forms.MessageBox.Show("This vulnerability requires multiple steps!" + Environment.NewLine + "Please ensure that all requirements are checked", "Multiple Steps", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                groupBox.Visibility = Visibility.Visible;
                                foreach (System.Windows.Controls.RadioButton rb in radioButtons)
                                {
                                    rb.IsChecked = false;
                                }
                                radioButton_1.IsChecked = true;
                                if (check.check3 == "none")
                                {
                                    radioButton_3.Visibility = Visibility.Hidden;
                                } else
                                {
                                    radioButton_3.Visibility = Visibility.Visible;
                                }

                            }
                            else
                            {
                                groupBox.Visibility = Visibility.Hidden;
                            }
                            if (String.IsNullOrWhiteSpace(SearchFiles(result)))
                            {
                                search_name.Text = "No Instance Found";
                            }
                            break;
                        }
                    }
                }
            }
        }

        private void searchfunction()
        {
            if (string.IsNullOrWhiteSpace(searchText.Text))
            {
                System.Windows.MessageBox.Show("Please fill in search field and try again.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                desc_name.Text = "";
                example_name.Text = "";
                title_name.Text = "";
                title_name.Visibility = Visibility.Hidden;
                comboBox_VulnID.SelectedIndex = -1;
                string input = searchText.Text;
                string checkedinput = input.Replace("\\", "");
                int quoteCount = (checkedinput.Length - checkedinput.Replace("\"", "").Length) / checkedinput.Length;
                if (quoteCount % 2 == 0)
                {
                    checkedinput = input.Replace("\"", "");
                }
                search_name.Text = "";
                search_name.Text = SearchFiles(checkedinput);
                if (String.IsNullOrWhiteSpace(SearchFiles(checkedinput)))
                {
                    search_name.Text = "No Instance Found";
                }
            }
        }

        private void HideGroupBox()
        {
            if (groupBox.Visibility == Visibility.Visible)
            {
                groupBox.Visibility = Visibility.Hidden;
            }
        }

        private void UpdateConfigInfo()
        {
            if (radioButton_switch.IsChecked == true)
            {
                dn_name.Content = ReadInfofromFiles("hostname");
                dip_name.Content = ReadInfofromFiles("ip address");
            }
            else
            {
                dn_name.Content = ReadInfofromFiles("SYS SysNAMe");
                dip_name.Content = ReadInfofromFiles("SystemIP");
                if (dip_name.Content.ToString() == "No Instance Found")
                {
                    dip_name.Content = ReadInfofromFiles("Router IP:");
                }
            }
            dl_name.Content = ReadInfofromFiles("SYS SysLOCation");
        }

        private void updateRadio()
        {
            if (ReadInfofromFiles("hostname") != "No Instance Found")
            {
                radioButton_switch.IsChecked = true;
            }
            else
            {
                radioButton_router.IsChecked = true;
            }
        }

        private string ReadInfofromFiles(string f)
        {
            string find = f;
            string result = null;
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.CreateNoWindow = true;
            startInfo.WorkingDirectory = NMATDir;
            startInfo.Arguments = "/C findstr /i /C:" + "\"" + find + "\"" + " *.txt";
            process.StartInfo = startInfo;
            process.Start();
            while (!process.StandardOutput.EndOfStream)
            {
                string output = process.StandardOutput.ReadLine();
                if (output.Contains("\""))
                {
                    foreach (Match match in Regex.Matches(output, "\"([^\"]*)"))
                        if (match != null && match.Length > 1)
                        {
                            result = match.Groups[1].ToString();
                        }
                }
                else if (output.Contains("IP:"))
                {
                    foreach (Match match in Regex.Matches(output, ":([^?]*)"))
                        if (match != null && match.Length > 1)
                        {
                            string outputmatch = match.Groups[1].ToString();
                            result = outputmatch.Substring(outputmatch.IndexOf(":") + 2);
                        }
                }
                else
                {
                    foreach (Match match in Regex.Matches(output, "=([^\"]*)"))
                        if (match != null && match.Length > 1)
                        {
                            string outputmatch = match.Groups[1].ToString();
                            result = outputmatch.Substring(0, outputmatch.LastIndexOf(" ")).Replace(" ", "");
                        }
                }
                if (string.IsNullOrWhiteSpace(result))
                {
                    result = "No Instance Found";
                }
                return result;
            }
            if (string.IsNullOrWhiteSpace(result))
            {
                result = "No Instance Found";
            }
            return result;
        }

        private string SearchFiles(string f)
        {
            string cd = NMATDir;
            string find = f;
            string result = null;
            string[] arrlist = new string[1];
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.CreateNoWindow = true;
            startInfo.WorkingDirectory = NMATDir;
            startInfo.Arguments = "/C findstr /i /C:" + "\"" + find + "\"" + " *.txt";
            process.StartInfo = startInfo;
            process.Start();
            while (!process.StandardOutput.EndOfStream)
            {
                string output = process.StandardOutput.ReadToEnd();
                if (output.Contains(" "))
                {
                    int i = 0;
                    arrlist[i] = output.ToString();
                    i++;
                }
                result = String.Join("\n" + Environment.NewLine, arrlist);
                return result;
            }
            return result;
        }

        //-----------Functions End-------------//

        //--------------Multiple Step Radio Buttons----------------//

        private void radioButton_1_Click(object sender, RoutedEventArgs e)
        {
            string value = comboBox_VulnID.SelectedValue.ToString();
            foreach (VulnID check in vulnids)
            {
                if (check.id == value)
                {
                    string result = check.check1.ToString();
                    searchText.Text = result;
                    search_name.Text = "";
                    search_name.Text = SearchFiles(result);
                    if (String.IsNullOrWhiteSpace(SearchFiles(result)))
                    {
                        search_name.Text = "No Instance Found";
                    }
                }
            }
        }

        private void radioButton_2_Click(object sender, RoutedEventArgs e)
        {
            string value = comboBox_VulnID.SelectedValue.ToString();
            foreach (VulnID check in vulnids)
            {
                if (check.id == value)
                {
                    string result = check.check2.ToString();
                    searchText.Text = result;
                    search_name.Text = "";
                    search_name.Text = SearchFiles(result);
                    if (String.IsNullOrWhiteSpace(SearchFiles(result)))
                    {
                        search_name.Text = "No Instance Found";
                    }
                }
            }
        }

        private void radioButton_3_Click(object sender, RoutedEventArgs e)
        {
            string value = comboBox_VulnID.SelectedValue.ToString();
            foreach (VulnID check in vulnids)
            {
                if (check.id == value)
                {
                    string result = check.check3.ToString();
                    searchText.Text = result;
                    search_name.Text = "";
                    search_name.Text = SearchFiles(result);
                    if (String.IsNullOrWhiteSpace(SearchFiles(result)))
                    {
                        search_name.Text = "No Instance Found";
                    }
                }
            }
        }

        //-----------Multiple Step Radio Buttons End----------------//

        //--------------Collapse/Expand Windows---------------//

        //when output window is updated, this resets the cursor to the top
        private void search_name_TargetUpdated_1(object sender, DataTransferEventArgs e)
        {
            search_name.ScrollToHome();
        }
        //when output window is updated, this resets the cursor to the top
        private void search_name_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            search_name.ScrollToHome();
        }
        //when output window is updated, this resets the cursor to the top
        private void search_name_TextInput_1(object sender, TextCompositionEventArgs e)
        {
            search_name.ScrollToHome();
        }

        private void details_Collapsed(object sender, RoutedEventArgs e)
        {
            if (example.IsExpanded)
            {
                search_name.MinHeight = 200;
                search_name.Height = 200;
                search_name.MaxHeight = 200;
            }
            else
            {
                search_name.MinHeight = 300;
                search_name.Height = 300;
                search_name.MaxHeight = 300;
            }
        }

        private void details_Expanded(object sender, RoutedEventArgs e)
        {
            if (example.IsExpanded)
            {
                search_name.MinHeight = 100;
                search_name.Height = 100;
                search_name.MaxHeight = 100;
            }
            else
            {
                search_name.MinHeight = 200;
                search_name.Height = 200;
                search_name.MaxHeight = 200;
            }
        }

        private void example_Collapsed(object sender, RoutedEventArgs e)
        {
            if (details.IsExpanded)
            {
                search_name.MinHeight = 200;
                search_name.Height = 200;
                search_name.MaxHeight = 200;
            }
            else
            {
                search_name.MinHeight = 300;
                search_name.Height = 300;
                search_name.MaxHeight = 300;
            }
        }

        private void example_Expanded(object sender, RoutedEventArgs e)
        {
            if (details.IsExpanded)
            {
                search_name.MinHeight = 100;
                search_name.Height = 100;
                search_name.MaxHeight = 100;
            }
            else
            {
                search_name.MinHeight = 200;
                search_name.Height = 200;
                search_name.MaxHeight = 200;
            }
        }

        //--------------Collapse/Expand Windows---------------//
    }

    //-------------End of MainWindow-------------//

    // Class to hold all values from XML Parser
    class VulnID
    {
        public string id;
        public string title;
        public string desc;
        public string example;
        public string check1;
        public string check2;
        public string check3;
    }
}
