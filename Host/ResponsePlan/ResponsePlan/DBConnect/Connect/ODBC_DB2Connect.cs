using System;
using System.Data.Odbc;

namespace DBConnect
{
    public class ODBC_DB2Connect : A_OdbcConnect
    {
        

        public ODBC_DB2Connect():base()
        {

        }         



        #region ==== ��Ʈw��l�� ====
        //��Ʈw��l��
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
