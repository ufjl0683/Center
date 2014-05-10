using System;
using System.Collections.Generic;
using System.Text;

namespace Execution.Category
{
    /// <summary>
    /// 裝飾物件：CSLS
    /// </summary>
    internal class CSLS : ADevice 
    {
        int decrease = 10;      //速度落差
        int maxSpeed = 100;     //最大速度
        int minSpeed = 0;       //最小速度
        int range = -1;

        const int UPStream1 = 1;
        const int UPStream2 = 2;
        const int UPStream3 = 3;
        const int UPStream4 = 4;
        const int UPStream5 = 5;
        const int RangerLimit = 6;

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
            //if (devNames.Length == 0||Math.Abs(devNames[0].Mileage - int.Parse(ht["FROM_MILEPOST1"].ToString())) > getValidRangle()) return displayht;
            //List<object> outputs = new List<object>();
            if (devNames == null || devNames.Length == 0) return displayht;
            RemoteInterface.HC.I_HC_FWIS hobj = EasyClient.getHost();
            com.select(DBConnect.DataType.CSLS, Command.GetSelectCmd.getSectionSpeed(ht["ORIGINAL_INC_LOCATION"].ToString()));

            int ColNum;

            if (type == CategoryType.GEN)
            {
                switch (degree)
                {
                    case Degree.L:
                        ColNum = 1;
                        break;
                    case Degree.M:
                        ColNum = 2;
                        break;
                    case Degree.H:
                        ColNum = 3;
                        break;
                    case Degree.S:
                        ColNum = 4;
                        break;
                    default:
                        ColNum = 1;
                        break;
                }
            }
            else
            {
                ColNum = Convert.ToInt32(ht["INC_CONGESTION"]);
            }
            string cmd = string.Format("Select SuggesSpeeD{0} From {1}.{2} csls,{1}.{3} rule where AlarmClass = {4} and csls.RuleID = rule.RuleID and rule.RUNING = 'Y';"
                    , ColNum, RSPGlobal.GlobaSchema, DBConnect.DB2TableName.tblRSPCSLSSpeed, DBConnect.DB2TableName.tblRSPRule, ht["INC_NAME"]);
            System.Data.DataTable DT = com.Select(cmd);
            if (DT != null && DT.Rows.Count > 0)
            {
                maxSpeed = Convert.ToInt32(DT.Rows[0][0]);
                if (maxSpeed == -1)
                {
                    return displayht;
                }
            }
            cmd = string.Format("Select * from {0}.{1} csls,{0}.{2} rule where csls.RuleID = rule.ruleid and rule.runing = 'Y' and SuggesSpeed = {3};"
                , RSPGlobal.GlobaSchema, DBConnect.DB2TableName.tblIIPCSLSParam, DBConnect.DB2TableName.tblRSPRule, maxSpeed);

            DT = com.Select(cmd);
            if (DT.Rows.Count < 1)
            {
                throw new Exception("not Mapping CSLS Speed");
            }
            System.Data.DataRow dr = DT.Rows[0];
            List<int> SpeedList = new List<int>();
            SpeedList.Add(Convert.ToInt32(dr[UPStream1]));
            SpeedList.Add(Convert.ToInt32(dr[UPStream2]));
            SpeedList.Add(Convert.ToInt32(dr[UPStream3]));
            SpeedList.Add(Convert.ToInt32(dr[UPStream4]));
            SpeedList.Add(Convert.ToInt32(dr[UPStream5]));

            range = Convert.ToInt32(dr[RangerLimit]) * 1000;
            int i = 0;
            int SetSpped = 90;

            int SectionMaxSpeed = 90;
            if (devNames.Length > 0)
            {
                cmd = string.Format("Select sec.MaxSpeed From {0}.{1} sec, {0}.{2} cfg where sec.sectionid = cfg.sectionID and cfg.DeviceName = '{3}';"
                            , RSPGlobal.GlobaSchema, DBConnect.DB2TableName.tblGroupSection, DBConnect.DB2TableName.tblDeviceConfig, devNames[0].DevName);
                DT = com.Select(cmd);
                if (DT.Rows.Count > 0)
                {
                    SectionMaxSpeed = (int)DT.Rows[0][0];
                }
            }


            for (int j = 0; j < devNames.Length;j++ )
            {
                if (maxSegId != 99 && (int)ht["INC_NAME"] == 31 && ((devNames[j].Mileage > Convert.ToInt32(ht["FROM_MILEPOST1"]) 
                    && devNames[j].Mileage < Convert.ToInt32(ht["TO_MILEPOST1"])) ||(devNames[j].Mileage < Convert.ToInt32(ht["FROM_MILEPOST1"]) 
                    && devNames[j].Mileage > Convert.ToInt32(ht["TO_MILEPOST1"])) ))
                {
                    SetSpped = 255;//熄滅
                }

                else if (devNames[j].Location == "D")//下游
                {
                    cmd = string.Format("Select sec.MaxSpeed From {0}.{1} sec, {0}.{2} cfg where sec.sectionid = cfg.sectionID and cfg.DeviceName = '{3}';"
                        , RSPGlobal.GlobaSchema, DBConnect.DB2TableName.tblGroupSection, DBConnect.DB2TableName.tblDeviceConfig, devNames[j].DevName);
                    DT = com.Select(cmd);
                    if (DT.Rows.Count > 0)
                    {
                        SetSpped = (int)DT.Rows[0][0];
                    }
                }
                else
                {
                    if (maxSegId != -99
                        && ((j == 0 && (Math.Abs(devNames[j].Mileage - Convert.ToInt32(ht["FROM_MILEPOST1"])) > range
                            && Math.Abs(devNames[j].Mileage - Convert.ToInt32(ht["TO_MILEPOST1"])) > range))
                        || (j > 0 && devNames[j-1].Location != "D" && Math.Abs(devNames[j].Mileage - devNames[j - 1].Mileage) > range)))
                        break;

                 
                    if (maxSegId == -99) //範圍內 最低速限
                    {
                        SetSpped = SpeedList[0];
                    }
                    else                //範圍外
                    {
                        if (i >= SpeedList.Count || SpeedList[i].Equals(-1))
                        {
                            break;
                        }
                        else
                        {
                            if (SectionMaxSpeed < SpeedList[i])
                                SetSpped = SectionMaxSpeed;
                            else
                                SetSpped = SpeedList[i];
                        } 
                    }
                    i++;
                }
                


                System.Data.DataSet DS = hobj.getSendDs("CSLS", "set_speed");
                DS.Tables[0].Rows[0]["speed"] = SetSpped;
                DS.AcceptChanges();
                List<object> output = new List<object>();
                output.AddRange(new object[] { SetSpped / 10, new RemoteInterface.HC.CSLSOutputData(DS) });
                if (!displayht.Contains(devNames[j].DevName))
                {
                    displayht.Add(devNames[j].DevName, output);
                }
               
            }


            #region OLD Function
            //if (maxSegId == -99)
            //{
            //    foreach (RemoteInterface.HC.FetchDeviceData devName in devNames)
            //    {
            //        System.Data.DataSet DS = hobj.getSendDs("CSLS", "set_speed");
            //        if (devName.minSpd > minSpeed)
            //            DS.Tables[0].Rows[0]["speed"] = devName.minSpd;  //速度
            //        else
            //            DS.Tables[0].Rows[0]["speed"] = minSpeed;  //速度
            //        DS.AcceptChanges();
            //        List<object> output = new List<object>();
            //        output.AddRange(new object[] { (int)(Convert.ToInt32(DS.Tables[0].Rows[0]["speed"]) / 10), new RemoteInterface.HC.CSLSOutputData(DS) });
            //        displayht.Add(devName.DevName, output);
            //    }
            //}
            //else
            //{
            //    int speed = minSpeed - decrease;
            //    foreach (RemoteInterface.HC.FetchDeviceData devName in devNames)
            //    {
            //        System.Data.DataSet DS = hobj.getSendDs("CSLS", "set_speed");
            //        if (range == -1)
            //        {
            //            if (speed < maxSpeed) speed += decrease;
            //            if (devName.maxSpd >= speed)
            //                DS.Tables[0].Rows[0]["speed"] = speed;  //速度
            //            else
            //                DS.Tables[0].Rows[0]["speed"] = devName.maxSpd;
            //        }
            //        else
            //        {
            //            if (devName.SegId < range)
            //            {
            //                if (devName.maxSpd >= speed)
            //                {
            //                    if (speed < maxSpeed) speed += decrease;
            //                    DS.Tables[0].Rows[0]["speed"] = speed;  //速度
            //                }
            //                else
            //                    DS.Tables[0].Rows[0]["speed"] = devName.maxSpd;
            //            }
            //            else
            //            {
            //                if (devName.maxSpd >= speed)
            //                {
            //                    DS.Tables[0].Rows[0]["speed"] = maxSpeed;  //速度
            //                }
            //                else
            //                    DS.Tables[0].Rows[0]["speed"] = devName.maxSpd;
            //            }
            //        }
            //        DS.AcceptChanges();
            //        List<object> output = new List<object>();
            //        output.AddRange(new object[] { (int)(Convert.ToInt32(DS.Tables[0].Rows[0]["speed"]) / 10), new RemoteInterface.HC.CSLSOutputData(DS) });
            //        displayht.Add(devName.DevName, output);
            //    }
            //}
            #endregion
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
        /// 設備距離超出多少公尺不顯示
        /// </summary>
        /// <returns></returns>
        private int getValidRangle()
        {
            return 2000;
        }
    }
}
