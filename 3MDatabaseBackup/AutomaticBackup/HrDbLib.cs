using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;


public class HrDbLib
    {
       
        public  string pathstr;
        public  string connString;
        public  List<string> qfields = new List<string>();
        public  List<string> qfilters = new List<string>();
        public  List<string> qresults = new List<string>();
        public List<string> qparams = new List<string>();
        public List<string> qgfilters = new List<string>();
        public List<string> qofilters = new List<string>();
        public  HrDbLib()
        {
           
            pathstr = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            pathstr = pathstr.Substring(6, pathstr.Length - 6);
            string temp = "\\bin\\Debug";
            if (pathstr.LastIndexOf(temp) > 0) pathstr = pathstr.Substring(0, pathstr.Length - temp.Length);
            connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + pathstr + "\\App_Data\\3mbackup.accdb";
     
        }

        public void InsertRec(string tbl,int no_of_fields)
        {
            string OleDb = "INSERT INTO ["+ tbl+ "] (";
            for (int i = 0; i < no_of_fields; i++) if (i < no_of_fields - 1) OleDb = OleDb +"["+ qfields[i] + "],"; else OleDb = OleDb + "["+qfields[i]+"]";
            OleDb = OleDb + ") values(";
            for (int i = 0; i < no_of_fields; i++) if (i < no_of_fields - 1) OleDb = OleDb + qparams[i] + ","; else OleDb = OleDb + qparams[i];
            OleDb = OleDb + ")";
            using (OleDbConnection connection = new OleDbConnection(connString))
            {
                     OleDbCommand cmd = new OleDbCommand(OleDb, connection);
                     connection.Open();
                     cmd.ExecuteNonQuery();
                     connection.Close();
            }

        }
        public bool isFieldThere(string tbl, string fld)
        {
            bool flag = true;
            string sqll = "SELECT "+fld+" FROM "+tbl+" TOP 1";
            try
            {
                using (OleDbConnection connection = new OleDbConnection(connString))
                {
                    OleDbCommand cmd = new OleDbCommand(sqll, connection);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ee)
            {
                flag = false;
            }
            return flag;


        }

        public void UpdateRec(string tbl, int no_of_fields , int no_of_filters)
        {
            string OleDb = "UPDATE " + tbl + " SET ";
            for (int i = 0; i < no_of_fields; i++) if (i < no_of_fields - 1) OleDb = OleDb +"["+ qfields[i] +"]"+ "=" + qparams[i] + ","; else OleDb = OleDb +"["+ qfields[i] +"] =" + qparams[i];
            
            OleDb = OleDb + " WHERE ";
            for (int i = 0; i < no_of_filters; i++) if (i < no_of_filters - 1) OleDb = OleDb + qfilters[i] + " AND "; else OleDb = OleDb + qfilters[i];

            using (OleDbConnection connection = new OleDbConnection(connString))
            {
                OleDbCommand cmd = new OleDbCommand(OleDb, connection);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }

        }
        public bool isFound(string tbl, string FieldName, string FieldValue)
        {
            bool flag = false;
            string OleDb = "SELECT COUNT(*) FROM " + tbl + " WHERE " + FieldName + "=" + FieldValue;
            using (OleDbConnection connection = new OleDbConnection(connString))
            {
                OleDbCommand cmd = new OleDbCommand(OleDb, connection);
                connection.Open();
                OleDbDataReader reader = cmd.ExecuteReader();
                qresults.Clear();
                string sss = "0";
                while (reader.Read())
                {
                    sss = reader[0].ToString();
                    if (sss == "") sss = "0";

                }
                if (sss != "0") flag = true;
                connection.Close();
            }

            return flag;
        }
        public void DeleteRec(string tbl, int no_of_filters)
        {
            string OleDb = "DELETE FROM " + tbl + " WHERE ";           
            for (int i = 0; i < no_of_filters; i++) if (i < no_of_filters - 1) OleDb = OleDb + qfilters[i] + " AND "; else OleDb = OleDb + qfilters[i];

            using (OleDbConnection connection = new OleDbConnection(connString))
            {
                OleDbCommand cmd = new OleDbCommand(OleDb, connection);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }

        }

        public void GetQData(string tbl, int no_of_fields, int no_of_filters)
        {
            string OleDb = "SELECT ";
            for (int i = 0; i < no_of_fields; i++) if (qfields[i].IndexOf("(") >= 0 || qfields[i].ToLower().IndexOf("distinct")>=0) { if (i < no_of_fields - 1)  OleDb = OleDb + qfields[i] + ","; else OleDb = OleDb + qfields[i]; }
                else 
                { if (i < no_of_fields - 1)  OleDb = OleDb + "[" + qfields[i] + "]" + ","; else OleDb = OleDb + "[" + qfields[i] + "]"; }
            OleDb = OleDb + " FROM " + "[" + tbl + "]";
           
            if (no_of_filters > 0)
            {
                OleDb = OleDb + " WHERE ";

                for (int i = 0; i < no_of_filters; i++) if (i < no_of_filters - 1) OleDb = OleDb + qfilters[i] + " AND "; else OleDb = OleDb + qfilters[i];
            }

            if (qgfilters.Count > 0) OleDb = OleDb + " GROUP BY " + qgfilters[0];
            if (qofilters.Count > 0) OleDb = OleDb + " ORDER BY " + qofilters[0];
            using (OleDbConnection connection = new OleDbConnection(connString))
            {
                OleDbCommand cmd = new OleDbCommand(OleDb, connection);
                connection.Open();
                OleDbDataReader reader = cmd.ExecuteReader();
                qresults.Clear();
                string sss = "";
                while (reader.Read())
                {
                    sss = "";
                    for (int i = 0; i < no_of_fields; i++) if (i < no_of_fields - 1) sss = sss + reader[i].ToString() + "~"; else sss = sss + reader[i].ToString();
                    qresults.Add(sss);

                }
                connection.Close();
            }

        }
        public void ProcessSql(string SqlStr)
        {
            string OleDb = SqlStr;
            using (OleDbConnection connection = new OleDbConnection(connString))
            {

                OleDbCommand cmd = new OleDbCommand(OleDb, connection);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();

            }

        }
        public bool IsHasRecords(string tbl)
        {
            bool flag = false;
            string sql = "SELECT COUNT(*) FROM " + tbl;
            using (OleDbConnection connection = new OleDbConnection(connString))
            {
                OleDbCommand cmd = new OleDbCommand(sql, connection);
                connection.Open();
                OleDbDataReader reader = cmd.ExecuteReader();
                qresults.Clear();
                string sss = "0";
                while (reader.Read())
                {
                    sss = reader[0].ToString();
                    if (sss == "") sss = "0";

                }
                if (sss != "0") flag = true;
                connection.Close();
            }

            return flag;
        }
    }

