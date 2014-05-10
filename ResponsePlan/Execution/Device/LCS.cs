using System;
using System.Collections.Generic;
using System.Text;

namespace Execution.Category
{
    /// <summary>
    /// 裝飾物件：LCS
    /// </summary>
    internal class LCS : ADevice 
    {
        bool isReturnFlash = false;//轉向訊息是否閃爍
        bool isEventFlash = false;//事件訊息是否閃爍
        int n_device = 3;//離事件點最遠N支LCS為轉向訊息


        public LCS(AEvent aDevice, CategoryType type, System.Collections.Hashtable ht, DeviceType devType, System.Collections.Hashtable DevRange, Degree degree)
        {
            Initialize(aDevice, type, ht, devType, DevRange, degree);
            this.GetMessRuleData += new GetMessRuleDataHandler(LCS_GetMessRuleData);
        }

        public override ExecutionObj produceExecution(ExecutionObj sender)
        {
            sender.Memo += "==>> LCS";
            sender.LCSOutputData = getDisplayContent(type);
            aDevice.produceExecution(sender);
            return sender;
        }

        protected override System.Collections.Hashtable setDisplay(RemoteInterface.HC.FetchDeviceData[] devNames, int maxSegId, MegType megType)
        {
            System.Collections.Hashtable displayht = new System.Collections.Hashtable();
            int mile_s = Convert.ToInt32(ht["FROM_MILEPOST1"]);
            int mile_e = Convert.ToInt32(ht["TO_MILEPOST1"]);
            if (devNames == null)
                return displayht;
            if (tunnel == null && devNames != null && devNames.Length>0)
            {
                tunnel = TunnelData.getBuilder().tunList.Find(new EventObj(devNames[0].LineId, devNames[0].Direction, devNames[0].Mileage));               
            }

            if (tunnel == null && mile_s != mile_e)
            {
                for (int i = 1; i < devNames.Length; i++)
                {
                    tunnel = TunnelData.getBuilder().tunList.Find(new EventObj(devNames[i].LineId, devNames[i].Direction, devNames[i].Mileage));
                    if (tunnel != null)
                        break;
                }
            }

            if (tunnel == null) return displayht;
            
            com.select(DBConnect.DataType.Tunnel, Command.GetSelectCmd.getTunnelData(tunnel.Id, tunnel.Direction));

            
            List<object> outputs = new List<object>();
            if (devNames == null || devNames.Length == 0) return displayht;
            RemoteInterface.HC.I_HC_FWIS hobj = EasyClient.getHost();

           
            //string direction = Convert.ToString(ht["INC_DIRECTION"]).Trim();

            //熄滅:0, ╳:2, ↓:1  ↘:3 , ↙:4
            //int laneCount = Convert.ToInt32(ht["LANE_COUNT"]);
            int laneCount = 2;
            byte[] laneNearMeg = getBlockageMessage(laneCount, ht["INC_BLOCKAGE"].ToString());  //車道數寫死為2車道
            byte[] laneMiddleMeg = new byte[laneCount];


            if (laneNearMeg[0] == (byte)LCSLight.down && laneNearMeg[1] == (byte)LCSLight.forbid)
            {
                laneMiddleMeg[0] = laneNearMeg[0];
                laneMiddleMeg[1] = (byte)LCSLight.leftDown;
            }
            else if (laneNearMeg[0] == 2 && laneNearMeg[1] == 1)
            {
                laneMiddleMeg[0] = (byte)LCSLight.rightDown;
                laneMiddleMeg[1] = laneNearMeg[1];
            }
            else
            {
                laneMiddleMeg = laneNearMeg;
            }
            

            for (int i = 0; i < laneCount; i++)
            {
                if (isReturnFlash)                                 
                    laneMiddleMeg[i] += 4;
                else if (isEventFlash)
                    laneNearMeg[i] += 4;
            }

            bool UpOne = false;
            int segID =0;
            if (devNames.Length > 0)
                segID = devNames[devNames.Length -1].SegId;
            for (int i = devNames.Length-1; i >= 0; i--)
            {
                if (UpOne)
                    break;
                System.Data.DataSet DS = hobj.getSendDs("LCS", "set_ctl_sign");

                DS.Tables[0].Rows[0]["sign_cnt"] = Convert.ToByte(laneCount);

                byte CommInside = 0;
                byte CommOutside = 0;

                if (type == CategoryType.OBS)
                {
                    if (UpDown(tunnel.StartMileage, devNames[i].Mileage, devNames[i].Direction))
                    {
                        CommInside = laneNearMeg[0];
                        CommOutside = laneNearMeg[1];
                    }
                    else
                    {
                        CommInside = laneMiddleMeg[0];
                        CommOutside = laneMiddleMeg[1];
                        UpOne = true;
                    }
                }
                else
                {
                    if (laneNearMeg[0] % 4 == (byte)LCSLight.forbid && laneNearMeg[1] % 4 == (byte)LCSLight.forbid)
                    {
                        if (devNames[i].SegId != segID)
                            break;
                        if (UpDown(mile_s, devNames[i].Mileage,devNames[i].Direction))
                        {
                            //CommInside = (byte)LCSLight.down;
                            //CommOutside = (byte)LCSLight.down;
                            CommInside = laneNearMeg[0];
                            CommOutside = laneNearMeg[1];
                        }
                        else
                        {
                            CommInside = laneNearMeg[0];
                            CommOutside = laneNearMeg[1];
                        }
                    }
                    else
                    {
                        if (UpDown(mile_s, devNames[i].Mileage, devNames[i].Direction))
                        {
                            //CommInside = (byte)LCSLight.down;
                            //CommOutside = (byte)LCSLight.down;
                            CommInside = laneNearMeg[0];
                            CommOutside = laneNearMeg[1];
                        }
                        else if (UpDown(tunnel.StartMileage, devNames[i].Mileage, devNames[i].Direction))
                        {
                            //if (i == 0 || (tunnel.UpstreamLocation != null && UpDown(devNames[i - 1].Mileage, tunnel.UpstreamLocation.EndMileage, devNames[i -1].Direction)))
                            if (i == 0 || (!UpDown(tunnel.StartMileage,devNames[i-1].Mileage,devNames[i].Direction) && Math.Abs(tunnel.StartMileage - devNames[i -1].Mileage) > 200))
                            {
                                CommInside = laneMiddleMeg[0];
                                CommOutside = laneMiddleMeg[1];
                                UpOne = true;
                            }
                            else
                            {
                                CommInside = laneNearMeg[0];
                                CommOutside = laneNearMeg[1];
                            }
                        }
                        else
                        {
                            if (Math.Abs(tunnel.StartMileage - devNames[i].Mileage) < 200)
                            {
                                CommInside = laneMiddleMeg[0];
                                CommOutside = laneMiddleMeg[1];
                                UpOne = true;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }



                //內車道
                System.Data.DataRow DShow = DS.Tables[1].NewRow();
                DShow["sign_no"] = Convert.ToByte(0);
                DShow["sign_status"] = CommInside;
                DS.Tables[1].Rows.Add(DShow);
                //外車道
                DShow = DS.Tables[1].NewRow();
                DShow["sign_no"] = Convert.ToByte(1);
                DShow["sign_status"] = CommOutside;
                DS.Tables[1].Rows.Add(DShow);
                DS.AcceptChanges();

                List<object> output = new List<object>();
                output.AddRange(new object[] { getPriority(), new RemoteInterface.HC.LCSOutputData(DS) });
                displayht.Add(devNames[i].DevName, output);
            }
            return displayht;
        }

        object LCS_GetMessRuleData(DBConnect.DataType type, object reader)
        {
            if (type == DBConnect.DataType.Tunnel)
            {
                System.Data.Odbc.OdbcDataReader dr = (System.Data.Odbc.OdbcDataReader)reader;
                if (dr[7].ToString() == "1")
                    isEventFlash = true;
                else
                    isEventFlash = false;
                if (dr[8].ToString() == "1")
                    isReturnFlash = true;
                else
                    isReturnFlash = false;
                n_device = Convert.ToInt32(dr[6]);
                return null;
            }
            else
                return null;
        }

        /// <summary>
        /// 取得LCS顯示訊息
        /// </summary>
        /// <param name="laneCount">車道數</param>
        /// <param name="blockage">阻斷車道字串</param>
        /// <returns></returns>
        private byte[] getBlockageMessage(int laneCount, string blockage)
        {            
            byte[] laneMeg = new byte[laneCount];
            string laneStr = blockage.Substring(0, 2);
            for (int i=0;i< laneStr.Length;i++)
            {
                if (laneStr[i] == '1')
                    laneMeg[i] = 2;
                else
                    laneMeg[i] = 1;         
            }
            if (blockage.Length >= 6)
            {
                if (blockage[5] == '1')
                    laneMeg[laneCount - 1] = 2;
                //else
                //    laneMeg[laneCount - 1] = 1;
            }
            return laneMeg;
        }

        private byte String2Byte(string inputStr)
        {
            byte returnInt = 0;
            switch (inputStr)
            {
                case "熄滅": returnInt = 0; break;
                case "↓": returnInt = 1; break;
                case "╳": returnInt = 2; break;
                case "↘": returnInt = 3; break;
                case "↙": returnInt = 4; break;
                default: returnInt = 0; break;
            }
            return returnInt;
        }

        private bool UpDown(int UpMileage, int DownMileage, string dir)
        {
            if (dir.Equals("N") || dir.Equals("W"))
            {
                return UpMileage > DownMileage ? true : false;
            }
            else
            {
                return DownMileage > UpMileage ? true : false;
            }
        }
    }
}
