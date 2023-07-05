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
using System.Xml;
using System.Threading.Tasks;
using System.Security.Principal;
using System.Security.AccessControl;

namespace _3m_db_backup
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mainfrm ff = new mainfrm();
            ff.ShowDialog();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetEveryoneAccess(@"C:\ProgramData\3M_CleanTraceDatabase");
      
           string pathstr = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            pathstr = pathstr.Substring(6, pathstr.Length - 6);
            string temp = "\\bin\\Debug";
            if (pathstr.LastIndexOf(temp) > 0) pathstr = pathstr.Substring(0, pathstr.Length - temp.Length);

            string winpath = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        
            if (pathstr.Substring(0, 2).ToLower() == winpath.Substring(0, 2).ToLower())
            {
                MessageBox.Show("This Application shouldn't be installed in the same drive of windows. Please uninstall the application and reinstall it in another drive", "warning", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                Application.Exit();
            }
            else
            {
                   

                if (!File.Exists(pathstr + "\\App_Data\\settings.xml"))
                {
                    using (XmlWriter writer = XmlWriter.Create(pathstr + "\\App_Data\\settings.xml"))
                    {
                        writer.WriteStartElement("settings");
                        writer.WriteElementString("path", pathstr+"\\App_Data");
                        writer.WriteElementString("backup_every", "1 Day");
                        writer.WriteElementString("keep", "2");
                        writer.WriteEndElement();
                        writer.Flush();
                    }

                  System.Diagnostics.Process.Start(pathstr + "\\AutomaticBackup.exe");
                }


          }
        }
         private bool SetEveryoneAccess(string dirName)
        {
           string _lastError = "";
            try
            {
                // Make sure directory exists
                if (Directory.Exists(dirName) == false)
                    throw new Exception(string.Format("Directory {0} does not exist, so permissions cannot be set.", dirName));

                // Get directory access info
                DirectoryInfo dinfo = new DirectoryInfo(dirName);
                DirectorySecurity dSecurity = dinfo.GetAccessControl();

                // Add the FileSystemAccessRule to the security settings. 
                dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));

                // Set the access control
                dinfo.SetAccessControl(dSecurity);

                _lastError = String.Format("Everyone FullControl Permissions were set for directory {0}", dirName);

                return true;

            } catch (Exception ex)
            {
                _lastError = ex.Message;
                return false;
            }


        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            if (!Directory.Exists(@"C:\ProgramData\3M_CleanTraceDatabase"))
            {
                MessageBox.Show("Make sure that 3m-clean-trace-hygiene-management is installed", "information",MessageBoxButtons.OK,MessageBoxIcon.Hand);
                Application.Exit();
            }
          
        }

        private void button2_Click(object sender, EventArgs e)
        {
            

        }
    }
}
