using System;
using System.Data.Odbc;

namespace DBConnect
{
    public class ODBC_DB2Connect : A_OdbcConnect
    {
        

        public ODBC_DB2Connect():base()
        {

        }         



        #region ==== 資料庫初始化 ====
        //資料庫初始化
        public override bool  Open()
        {
            try
            {
                if (conn == null)
                {
                    lock (typeof(OdbcConnection))
                    {
                        if (conn == null)
                        {
                            string sConnString = "dsn=TCS;uid=db2inst1;pwd=db2inst1";
                            if (conn == null)
                            {
                                conn = new OdbcConnection(sConnString);
                                conn.Open();
                            }
                        }
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}
