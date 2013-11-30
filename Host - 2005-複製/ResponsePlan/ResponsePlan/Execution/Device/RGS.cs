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
            if (devNames == null) return displayht;
            
            foreach (RemoteInterface.HC.FetchDeviceData devName in devNames)
            {
                int distance = getDeviceDistance(devName.SegId, maxSegId);
                data = rgslist.Find(devName.DevName);
                outputs = (List<object>)com.select(DBConnect.DataType.RgsCategory, Command.GetSelectCmd.getRGSCategory(Convert.ToInt32(DevRange["RULEID"]), (int)secType, devType.ToString(), distance, devName.DevName, megType.ToString(), ht["INC_LINEID"].ToString().Trim()));
                foreach (object obj in outputs)
                {
                    List<object> output = new List<object>();
                    output.AddRange(new object[] { getPriority(), obj });
                    displayht.Add(devName.DevName, output);
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
            this.mode = mode;
            RemoteInterface.MFCC.RGS_GenericDisplay_Data outputData = null;
            if (mode == 0)//都會路網沒有文字
            {
                byte g = 0;
                if (data != null)
                    g = (byte)data.G_City;

                outputData = new RemoteInterface.MFCC.RGS_GenericDisplay_Data((byte)mode, g, new RemoteInterface.MFCC.RGS_Generic_ICON_Data[0], new RemoteInterface.MFCC.RGS_Generic_Message_Data[0], new RemoteInterface.MFCC.RGS_Generic_Section_Data[0]);
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
                    string mag = setMessage(no);

                    message = message.Replace(text, mag);
                    MessColor messColor = (MessColor)messColorsHT[no];
                    rspMegColors.Add(new RSPMegColor(mag, messColor.Forecolor, messColor.BackColor));
                }

                RemoteInterface.MFCC.RGS_Generic_Message_Data[] megs = getMessageData(message, rspMegColors, isIcon == 0 ? false : true, mode);

                //改為4行
                RemoteInterface.MFCC.RGS_Generic_Message_Data[] megs4 = new RemoteInterface.MFCC.RGS_Generic_Message_Data[4];
                if (mode == 1)
                {                    
                    for (int i = 0; i < 4; i++)
                    {
                        RemoteInterface.MFCC.RGS_Generic_Message_Data meg1 = megs[i * 2];
                        RemoteInterface.MFCC.RGS_Generic_Message_Data meg2 = megs[i * 2 + 1];
                        int wordcount = Encoding.GetEncoding("Big5").GetByteCount(meg1.messgae);
                        System.Drawing.Color[] foreColor = new System.Drawing.Color[meg1.forecolor.Length + meg2.backcolor.Length + 2 + 4 - wordcount];
                        System.Drawing.Color[] backColor = new System.Drawing.Color[meg1.backcolor.Length + meg2.backcolor.Length + 2 + 4 - wordcount];
                        int k = 0;
                        for (int j = 0; j < meg1.forecolor.Length; j++)
                        {
                            foreColor[k] = meg1.forecolor[j];
                            backColor[k] = meg1.backcolor[j];
                            k++;
                        }

                        string msg;
                        if (wordcount == 4)
                        {
                            msg = meg1.messgae + "　　" + meg2.messgae;
                        }
                        else
                        {
                            msg = meg1.messgae;
                            for (int x = 0; x < 4 - wordcount; x++)
                            {
                                msg += " ";
                            }
                            msg += "　　" + meg2.messgae;
                            for (int j = 0; j < 4 - wordcount; j++)
                            {
                                foreColor[k] = System.Drawing.Color.Red;
                                backColor[k] = System.Drawing.Color.Black;
                                k++;
                            }

                        }
                        for (int j = 0; j < 2; j++)
                        {
                            foreColor[k] = System.Drawing.Color.Red;
                            backColor[k] = System.Drawing.Color.Black;
                            k++;
                        }

                        for (int j = 0; j < meg2.forecolor.Length; j++)
                        {
                            foreColor[k] = meg2.forecolor[j];
                            backColor[k] = meg2.backcolor[j];
                            k++;
                        }

                        megs4[i] = new RemoteInterface.MFCC.RGS_Generic_Message_Data(msg, foreColor, backColor, meg1.x, meg1.y);
                    }
                }
                if (mode == 1)
                {
                    byte g = 0;
                    if (data != null)
                        g = (byte)data.G_Path;
                    
                    //outputData = new RemoteInterface.MFCC.RGS_GenericDisplay_Data((byte)mode, g, new RemoteInterface.MFCC.RGS_Generic_ICON_Data[0], megs, new RemoteInterface.MFCC.RGS_Generic_Section_Data[0]);
                    outputData = new RemoteInterface.MFCC.RGS_GenericDisplay_Data((byte)mode, g, new RemoteInterface.MFCC.RGS_Generic_ICON_Data[0], megs4, new RemoteInterface.MFCC.RGS_Generic_Section_Data[0]);
                }
                else
                {
                    RemoteInterface.MFCC.RGS_Generic_ICON_Data icon = null;
                    if (isIcon == 1)
                        icon = new RemoteInterface.MFCC.RGS_Generic_ICON_Data(Convert.ToByte(ht["LINE_ICON"]), 0, 0);
                    else if (isIcon == 2)
                        icon = new RemoteInterface.MFCC.RGS_Generic_ICON_Data(Convert.ToByte(ht["ACC_ICON"]), 0, 0);
                    outputData = new RemoteInterface.MFCC.RGS_GenericDisplay_Data((byte)mode, 0, new RemoteInterface.MFCC.RGS_Generic_ICON_Data[] { icon }, megs, new RemoteInterface.MFCC.RGS_Generic_Section_Data[0]);
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
            for (int i = 0; i < newMeg.Length; i++)
            {
                foreColor.Add(System.Drawing.Color.Red);
                backColor.Add(System.Drawing.Color.Black);
            }

            foreach (RSPMegColor de in rspMegColors)
            {
                for (int i = newMeg.IndexOf(de.Message); i <= newMeg.IndexOf(de.Message) + de.Message.Length - 1; i++)
                {
                    if (i == -1) continue;
                    foreColor[i] = de.ForeColor;
                    backColor[i] = de.BackColor;
                    if (newMeg[i] == '㊣' && mode != 1)
                    {
                        foreColor.Insert(i, foreColor[i]);
                        backColor.Insert(i, backColor[i]);
                    }
                }
            }
            System.Drawing.Point[] points = getMegLocation(hasIcon, mode);
            if (mode == 1)
            {
                megs = new RemoteInterface.MFCC.RGS_Generic_Message_Data[rowCount_mode1 * 2];
                BlockMegAndColor megColor = new BlockMegAndColor(message, foreColor.ToArray(), backColor.ToArray());

                BlockMegAndColor[] sepMegColors = separateMegColor(megColor);

                for (int i = 0; i < sepMegColors.Length; i++)
                {
                    if (sepMegColors[i] == null)
                    {
                        megs = new RemoteInterface.MFCC.RGS_Generic_Message_Data[0];
                        break;
                    }
                    else
                        megs[i] = new RemoteInterface.MFCC.RGS_Generic_Message_Data(sepMegColors[i].Message, sepMegColors[i].ForeColor, sepMegColors[i].BackColor, (ushort)points[i].X, (ushort)points[i].Y);
                }
            }
            else
            {
                megs = new RemoteInterface.MFCC.RGS_Generic_Message_Data[rowCount_mode2];
                int chrCount = 0; 
                if (hasIcon)
                    chrCount = 6;
                else
                    chrCount = 8;

                BlockMegAndColor megColor = new BlockMegAndColor(message.Replace("㊣", "　　"), foreColor.ToArray(), backColor.ToArray());
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
            }
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
