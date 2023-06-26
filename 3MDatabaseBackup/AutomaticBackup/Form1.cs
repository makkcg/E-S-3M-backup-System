using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;
using System.ServiceProcess;
using System.IO.Compression;
using System.IO;
using System.Xml;
namespace AutomaticBackup
{
    public partial class Form1 : Form
    {
        string bakfile = "";
        string pathstr = "";
        string bpath = "";
        string backup_every = "";
        string keep = "";
        int bcount = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pathstr = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            pathstr = pathstr.Substring(6, pathstr.Length - 6);
            string temp = "\\bin\\Debug";
            if (pathstr.LastIndexOf(temp) > 0) pathstr = pathstr.Substring(0, pathstr.Length - temp.Length);
          
        }
        private void backupit(string backp, string bdate)
        {
            List<string> prevbaks = new List<string>();
            List<string> bfileslist = new List<string>();
            //var files = Directory.GetFiles(@"C:\ProgramData\3M_CleanTraceDatabase", "*.*");
            // StopWindowsService();
            bool flag = true;
            progressBar1.Value = 30;
            Application.DoEvents();
            System.Threading.Thread.Sleep(3000);
            DirectoryInfo info = new DirectoryInfo(@"C:\ProgramData\3M_CleanTraceDatabase");
            bfileslist.Clear();
            FileInfo[] files = info.GetFiles().OrderBy(p => p.CreationTime).ToArray();
            foreach (FileInfo file in files)
            {

                bfileslist.Add(file.Name.ToString());
            }
            string mdfarc = "";
            string ldfarc = "";

            for (int i = bfileslist.Count - 1; i >= 0; i--)
            {
                if (bfileslist[i].ToLower().Contains(".mdf") && bfileslist[i].ToLower().Contains("spark_data") && mdfarc == "") mdfarc = bfileslist[i];
                if (bfileslist[i].ToLower().Contains(".ldf") && bfileslist[i].ToLower().Contains("spark_log") && ldfarc == "") ldfarc = bfileslist[i];


            }
            string bdir = bdate.Replace('/', '_').Replace(" ", "_").Replace(":", "_");
            if (!Directory.Exists(backp + "\\" + bdir))
            {
                Directory.CreateDirectory(backp + "\\" + bdir);
            }


            if (mdfarc != "") File.Copy(@"C:\ProgramData\3M_CleanTraceDatabase" + "\\" + mdfarc, backp + "\\" + bdir + "\\" + mdfarc);
            if (ldfarc != "") File.Copy(@"C:\ProgramData\3M_CleanTraceDatabase" + "\\" + ldfarc, backp + "\\" + bdir + "\\" + ldfarc);
            progressBar1.Value = 70;

            Application.DoEvents();
            System.Threading.Thread.Sleep(3000);
            progressBar1.Value = 100;
            Application.DoEvents();
            System.Threading.Thread.Sleep(10000);
            if (bcount == 0)
            {
                using (StreamWriter writer = new StreamWriter(pathstr + "\\App_Data\\backups.xml"))
                {
                    writer.WriteLine(bdate.Replace('/', '_').Replace(":", "_").Replace(" ", "_") + ".zip" + "~" + bdate);

                }


            }
            else
            {

                prevbaks.Clear();


                using (var streamReader = new StreamReader(pathstr + "\\App_Data\\backups.xml"))
                {
                    String line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        prevbaks.Add(line);

                    }
                }


                if ((bcount + 1) <= Convert.ToInt32(keep))
                {
                    using (StreamWriter writer = new StreamWriter(pathstr + "\\App_Data\\backups.xml"))
                    {
                        for (int i = 0; i < prevbaks.Count; i++) writer.WriteLine(prevbaks[i]);
                        writer.WriteLine(bdate.Replace('/', '_').Replace(":", "_").Replace(" ", "_") + ".zip" + "~" + bdate);

                    }
                    bcount = bcount + 1;
                }

                else
                {
                    using (StreamWriter writer = new StreamWriter(pathstr + "\\App_Data\\backups.xml"))
                    {
                        for (int i = 1; i < prevbaks.Count; i++) writer.WriteLine(prevbaks[i]);
                        writer.WriteLine(bdate.Replace('/', '_').Replace(":", "_").Replace(" ", "_") + ".zip" + "~" + bdate);

                    }
                    string ss = prevbaks[0];
                    if (File.Exists(bpath + "\\" + ss[0])) File.Delete(bpath + "\\" + ss[0]);
                }


            }
            
        }

        
       
        private void Form1_Shown(object sender, EventArgs e)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(pathstr + "\\App_Data\\settings.xml");
            XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("/settings");

            foreach (XmlNode node in nodeList)
            {
                bpath = node.SelectSingleNode("path").InnerText;
                backup_every = node.SelectSingleNode("backup_every").InnerText;
                keep = node.SelectSingleNode("keep").InnerText;

            }
            DateTime thisDay = DateTime.Now;
            int days = 2;
                if (!File.Exists(pathstr + "\\App_Data\\backups.xml"))
                {

                    backupit(bpath, thisDay.ToString());
                    Application.Exit();
                }
                else
                {
                    string lastDate = getLastDate();
                   switch (backup_every)
                    {
                        case "1 Day": days = 1; break;
                        case "3 Days": days = 3; break;
                        case "1 Week": days = 7; break;
                        case "2 Weeks": days = 14; break;
                        case "Month": days = 30; break;
                    }

                    double numd = (thisDay - Convert.ToDateTime(lastDate)).TotalDays;
                    if (numd >= days)
                    {
                        backupit(bpath, thisDay.ToString());
                        Application.Exit();
                    }
                    else
                    {
                        progressBar1.Value = 100;
                        Application.DoEvents();
                        System.Threading.Thread.Sleep(3000);
                        Application.Exit();
                    }

                }
           
        }

      private string getLastDate()
        {
            string temp="";
            using (var streamReader = new StreamReader(pathstr + "\\App_Data\\backups.xml"))
            {
                String line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    string[] ss = line.Split('~');
                    temp = ss[1];
                }
            }
            return temp;

        }
       
    }
}
