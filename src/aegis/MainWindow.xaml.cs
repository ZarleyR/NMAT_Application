using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

namespace NMAT
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool vulnID = false;
        public const string version_name = "Aegis";
        public const string version_num = "V1.0.0";
        public string NMATDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\NMAT_Temp\";
        public string NMATConfigDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\NMAT_Temp\Config\";
        Dictionary<string, string> dictionary_VulnID = new Dictionary<string, string>() { };
        public MainWindow()

        {
            InitializeComponent();
            version_label.Content = version_num;
            if (Directory.Exists(NMATConfigDir))
            {
                if (File.Exists(NMATConfigDir + "Import_VulnID.nmat"))
                {
                    vulnID = true;
                    dictionary_VulnID = File.ReadLines(NMATConfigDir + "Import_VulnID.nmat").Select(line => line.Split(':')).ToDictionary(line => line[0], line => line[1]);
                }
                else
                {
                    vulnID = false;
                }

                dn_name.Content = ReadInfofromFiles("SYS SysNAMe");
                dip_name.Content = ReadInfofromFiles("IP NETaddr");
                if (dip_name.Content.ToString() == "No Instance Found")
                {
                    dip_name.Content = ReadInfofromFiles("Router IP:");
                }
                dl_name.Content = ReadInfofromFiles("SYS SysLOCation");
                //populate List Box from import  NOTE this will change when dictionary is referenced...
                //NOTE could add populateComboBox() function which will reference the dictionary to populate.
                if (vulnID)
                {
                    foreach (KeyValuePair<string, string> pair in dictionary_VulnID)
                    {
                        comboBox_VulnID.Items.Add(pair.Key);
                    }
                }
                else
                {
                    comboBox_VulnID.Items.Add("Import .nmat file...");
                }


                //                string[] lines = File.ReadAllLines(NMATDir + "import_test.txt");
                //                foreach (string line in lines)
                //                {
                //                    comboBox_VulnID.Items.Add(line);
                //                }

                //               listBox_VulnID.Items.AddRange(File.ReadAllLines(NMATDir + "import_test.txt"));
            }
            else
            {
                Directory.CreateDirectory(NMATDir);
                Directory.CreateDirectory(NMATConfigDir);
            };
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            LoadNewConfigs();
            dn_name.Content = ReadInfofromFiles("SYS SysNAMe");
            dip_name.Content = ReadInfofromFiles("IP NETaddr");
            if (dip_name.Content.ToString() == "No Instance Found")
            {
                dip_name.Content = ReadInfofromFiles("Router IP:");
            }
            dl_name.Content = ReadInfofromFiles("SYS SysLOCation");

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

        private void DeleteFilesinDir()
        {
            Array.ForEach(Directory.GetFiles(NMATDir), File.Delete);
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

        private void searchText_KeyPress(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string input = searchText.Text;
                search_name.Text = "";
                search_name.Text = SearchFiles(input);
                if (String.IsNullOrWhiteSpace(SearchFiles(input)))
                {
                    search_name.Text = "No Instance Found";
                }
            }
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchText.Text))
            {
                System.Windows.MessageBox.Show("Please fill in search field and try again.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                string input = searchText.Text;
                search_name.Text = SearchFiles(input);
                if (String.IsNullOrWhiteSpace(SearchFiles(input)))
                {
                    search_name.Text = "No Instance Found";
                }
            }
        }
        private void search_name_TargetUpdated_1(object sender, DataTransferEventArgs e)
        {
            search_name.ScrollToHome();
        }

        private void search_name_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            search_name.ScrollToHome();
        }

        private void search_name_TextInput_1(object sender, TextCompositionEventArgs e)
        {
            search_name.ScrollToHome();
        }

        private void comboBox_VulnID_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBox_VulnID.SelectedIndex < 0) { }
            else
            {
                string input = comboBox_VulnID.SelectedValue.ToString();
                foreach (KeyValuePair<string, string> pair in dictionary_VulnID)
                {
                    if (pair.Key == input)
                    {
                        searchText.Text = pair.Value;
                        search_name.Text = SearchFiles(pair.Value);
                        if (String.IsNullOrWhiteSpace(SearchFiles(pair.Value)))
                        {
                            search_name.Text = "No Instance Found";
                        }
                    }
                }
                if (input == "Import .nmat file...")
                {
                    if (File.Exists(NMATConfigDir + "Import_VulnID.nmat"))
                    {
                        vulnID = true;
                        dictionary_VulnID = File.ReadLines(NMATConfigDir + "Import_VulnID.nmat").Select(line => line.Split(':')).ToDictionary(line => line[0], line => line[1]);
                        comboBox_VulnID.Items.Clear();
                        foreach (KeyValuePair<string, string> pair in dictionary_VulnID)
                        {
                            comboBox_VulnID.Items.Add(pair.Key);
                        }
                    }
                    else
                    {
                        Directory.CreateDirectory(NMATConfigDir);
                        System.Windows.Forms.MessageBox.Show("Please place .nmat file within the \\NMAT_Temp\\Config directory", "Missing .nmat File", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }

        private void about_button_Click(object sender, RoutedEventArgs e)
        {
            about f2 = new about();
            f2.ShowDialog();
        }

        private void exit_button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void help_button_Click(object sender, RoutedEventArgs e)
        {
            help f3 = new help();
            f3.ShowDialog();
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
                    dictionary_VulnID = File.ReadLines(NMATConfigDir + "Import_VulnID.nmat").Select(line => line.Split(':')).ToDictionary(line => line[0], line => line[1]);
                        foreach (KeyValuePair<string, string> pair in dictionary_VulnID)
                        {
                            comboBox_VulnID.Items.Add(pair.Key);
                        }
                        System.Windows.Forms.MessageBox.Show("Import Successful", "Success!", MessageBoxButtons.OK, MessageBoxIcon.None);
                }
                else
                    {
                        System.Windows.Forms.MessageBox.Show("Import Successful", "Success!", MessageBoxButtons.OK, MessageBoxIcon.None);
                    }
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            System.Windows.Application.Current.Shutdown();
        }
    }
}
