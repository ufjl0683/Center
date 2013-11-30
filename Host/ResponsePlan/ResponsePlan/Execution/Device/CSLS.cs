using System;
using System.Collections.Generic;
using System.Text;

namespace Execution.Category
{
    /// <summary>
    /// �˹�����GCSLS
    /// </summary>
    internal class CSLS : ADevice 
    {
        int decrease = 10;      //�t�׸��t
        int maxSpeed = 100;     //�̤j�t��
        int minSpeed = 0;       //�̤p�t��
        int range = -1;

        public CSLS(AEvent aDevice, CategoryType type, System.Collections.Hashtable ht, DeviceType devType, System.Collections.Hashtable DevRange, Degree degree)
        {
            Initialize(aDevice, type, ht, devType, DevRange, degree);
            this.GetMessRuleData += new GetMessRuleDataHandler(CSLS_GetMessRuleData);
        }

        public override ExecutionObj produceExecution(ExecutionObj sender)
        {
            sender.Memo += "==>> CSLS";
            sender.CSLSOutputData = getDisplayContent(type);
            aDevice.produceExecution(sender);
            return sender;
        }

        protected override System.Collections.Hashtable setDisplay(RemoteInterface.HC.FetchDeviceData[] devNames, int maxSegId, MegType megType)
        {            
            System.Collections.Hashtable displayht = new System.Collections.Hashtable();
            if (devNames.Length == 0||Math.Abs(devNames[0].Mileage - int.Parse(ht["FROM_MILEPOST1"].ToString())) > getValidRangle()) return displayht;
            List<object> outputs = new List<object>();
            if (devNames == null) return displayht;
            RemoteInterface.HC.I_HC_FWIS hobj = EasyClient.getHost();

            com.select(DBConnect.DataType.CSLS, Command.GetSelectCmd.getSectionSpeed(ht["INC_LOCATION"].ToString()));
            
            if (maxSegId == -99)
            {
                foreach (RemoteInterface.HC.FetchDeviceData devName in devNames)
                {
                    System.Data.DataSet DS = hobj.getSendDs("CSLS", "set_speed");
                    if (devName.minSpd > minSpeed)
                        DS.Tables[0].Rows[0]["speed"] = devName.minSpd;  //�t��
                    else
                        DS.Tables[0].Rows[0]["speed"] = minSpeed;  //�t��
                    DS.AcceptChanges();
                    List<object> output = new List<object>();
                    output.AddRange(new object[] { (int)(Convert.ToInt32(DS.Tables[0].Rows[0]["speed"]) / 10), new RemoteInterface.HC.CSLSOutputData(DS) });
                    displayht.Add(devName.DevName, output);
                }
            }
            else
            {
                int speed = minSpeed - decrease;
                foreach (RemoteInterface.HC.FetchDeviceData devName in devNames)
                {
                    System.Data.DataSet DS = hobj.getSendDs("CSLS", "set_speed");
                    if (range == -1)
                    {
                        if (speed < maxSpeed) speed += decrease;
                        if (devName.maxSpd >= speed)
                            DS.Tables[0].Rows[0]["speed"] = speed;  //�t��
                        else
                            DS.Tables[0].Rows[0]["speed"] = devName.maxSpd;
                    }
                    else
                    {
                        if (devName.SegId < range)
                        {
                            if (devName.maxSpd >= speed)
                            {
                                if (speed < maxSpeed) speed += decrease;
                                DS.Tables[0].Rows[0]["speed"] = speed;  //�t��
                            }
                            else
                                DS.Tables[0].Rows[0]["speed"] = devName.maxSpd;
                        }
                        else
                        {
                            if (devName.maxSpd >= speed)
                            {
                                DS.Tables[0].Rows[0]["speed"] = maxSpeed;  //�t��
                            }
                            else
                                DS.Tables[0].Rows[0]["speed"] = devName.maxSpd;
                        }
                    }
                    DS.AcceptChanges();
                    List<object> output = new List<object>();
                    output.AddRange(new object[] { (int)(Convert.ToInt32(DS.Tables[0].Rows[0]["speed"]) / 10), new RemoteInterface.HC.CSLSOutputData(DS) });
                    displayht.Add(devName.DevName, output);
                }
            }
            return displayht;            
        }

        object CSLS_GetMessRuleData(DBConnect.DataType type, object reader)
        {
            if (type == DBConnect.DataType.CSLS)
            {
                System.Data.Odbc.OdbcDataReader dr = (System.Data.Odbc.OdbcDataReader)reader;
                decrease = Convert.ToInt32(dr[0]);
                range = Convert.ToInt32(dr[1]);
                switch (degree)
                {
                    case Degree.L:
                        minSpeed = Convert.ToInt32(dr[3]);
                        maxSpeed = Convert.ToInt32(dr[2]);
                        break;
                    case Degree.M:
                        minSpeed = Convert.ToInt32(dr[5]);
                        maxSpeed = Convert.ToInt32(dr[4]);
                        break;
                    case Degree.H:
                        minSpeed = Convert.ToInt32(dr[7]);
                        maxSpeed = Convert.ToInt32(dr[6]);
                        break;
                    case Degree.S:
                        minSpeed = Convert.ToInt32(dr[9]);
                        maxSpeed = Convert.ToInt32(dr[8]);
                        break;
                    default:
                        minSpeed = maxSpeed;
                        break;
                }
                return null;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// �]�ƶZ���W�X�h�֤��ؤ����
        /// </summary>
        /// <returns></returns>
        private int getValidRangle()
        {
            return 2000;
        }
    }
}
