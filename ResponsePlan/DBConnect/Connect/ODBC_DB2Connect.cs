using System;
using System.Data.Odbc;
//using IBM.Data;

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
                    //lock (typeof(IBM.Data.DB2.DB2Connection))
                    {
                        if (conn == null)
                        {
                            string sConnString = "dsn=TCS;uid=db2inst1;pwd=db2inst1";
                            //string sConnString = "Database=TCS;UserID=db2inst1;Password=db2inst1;Server=10.21.50.31:50000";
                            if (conn == null)
                            {
                                conn = new OdbcConnection(sConnString);
                                //conn = new IBM.Data.DB2.DB2Connection(sConnString);
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
