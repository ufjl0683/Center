using System;
using System.Collections.Generic;
using System.Text;
using Execution.Command;

namespace Execution.Category
{
    /// <summary>
    /// 裝飾物件：RGS
    /// </summary>
    internal class RGS : A_DisplayBlock  
    {
        System.Collections.Hashtable messColorsHT;
        /// <summary>
        /// ＣＭＳ模式為２行
        /// </summary>
        int rowCount_mode2 = 2; //ＣＭＳ模式為２行
        /// <summary>
        /// 路徑導引模式４行
        /// </summary>
        int rowCount_mode1 = 4; //路徑導引模式４行
        RGSDeviceList rgslist;
        RGSData data;
        public RGS(AEvent aDevice, CategoryType type, System.Collections.Hashtable ht, DeviceType devType, System.Collections.Hashtable DevRange, Degree degree)
        {
            Initialize(aDevice, type, ht, devType, DevRange, degree);
            rgslist = new RGSDeviceList();
            this.GetMessRuleData += new GetMessRuleDataHandler(RGS_GetMessRuleData);
            List<object> messColors = (List<object>)com.select(DBConnect.DataType.MessColor, Command.GetSelectCmd.getMessColor());
            messColorsHT = new System.Collections.Hashtable();
            foreach (object obj in messColors)
            {
                MessColor mess = (MessColor)obj;
                messColorsHT.Add(mess.ID, mess);
            } 
        }

        public override ExecutionObj produceExecution(ExecutionObj sender)
        {
            sender.Memo += "==>> RGS";
            sender.RGSOutputData = getDisplayContent(type);
            aDevice.produceExecution(sender);
            return sender;
        }

        protected override System.Collections.Hashtable setDisplay(RemoteInterface.HC.FetchDeviceData[] devNames, int maxSegId, MegType megType)
        {
            System.Collections.Hashtable displayht = new System.Collections.Hashtable();
            com.select(DBConnect.DataType.RGS, Command.GetSelectCmd.getRGSMode());
            List<object> outputs = new List<object>();
            if (devNames == null || devNames.Length == 0) return displayht;
            
            foreach (RemoteInterface.HC.FetchDeviceData devName in devNames)
            {
                int distance = getDeviceDistance(devName.SegId, maxSegId);
                data = rgslist.Find(devName.DevName);
                outputs = (List<object>)com.select(DBConnect.DataType.RgsCategory, Command.GetSelectCmd.getRGSCategory(Convert.ToInt32(DevRange["RULEID"]), (int)secType, devType.ToString(), distance, devName.DevName, megType.ToString(), ht["INC_LINEID"].ToString().Trim(),devName.Location,devName.LineId));
                foreach (object obj in outputs)
                {
                    List<object> output = new List<object>();
                    output.AddRange(new object[] { getPriority(), obj });
                    if (!displayht.Contains(devName.DevName))
                        displayht.Add(devName.DevName, output);
                    else if (devName.Location == "L")
                    {
                        displayht[devName.DevName] = output;
                    }
                }
              
                data = null;
            }
            return displayht;
        }

        object RGS_GetMessRuleData(DBConnect.DataType type, object reader)
        {
            if (type == DBConnect.DataType.RgsCategory)
            {
                System.Data.Odbc.OdbcDataReader dr = (System.Data.Odbc.OdbcDataReader)reader;
                return SetRGSDislay(dr[6].ToString(), Convert.ToInt32(dr[7]), Convert.ToInt32(dr[9]));
            }
            else if (type == DBConnect.DataType.MessColor)
            {
                System.Data.Odbc.OdbcDataReader dr = (System.Data.Odbc.OdbcDataReader)reader;
                return new MessColor(Convert.ToInt32(dr[0]), dr[1].ToString(), System.Drawing.Color.FromArgb(Convert.ToInt32(dr[3].ToString(), 16)), System.Drawing.Color.FromArgb(Convert.ToInt32(dr[4].ToString(), 16)));
            }
            else if (type == DBConnect.DataType.BlockMeg1)
            {
                System.Data.Odbc.OdbcDataReader dr = (System.Data.Odbc.OdbcDataReader)reader;
                return new BlockMeg1(Convert.ToInt32(dr[0]), dr[1].ToString(), (dr[2].ToString() == "1") ? true : false);
            }
            else if (type == DBConnect.DataType.BlockMeg2)
            {
                System.Data.Odbc.OdbcDataReader dr = (System.Data.Odbc.OdbcDataReader)reader;
                return dr[3].ToString();
            }
            else if (type == DBConnect.DataType.RGS)
            {
                System.Data.Odbc.OdbcDataReader dr = (System.Data.Odbc.OdbcDataReader)reader;
                RGSData data = new RGSData(dr[0].ToString(), Convert.ToInt32(dr[1]), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(),
                           Convert.ToInt32(dr[5]), Convert.ToInt32(dr[6]));
                rgslist.Add(data);
                return null;
            }
            else
                return null;
        }

        /// <summary>
        /// 依據RGS的種類取得顯示內容
        /// </summary>
        /// <param name="message">訊息</param>
        /// <param name="isIcon">面板圖形顯示(0:都不秀,1:秀國道圖示,2:秀事故圖示,3:秀低圖)</param>
        /// <param name="mode">模式</param>
        /// <returns></returns>
        private RemoteInterface.MFCC.RGS_GenericDisplay_Data SetRGSDislay(string message, int isIcon, int mode)
        {
            bool isAccident = false;
            if ((int)ht["INC_NAME"] == 48) //路徑轉向寫死
            {
                mode = 1;

                if (data.DeviceName == "RGS-N3-N-212.8")
                {
                    string cmd = string.Format("Select from_Milepost1 from {0}.TBLIIPEVENT where inc_name = 30 " //and Inc_Status = 3 " 
                        + "AND INC_LINEID = 'N1' and INC_DIRECTION = 'N' and from_milepost1 > 174200 and from_milepost1 < 192800 order by from_milepost1 desc fetch first 1 rows only;"
                       , RSPGlobal.GlobaSchema);

                    System.Data.DataTable DT = com.Select(cmd);

                    if (DT != null && DT.Rows.Count > 0)
                    {
                        isAccident = true;
                        string Mile = string.Format("{0,4:###k}", (int)DT.Rows[0][0] / 1000);
                        message = "國1 北上[1]" + Mile + "事故[1]台中以北[1]請改道台74";
                    }
                    else
                    {
                        isAccident = false;
                    }
                }
                else
                {

                    string div = string.Empty;
                    string cmd = string.Format("Select start_divName from db2inst1.TBLCOMPARETRAVELTIMEDETAIL where DeviceName = '{0}' order by order fetch first rows only;", data.DeviceName);
                    System.Data.DataTable DT = com.Select(cmd);


                    if (DT != null && DT.Rows.Count > 0)
                    {
                        div = DT.Rows[0][0].ToString();
                        div = div.Substring(0, 2);
                    }
                    string dir = string.Empty;
                    string con = string.Empty;
                    string desc = string.Empty;
                    switch (data.Direction)
                    {
                        case "N":
                            dir = "北";
                            con = "<";
                            desc = "desc";
                            break;
                        case "S":
                            dir = "南";
                            con = ">";
                            break;
                        case "E":
                            dir = "東";
                            con = ">";
                            break;
                        case "W":
                            dir = "西";
                            con = "<";
                            desc = "desc";
                            break;
                    }
                    cmd = string.Format("Select from_Milepost1 from {0}.TBLIIPEVENT where inc_name = 30 and Inc_Status = 3 and from_milepost1 {1} {2} order by from_milepost1 {3} fetch first 1 rows only;"
                        , RSPGlobal.GlobaSchema, con, ht["FROM_MILEPOST1"], desc);

                    DT = com.Select(cmd);
                    string Mile = string.Empty;
                    if (DT != null && DT.Rows.Count > 0)
                    {
                        Mile = string.Format("{0,4:###k}", (int)DT.Rows[0][0] / 1000);
                    }

                    message = Mile + "　　" + div + "以" + dir + "[1]事故　　請改道";
                    //message = "　　　　" + div + "以" + dir + "[1]事故　　請改道";
                }
            }

            this.mode = mode;
            RemoteInterface.MFCC.RGS_GenericDisplay_Data outputData = null;
            if (mode == 0)//都會路網沒有文字
            {
                byte g = 0;
                if (data != null)
                    g = (byte)data.G_City;

               
                string cmd = string.Format("Select Section_ID from {0}.VWRGSPOLYGONSECTION where RGSDEVICENAME = '{1}' and G_CODE_ID = {2} Group by Section_ID order by Section_ID;"
                    , RSPGlobal.GlobaSchema, data.DeviceName, data.G_City);
                System.Data.DataTable tmpDT = com.Select(cmd);

                RemoteInterface.MFCC.RGS_Generic_Section_Data[] SectionDatas = new RemoteInterface.MFCC.RGS_Generic_Section_Data[tmpDT.Rows.Count];
                for (int i = 0; i < tmpDT.Rows.Count; i++)
                {
                    SectionDatas[i] = new RemoteInterface.MFCC.RGS_Generic_Section_Data(Convert.ToByte(tmpDT.Rows[i][0]), (byte)255);
                }

                outputData = new RemoteInterface.MFCC.RGS_GenericDisplay_Data((byte)mode, g, new RemoteInterface.MFCC.RGS_Generic_ICON_Data[0], new RemoteInterface.MFCC.RGS_Generic_Message_Data[0], SectionDatas);
            }
            else//CMS,路徑導引有文字
            {
                List<string> changeTexts = new List<string>();
                string str = "";
                foreach (char word in message)
                {
                    if (word == '[') str = "";

                    str += word;

                    if (word == ']')
                    {
                        changeTexts.Add(str);
                    }
                }


                System.Collections.Generic.List<RSPMegColor> rspMegColors = new System.Collections.Generic.List<RSPMegColor>();
                foreach (string text in changeTexts)
                {
                    int no = Convert.ToInt32(text.Substring(text.IndexOf('[') + 1, text.IndexOf(']') - 1));
                    string mag = setMessage(no,"F");

                    if (mag != "\r")
                    {
                        message = message.Replace(text, '\b' + mag + '\f');
                    }
                    else
                    {
                        message = message.Replace(text, mag);
                    }
                    MessColor messColor = (MessColor)messColorsHT[no];
                    rspMegColors.Add(new RSPMegColor(mag, messColor.Forecolor, messColor.BackColor));
                }

                RemoteInterface.MFCC.RGS_Generic_Message_Data[] megs = getMessageData(message, rspMegColors, isIcon == 0 ? false : true, mode);

                //改為4行
                RemoteInterface.MFCC.RGS_Generic_Message_Data[] megs4 = new RemoteInterface.MFCC.RGS_Generic_Message_Data[4];
                if (mode == 1)
                {
                    if (data.DeviceName == "RGS-N3-N-212.8" && isAccident)
                    {
                        byte g = 0;
                        if (data != null)
                            g = (byte)data.G_Path;
                        string[] msgs = message.Split('\r');
                        for (int i = 0; i < 4; i++)
                        {
                            System.Drawing.Color[] foreColor = new System.Drawing.Color[msgs[i].Length];
                            System.Drawing.Color[] backColor = new System.Drawing.Color[msgs[i].Length];
                            for (int j = 0; j < msgs[i].Length; j++)
                            {
                                foreColor[j] = ((MessColor)messColorsHT[0]).Forecolor;
                                backColor[j] = System.Drawing.Color.Black;
                            }
                            if (i == 1)
                            {
                                foreColor[foreColor.Length - 1] = System.Drawing.Color.Red;
                                foreColor[foreColor.Length - 2] = System.Drawing.Color.Red;
                            }
                            megs4[i] = new RemoteInterface.MFCC.RGS_Generic_Message_Data(msgs[i], foreColor, backColor, 0, (ushort)(i * 64));
                            
                        }
                        outputData = new RemoteInterface.MFCC.RGS_GenericDisplay_Data((byte)mode, g, new RemoteInterface.MFCC.RGS_Generic_ICON_Data[0], megs4, new RemoteInterface.MFCC.RGS_Generic_Section_Data[0]);
                        return outputData;
                    }
                    else
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            string msg = string.Empty;

                            System.Drawing.Color[] foreColor = new System.Drawing.Color[0];
                            System.Drawing.Color[] backColor = new System.Drawing.Color[0];

                            switch (i)
                            {
                                case 1:
                                case 2:
                                    msg = megs[i - 1].messgae;
                                    foreColor = megs[i - 1].forecolor;
                                    backColor = megs[i - 1].backcolor;
                                    break;
                            }
                            megs4[i] = new RemoteInterface.MFCC.RGS_Generic_Message_Data(msg, foreColor, backColor, 0, (ushort)(i * 64));
                        }
                    }
                }
                if (mode == 1)
                {
                    byte g = 0;
                    if (data != null)
                        g = (byte)data.G_Path;
                  
                    //outputData = new RemoteInterface.MFCC.RGS_GenericDisplay_Data((byte)mode, g, new RemoteInterface.MFCC.RGS_Generic_ICON_Data[0], megs, new RemoteInterface.MFCC.RGS_Generic_Section_Data[0]);
                    outputData = new RemoteInterface.MFCC.RGS_GenericDisplay_Data((byte)mode, g, new RemoteInterface.MFCC.RGS_Generic_ICON_Data[0], megs4, new RemoteInterface.MFCC.RGS_Generic_Section_Data[0]);
                    if (data.DeviceName == "RGS-N3-N-212.8" && !isAccident)
                    {
                        outputData.main_display_template = "        @";
                        outputData.opt_display_template = "        @";
                        outputData.alarm_class = 173;
                        foreach (var msg in outputData.msgs)
                        {
                            msg.messgae = string.Empty;
                            msg.backcolor = new System.Drawing.Color[0];
                            msg.forecolor = new System.Drawing.Color[0];
                        }
                    }
                
                }
                else
                {
                    RemoteInterface.MFCC.RGS_Generic_ICON_Data icon = null;             
                    if (isIcon == 1)
                        icon = new RemoteInterface.MFCC.RGS_Generic_ICON_Data(Convert.ToByte(ht["LINE_ICON"]), 0, 0);
                    else if (isIcon == 2)
                        icon = new RemoteInterface.MFCC.RGS_Generic_ICON_Data(Convert.ToByte(ht["ACC_ICON"]), 0, 0);                 


                    if (icon != null)
                        outputData = new RemoteInterface.MFCC.RGS_GenericDisplay_Data((byte)mode, 0, new RemoteInterface.MFCC.RGS_Generic_ICON_Data[] { icon }, megs, new RemoteInterface.MFCC.RGS_Generic_Section_Data[0]);
                    else
                        outputData = new RemoteInterface.MFCC.RGS_GenericDisplay_Data((byte)mode, 0, new RemoteInterface.MFCC.RGS_Generic_ICON_Data[] { }, megs, new RemoteInterface.MFCC.RGS_Generic_Section_Data[0]);
                }
            }
            return outputData;
        }

        private RemoteInterface.MFCC.RGS_Generic_Message_Data[] getMessageData(string message, System.Collections.Generic.List<RSPMegColor> rspMegColors, bool hasIcon, int mode)
        {
            string newMeg = message.Replace("\r", "");
            RemoteInterface.MFCC.RGS_Generic_Message_Data[] megs = null;
            List<System.Drawing.Color> foreColor = new List<System.Drawing.Color>();
            List<System.Drawing.Color> backColor = new List<System.Drawing.Color>();
            System.Drawing.Color NormalColor = ((MessColor)messColorsHT[0]).Forecolor;
            for (int i = 0; i < newMeg.Length; i++)
            {                
                //foreColor.Add(System.Drawing.Color.Red);
                foreColor.Add(NormalColor);                 //普通文字顏色
                backColor.Add(System.Drawing.Color.Black);
            }

            string tmpMesage = newMeg;
            int tmpK = 0;
            foreach (RSPMegColor de in rspMegColors)
            {
                if (tmpMesage.IndexOf(de.Message) == -1) continue;
                tmpK += tmpMesage.IndexOf(de.Message);
                
                for (int i = tmpK; i <= tmpK + de.Message.Length - 1; i++)
                {                 
                    if (newMeg[i] != '㊣' || mode != 1)
                    {
                        foreColor[i] = de.ForeColor;
                        backColor[i] = de.BackColor;
                    }
                    if (newMeg[i] == '㊣')
                    {
                        foreColor.Insert(i, foreColor[i]);
                        backColor.Insert(i, backColor[i]);
                    }
                }
                tmpK += de.Message.Length;
                tmpMesage = tmpMesage.Substring(tmpMesage.IndexOf(de.Message) + de.Message.Length);
            }
            System.Drawing.Point[] points = getMegLocation(hasIcon, mode);
            //if (mode == 1)
            //{
            //    megs = new RemoteInterface.MFCC.RGS_Generic_Message_Data[rowCount_mode1 * 2];
            //    BlockMegAndColor megColor = new BlockMegAndColor(message, foreColor.ToArray(), backColor.ToArray());

            //    BlockMegAndColor[] sepMegColors = separateMegColor(megColor);

            //    for (int i = 0; i < sepMegColors.Length; i++)
            //    {
            //        if (sepMegColors[i] == null)
            //        {
            //            megs = new RemoteInterface.MFCC.RGS_Generic_Message_Data[0];
            //            break;
            //        }
            //        else
            //            megs[i] = new RemoteInterface.MFCC.RGS_Generic_Message_Data(sepMegColors[i].Message, sepMegColors[i].ForeColor, sepMegColors[i].BackColor, (ushort)points[i].X, (ushort)points[i].Y);
            //    }
            //}
            //else
            //{
                megs = new RemoteInterface.MFCC.RGS_Generic_Message_Data[rowCount_mode2];
                int chrCount = 0; 
                if (hasIcon)
                    chrCount = 6;
                else
                    chrCount = 8;

                BlockMegAndColor megColor = new BlockMegAndColor(message, foreColor.ToArray(), backColor.ToArray());
                BlockMegAndColor[] sepMegColors = separateMegColor(devType, rowCount_mode2, chrCount, megColor);

                for (int i = 0; i < sepMegColors.Length; i++)
                {
                    if (sepMegColors[i] == null)
                    {
                        //megs[i] = new RemoteInterface.MFCC.RGS_Generic_Message_Data[0];
                        megs[i] = new RemoteInterface.MFCC.RGS_Generic_Message_Data("", new System.Drawing.Color[0], new System.Drawing.Color[0], (ushort)points[i].X, (ushort)points[i].Y);
                        break;
                    }
                    else
                        megs[i] = new RemoteInterface.MFCC.RGS_Generic_Message_Data(sepMegColors[i].Message, sepMegColors[i].ForeColor, sepMegColors[i].BackColor, (ushort)points[i].X, (ushort)points[i].Y);
                }
            //}
            return megs;
        } 

        
        /// <summary>
        /// 取得每行文字的開始位置
        /// </summary>
        /// <param name="hasIcon">是否有icon</param>
        /// <param name="mode">RGS模式(路徑導引:1,cms:2,)</param>
        /// <returns>文字的開始位置集合</returns>
        private System.Drawing.Point[] getMegLocation(bool hasIcon, int mode)
        {
            System.Drawing.Point[] locations;
            switch (mode)
            {
                case 0:
                    return null;   //都會路網沒有文字位置
                case 1:
                    int k=0;
                    locations = new System.Drawing.Point[rowCount_mode1 * 2];
                    for (int i = 0; i < rowCount_mode1; i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            locations[k] = new System.Drawing.Point(j * 256, i * 64);
                            k++;
                        }
                    }
                    break;
                default:
                    locations = new System.Drawing.Point[rowCount_mode2];
                    if (hasIcon)
                    {
                        for (int i = 0; i < locations.Length; i++)
                        {
                            locations[i] = new System.Drawing.Point(130, i * 64);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < locations.Length; i++)
                        {
                            locations[i] = new System.Drawing.Point(0, i * 64);
                        }
                    }
                    break;
            }
            return locations;
        }
    }


    class RGSData : DeviceData
    {
        int g_code_id_Path=0;
        int g_code_id_City=0;
        public RGSData(string devName, int mileage, string lineId, string secId, string direction, int g_code_id_City, int g_code_id_Path)
        {
            this.devName = devName;
            this.mileage = mileage;
            this.lineId = lineId;
            this.secId = secId;
            this.direction = direction;
            this.g_code_id_Path = g_code_id_Path;
            this.g_code_id_City = g_code_id_City;
        }
        /// <summary>
        /// RSP路徑導引底圖編碼
        /// </summary>
        public int G_Path
        {
            get { return g_code_id_Path; }
        }
        /// <summary>
        /// RSP都會路網底圖編碼
        /// </summary>
        public int G_City
        {
            get { return g_code_id_City; }
        }

        public override bool Equals(object obj)
        {
            return this.devName == (string)obj;
        }

        public override int GetHashCode()
        {
            return this.devName.GetHashCode();
        }

        public override string ToString()
        {
            return this.devName;
        }
    }

    class RGSDeviceList : MyList<RGSData>
    {
        public override RGSData Find(object sender)
        {
            string devName = (string)sender;
            foreach (RGSData RGS in this.getList())
            {
                if (RGS.Equals(devName)) return RGS;
            }
            return null;
        }
    }
}
