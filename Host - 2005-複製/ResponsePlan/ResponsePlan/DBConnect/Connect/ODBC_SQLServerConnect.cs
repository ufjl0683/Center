using System;
using System.Data.Odbc;

namespace DBConnect.Connect
{
    class ODBC_SQLServerConnect : A_OdbcConnect
    {
        public ODBC_SQLServerConnect()
            : base()
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
                            string sConnString = "dsn=ControlCenter;uid=db2inst1;pwd=db2inst1";
                            conn = new OdbcConnection(sConnString);
                            conn.Open();
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
