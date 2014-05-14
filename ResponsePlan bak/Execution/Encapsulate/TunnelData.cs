using System;
using System.Collections.Generic;
using System.Text;
using DBConnect;

namespace Execution
{
    [Serializable]
    class TunnelData
    {
        private static TunnelData tunnelData;
        public TunnelList tunList;         //隧道集合
        private Tunnel beforeTunnel = null;
        private ODBC_DB2Connect dbcmd;            //資料庫物件
        private TunnelData()
        {
            tunList = new TunnelList();
            dbcmd = new ODBC_DB2Connect();
            dbcmd.GetReaderData += new GetReaderDataHandler(dbcmd_GetReaderData);
            dbcmd.select(DataType.Tunnel, Command.GetSelectCmd.getTunnelData());
        }

        /// <summary>
        /// 獨體建構者
        /// </summary>
        /// <returns>建構元</returns>
        public static TunnelData getBuilder()
        {
            if (tunnelData == null)
            {
                lock (typeof(TunnelData))
                {
                    if (tunnelData == null)
                        tunnelData = new TunnelData();
                }
            }

            return tunnelData;
        }

        object dbcmd_GetReaderData(DataType type, object reader)
        {
            if (type == DataType.Tunnel)
            {
                System.Data.Odbc.OdbcDataReader dr = (System.Data.Odbc.OdbcDataReader)reader;
                Tunnel tunnel = null;
                Line line = new Line(dr[4].ToString(), dr[5].ToString(), Convert.ToInt32(dr[8]), Convert.ToInt32(dr[11]));
                tunnel = new Tunnel(dr[1].ToString(), dr[3].ToString(), line, dr[7].ToString(), Convert.ToInt32(dr[9]), Convert.ToInt32(dr[10]), beforeTunnel);
                beforeTunnel = tunnel;
                tunList.Add(tunnel);                
            }
            return null;
        }
    }
}
