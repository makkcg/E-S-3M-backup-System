using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

//using System.Data.OleDb;


public class HrDbLib
    {
       
        public  string pathstr;
        public  string connString;
        public  List<string> qfields = new List<string>();
        public  List<string> qfilters = new List<string>();
        public  List<string> qresults = new List<string>();
        public List<string> qparams = new List<string>();
    public  HrDbLib(string dbdir,string db,int source)
        {
            GrantAccess(dbdir);
           // connString = "Data Source=.\\DESKTOP-32A7GCV;AttachDbFilename=D:\\3M_CleanTraceDatabase\\Spark_Data_19-06-2023_15_01_39.mdf;Integrated Security=True;Connect Timeout=30; User Instance=True";
           //connString = "Data Source=HANY-PC;Network Library=DBMSSOCN; Initial Catalog=attendance;User ID='att';Password='att123';";
            connString = "";
            if(source>0) connString = "Data Source=" + Environment.MachineName.ToString() + "\\SPARK;AttachDbFilename=" + dbdir + "\\" + db + ";Integrated Security=True;";
            else
                connString = "Data Source=" + Environment.MachineName.ToString() + "\\SPARK;Database=Spark;Integrated Security=True;";

           pathstr = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            pathstr = pathstr.Substring(6, pathstr.Length - 6);
            string temp = "\\bin\\Debug";
            if (pathstr.LastIndexOf(temp) > 0) pathstr = pathstr.Substring(0, pathstr.Length - temp.Length);

            
        }
    public void GrantAccess(string fullPath)
    {
        DirectoryInfo dInfo = new DirectoryInfo(fullPath);
        DirectorySecurity dSecurity = dInfo.GetAccessControl();
        dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
        dInfo.SetAccessControl(dSecurity);
    }

        public void InsertRec(string tbl,int no_of_fields)
        {
            string sql = "INSERT INTO " + tbl + "(";
            for (int i = 0; i < no_of_fields; i++) if (i < no_of_fields - 1) sql = sql + qfields[i] + ","; else sql = sql + qfields[i];
            sql = sql + ") values(";
            for (int i = 0; i < no_of_fields; i++) if (i < no_of_fields - 1) sql = sql + qparams[i] + ","; else sql = sql + qparams[i];
            sql = sql + ")";
            using (SqlConnection connection = new SqlConnection(connString))
            {
                     SqlCommand cmd = new SqlCommand(sql, connection);
                     connection.Open();
                     cmd.ExecuteNonQuery();
                     connection.Close();
            }

        }

        public void InsertRecX(string tbl, int no_of_fields)
        {
            string sql = "SET IDENTITY_INSERT " + tbl + " ON; INSERT INTO " + tbl + "(";
            for (int i = 0; i < no_of_fields; i++) if (i < no_of_fields - 1) sql = sql + qfields[i] + ","; else sql = sql + qfields[i];
            sql = sql + ") values(";
            for (int i = 0; i < no_of_fields; i++) if (i < no_of_fields - 1) sql = sql + qparams[i] + ","; else sql = sql + qparams[i];
            sql = sql + ")";
            using (SqlConnection connection = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand(sql, connection);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }

        }

        public void UpdateRec(string tbl, int no_of_fields , int no_of_filters)
        {
            string sql = "UPDATE " + tbl + " SET ";
            for (int i = 0; i < no_of_fields; i++) if (i < no_of_fields - 1) sql = sql + qfields[i] + "=" + qparams[i] + ","; else sql = sql + qfields[i] + "=" + qparams[i];
            
            sql = sql + " WHERE ";
            for (int i = 0; i < no_of_filters; i++) if (i < no_of_filters - 1) sql = sql + qfilters[i] + " AND "; else sql = sql + qfilters[i];

            using (SqlConnection connection = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand(sql, connection);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }

        }
        public bool isFound(string tbl, string FieldName, string FieldValue)
        {
            bool flag = false;
            string sql = "SELECT COUNT(*) FROM " + tbl + " WHERE " + FieldName + "=" + FieldValue;
            using (SqlConnection connection = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand(sql, connection);
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
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
            string sql = "DELETE FROM " + tbl + " WHERE ";           
            for (int i = 0; i < no_of_filters; i++) if (i < no_of_filters - 1) sql = sql + qfilters[i] + " AND "; else sql = sql + qfilters[i];

            using (SqlConnection connection = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand(sql, connection);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }

        }

        public void GetQData(string tbl, int no_of_fields, int no_of_filters)
        {
            string sql = "SELECT ";
            for (int i = 0; i < no_of_fields; i++) if (i < no_of_fields - 1) sql = sql + qfields[i] + ","; else sql = sql + qfields[i];
            sql = sql + " FROM " + tbl;
            
            if (no_of_filters > 0)
            {
                sql = sql + " WHERE ";

                for (int i = 0; i < no_of_filters; i++) if (i < no_of_filters - 1) sql = sql + qfilters[i] + " AND "; else sql = sql + qfilters[i];
            }
            using (SqlConnection connection = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand(sql, connection);
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
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

        public void CallStoredProc(string proc_name,int no_of_in_param,int no_of_out_param)
 
        {
          using (SqlConnection connection = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand(proc_name, connection);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                for (int i = 0; i < no_of_in_param; i++)
                {
                    string[] ss= qparams[i].Split('~');
                    switch (ss[1])
                    {
                        case "text": cmd.Parameters.Add(ss[0], System.Data.SqlDbType.VarChar).Value = ss[2];
                            cmd.Parameters[i].Direction = System.Data.ParameterDirection.Input; break;
                        case "number": cmd.Parameters.Add(ss[0], System.Data.SqlDbType.Int).Value = Convert.ToInt16(ss[2]);
                            cmd.Parameters[i].Direction = System.Data.ParameterDirection.Input; break;
                        case "date": cmd.Parameters.Add(ss[0], System.Data.SqlDbType.Date).Value = Convert.ToDateTime(ss[2]);
                            cmd.Parameters[i].Direction = System.Data.ParameterDirection.Input; break;
                        case "yesno": cmd.Parameters.Add(ss[0], System.Data.SqlDbType.Bit).Value = Convert.ToBoolean(ss[2]);
                            cmd.Parameters[i].Direction = System.Data.ParameterDirection.Input; break;

                         }

                }

                for (int i = no_of_in_param; i < no_of_in_param + no_of_out_param; i++)
                {
                    string[] ss = qparams[i].Split('~');
                    switch (ss[1])
                    {
                        case "text": cmd.Parameters.Add(ss[0], System.Data.SqlDbType.VarChar);
                                     cmd.Parameters[i].Direction = System.Data.ParameterDirection.Output;break;
                        case "number": cmd.Parameters.Add(ss[0], System.Data.SqlDbType.Int);
                                    cmd.Parameters[i].Direction = System.Data.ParameterDirection.Output; break;
                        case "date": cmd.Parameters.Add(ss[0], System.Data.SqlDbType.Date);
                                    cmd.Parameters[i].Direction = System.Data.ParameterDirection.Output;  break;
                        case "yesno": cmd.Parameters.Add(ss[0], System.Data.SqlDbType.Bit);
                                      cmd.Parameters[i].Direction = System.Data.ParameterDirection.Output;  break;

                    }

                }

                connection.Open();
                cmd.ExecuteNonQuery();

                for (int i = no_of_in_param; i < no_of_in_param + no_of_out_param; i++)
                {
                    string[] ss = qparams[i].Split('~');

                    qresults.Add(cmd.Parameters[ss[0]].Value.ToString());
                }

                connection.Close();
          }
           }

        public void ExecrdProc(string sql)
        {

            using (SqlConnection connection = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand(sql, connection);
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        public bool IsHasRecords(string tbl)
        {
            bool flag = false;
            string sql = "SELECT COUNT(*) FROM " + tbl ;
            using (SqlConnection connection = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand(sql, connection);
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
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
       
    

        ////public  HrDbLib()
        ////{
        ////    connString = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source=D:\\installment\\installment.mdb";
     
        ////    pathstr = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
        ////    pathstr = pathstr.Substring(6, pathstr.Length - 6);
        ////    string temp = "\\bin\\Debug";
        ////    if (pathstr.LastIndexOf(temp) > 0) pathstr = pathstr.Substring(0, pathstr.Length - temp.Length);
        ////    connString=connString.Replace("D:\\installment\\installment.mdb", pathstr + "\\App_Data\\dental.mdb");
        ////}
        ////public void InsertRec(string tbl,int no_of_fields)
        ////{
        ////    string OleDb = "INSERT INTO ["+ tbl+ "] (";
        ////    for (int i = 0; i < no_of_fields; i++) if (i < no_of_fields - 1) OleDb = OleDb +"["+ qfields[i] + "],"; else OleDb = OleDb + "["+qfields[i]+"]";
        ////    OleDb = OleDb + ") values(";
        ////    for (int i = 0; i < no_of_fields; i++) if (i < no_of_fields - 1) OleDb = OleDb + qparams[i] + ","; else OleDb = OleDb + qparams[i];
        ////    OleDb = OleDb + ")";
        ////    using (OleDbConnection connection = new OleDbConnection(connString))
        ////    {
        ////             OleDbCommand cmd = new OleDbCommand(OleDb, connection);
        ////             connection.Open();
        ////             cmd.ExecuteNonQuery();
        ////             connection.Close();
        ////    }

        ////}

        ////public void UpdateRec(string tbl, int no_of_fields , int no_of_filters)
        ////{
        ////    string OleDb = "UPDATE " + tbl + " SET ";
        ////    for (int i = 0; i < no_of_fields; i++) if (i < no_of_fields - 1) OleDb = OleDb +"["+ qfields[i] +"]"+ "=" + qparams[i] + ","; else OleDb = OleDb +"["+ qfields[i] +"] =" + qparams[i];
            
        ////    OleDb = OleDb + " WHERE ";
        ////    for (int i = 0; i < no_of_filters; i++) if (i < no_of_filters - 1) OleDb = OleDb + qfilters[i] + " AND "; else OleDb = OleDb + qfilters[i];

        ////    using (OleDbConnection connection = new OleDbConnection(connString))
        ////    {
        ////        OleDbCommand cmd = new OleDbCommand(OleDb, connection);
        ////        connection.Open();
        ////        cmd.ExecuteNonQuery();
        ////        connection.Close();
        ////    }

        ////}
        ////public bool isFound(string tbl, string FieldName, string FieldValue)
        ////{
        ////    bool flag = false;
        ////    string OleDb = "SELECT COUNT(*) FROM " + tbl + " WHERE " + FieldName + "=" + FieldValue;
        ////    using (OleDbConnection connection = new OleDbConnection(connString))
        ////    {
        ////        OleDbCommand cmd = new OleDbCommand(OleDb, connection);
        ////        connection.Open();
        ////        OleDbDataReader reader = cmd.ExecuteReader();
        ////        qresults.Clear();
        ////        string sss = "0";
        ////        while (reader.Read())
        ////        {
        ////            sss = reader[0].ToString();
        ////            if (sss == "") sss = "0";

        ////        }
        ////        if (sss != "0") flag = true;
        ////        connection.Close();
        ////    }

        ////    return flag;
        ////}
        ////public void DeleteRec(string tbl, int no_of_filters)
        ////{
        ////    string OleDb = "DELETE FROM " + tbl + " WHERE ";           
        ////    for (int i = 0; i < no_of_filters; i++) if (i < no_of_filters - 1) OleDb = OleDb + qfilters[i] + " AND "; else OleDb = OleDb + qfilters[i];

        ////    using (OleDbConnection connection = new OleDbConnection(connString))
        ////    {
        ////        OleDbCommand cmd = new OleDbCommand(OleDb, connection);
        ////        connection.Open();
        ////        cmd.ExecuteNonQuery();
        ////        connection.Close();
        ////    }

        ////}

        ////public void GetQData(string tbl, int no_of_fields, int no_of_filters)
        ////{
        ////    string OleDb = "SELECT ";
        ////    for (int i = 0; i < no_of_fields; i++) if (i < no_of_fields - 1) OleDb = OleDb + qfields[i] + ","; else OleDb = OleDb + qfields[i];
        ////    OleDb = OleDb + " FROM " + tbl;
            
        ////    if (no_of_filters > 0)
        ////    {
        ////        OleDb = OleDb + " WHERE ";

        ////        for (int i = 0; i < no_of_filters; i++) if (i < no_of_filters - 1) OleDb = OleDb + qfilters[i] + " AND "; else OleDb = OleDb + qfilters[i];
        ////    }
        ////    using (OleDbConnection connection = new OleDbConnection(connString))
        ////    {
        ////        OleDbCommand cmd = new OleDbCommand(OleDb, connection);
        ////        connection.Open();
        ////        OleDbDataReader reader = cmd.ExecuteReader();
        ////        qresults.Clear();
        ////        string sss = "";
        ////        while (reader.Read())
        ////        {
        ////            sss = "";
        ////            for (int i = 0; i < no_of_fields; i++) if (i < no_of_fields - 1) sss = sss + reader[i].ToString() + "~"; else sss = sss + reader[i].ToString();
        ////            qresults.Add(sss);

        ////        }
        ////        connection.Close();
        ////    }

        ////}

        ////public bool IsHasRecords(string tbl)
        ////{
        ////    bool flag = false;
        ////    string sql = "SELECT COUNT(*) FROM " + tbl;
        ////    using (OleDbConnection connection = new OleDbConnection(connString))
        ////    {
        ////        OleDbCommand cmd = new OleDbCommand(sql, connection);
        ////        connection.Open();
        ////        OleDbDataReader reader = cmd.ExecuteReader();
        ////        qresults.Clear();
        ////        string sss = "0";
        ////        while (reader.Read())
        ////        {
        ////            sss = reader[0].ToString();
        ////            if (sss == "") sss = "0";

        ////        }
        ////        if (sss != "0") flag = true;
        ////        connection.Close();
        ////    }

        ////    return flag;
        ////}
    }

