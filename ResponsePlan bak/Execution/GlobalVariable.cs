using System;
using System.Collections.Generic;
using System.Text;

namespace Execution
{
    class RSPGlobal
    {
        private static System.Data.DataTable LineNameDT;
        private static System.Data.DataTable CMSTypeDT;
        internal static string GlobaSchema = "db2inst1";
        private static System.Data.DataTable DeviceDT;
        internal static  int DeviceName = 0;
        internal static  int Location = 1;
        private static System.Data.DataTable DivisionDT;
        private static System.Data.DataTable CloverleafDT;

        internal static System.Data.DataTable GetCloverleafDT()
        {
            if (CloverleafDT == null)
            {
                lock (typeof(System.Data.DataTable))
                {
                    if (CloverleafDT == null)
                    {
                        DBConnect.ODBC_DB2Connect conn = new DBConnect.ODBC_DB2Connect();
                        string cmd = string.Format("Select LineID1,mileage1,lineid2,Mileage2,direction2 from {0}.VWCLOVERLEAF " 
                            + " where direction1 in ('N','E');", GlobaSchema);
                        CloverleafDT = conn.Select(cmd);
                    }
                }
            }
            return CloverleafDT;
        }



        internal static System.Data.DataTable GetLineNameDT()
        {
            if (LineNameDT == null)
            {
                lock (typeof(System.Data.DataTable))
                {
                    if (LineNameDT == null)
                    {
                        DBConnect.ODBC_DB2Connect conn = new DBConnect.ODBC_DB2Connect();
                        string cmd = string.Format("Select LineID,LineName,startMileage,endMileage from {0}.{1};", GlobaSchema, DBConnect.DB2TableName.tblGroupLine);
                        LineNameDT = conn.Select(cmd);
                        LineNameDT.PrimaryKey = new System.Data.DataColumn[] { LineNameDT.Columns[0] };
                    }
                }
            }
            return LineNameDT;
        }

        internal static System.Data.DataTable GetDeviceDT()
        {
            if (DeviceDT == null)
            {
                lock (typeof(System.Data.DataTable))
                {
                    if (DeviceDT == null)
                    {
                        DBConnect.ODBC_DB2Connect conn = new DBConnect.ODBC_DB2Connect();
                        string cmd = string.Format("Select DeviceName,Location,Device_Type,Mile_M from {0}.{1};", GlobaSchema, DBConnect.DB2TableName.tblDeviceConfig);
                        DeviceDT = conn.Select(cmd);
                        DeviceDT.PrimaryKey = new System.Data.DataColumn[] { DeviceDT.Columns[0] };
                    }
                }
            }
            return DeviceDT;
        }

        internal static System.Data.DataTable GetCMSTypeDT()
        {
            if (CMSTypeDT == null)
            {
                lock (typeof(System.Data.DataTable))
                {
                    if (CMSTypeDT == null)
                    {
                        DBConnect.ODBC_DB2Connect conn = new DBConnect.ODBC_DB2Connect();
                        string cmd = string.Format("Select DeviceName,show_icon From {0}.{1};", GlobaSchema, DBConnect.DB2TableName.tblDeviceConfig);
                        CMSTypeDT = conn.Select(cmd);
                        CMSTypeDT.PrimaryKey = new System.Data.DataColumn[] { CMSTypeDT.Columns[0] };
                    }
                }
            }
            return CMSTypeDT;
        }

        internal static System.Data.DataTable GetDivisionDT()
        {
            if (DivisionDT == null)
            {
                lock (typeof(System.Data.DataTable))
                {
                    if (DivisionDT == null)
                    {
                        DBConnect.ODBC_DB2Connect conn = new DBConnect.ODBC_DB2Connect();
                        string cmd = string.Format("Select DivisionName,DivisionType,mileage,LineID,DivisionID from {0}.{1};", GlobaSchema, DBConnect.DB2TableName.tblGroupDivision);
                        DivisionDT = conn.Select(cmd);
                    }
                }
            }
            return DivisionDT;
        }

        
    }
}
