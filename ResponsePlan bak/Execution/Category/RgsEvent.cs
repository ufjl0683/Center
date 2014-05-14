using System;
using System.Collections.Generic;
using System.Text;

namespace Execution.Category
{
    class RgsEvent:AEvent
    {
        internal RgsEvent(System.Collections.Hashtable ht, CategoryType type)
        {
            Initialize(ht, type);
        }

        protected override AEvent factory()
        {
            AEvent myObj = this;
            com.GetReaderData += new DBConnect.GetReaderDataHandler(com_GetReaderData);
            Degree degree = getDegree();
            if (degree != Degree.N)
            {
                List<object> Decorators = (List<object>)com.select(DBConnect.DataType.Decorators, Command.GetSelectCmd.getDisplyDevTypes(degree, secType, ht["INC_LOCATION"].ToString().Trim()));
                com.GetReaderData -= new DBConnect.GetReaderDataHandler(com_GetReaderData);

                System.Collections.Hashtable HT = (System.Collections.Hashtable)Decorators[0];
                Decorators.Clear();

                List<DeviceType> adorns = new List<DeviceType>();
                adorns.Add(DeviceType.RGS);
                Decorators.Add(HT.Clone());
                return getAdorns(adorns, myObj, Decorators, degree);
            }
            else
                return null;
        }

        object com_GetReaderData(DBConnect.DataType type, object reader)
        {
            if (type == DBConnect.DataType.Decorators)
            {            
                System.Data.Odbc.OdbcDataReader dr = (System.Data.Odbc.OdbcDataReader)reader;
                System.Collections.Hashtable decHT = new System.Collections.Hashtable();
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    if (!decHT.ContainsKey(dr.GetName(i))) decHT.Add(dr.GetName(i), dr[i]);
                }
                return decHT;
            }
            else if (type == DBConnect.DataType.LaneCount)
            {
                System.Data.Odbc.OdbcDataReader dr = (System.Data.Odbc.OdbcDataReader)reader;
                System.Collections.Hashtable laneCount = new System.Collections.Hashtable();
                laneCount.Add(0, Convert.ToString(dr[2]));
                laneCount.Add(1, Convert.ToString(dr[3]));
                laneCount.Add(2, Convert.ToString(dr[4]));
                laneCount.Add(3, Convert.ToString(dr[5]));
                laneCount.Add(4, Convert.ToString(dr[6]));
                laneCount.Add(5, Convert.ToString(dr[7]));
                laneCount.Add(6, Convert.ToString(dr[8]));
                laneCount.Add(7, Convert.ToString(dr[9]));
                laneCount.Add(8, Convert.ToString(dr[10]));
                laneCount.Add(9, Convert.ToString(dr[11]));
                laneCount.Add(10, Convert.ToString(dr[12]));
                laneCount.Add(11, Convert.ToString(dr[13]));
                laneCount.Add(12, Convert.ToString(dr[14]));
                laneCount.Add(13, Convert.ToString(dr[15]));
                laneCount.Add(14, Convert.ToString(dr[16]));
                return laneCount;
            }
            else
                return null;
        }

    }
}
