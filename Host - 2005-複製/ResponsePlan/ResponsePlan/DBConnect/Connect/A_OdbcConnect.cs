using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Odbc;

namespace DBConnect
{
    public abstract class A_OdbcConnect : I_Connect
    {
        public event GetReaderDataHandler GetReaderData;
        static public OdbcConnection conn;
        private OdbcCommand cmd;     
        public static string schema="DB2INST1";

        public A_OdbcConnect()
        {
            Open();
        }

        public void setSchema(string sch)
        {
            schema = sch;
        }

        public abstract bool Open();

        /// <summary>
        /// ��Ʈw�d�M��k
        /// </summary>
        /// <param name="selectcmd"></param>
        /// <returns></returns>
        public System.Data.DataTable select(ICommand selectcmd)
        {
            string selectCmd = selectcmd.getCommand();
            try
            {
                cmd = new OdbcCommand(selectCmd, conn);

                OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                System.Data.DataTable DT = new System.Data.DataTable();
                da.Fill(DT);
                da.Dispose();
                return DT;
            }
            catch
            {
                try
                {
                    conn.Close();
                    conn.Open();
                    cmd = new OdbcCommand(selectCmd, conn);

                    OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                    System.Data.DataTable DT = new System.Data.DataTable();
                    da.Fill(DT);
                    da.Dispose();
                    return DT;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        
        /// <summary>
        /// ��Ʈw�d�M��k
        /// </summary>
        /// <param name="type"></param>
        /// <param name="selectcmd"></param>
        /// <returns></returns>
        public object select(DataType type, ICommand selectcmd)
        {
            System.Collections.Generic.List<object> list = new System.Collections.Generic.List<object>();
            string selectCmd = selectcmd.getCommand();
            OdbcDataReader dr;

            try
            {
                cmd = new OdbcCommand(selectCmd, conn);
                dr = cmd.ExecuteReader();
            }
            catch
            {
                try
                {
                    conn.Close();
                    conn.Open();
                    cmd = new OdbcCommand(selectCmd, conn);
                    dr = cmd.ExecuteReader();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            bool Found = false;
            while (dr.Read())
            {
                Found = true;
                if (GetReaderData != null)
                {
                    list.Add(GetReaderData(type, dr));
                }
            }
            if (!Found)
                throw new Exception("Not Found " + selectCmd);
            return list;
        }


        /// <summary>
        /// ��Ʈw��s��k
        /// </summary>
        /// <param name="updata"></param>
        public void update(ICommand updata)
        {           
            string myCmd = updata.getCommand();
            try
            {
                cmd = new OdbcCommand(myCmd, conn);
                cmd.ExecuteNonQuery();
            }
            catch 
            {
                try
                {
                    conn.Close();
                    conn.Open();
                    cmd = new OdbcCommand(myCmd, conn);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }        

        /// <summary>
        /// ��Ʈw�s�W��k
        /// </summary>
        /// <param name="insert"></param>
        public void insert(ICommand insert)
        {
            string myCmd = insert.getCommand();
            try
            {                
                cmd = new OdbcCommand(myCmd, conn);
                cmd.ExecuteNonQuery();
            }
            catch
            {
                try
                {
                    conn.Close();
                    conn.Open();
                    cmd = new OdbcCommand(myCmd, conn);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
        
        /// <summary>
        /// ������Ʈw
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            try
            {
                if (conn != null) conn.Close();
                return true;
            }
            catch 
            {
                return false;
            }
        }


        ~A_OdbcConnect()
        {
            //Close();
        }
    }
}
