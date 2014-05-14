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
       
        //static public IBM.Data.DB2.DB2Connection conn;
        //private IBM.Data.DB2.DB2Command cmd;
        public static string schema="DB2INST1";

        public A_OdbcConnect()
        {
            Open();
        }

        public void setSchema(string sch)
        {
            schema = sch;
        }

        //protected virtual OdbcConnection GetConn()
        //{
        //    return conn;
        //}

        protected virtual OdbcConnection GetConn()
        {
            return conn;
        }

        public abstract bool Open();

        public System.Data.DataTable Select(string cmd)
        {
            try
            {
                OdbcDataAdapter da = new OdbcDataAdapter(cmd, GetConn());
                //IBM.Data.DB2.DB2DataAdapter da = new IBM.Data.DB2.DB2DataAdapter(cmd, GetConn());
                System.Data.DataTable DT = new System.Data.DataTable();
                da.Fill(DT);
                da.Dispose();
                return DT;
            }
            catch
            {
                try
                {
                    lock (typeof(OdbcConnection))
                    {
                        GetConn().Close();
                        GetConn().Open();

                        OdbcDataAdapter da = new OdbcDataAdapter(cmd, GetConn());
                        //IBM.Data.DB2.DB2DataAdapter da = new IBM.Data.DB2.DB2DataAdapter(cmd, GetConn());
                        System.Data.DataTable DT = new System.Data.DataTable();
                        da.Fill(DT);
                        da.Dispose();

                        return DT;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }                
            }            
        }

        

        /// <summary>
        /// 資料庫查尋方法
        /// </summary>
        /// <param name="selectcmd"></param>
        /// <returns></returns>
        public System.Data.DataTable select(ICommand selectcmd)
        {
            string selectCmd = selectcmd.getCommand();
            try
            {
                cmd = new OdbcCommand(selectCmd, GetConn());
                OdbcDataAdapter da = new OdbcDataAdapter(cmd);

                //cmd = new IBM.Data.DB2.DB2Command(selectCmd, GetConn());                
                //IBM.Data.DB2.DB2DataAdapter da = new IBM.Data.DB2.DB2DataAdapter(cmd);
                System.Data.DataTable DT = new System.Data.DataTable();
                da.Fill(DT);
                da.Dispose();
                return DT;
            }
            catch
            {
                try
                {
                    lock (typeof(OdbcConnection))
                    {
                        GetConn().Close();
                        GetConn().Open();
                        cmd = new OdbcCommand(selectCmd, GetConn());

                        OdbcDataAdapter da = new OdbcDataAdapter(cmd);
                        //cmd = new IBM.Data.DB2.DB2Command(selectCmd, GetConn());
                        //IBM.Data.DB2.DB2DataAdapter da = new IBM.Data.DB2.DB2DataAdapter(cmd);
                        System.Data.DataTable DT = new System.Data.DataTable();
                        da.Fill(DT);
                        da.Dispose();

                        return DT;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        
        /// <summary>
        /// 資料庫查尋方法
        /// </summary>
        /// <param name="type"></param>
        /// <param name="selectcmd"></param>
        /// <returns></returns>
        public object select(DataType type, ICommand selectcmd)
        {
            System.Collections.Generic.List<object> list = new System.Collections.Generic.List<object>();
            string selectCmd = selectcmd.getCommand();
            OdbcDataReader dr;
            //System.Data.Odbc.OdbcDataReader dr;

            try
            {
                //cmd = new IBM.Data.DB2.DB2Command(selectCmd, GetConn());
                cmd = new OdbcCommand(selectCmd, GetConn());
                dr = cmd.ExecuteReader();              
            }
            catch
            {
                try
                {
                    lock (typeof(OdbcConnection))
                    {
                        GetConn().Close();
                        GetConn().Open();

                        cmd = new OdbcCommand(selectCmd, GetConn());
                        //cmd = new IBM.Data.DB2.DB2Command(selectCmd, GetConn());
                        dr = cmd.ExecuteReader();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            //bool Found = false;
            //try
            //{
                while (dr.Read())
                {
                    //Found = true;
                    if (GetReaderData != null)
                    {
                        list.Add(GetReaderData(type, dr));
                    }
                }
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message + "\r\n" + selectCmd);
            //}
            //if (!Found)
            //    throw new Exception("Not Found " + selectCmd);
            return list;
        }


        /// <summary>
        /// 資料庫更新方法
        /// </summary>
        /// <param name="updata"></param>
        public void update(ICommand updata)
        {           
            string myCmd = updata.getCommand();
            try
            {
                cmd = new OdbcCommand(myCmd, GetConn());
                //cmd = new IBM.Data.DB2.DB2Command(myCmd, GetConn());
                cmd.ExecuteNonQuery();
            }
            catch 
            {
                try
                {
                    lock (typeof(OdbcConnection))
                    {
                        GetConn().Close();
                        GetConn().Open();

                        cmd = new OdbcCommand(myCmd, GetConn());
                        //cmd = new IBM.Data.DB2.DB2Command(myCmd, GetConn());
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }        

        /// <summary>
        /// 資料庫新增方法
        /// </summary>
        /// <param name="insert"></param>
        public void insert(ICommand insert)
        {
            string myCmd = insert.getCommand();
            try
            {                
                cmd = new OdbcCommand(myCmd, GetConn());
                //cmd = new IBM.Data.DB2.DB2Command(myCmd, GetConn());
                cmd.ExecuteNonQuery();
            }
            catch (OdbcException dobcEx)
            {
                if (dobcEx.ErrorCode == -2146232009)
                {
                    return;
                }

                try
                {
                    lock (typeof(OdbcConnection))
                    {
                        //cmd = new IBM.Data.DB2.DB2Command(myCmd, GetConn());
                        cmd = new OdbcCommand(myCmd, GetConn());
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
        
        /// <summary>
        /// 關閉資料庫
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            try
            {
                if (GetConn() != null) GetConn().Close();
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
