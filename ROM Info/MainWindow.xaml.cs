using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace ROM_Info
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string mamePath = @"c:\mame\";
        private string romsPath = @"roms\";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void makeXml_Click(object sender, RoutedEventArgs e)
        {
            enable(false);
            Process proc = Process.Start("cmd", "/c \"cd " + mamePath + " && mame -listxml > mame.xml\"");
            proc.WaitForExit();
            MessageBox.Show(@"Made mame.xml");
            enable(true);
        }

        private void makeCsv_Click(object sender, RoutedEventArgs e)
        {
            enable(false);
            MessageBox.Show(@"Making roms.csv, this could take a while...");
            createCsvFile();
            MessageBox.Show(@"Made roms.csv");
            enable(true);
        }

        private string segment(string input, int len=0) {
            string result = input.Replace(",", "");
            if (len > 0 && result.Length > len) {
                result = result.Substring(0, len);
            }
            while (len > 0 && result.Length < len) {
                result = result + " ";
            }
            return result;
        }

        private void createCsvFile() {
            XmlDataDocument xmldoc = new XmlDataDocument();
            string name = null;
            string year = null;
            string corp = null;
            string line = null;
            string code = null;
            XmlNodeList childs = null;
            StreamWriter file = new StreamWriter(mamePath + "roms.csv", false);
            FileStream fs = new FileStream(mamePath + "mame.xml", FileMode.Open, FileAccess.Read);
            xmldoc.Load(fs);
            XmlNodeList machineNodes = xmldoc.GetElementsByTagName("machine");

            for (int i = 0; i < machineNodes.Count; i++)
            {
                code = machineNodes[i].Attributes.GetNamedItem("name").InnerText.Trim();
                childs = machineNodes[i].ChildNodes;
                name = childs.Count > 0 ?  childs.Item(0).InnerText.Trim() : "";
                year = childs.Count > 1 ? childs.Item(1).InnerText.Trim() : "";
                corp = childs.Count > 2 ? childs.Item(2).InnerText.Trim() : "";
                line = segment(code) + ", " + segment(name)  + ", " + segment(year) + ", " + segment(corp);
                file.WriteLine(line);
                //if (i > 10) {
                //    break;
                //}
            }
            file.Close();
            fs.Close();
        }

        private void enable(Boolean enabled = true)
        {
            makeCsv.IsEnabled = enabled;
            makeXml.IsEnabled = enabled;
        }
    }
}
