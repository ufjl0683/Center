using System;
using System.Collections.Generic;
using System.Text;
using DBConnect;

namespace Execution.Category
{
    /// <summary>
    /// ㄆン摸
    /// </summary>
    internal class GenEvent : AEvent
    {
        ODBC_DB2Connect cod = null;

        public GenEvent(System.Collections.Hashtable ht, CategoryType type)
        {
            Initialize(ht, type);
            cod = new ODBC_DB2Connect();
            cod.GetReaderData += new DBConnect.GetReaderDataHandler(cod_GetReaderData);
        }

        object cod_GetReaderData(DBConnect.DataType type, object reader)
        {
            if (type == DBConnect.DataType.LaneCount)
            {
                System.Data.Odbc.OdbcDataReader dr = (System.Data.Odbc.OdbcDataReader)reader;
                System.Collections.Hashtable laneCount = new System.Collections.Hashtable();
                laneCount.Add(0, Convert.ToString(dr[7]));
                laneCount.Add(1, Convert.ToString(dr[6]));
                laneCount.Add(2, Convert.ToString(dr[5]));
                laneCount.Add(3, Convert.ToString(dr[4]));
                laneCount.Add(4, Convert.ToString(dr[3]));
                laneCount.Add(5, Convert.ToString(dr[2]));
                return laneCount;
            }
            else
                return null;
        }

        protected override Degree getDegree()
        {
            if (ht["INC_SERVERITY"].ToString() != "" && ht["INC_SERVERITY"].ToString() != "0")
            {
                return (Degree)Enum.Parse(typeof(Degree), ht["INC_SERVERITY"].ToString());
            }
            else
            {
                int laneCount = getBlockCount(ht["INC_BLOCKAGE"].ToString());
                List<object> list = (List<object>)cod.select(DBConnect.DataType.LaneCount, Command.GetSelectCmd.getSerious(Convert.ToInt32(ht["LANE_COUNT"])));
                if (list.Count == 0)
                {
                    //this.serMeg.setAlarmMeg("ㄆン腨祘把计耞⊿Τ祘(叫琩隔琿ó笵计琌㎝耞ó笵计癸莱)!!!");
                    return Degree.N;
                }
                System.Collections.Hashtable myht = (System.Collections.Hashtable)list[0];
                if (myht.Contains(laneCount))
                {
                    switch (myht[laneCount].ToString())
                    {
                        case "L":
                            return Degree.L;
                        case "M":
                            return Degree.M;
                        case "H":
                            return Degree.H;
                        case "S":
                            return Degree.S;
                        default:
                            {
                                //this.serMeg.setAlarmMeg("ㄆン腨祘把计耞⊿Τ祘(叫琩隔琿ó笵计琌㎝耞ó笵计癸莱)!!!");
                                return Degree.N;
                            }
                    }
                }
                else
                {
                    return Degree.N;
                }
            }
        }

        /// <summary>
        /// 眔耞ó笵计-1隔0礚耞1~8耞ó笵计)
        /// </summary>
        /// <returns>耞ó笵计</returns>
        private int getBlockCount(string block)
        {
            int count = 0;
            if (string.IsNullOrEmpty(block))
                count = 0;
            else
            {
                int i = 1;
                foreach (char ch in block.Substring(0,6))
                {
                    if (ch != '0')
                    {
                        count++;
                    }
                    i++;
                }
            }
            return count;
        }
    }
}
