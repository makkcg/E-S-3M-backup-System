using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Permissions;
using Microsoft.Win32;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Security;
using System.Threading.Tasks;
using System.ServiceProcess;
using System.Xml;
using System.Data.SqlClient;



namespace _3m_db_backup
{
    public partial class mainfrm : Form
    {
        string bakfile = "";
        string pathstr = "";
        string bpath="";
        string backup_every = "";
        string keep = "";
        int bcount = 0;

        public mainfrm()
        {
            InitializeComponent();
        }

        private void mainfrm_Load(object sender, EventArgs e)
        {
            //StopWindowsService();
            /*
            HrDbLib cc = new HrDbLib();
            cc.qfields.Add("path");
            cc.qfields.Add("keep");
            cc.qfilters.Add("ID=1");
            cc.GetQData("settings", 2, 1);
            string[] ss = cc.qresults[0].Split('~');
            label2.Text =  ss[0];
            if (Convert.ToInt32(ss[1]) == 2) radioButton6.Checked = true;
            else if (Convert.ToInt32(ss[1]) == 3) radioButton7.Checked = true;
            else if (Convert.ToInt32(ss[1]) == 5) radioButton8.Checked = true;
            else {
                textBox1.Text = Convert.ToInt32(ss[1]).ToString();
                radioButton9.Checked = true;
                }
             */
            pathstr = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            pathstr = pathstr.Substring(6, pathstr.Length - 6);
            string temp = "\\bin\\Debug";
            if (pathstr.LastIndexOf(temp) > 0) pathstr = pathstr.Substring(0, pathstr.Length - temp.Length);
            getprevbackups();

            SetStartup(true);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(pathstr+"\\App_Data\\settings.xml");
            XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("/settings");
            
            foreach (XmlNode node in nodeList)
            {
                bpath = node.SelectSingleNode("path").InnerText;
                backup_every = node.SelectSingleNode("backup_every").InnerText;
                keep = node.SelectSingleNode("keep").InnerText;
                
            }

            label2.Text = bpath;
            if (Convert.ToInt32(keep) == 2) radioButton6.Checked = true;
            else if (Convert.ToInt32(keep) == 3) radioButton7.Checked = true;
            else if (Convert.ToInt32(keep) == 5) radioButton8.Checked = true;
            else
            {
                textBox1.Text = Convert.ToInt32(keep).ToString();
                radioButton9.Checked = true;
            }
            Adjustevery();
        }

        private void Adjustevery()
        {
            if(backup_every == "1 Day") radioButton1.Checked=true;
            if (backup_every == "3 Days") radioButton2.Checked = true;
            if (backup_every == "1 Week") radioButton3.Checked = true;
            if (backup_every == "2 Weeks") radioButton4.Checked = true;
            if (backup_every == "Month") radioButton5.Checked = true;
           
        }

        private void mainfrm_Paint(object sender, PaintEventArgs e)
        {

 
        }

        private void tabPage1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void tabPage2_Paint(object sender, PaintEventArgs e)
        {
          
        }

        private void tabPage3_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
               
                bpath = folderBrowserDialog1.SelectedPath;
               /*
                HrDbLib cc = new HrDbLib();
               cc.qfields.Add("path");
               cc.qparams.Add("'"+bpath+"'");
               cc.qfilters.Add("ID>0");
               cc.UpdateRec("settings",1,1);
                */
                savesttings();
            }  
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
           
            /*

            if (radioButton1.Checked)
            {
                HrDbLib cc = new HrDbLib();
                cc.qfields.Add("backup_every");
                cc.qparams.Add("'" + radioButton1.Text + "'");
                cc.qfilters.Add("ID>0");
                cc.UpdateRec("settings", 1, 1);
            }
             */ 
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            backup_every = "3 Days";
            /*
            if (radioButton2.Checked)
            {
                HrDbLib cc = new HrDbLib();
                cc.qfields.Add("backup_every");
                cc.qparams.Add("'" + radioButton2.Text + "'");
                cc.qfilters.Add("ID>0");
                cc.UpdateRec("settings", 1, 1);
            }
             */

            savesttings();

        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            backup_every = "1 Week";
            /*
            if (radioButton3.Checked)
            {
                HrDbLib cc = new HrDbLib();
                cc.qfields.Add("backup_every");
                cc.qparams.Add("'" + radioButton3.Text + "'");
                cc.qfilters.Add("ID>0");
                cc.UpdateRec("settings", 1, 1);
            }
             */
            savesttings();

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            backup_every = "1 Day";
         /*
            if (radioButton1.Checked)
            {
                HrDbLib cc = new HrDbLib();
                cc.qfields.Add("backup_every");
                cc.qparams.Add("'" + radioButton1.Text + "'");
                cc.qfilters.Add("ID>0");
                cc.UpdateRec("settings", 1, 1);
            }
          */
            savesttings();
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            backup_every = "2 Weeks";
            /*
            if (radioButton4.Checked)
            {
                HrDbLib cc = new HrDbLib();
                cc.qfields.Add("backup_every");
                cc.qparams.Add("'" + radioButton4.Text + "'");
                cc.qfilters.Add("ID>0");
                cc.UpdateRec("settings", 1, 1);
            }
             */
            savesttings();

        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            backup_every = "Month";
            savesttings();
        }

        private void savesttings()
        {
            using (XmlWriter writer = XmlWriter.Create(pathstr + "\\App_Data\\settings.xml"))
            {
                writer.WriteStartElement("settings");
                writer.WriteElementString("path", bpath);
                writer.WriteElementString("backup_every", backup_every);
                writer.WriteElementString("keep", keep);
                writer.WriteEndElement();
                writer.Flush();
            }  
        }

        private void getprevbackups()
        {
            int i=0;
            dataGridView1.Rows.Clear();
            if (File.Exists(pathstr + "\\App_Data\\backups.xml"))
            { 
                using (var streamReader = new StreamReader(pathstr + "\\App_Data\\backups.xml"))
                {
                    String line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        i++;
                       string[] ss=line.Split('~');

                        dataGridView1.Rows.Add(ss[0], ss[1]);
                    }
                }
                   
             }
            bcount = i;
          }
             
     

        private void SetStartup(bool enable)
        {

         string pathstr = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            pathstr = pathstr.Substring(6, pathstr.Length - 6);
            string temp = "\\bin\\Debug";
            if (pathstr.LastIndexOf(temp) > 0) pathstr = pathstr.Substring(0, pathstr.Length - temp.Length);
            
            
            string user = Environment.UserDomainName + "\\"
            + Environment.UserName;

            // Create a security object that grants no access.
            RegistrySecurity mSec = new RegistrySecurity();

            RegistryAccessRule rule = new RegistryAccessRule(user,
            RegistryRights.FullControl,
            InheritanceFlags.ContainerInherit,
            PropagationFlags.None,
            AccessControlType.Allow);
            mSec.AddAccessRule(rule);


            string runKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
           // RegistryKey startupKey = Registry.LocalMachine.OpenSubKey(runKey);
            //RegistryKey startupKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");

            if (enable)
            {


                RegistryKey startupKey = Registry.LocalMachine.OpenSubKey(runKey, true);
                    startupKey.SetValue("3MAutoBackup", "\"" + pathstr + "\\AutomaticBackup.exe" + "\"");
                    startupKey.Close();

                    startupKey = Registry.CurrentUser.OpenSubKey(runKey, true);
                    startupKey.SetValue("3MAutoBackup", "\"" + pathstr + "\\AutomaticBackup.exe"+ "\"" );
                    startupKey.Close();
                    
                   
                
            }
            
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++) dataGridView1.Rows[i].Selected = false;
            dataGridView1.Rows[e.RowIndex].Selected = true;
            if (dataGridView1.Rows[e.RowIndex].Cells[1].Value != null) bakfile = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            else bakfile = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            List<string> bfileslist = new List<string>();
            
           // try{
            button3.Enabled = false;
            
                string bkpath =bpath;
                
               
                if (bakfile != "")
                {
                    DialogResult dialogResult = MessageBox.Show("Restore! Are your sure?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                    if (dialogResult == DialogResult.Yes)
                    {
                        // StopWindowsService();
                        progressBar2.Value = 30;
                        Application.DoEvents();
                        label3.Text = "Please wait...";
                        Application.DoEvents();
                        System.Threading.Thread.Sleep(3000);
                        //ziplib.ExtractZipFile(bkpath + "\\" + bakfile, "", @"C:\ProgramData");
                        string bak_path = bkpath + "\\" + bakfile.Replace(".zip", "");
                        string[] bfiles = Directory.GetFiles(bak_path);
                        DirectoryInfo info = new DirectoryInfo(@"C:\ProgramData\3M_CleanTraceDatabase");
                        bfileslist.Clear();
                        FileInfo[] files = info.GetFiles().OrderBy(p => p.CreationTime).ToArray();
                        foreach (FileInfo file in files)
                        {

                            bfileslist.Add(file.Name.ToString());
                        }

                        string mdfarc = "";
                        
                        for (int i = bfileslist.Count - 1; i >= 0;i-- )
                        {
                            if (bfileslist[i].ToLower().Contains(".mdf") && bfileslist[i].ToLower().Contains("spark_data") && mdfarc == "") mdfarc = bfileslist[i];
                         

                        }
                        string mdfarcs = "";
                        
                        for (int i = bfiles.Length - 1; i >= 0; i--)
                        {
                            if (bfiles[i].ToLower().Contains(".mdf") && bfiles[i].ToLower().Contains("spark_data") && mdfarcs == "") mdfarcs = bfiles[i];
                         }
                       // MessageBox.Show(bak_path);
                        //MessageBox.Show(mdfarcs.Replace(bak_path+"\\",""));
                        ///MessageBox.Show(mdfarc);
                        
                        //resoredata(bak_path, mdfarcs.Replace(bak_path + "\\", ""), @"C:\ProgramData\3M_CleanTraceDatabase", mdfarc);
                        copytables(bak_path, mdfarcs.Replace(bak_path + "\\", ""), @"C:\ProgramData\3M_CleanTraceDatabase");
                       
                        progressBar2.Value = 100;
                        label3.Text = "";
                        MessageBox.Show("Restored successfully", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //StartWindowsService();
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        //do something else
                    }
                }
                else MessageBox.Show("Please Select Backup firstly", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                button3.Enabled = true;
           /* }
            catch {
                MessageBox.Show("Program will exit to unlock files please run program again and do your restore","information",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                Application.Exit();
            }
              */
        }

        public static void StopWindowsService()
        {
            System.Diagnostics.Process.GetProcesses().Where(x => x.ProcessName == "MSSQLSERVER").ToList().ForEach(x => x.Kill());
            System.Diagnostics.Process.GetProcesses().Where(x => x.ProcessName == "MSSQLSERVER").ToList().ForEach(x => x.Kill());
          try
          {
              var process = System.Diagnostics.Process.GetProcesses().SingleOrDefault(p => p.ProcessName == "SQL Server (MSSQLSERVER)");
              process.WaitForExit();
            
          }
          catch (Exception)
          {
              //throw;
            
          }
            
            try
          {
              string serviceName = "MSSQLSERVER";
              ServiceController serviceController = new ServiceController(serviceName);
              TimeSpan timeout = TimeSpan.FromMilliseconds(1000);
              serviceController.Stop();
              serviceController.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
          }
          catch { }
        }

        public static void StartWindowsService()
        {
            string serviceName = "MSSQLSERVER";
            ServiceController serviceController = new ServiceController(serviceName);
            TimeSpan timeout = TimeSpan.FromMilliseconds(1000);
            serviceController.Start();
            serviceController.WaitForStatus(ServiceControllerStatus.Running, timeout);
        }

        private void button2_Click(object sender, EventArgs e)
        {

            try
            {
                label4.Text = "Please wait ...";
                button2.Enabled = false;
                /* HrDbLib cc = new HrDbLib();
                 cc.qfields.Add("path");
                 cc.qfields.Add("backup_every");
                 cc.qfilters.Add("ID=1");
                 cc.GetQData("settings", 2, 1);
                 string period = "";
                 string bpath = "";
                 string lastDate = "";
                 double days = 0;
                 if (cc.qresults.Count > 0)
                 {
                     string[] ss = cc.qresults[0].Split('~');
                     bpath = ss[0];
                     period = ss[1];
                 }
                 */
                DateTime thisDay = DateTime.Now;


                backupit(bpath, thisDay.ToString());
                getprevbackups();

                button2.Enabled = true;
                label4.Text = "";
            }
            catch
            {
                label4.Text = "";
                MessageBox.Show("Program will exit to unlock files please run program again and do your backup", "information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Application.Exit();
            }
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
            if (!Directory.Exists(backp+"\\"+bdir))
            {
                Directory.CreateDirectory(backp + "\\" + bdir);
            }

           
          if(mdfarc!="")   File.Copy(@"C:\ProgramData\3M_CleanTraceDatabase" + "\\" + mdfarc, backp + "\\" + bdir + "\\" + mdfarc);
          if (ldfarc != "") File.Copy(@"C:\ProgramData\3M_CleanTraceDatabase" + "\\" + ldfarc, backp + "\\" + bdir + "\\" + ldfarc);
             progressBar1.Value = 70;
        
            Application.DoEvents();
            System.Threading.Thread.Sleep(3000);
            progressBar1.Value = 100;
            Application.DoEvents();
            System.Threading.Thread.Sleep(10000);
            if(bcount==0)
            {
                using (StreamWriter writer = new StreamWriter(pathstr + "\\App_Data\\backups.xml"))
                {
                    writer.WriteLine(bdate.Replace('/', '_').Replace(":", "_").Replace(" ", "_") + ".zip"+"~"+bdate);
                    
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
               
                
                if((bcount+1)<=Convert.ToInt32(keep))
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
           
            getprevbackups();
            MessageBox.Show("Backup taken successfully","Information",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }
        private void DelExtra()
        {
            /*
            HrDbLib cc = new HrDbLib();
            cc.qfields.Add("path");
            cc.qfields.Add("keep");
            cc.qfilters.Add("ID=1");
            cc.GetQData("settings", 2, 1);
            string bpath = "";
            int keep = 2;
            if (cc.qresults.Count > 0)
            {
                string[] ss = cc.qresults[0].Split('~');
                bpath = ss[0];
                keep = Convert.ToInt32(ss[1]);
            }
            cc.qfields.Clear();
            cc.qfilters.Clear();
            cc.qparams.Clear();
            cc.qresults.Clear();

            cc.qfields.Add("backup");
            cc.qfilters.Add("ID>1 ORDER BY [ID]");
            cc.GetQData("backups", 1, 1);
            int count = 0;
            string bfile = "";
            if (cc.qresults.Count > 0)
            {
                count = cc.qresults.Count;
                bfile = cc.qresults[0];
            }
            if (count > keep)
            {
                File.Delete(bpath + "\\" + bfile);
                cc.qfields.Clear();
                cc.qfilters.Clear();
                cc.qparams.Clear();
                cc.qresults.Clear();

                cc.qfilters.Add("backup='" + bfile + "'");
                cc.DeleteRec("backups", 1);
            }
            */
        }

        private void mainfrm_FormClosing(object sender, FormClosingEventArgs e)
        {
         
        }

        private void button4_Click(object sender, EventArgs e)
        {

           
            
        }

        private void addtorun()
        {

            string pathstr = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            pathstr = pathstr.Substring(6, pathstr.Length - 6);
            string temp = "\\bin\\Debug";
            if (pathstr.LastIndexOf(temp) > 0) pathstr = pathstr.Substring(0, pathstr.Length - temp.Length);

            RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            key.SetValue("3MAutoBackup", "\"" + pathstr + "\\AutomaticBackup.exe" + "\"");
            key.Close();
        }

        private void mainfrm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            string ss = textBox1.Text;
            ss = (Convert.ToInt32(ss) + 1).ToString();
            textBox1.Text = ss;
            keep = textBox1.Text;
            savesttings();

        }

        private void button5_Click(object sender, EventArgs e)
        {
            string ss = textBox1.Text;
            if (Convert.ToInt32(ss) > 1)
            {
                ss = (Convert.ToInt32(ss) - 1).ToString();
                textBox1.Text = ss;
            }
            keep = textBox1.Text;
            savesttings();
        }

        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton9.Checked)
            {
/*                HrDbLib cc = new HrDbLib();
                cc.qfields.Add("keep");
                cc.qparams.Add(textBox1.Text);
                cc.qfilters.Add("ID>0");
                cc.UpdateRec("settings", 1, 1);
 */

                keep = textBox1.Text;
                savesttings();
            }
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton6.Checked)
            {
                /*
                HrDbLib cc = new HrDbLib();
                cc.qfields.Add("keep");
                cc.qparams.Add(radioButton6.Text.Replace(" Backups",""));
                cc.qfilters.Add("ID>0");
                cc.UpdateRec("settings", 1, 1);
                 */

                keep = radioButton6.Text.Replace(" Backups", "");
                savesttings();
            }
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton7.Checked)
            {
                /*
                HrDbLib cc = new HrDbLib();
                cc.qfields.Add("keep");
                cc.qparams.Add(radioButton7.Text.Replace(" Backups", ""));
                cc.qfilters.Add("ID>0");
                cc.UpdateRec("settings", 1, 1);
                 */

                keep = radioButton7.Text.Replace(" Backups", "");
                savesttings();
            }
        }

        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton8.Checked)
            {
                /*
                HrDbLib cc = new HrDbLib();
                cc.qfields.Add("keep");
                cc.qparams.Add(radioButton8.Text.Replace(" Backups", ""));
                cc.qfilters.Add("ID>0");
                cc.UpdateRec("settings", 1, 1);
                 */

                keep = radioButton8.Text.Replace(" Backups", "");
                savesttings();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", label2.Text);
        }
        private static bool IsFileLocked(string filePath)
        {
            bool lockStatus = false;
            try
            {
                using (FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    // File/Stream manipulating code here

                    lockStatus = fileStream.CanWrite;

                }
            }
            catch
            {
                //check here why it failed and ask user to retry if the file is in use.
                lockStatus = false;
            }
            return lockStatus;
        }
        public void GrantAccess(string fullPath)
        {
            DirectoryInfo dInfo = new DirectoryInfo(fullPath);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            dInfo.SetAccessControl(dSecurity);
        }
 
        
        public  void copytables(string sdir,string sdb , string distdir  )
        {
            GrantAccess(sdir);
            GrantAccess(distdir);
           // connString = "Data Source=.\\DESKTOP-32A7GCV;AttachDbFilename=D:\\3M_CleanTraceDatabase\\Spark_Data_19-06-2023_15_01_39.mdf;Integrated Security=True;Connect Timeout=30; User Instance=True";
           //connString = "Data Source=HANY-PC;Network Library=DBMSSOCN; Initial Catalog=attendance;User ID='att';Password='att123';";

            string connsource = "Data Source=" + Environment.MachineName.ToString() + "\\SPARK;AttachDbFilename=" + sdir + "\\" + sdb + ";Integrated Security=True;";

            string conndist = "Data Source=" + Environment.MachineName.ToString() + "\\SPARK;Database=Spark;Integrated Security=True;";
           SqlConnection source = new SqlConnection(connsource);
           // Create destination connection
           SqlConnection destination = new SqlConnection(conndist);
          /// copytable(source, destination, "AdhocEditMeasurement", 0);
  
           //copytable(source, destination, "ApplicationInformation",0);
           //copytable(source, destination, "ApplicationMaster",0);
          /// copytable(source, destination, "ArchivalMaster", 0);
          /// copytable(source, destination, "AuditLogMaster", 0);
           //copytable(source, destination, "Cities_Master",1);
          /// copytable(source, destination, "CommonTasks", 0);
           //copytable(source, destination, "CountryTimezoneMapping",1);
           //copytable(source, destination, "CustomCategory",1);
           ///////////////////////copytable(source, destination, "CustomParameterLocationMapping", 1);
          /// copytable(source, destination, "EditMeasurement",0);
          ////////////////////////////////////// copytable(source, destination, "EntityLocationMapping", 1);
          /// copytable(source, destination, "FailedLoginAttempts", 0);
           //copytable(source, destination, "FeatureGroup",1);
          /// copytable(source, destination, "LicenseTypeMaster", 0);
           //copytable(source, destination, "Menu",1);
          /// copytable(source, destination, "OrganizationMaster", 1);
           //copytable(source, destination, "PermissionMaster",1);
           //copytable(source, destination, "RegionMaster",0);
          /// copytable(source, destination, "ReportFilterMaster", 1);
           //copytable(source, destination, "ReportMaster",0);
          /// copytable(source, destination, "ResultScheduleMapping", 1);
           //copytable(source, destination, "RoleFeatureMaster",0);
           //copytable(source, destination, "RoleFeaturesGroup",0);
           //copytable(source, destination, "RoleMaster",0);
           //copytable(source, destination, "RolePermissionsForAudit",0);
          /// copytable(source, destination, "ServiceRunLog", 1);
           //copytable(source, destination, "States_Master",0);
          /// copytable(source, destination, "SupportedVersionMaster", 1);
       
          /// copytable(source, destination, "TestResultCustomParameterMapping", 1);
          /// copytable(source, destination, "TimezoneMaster", 1);
          /// copytable(source, destination, "UnitMaster", 1);
          ///// copytable(source, destination, "UserDashboardFilters", 1);
           //copytable(source, destination, "UserMaster",0);
           /////////////////////////////////////////copytable(source, destination, "UserTestPointFilters", 1);
          /// copytable(source, destination, "WorkFlowMaster", 0);
          
           //copytable(source, destination, "AuditLogDetails",0);
          /// copytable(source, destination, "CapaComments", 1);
           //copytable(source, destination, "CountryMaster",0);
          /// copytable(source, destination, "DeviceSyncDetails", 1);
          /// copytable(source, destination, "EditTestResultCustomParameterMapping", 1);
          /// copytable(source, destination, "FeatureMaster", 1);
           //copytable(source, destination, "LocationLevelMaster",0);
          
  
           //copytable(source, destination, "OrganizationCategoryMaster",0);
          /// copytable(source, destination, "OrganizationComments", 1);
           //copytable(source, destination, "OrganizationConfiguration",0);
          /// copytable(source, destination, "OrganizationImageMaster", 1);
           //copytable(source, destination, "OrganizationRoleMaster",0);
          /// copytable(source, destination, "OrganizationTestMethodMaster", 1);
           //copytable(source, destination, "OrganizationTypeMaster",0);
           //copytable(source, destination, "RolePermissions",0);
          /// copytable(source, destination, "ScheduleDefinitionMaster", 1);
          /// copytable(source, destination, "ScheduleMaster", 1);
           //copytable(source, destination, "SwabTypeMaster",0);
           //copytable(source, destination, "TestMethodMaster",0);
           //copytable(source, destination, "TestPlanTypeMaster",0);
       
          /// copytable(source, destination, "UserDashboardPreferences", 1);
           //copytable(source, destination, "UserPreferences",0);
          /// copytable(source, destination, "UserReportPreferences", 1);
          /// copytable(source, destination, "DashboardSPMapping", 1);
          ////////////////////////////////// copytable(source, destination, "LocationPreference", 1);
          
          /// copytable(source, destination, "OrganizationCustomParameterMaster", 1);
           //copytable(source, destination, "OrganizationRoleUserMapping",0);
          
          /// copytable(source, destination, "ReportLocationMapping", 1);
          /// copytable(source, destination, "SamplePlanSchedule", 1);
          /// copytable(source, destination, "ScheduleDaysOfMonth", 1);
          /// copytable(source, destination, "ScheduleDaysOfWeek", 1);
          /// copytable(source, destination, "ScheduleMonthsOfYear", 1);
          /// copytable(source, destination, "ScheduleWeeksOfMonth", 1);
 
         
          /// copytable(source, destination, "UserCountryMapping", 1);
          /// copytable(source, destination, "OrganizationCustomParameterVersions", 1);
         //  copytable(source, destination, "TestPlanTestPointMapping", 0);
         

           copytable(source, destination, "AdhocTestPlanResult", 0);
           copytable(source, destination, "TestPlanResult", 0);
           copytable(source, destination, "AdhocTestPointResult", 0);
           //copytable(source, destination, "LocationMaster", 0);
           //copytable(source, destination, "LuminometerMaster", 0);
           copytable(source, destination, "TestPointResult", 0);
           //copytable(source, destination, "LuminometerPlantMapping", 0);  
        
           //copytable(source, destination, "OrganizationTestMethodVersions", 0);
           copytable(source, destination, "TestPointMaster", 0);
            copytable(source, destination, "TestPlanMaster", 0);

           copytable(source, destination, "TestPointTestMethodMapping", 0);
           copytable(source, destination, "TestPlanTestPointMapping", 0);

           copytable(source, destination, "TestPlanUserMapping", 0);
           //copytable(source, destination, "TestPointCustomParameterMapping", 0); 





  








        
          

            


        }


        private void copytable(SqlConnection s,SqlConnection d, string tname,int flag)
        {
           SqlCommand cmd = new SqlCommand("DELETE FROM " + tname, d);
           if (tname == "TestPointMaster") cmd = new SqlCommand("DELETE FROM TestPointTestMethodMapping ;DELETE FROM TestPlanTestPointMapping; DELETE FROM " + tname, d);
           if (tname == "TestPlanMaster") cmd = new SqlCommand("DELETE FROM TestPlanUserMapping ; DELETE FROM " + tname, d);
           
            // Open source and destination connections.
                // source.Open();
       
                d.Open();
                if (flag < 1)
                {

                    cmd.ExecuteNonQuery();
                }
                d.Close();

           
                // cmd = new SqlCommand("SELECT * FROM "+tname, s);
                // Execute reader
                //s.Open();
                SqlDataAdapter dap = new SqlDataAdapter("SELECT * FROM " + tname, s);
                DataSet ds = new DataSet();
                dap.Fill(ds);

                //SqlDataReader reader = cmd.ExecuteReader();
                // Create SqlBulkCopy
                SqlBulkCopy bulkData = new SqlBulkCopy(d);
                d.Open();
                // Set destination table name
                bulkData.DestinationTableName = tname;
                // Write data
                bulkData.WriteToServer(ds.Tables[0]);
                // Close objects

                bulkData.Close();
                d.Close();
                s.Close();
        }
        private void resoredata(string srcdir, string srcdb, string distdir, string distdb)
        {
            HrDbLib cc = new HrDbLib(srcdir, srcdb,1);
            HrDbLib dd = new HrDbLib(distdir, distdb,0);
            string sdate = "";
                cc.qfields.Clear();
                cc.qparams.Clear();
                cc.qresults.Clear();
                cc.qfilters.Clear();
            cc.qfields.Add("Id");
            cc.qfields.Add("AdhocResultId");
            cc.qfields.Add("AdhocMeasurementId");
            cc.qfields.Add("ReasonForChange");
            cc.qfields.Add("OldResult");
            cc.qfields.Add("OldResultValue");
            cc.qfields.Add("NewResult");
            cc.qfields.Add("NewResultValue");
            cc.qfields.Add("OldCapaComments");
            cc.qfields.Add("NewCapaComments");
            cc.qfields.Add("OldTester");
            cc.qfields.Add("NewTester");
            cc.qfields.Add("EditDate");
            cc.qfields.Add("EditedBy");
            cc.qfilters.Add("Id>0");
            cc.GetQData("AdhocEditMeasurement", cc.qfields.Count, 1);

     for (int i = 0; i < cc.qresults.Count; i++)
            {
                dd.qfields.Clear();
                dd.qparams.Clear();
                dd.qresults.Clear();
                dd.qfilters.Clear();
           string[] ss = cc.qresults[i].Split('~');
           dd.qfields.Add("Id"); dd.qparams.Add("'" + ss[0] + "'");
       dd.qfields.Add("AdhocResultId");dd.qparams.Add("'"+ss[1]+"'");
       dd.qfields.Add("AdhocMeasurementId");dd.qparams.Add("'"+ss[2]+"'");
       dd.qfields.Add("ReasonForChange");dd.qparams.Add("'"+ss[3]+"'");
       dd.qfields.Add("OldResult");dd.qparams.Add("'"+ss[4]+"'");
       dd.qfields.Add("OldResultValue");dd.qparams.Add("'"+ss[5]+"'");
       dd.qfields.Add("NewResult");dd.qparams.Add("'"+ss[6]+"'");
       dd.qfields.Add("NewResultValue");dd.qparams.Add("'"+ss[7]+"'");
       dd.qfields.Add("OldCapaComments");dd.qparams.Add("'"+ss[8]+"'");
       dd.qfields.Add("NewCapaComments");dd.qparams.Add("'"+ss[9]+"'");
       dd.qfields.Add("OldTester");dd.qparams.Add("'"+ss[10]+"'");
       dd.qfields.Add("NewTester");dd.qparams.Add("'"+ss[11]+"'");
       if (ss[12]!="") sdate = Convert.ToDateTime(ss[12].Replace("م", "PM").Replace("ص", "AM")).ToString("yyyy-MM-dd");
       else sdate="";
         dd.qfields.Add("EditDate");dd.qparams.Add("'"+sdate+"'");
       dd.qfields.Add("EditedBy");dd.qparams.Add("'"+ss[13]+"'");
       dd.InsertRec("AdhocEditMeasurement", dd.qfields.Count);
     }
     cc.qfields.Clear();
     cc.qparams.Clear();
     cc.qresults.Clear();
     cc.qfilters.Clear();

     cc.qfields.Add("AdhocResultId");
     cc.qfields.Add("OrganizationId");
     cc.qfields.Add("TestPlanId");
     cc.qfields.Add("TestPlanVersion");
     cc.qfields.Add("TestPlanName");
     cc.qfields.Add("LocationId");
     cc.qfields.Add("LocationVersion");
     cc.qfields.Add("Status");
     cc.qfields.Add("OpenedDate");
     cc.qfields.Add("OpenedBy");
     cc.qfields.Add("DeviceId");
     cc.qfields.Add("LastEditedBy");
     cc.qfields.Add("LastEditedDate");

     cc.qfilters.Add("AdhocResultId>0");
     cc.GetQData("AdhocTestPlanResult", cc.qfields.Count, 1);
     for (int i = 0; i < cc.qresults.Count; i++)
     {
         dd.qfields.Clear();
         dd.qparams.Clear();
         dd.qresults.Clear();
         dd.qfilters.Clear();
         string[] ss = cc.qresults[i].Split('~');
         dd.qfields.Add("AdhocResultId"); dd.qparams.Add("'" + ss[0] + "'");
         dd.qfields.Add("OrganizationId"); dd.qparams.Add("'" + ss[1] + "'");
         dd.qfields.Add("TestPlanId"); dd.qparams.Add("'" + ss[2] + "'");
         dd.qfields.Add("TestPlanVersion"); dd.qparams.Add("'" + ss[3] + "'");
         dd.qfields.Add("TestPlanName"); dd.qparams.Add("'" + ss[4] + "'");
         dd.qfields.Add("LocationId"); dd.qparams.Add("'" + ss[5] + "'");
         dd.qfields.Add("LocationVersion"); dd.qparams.Add("'" + ss[6] + "'");
         dd.qfields.Add("Status"); dd.qparams.Add("'" + ss[7] + "'");
          if (ss[8]!="") sdate = Convert.ToDateTime(ss[8].Replace("م", "PM").Replace("ص", "AM")).ToString("yyyy-MM-dd");
         else sdate="";
         dd.qfields.Add("OpenedDate"); dd.qparams.Add("'" +sdate + "'");
         dd.qfields.Add("OpenedBy"); dd.qparams.Add("'" + ss[9] + "'");
         dd.qfields.Add("DeviceId"); dd.qparams.Add("'" + ss[10] + "'");
         dd.qfields.Add("LastEditedBy"); dd.qparams.Add("'" + ss[11] + "'");
         if (ss[12]!="") sdate = Convert.ToDateTime(ss[12].Replace("م", "PM").Replace("ص", "AM")).ToString("yyyy-MM-dd");
         else sdate="";
         dd.qfields.Add("LastEditedDate"); dd.qparams.Add("'" + sdate + "'");
         dd.InsertRec("AdhocTestPlanResult", dd.qfields.Count);
     }
     cc.qfields.Clear();
     cc.qparams.Clear();
     cc.qresults.Clear();
     cc.qfilters.Clear();
     cc.qfields.Add("AdhocResultId");
     cc.qfields.Add("AdhocMeasurementId");
     cc.qfields.Add("TestPointId");
     cc.qfields.Add("TestPointVersion");
     cc.qfields.Add("TestPointName");
     cc.qfields.Add("TestPointLocationId");
     cc.qfields.Add("LocationName");
     cc.qfields.Add("TestMethodId");
     cc.qfields.Add("TestMethodVersion");
     cc.qfields.Add("TestMethodName");
     cc.qfields.Add("TestType");
     cc.qfields.Add("IsAdhoc");
     cc.qfields.Add("IsRetest");
     cc.qfields.Add("Status");
     cc.qfields.Add("Result");
     cc.qfields.Add("ResultValue");
     cc.qfields.Add("ResultDate");
     cc.qfields.Add("IsEdited");
     cc.qfields.Add("ResultTakenBy");
     cc.qfields.Add("ResultTakenByName");
     cc.qfields.Add("OriginalAdhocMeasurementId");
     cc.qfields.Add("CapaComments");
     cc.qfields.Add("CreatedBy");
     cc.qfields.Add("CreatedDate");
     cc.qfields.Add("LastEditDate");
     cc.qfields.Add("LastEditedBy");
     cc.qfields.Add("ATPPassThreshold");
     cc.qfields.Add("ATPFailThreshold");
     cc.qfields.Add("ThresholdType");
     cc.qfields.Add("UnitId");
     cc.qfields.Add("UnitName");
     cc.qfields.Add("IsMapped");
     cc.qfields.Add("Is3MSwab");
     cc.qfields.Add("IsFinal");
     cc.qfields.Add("TestOrder");
     cc.qfields.Add("MinRange");
     cc.qfields.Add("MaxRange");
     cc.qfilters.Add("AdhocResultId>0");
      cc.GetQData("AdhocTestPointResult", cc.qfields.Count, 1);
     for (int i = 0; i < cc.qresults.Count; i++)
     {
         dd.qfields.Clear();
         dd.qparams.Clear();
         dd.qresults.Clear();
         dd.qfilters.Clear();
         string[] ss = cc.qresults[i].Split('~');
         dd.qfields.Add("AdhocResultId"); dd.qparams.Add("'" + ss[0] + "'");
         dd.qfields.Add("AdhocMeasurementId"); dd.qparams.Add("'" + ss[1] + "'");
         dd.qfields.Add("TestPointId"); dd.qparams.Add("'" + ss[2] + "'");
         dd.qfields.Add("TestPointVersion"); dd.qparams.Add("'" + ss[3] + "'");
         dd.qfields.Add("TestPointName"); dd.qparams.Add("'" + ss[4] + "'");
         dd.qfields.Add("TestPointLocationId"); dd.qparams.Add("'" + ss[5] + "'");
         dd.qfields.Add("LocationName"); dd.qparams.Add("'" + ss[6] + "'");
         dd.qfields.Add("TestMethodId"); dd.qparams.Add("'" + ss[7] + "'");
         dd.qfields.Add("TestMethodVersion"); dd.qparams.Add("'" + ss[8] + "'");
         dd.qfields.Add("TestMethodName"); dd.qparams.Add("'" + ss[9] + "'");
         dd.qfields.Add("TestType"); dd.qparams.Add("'" + ss[10] + "'");
         dd.qfields.Add("IsAdhoc"); dd.qparams.Add("'" + ss[11] + "'");
         dd.qfields.Add("IsRetest"); dd.qparams.Add("'" + ss[12] + "'");
         dd.qfields.Add("Status"); dd.qparams.Add("'" + ss[11] + "'");
         dd.qfields.Add("Result"); dd.qparams.Add("'" + ss[13] + "'");
         dd.qfields.Add("ResultValue"); dd.qparams.Add("'" + ss[14] + "'");
          if (ss[15]!="") sdate = Convert.ToDateTime(ss[15].Replace("م", "PM").Replace("ص", "AM")).ToString("yyyy-MM-dd");
          else sdate="";
         dd.qfields.Add("ResultDate"); dd.qparams.Add("'" + sdate + "'");
         dd.qfields.Add("IsEdited"); dd.qparams.Add("'" + ss[16] + "'");
         dd.qfields.Add("ResultTakenBy"); dd.qparams.Add("'" + ss[17] + "'");
         dd.qfields.Add("ResultTakenByName"); dd.qparams.Add("'" + ss[18] + "'");
         dd.qfields.Add("OriginalAdhocMeasurementId"); dd.qparams.Add("'" + ss[19] + "'");
         dd.qfields.Add("CapaComments"); dd.qparams.Add("'" + ss[20] + "'");
         dd.qfields.Add("CreatedBy"); dd.qparams.Add("'" + ss[21] + "'");
        if (ss[22]!="") sdate = Convert.ToDateTime(ss[22].Replace("م", "PM").Replace("ص", "AM")).ToString("yyyy-MM-dd");
        else sdate="";
         dd.qfields.Add("CreatedDate"); dd.qparams.Add("'" + sdate + "'");
          if (ss[23]!="") sdate = Convert.ToDateTime(ss[23].Replace("م", "PM").Replace("ص", "AM")).ToString("yyyy-MM-dd");
        else sdate="";
         dd.qfields.Add("LastEditDate"); dd.qparams.Add("'" + sdate + "'");
         dd.qfields.Add("LastEditedBy"); dd.qparams.Add("'" + ss[24] + "'");
         dd.qfields.Add("ATPPassThreshold"); dd.qparams.Add("'" + ss[25] + "'");
         dd.qfields.Add("ATPFailThreshold"); dd.qparams.Add("'" + ss[26] + "'");
         dd.qfields.Add("ThresholdType"); dd.qparams.Add("'" + ss[27] + "'");
         dd.qfields.Add("UnitId"); dd.qparams.Add("'" + ss[28] + "'");
         dd.qfields.Add("UnitName"); dd.qparams.Add("'" + ss[29] + "'");
         dd.qfields.Add("IsMapped"); dd.qparams.Add("'" + ss[30] + "'");
         dd.qfields.Add("Is3MSwab"); dd.qparams.Add("'" + ss[31] + "'");
         dd.qfields.Add("IsFinal"); dd.qparams.Add("'" + ss[32] + "'");
         dd.qfields.Add("TestOrder"); dd.qparams.Add("'" + ss[33] + "'");
         dd.qfields.Add("MinRange"); dd.qparams.Add("'" + ss[34] + "'");
         dd.qfields.Add("MaxRange"); dd.qparams.Add("'" + ss[35] + "'");
         dd.InsertRec("AdhocTestPointResult", dd.qfields.Count);
     }

     cc.qfields.Clear();
     cc.qparams.Clear();
     cc.qresults.Clear();
     cc.qfilters.Clear();

     cc.qfields.Add("Id");
     cc.qfields.Add("ResultId");
     cc.qfields.Add("MeasurementId");
     cc.qfields.Add("ReasonForChange");
     cc.qfields.Add("OldResult");
     cc.qfields.Add("OldResultValue");
     cc.qfields.Add("NewResult");
     cc.qfields.Add("NewResultValue");
     cc.qfields.Add("OldCapaComments");
     cc.qfields.Add("NewCapaComments");
     cc.qfields.Add("OldTester");
     cc.qfields.Add("NewTester");
     cc.qfields.Add("EditDate");
     cc.qfields.Add("EditedBy");
     cc.qfilters.Add("Id>0");

     cc.GetQData("EditMeasurement", cc.qfields.Count, 1);
     for (int i = 0; i < cc.qresults.Count; i++)
     {
         dd.qfields.Clear();
         dd.qparams.Clear();
         dd.qresults.Clear();
         dd.qfilters.Clear();
         string[] ss = cc.qresults[i].Split('~');
         dd.qfields.Add("Id"); dd.qparams.Add("'" + ss[0] + "'");
         dd.qfields.Add("ResultId"); dd.qparams.Add("'" + ss[1] + "'");
         dd.qfields.Add("MeasurementId"); dd.qparams.Add("'" + ss[2] + "'");
         dd.qfields.Add("ReasonForChange"); dd.qparams.Add("'" + ss[3] + "'");
         dd.qfields.Add("OldResult"); dd.qparams.Add("'" + ss[4] + "'");
         dd.qfields.Add("OldResultValue"); dd.qparams.Add("'" + ss[5] + "'");
         dd.qfields.Add("NewResult"); dd.qparams.Add("'" + ss[6] + "'");
         dd.qfields.Add("NewResultValue"); dd.qparams.Add("'" + ss[7] + "'");
         dd.qfields.Add("OldCapaComments"); dd.qparams.Add("'" + ss[8] + "'");
         dd.qfields.Add("NewCapaComments"); dd.qparams.Add("'" + ss[9] + "'");
         dd.qfields.Add("OldTester"); dd.qparams.Add("'" + ss[10] + "'");
         dd.qfields.Add("NewTester"); dd.qparams.Add("'" + ss[11] + "'");
          if (ss[12]!="") sdate = Convert.ToDateTime(ss[12].Replace("م", "PM").Replace("ص", "AM")).ToString("yyyy-MM-dd");
        else sdate="";
         dd.qfields.Add("EditDate"); dd.qparams.Add("'" + sdate + "'");
         dd.qfields.Add("EditedBy"); dd.qparams.Add("'" + ss[13] + "'");
         dd.InsertRec("EditMeasurement", dd.qfields.Count);
     }

     cc.qfields.Clear();
     cc.qparams.Clear();
     cc.qresults.Clear();
     cc.qfilters.Clear();

     cc.qfields.Add("EditId");
     cc.qfields.Add("Id");
     cc.qfields.Add("OldOrganizationCategoryId");
     cc.qfields.Add("NewOrganizationCategoryId");
     cc.qfields.Add("OldParameterId");
     cc.qfields.Add("NewParameterId");
     cc.qfields.Add("EditedBy");
     cc.qfields.Add("EditedDate");
     cc.qfilters.Add("Id>0");

     cc.GetQData("EditTestResultCustomParameterMapping", cc.qfields.Count, 1);
     for (int i = 0; i < cc.qresults.Count; i++)
     {
         dd.qfields.Clear();
         dd.qparams.Clear();
         dd.qresults.Clear();
         dd.qfilters.Clear();
         string[] ss = cc.qresults[i].Split('~');
         dd.qfields.Add("EditId"); dd.qparams.Add("'" + ss[0] + "'");
         dd.qfields.Add("Id"); dd.qparams.Add("'" + ss[1] + "'");
         dd.qfields.Add("OldOrganizationCategoryId"); dd.qparams.Add("'" + ss[2] + "'");
         dd.qfields.Add("NewOrganizationCategoryId"); dd.qparams.Add("'" + ss[3] + "'");
         dd.qfields.Add("OldParameterId"); dd.qparams.Add("'" + ss[4] + "'");
         dd.qfields.Add("NewParameterId"); dd.qparams.Add("'" + ss[5] + "'");
         dd.qfields.Add("EditedBy"); dd.qparams.Add("'" + ss[6] + "'");
          if (ss[7]!="") sdate = Convert.ToDateTime(ss[7].Replace("م", "PM").Replace("ص", "AM")).ToString("yyyy-MM-dd");
        else sdate="";
         dd.qfields.Add("EditedDate"); dd.qparams.Add("'" + sdate + "'");
         dd.InsertRec("EditTestResultCustomParameterMapping", dd.qfields.Count);

     }


     cc.qfields.Clear();
     cc.qparams.Clear();
     cc.qresults.Clear();
     cc.qfilters.Clear();


     cc.qfields.Add("ResultId");
     cc.qfields.Add("OrganizationId");
     cc.qfields.Add("TestPlanId");
     cc.qfields.Add("TestPlanVersion");
     cc.qfields.Add("TestPlanName");
     cc.qfields.Add("LocationId");
     cc.qfields.Add("LocationVersion");
     cc.qfields.Add("Status");
     cc.qfields.Add("OpenedDate");
     cc.qfields.Add("OpenedBy");
     cc.qfields.Add("DeviceId");
     cc.qfields.Add("LastEditDate");
     cc.qfields.Add("LastEditedBy");
     cc.qfields.Add("ImportId");
     cc.qfilters.Add("ResultId>0");

     cc.GetQData("TestPlanResult", cc.qfields.Count, 1);
     for (int i = 0; i < cc.qresults.Count; i++)
     {
         dd.qfields.Clear();
         dd.qparams.Clear();
         dd.qresults.Clear();
         dd.qfilters.Clear();
         string[] ss = cc.qresults[i].Split('~');
         for(int j=0 ;j< ss.Length;j++) if(ss[j]=="") ss[j]="0"; 
         dd.qfields.Add("ResultId"); dd.qparams.Add( ss[0] );
         dd.qfields.Add("OrganizationId"); dd.qparams.Add( ss[1]  );
         dd.qfields.Add("TestPlanId"); dd.qparams.Add( ss[2] );
         dd.qfields.Add("TestPlanVersion"); dd.qparams.Add( ss[3] );
         dd.qfields.Add("TestPlanName"); dd.qparams.Add("'" + ss[4] + "'");
         dd.qfields.Add("LocationId"); dd.qparams.Add(ss[5]);
         dd.qfields.Add("LocationVersion"); dd.qparams.Add( ss[6] );
         dd.qfields.Add("Status"); dd.qparams.Add( ss[7] );
          if (ss[8]!="") sdate = Convert.ToDateTime(ss[8].Replace("م", "PM").Replace("ص", "AM")).ToString("yyyy-MM-dd");
        else sdate="";
         dd.qfields.Add("OpenedDate"); dd.qparams.Add("'" + sdate + "'");

         dd.qfields.Add("OpenedBy"); dd.qparams.Add(  ss[9] );
         dd.qfields.Add("DeviceId"); dd.qparams.Add("'" + ss[10] + "'");
        // dd.qfields.Add("LastEditDate"); dd.qparams.Add("'" + ss[11] + "'");
        dd.qfields.Add("LastEditedBy"); dd.qparams.Add("'" + ss[12] + "'");
         dd.qfields.Add("ImportId"); dd.qparams.Add("'" + ss[13] + "'");
         dd.InsertRecX("TestPlanResult", dd.qfields.Count);

     }


     cc.qfields.Clear();
     cc.qparams.Clear();
     cc.qresults.Clear();
     cc.qfilters.Clear();

            cc.qfields.Add("ResultId");
            cc.qfields.Add("MeasurementId");
            cc.qfields.Add("TestPointId");
            cc.qfields.Add("TestPointVersion");
            cc.qfields.Add("TestPointName");
            cc.qfields.Add("TestPointLocationId");
            cc.qfields.Add("LocationName");
            cc.qfields.Add("TestMethodId");
            cc.qfields.Add("TestMethodVersion");
            cc.qfields.Add("TestMethodName");
            cc.qfields.Add("TestType");
            cc.qfields.Add("IsRetest");
            cc.qfields.Add("IsAdhoc");
            cc.qfields.Add("Status");
            cc.qfields.Add("Result");
            cc.qfields.Add("ResultValue");
            cc.qfields.Add("ResultDate");
            cc.qfields.Add("IsEdited");
            cc.qfields.Add("ResultTakenBy");
            cc.qfields.Add("ResultTakenByName");
            cc.qfields.Add("OriginalMeasurementId");
            cc.qfields.Add("CapaComments");
            cc.qfields.Add("CreatedDate");
            cc.qfields.Add("CreatedBy");
            cc.qfields.Add("LastEditDate");
            cc.qfields.Add("LastEditedBy");
            cc.qfields.Add("ATPPassThreshold");
            cc.qfields.Add("ATPFailThreshold");
            cc.qfields.Add("ThresholdType");
            cc.qfields.Add("UnitId");
            cc.qfields.Add("UnitName");
            cc.qfields.Add("ReasonToAdd");
            cc.qfields.Add("Is3MSwab");
            cc.qfields.Add("IsFinal");
            cc.qfields.Add("TestOrder");
            cc.qfields.Add("MinRange");
            cc.qfields.Add("MaxRange");
            cc.qfilters.Add("ResultId>0");
            cc.GetQData("TestPointResult", cc.qfields.Count, 1);


            for (int i = 0; i < cc.qresults.Count; i++)
            {
                dd.qfields.Clear();
                dd.qparams.Clear();
                dd.qresults.Clear();
                dd.qfilters.Clear();

                string[] ss = cc.qresults[i].Split('~');
                dd.qfields.Add("ResultId"); dd.qparams.Add("'" + ss[0] + "'");
                dd.qfields.Add("MeasurementId"); dd.qparams.Add("'" + ss[1] + "'");
                dd.qfields.Add("TestPointId"); dd.qparams.Add("'" + ss[2] + "'");
                dd.qfields.Add("TestPointVersion"); dd.qparams.Add("'" + ss[3] + "'");
                dd.qfields.Add("TestPointName"); dd.qparams.Add("'" + ss[4] + "'");
                dd.qfields.Add("TestPointLocationId"); dd.qparams.Add("'" + ss[5] + "'");
                dd.qfields.Add("LocationName"); dd.qparams.Add("'" + ss[6] + "'");
                dd.qfields.Add("TestMethodId"); dd.qparams.Add("'" + ss[7] + "'");
                dd.qfields.Add("TestMethodVersion"); dd.qparams.Add("'" + ss[8] + "'");
                dd.qfields.Add("TestMethodName"); dd.qparams.Add("'" + ss[9] + "'");
                dd.qfields.Add("TestType"); dd.qparams.Add("'" + ss[10] + "'");
                dd.qfields.Add("IsRetest"); dd.qparams.Add("'" + ss[11] + "'");
                dd.qfields.Add("IsAdhoc"); dd.qparams.Add("'" + ss[12] + "'");
                dd.qfields.Add("Status"); dd.qparams.Add("'" + ss[13] + "'");
                dd.qfields.Add("Result"); dd.qparams.Add("'" + ss[14] + "'");
                dd.qfields.Add("ResultValue"); dd.qparams.Add("'" + ss[15] + "'");
                 if (ss[16]!="") sdate = Convert.ToDateTime(ss[16].Replace("م", "PM").Replace("ص", "AM")).ToString("yyyy-MM-dd");
        else sdate="";
                dd.qfields.Add("ResultDate"); dd.qparams.Add("'" + sdate+ "'");
                dd.qfields.Add("IsEdited"); dd.qparams.Add("'" + ss[17] + "'");
                dd.qfields.Add("ResultTakenBy"); dd.qparams.Add("'" + ss[18] + "'");
                dd.qfields.Add("ResultTakenByName"); dd.qparams.Add("'" + ss[19] + "'");
                dd.qfields.Add("OriginalMeasurementId"); dd.qparams.Add("'" + ss[20] + "'");
                dd.qfields.Add("CapaComments"); dd.qparams.Add("'" + ss[21] + "'");
                 if (ss[22]!="") sdate = Convert.ToDateTime(ss[22].Replace("م", "PM").Replace("ص", "AM")).ToString("yyyy-MM-dd");
        else sdate="";
                dd.qfields.Add("CreatedDate"); dd.qparams.Add("'" + sdate + "'");
                dd.qfields.Add("CreatedBy"); dd.qparams.Add("'" + ss[23] + "'");
                 if (ss[24]!="") sdate = Convert.ToDateTime(ss[24].Replace("م", "PM").Replace("ص", "AM")).ToString("yyyy-MM-dd");
        else sdate="";
                dd.qfields.Add("LastEditDate"); dd.qparams.Add("'" + ss[24] + "'");
                dd.qfields.Add("LastEditedBy"); dd.qparams.Add("'" + ss[25] + "'");
                dd.qfields.Add("ATPPassThreshold"); dd.qparams.Add("'" + ss[26] + "'");
                dd.qfields.Add("ATPFailThreshold"); dd.qparams.Add("'" + ss[27] + "'");
                dd.qfields.Add("ThresholdType"); dd.qparams.Add("'" + ss[28] + "'");
                dd.qfields.Add("UnitId"); dd.qparams.Add("'" + ss[29] + "'");
                dd.qfields.Add("UnitName"); dd.qparams.Add("'" + ss[30] + "'");
                dd.qfields.Add("ReasonToAdd"); dd.qparams.Add("'" + ss[31] + "'");
                dd.qfields.Add("Is3MSwab"); dd.qparams.Add("'" + ss[32] + "'");
                dd.qfields.Add("IsFinal"); dd.qparams.Add("'" + ss[33] + "'");
                dd.qfields.Add("TestOrder"); dd.qparams.Add("'" + ss[34] + "'");
                dd.qfields.Add("MinRange"); dd.qparams.Add("'" + ss[35] + "'");
                dd.qfields.Add("MaxRange"); dd.qparams.Add("'" + ss[36] + "'");
                dd.InsertRec("TestPointResult", dd.qfields.Count);
            }
            cc.qfields.Clear();
            cc.qparams.Clear();
            cc.qresults.Clear();
            cc.qfilters.Clear();

            cc.qfields.Add("ResultId");
            cc.qfields.Add("MeasurementId");
            cc.qfields.Add("OrganizationCategoryId");
            cc.qfields.Add("ParameterId");
            cc.qfields.Add("ParameterVersion");
            cc.qfields.Add("EditedBy");
            cc.qfields.Add("EditedDate");
            cc.qfilters.Add("ResultId>0");
            cc.GetQData("TestResultCustomParameterMapping", cc.qfields.Count, 1);
            for (int i = 0; i < cc.qresults.Count; i++)
            {
                dd.qfields.Clear();
                dd.qparams.Clear();
                dd.qresults.Clear();
                dd.qfilters.Clear();

                string[] ss = cc.qresults[i].Split('~');
                dd.qfields.Add("ResultId"); dd.qparams.Add("'" + ss[0] + "'");
                dd.qfields.Add("MeasurementId"); dd.qparams.Add("'" + ss[1] + "'");
                dd.qfields.Add("OrganizationCategoryId"); dd.qparams.Add("'" + ss[2] + "'");
                dd.qfields.Add("ParameterId"); dd.qparams.Add("'" + ss[3] + "'");
                dd.qfields.Add("ParameterVersion"); dd.qparams.Add("'" + ss[4] + "'");
                dd.qfields.Add("EditedBy"); dd.qparams.Add("'" + ss[5] + "'");
                 if (ss[6]!="") sdate = Convert.ToDateTime(ss[6].Replace("م", "PM").Replace("ص", "AM")).ToString("yyyy-MM-dd");
        else sdate="";
                dd.qfields.Add("EditedDate"); dd.qparams.Add("'" + sdate + "'");
                dd.InsertRec("TestPointResult", dd.qfields.Count);

            }


        }
    }
}
