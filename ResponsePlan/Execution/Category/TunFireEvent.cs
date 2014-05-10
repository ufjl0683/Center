using System;
using System.Collections.Generic;
using System.Text;
using DBConnect;


namespace Execution.Category
{
    internal class TunFireEvent:AEvent
    {
        public TunFireEvent(System.Collections.Hashtable ht, CategoryType type)
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
                string cmd = string.Format("Select * from {0}.{1} where alarmType = 'GEN'", RSPGlobal.GlobaSchema, DBConnect.DB2TableName.tblRSPDevice);
                System.Data.DataTable DeviceTypeDT = com.Select(cmd);


                List<object> Decorators = (List<object>)com.select(DataType.Decorators, Command.GetSelectCmd.getDisplyDevTypes(degree, secType, ht["INC_LOCATION"].ToString().Trim()));
                com.GetReaderData -= new GetReaderDataHandler(com_GetReaderData);

                System.Collections.Hashtable HT = (System.Collections.Hashtable)Decorators[0];
                Decorators.Clear();

                List<DeviceType> adorns = new List<DeviceType>();
                System.Data.DataRow dr = DeviceTypeDT.Rows[0];
                foreach (System.Data.DataColumn dc in DeviceTypeDT.Columns)
                {
                    if (dr[dc].Equals(1))
                    {
                        if (Enum.IsDefined(typeof(DeviceType), dc.ColumnName))
                        {
                            adorns.Add((DeviceType)Enum.Parse(typeof(DeviceType), dc.ColumnName));
                            Decorators.Add(HT.Clone());
                        }
                    }
                }
                return getAdorns(adorns, myObj, Decorators, degree);
            }
            else
                return null;
        }

        protected override Degree getDegree()
        {
            ht["INC_BLOCKAGE"] = "1100000000";
            return Degree.H;
        }
    }
}
