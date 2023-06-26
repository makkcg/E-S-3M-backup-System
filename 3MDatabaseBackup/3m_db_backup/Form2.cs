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
namespace _3m_db_backup
{
    public partial class Form2 : Form
    {
        int index = 0;
        string pathstr = "";
        public Form2()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (index < 100) { ++index; progressBar1.Value = index; Application.DoEvents(); }
            else this.Close();
           
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            
             
        }
    }
}
