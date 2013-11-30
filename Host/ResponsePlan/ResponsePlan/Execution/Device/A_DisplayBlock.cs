using System;
using System.Collections.Generic;
using System.Text;

namespace Execution.Category
{
    class A_DisplayBlock : ADevice
    {
        /// <summary>
        /// 取得CMS顏色碼
        /// </summary>
        /// <param name="forecolor">前景顏色</param>
        /// <param name="backColor">背景顏色</param>
        /// <returns>CMS顏色碼</returns>
        protected byte getCMSWordColor(System.Drawing.Color forecolor, System.Drawing.Color backColor)
        {
            return (byte)((ToCmsColor(changeColor(forecolor)) << 4) + ToCmsColor(changeColor(backColor)));
        }

        /// <summary>
        /// 轉換一般顏色碼為"紅、橙、綠、黑"
        /// </summary>
        /// <param name="color">待轉換的顏色</param>
        /// <returns>轉換後的顏色</returns>
        private System.Drawing.Color changeColor(System.Drawing.Color color)
        {
            int G, R;
            if (color.R > 177)
            {
                R = 255;
                if (color.G > 82)
                    G = 165;
                else
                    G = 0;
            }
            else
            {
                R = 0;

                if (color.G > 177)
                    G = 255;
                else
                    G = 0;
            }
            return System.Drawing.Color.FromArgb(R, G, 0);
        }

        /// <summary>
        /// 判斷顏色是否為黑、紅、黃、綠，並轉換四色對應的二進位碼 2009-8-2８(陳老師)
        /// </summary>
        /// <param name="color">要判斷的顏色</param>
        /// <returns>CMS顏色碼</returns>
        private static byte ToCmsColor(System.Drawing.Color color)
        {
            switch (color.Name.ToUpper())
            {
                case "BLACK":
                case "FF000000":
                    return 0;
                case "GREEN":
                case "FF00FF00":
                    return 1;
                case "RED":
                case "FFFF0000":
                    return 2;
                case "ORANGE":
                case "FFFFA500":
                    return 3;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// 取得優先度
        /// </summary>
        /// <returns>優先度</returns>
        override protected int getPriority()
        {
            List<object> list = (List<object>)com.select(DBConnect.DataType.Priority, Command.GetSelectCmd.getPriority(secType, megType));
            if (list.Count > 0)
            {
                return (int)list[0];
            }

            return -1;
        }


        /// <summary>
        /// 取得設備至事件點的距離(0:近,1:中,2:遠)
        /// </summary>
        /// <param name="segId">路段的編碼</param>
        /// <param name="maxSegId">最遠路段的編碼</param>
        /// <returns>0:近,1:中,2:遠,4:範圍內</returns>
        protected int getDeviceDistance(int segId, int maxSegId)
        {
            int distance = 0;
            if (maxSegId == -99) return 4;
            if (segId == maxSegId)
                distance = 2;       //遠
            else if (segId == 0)
                distance = 0;       //近
            else
                distance = 1;       //中
            return distance;
        }

        /// <summary>
        /// 組合編號對應的字串
        /// </summary>
        /// <param name="no">編號</param>
        /// <returns>字串</returns>
        protected string setMessage(int no)
        {
            string result = "";
            string start ;
            string end;
            switch (no)
            {
                case 1://跳行
                    result = "\r";
                    break;
                case 2://起迄地點名稱(整數)
                    List<object> list = (List<object>)com.select(DBConnect.DataType.EndSectionName, Command.GetSelectCmd.getEndSectionName(ht["INC_LINEID"].ToString(), ht["INC_DIRECTION"].ToString(), Convert.ToInt32(ht["TO_MILEPOST1"])));
                    string startName = ht["SECTIONNAME"].ToString();
                    string endName = list[0].ToString();
                    result = string.Format("{0}-{1}", startName.Split('-')[0], endName.Split('-')[1]);
                    //result = ht["SECTIONNAME"].ToString();

                    break;
                case 3://起迄里程數
                    start = Math.Round(Convert.ToDouble(ht["FROM_MILEPOST1"]) / 1000) + "K";
                    end = Math.Round(Convert.ToDouble(ht["TO_MILEPOST1"]) / 1000) + "K";
                    result = start + "~" + end;
                    break;
                case 4://事件次類別
                    result = ht["SHORTNAME"].ToString();
                    break;
                case 5://阻斷位置說明
                    result = getBlockageMessage(Convert.ToInt32(ht["LANE_COUNT"]), ht["INC_BLOCKAGE"].ToString());
                    break;
                case 6://壅塞長度
                    result = Math.Round((Convert.ToDouble(ht["TO_MILEPOST1"]) - Convert.ToDouble(ht["FROM_MILEPOST1"])) / 1000) + "公里";
                    break;
                case 7://跳一格
                    result = "　";
                    break;
                case 8: //跳二格
                    result = "㊣";
                    break;
                case 9: //方向
                    switch (ht["INC_DIRECTION"].ToString())
                    {
                        case "E":
                            result = "東";
                            break;
                        case "W":
                            result = "西";
                            break;
                        case "S":
                            result = "南";
                            break;
                        case "N":
                            result = "北";
                            break;
                    }
                    break;
                case 10://起迄里程數(小數)
                    start = Math.Round(Convert.ToDouble(ht["FROM_MILEPOST1"]) / 1000, 1) + "K";
                    end = Math.Round(Convert.ToDouble(ht["TO_MILEPOST1"]) / 1000, 1) + "K";
                    result = start + "~" + end;
                    break;
                case 11://起點里程數(整數)
                    result = Math.Round(Convert.ToDouble(ht["FROM_MILEPOST1"]) / 1000) + "K";
                    break;
                case 12://起點里程數(小數)
                    result = Math.Round(Convert.ToDouble(ht["FROM_MILEPOST1"]) / 1000, 1) + "K";
                    break;
                case 13://施工內容
                    result = ht["MC_Memo"].ToString();
                    break;
            }
            return result;
        }

        /// <summary>
        /// 阻斷車道數組合字串
        /// </summary>
        /// <param name="laneCount">車道數</param>
        /// <param name="blockage">阻斷車道</param>
        /// <returns>字串</returns>
        protected string getBlockageMessage(int laneCount, string blockage)
        {
            string lane = "";
            string other = "";
            try
            {
                if (ht["BLOCKTYPEID"].ToString() == "") ht["BLOCKTYPEID"] = "1";//要請老師寫入

                List<object> megs = (List<object>)com.select(DBConnect.DataType.BlockMeg1, Command.GetSelectCmd.getBlockMeg1(ht["BLOCKTYPEID"].ToString()));

                foreach (object obj in megs)
                {
                    BlockMeg1 meg = (BlockMeg1)obj;
                    if (meg.IfChild)
                    {
                        if ((meg.TypeID == 1 || meg.TypeID == 16) && !string.IsNullOrEmpty(blockage))
                        {
                            if (blockage.Length != 10)
                                throw new Exception("阻斷車道字串組合錯誤!!");
                            else
                            {
                                string laneStr = blockage.Substring(0, 6);
                                string otherStr = blockage.Substring(5, 4);
                                List<object> list1 = (List<object>)com.select(DBConnect.DataType.BlockMeg2, Command.GetSelectCmd.getBlockMeg2(meg.TypeID, laneCount, laneStr));

                                List<object> list2 = (List<object>)com.select(DBConnect.DataType.BlockMeg2, Command.GetSelectCmd.getBlockMeg2(16, 0, otherStr));

                                if (!string.IsNullOrEmpty(list1[0].ToString()))
                                    lane += string.Format("{0}車道", list1[0].ToString()) + "及";
                                lane += list2[0].ToString() + "及";
                            }
                        }
                        else
                        {
                            if (meg.TypeID == 7 && !string.IsNullOrEmpty(blockage))
                            {
                                other += "第";
                                string laneStr = blockage.Substring(0, 10);

                                for (int i = 0; i < laneStr.Length; i++)
                                {
                                    if (laneStr[i] == '1')
                                    {
                                        other += string.Format("{0}及", i + 1);
                                    }
                                }
                                other = other.TrimEnd('及') + "車道";
                            }
                        }
                    }
                    else
                    {
                        other += meg.Message + "及";
                    }
                }
            }
            catch
            {
                throw new Exception("阻斷車道字串組合錯誤!!");
            }
            string message = other + lane;
            if (!string.IsNullOrEmpty(message))
            {
                message = message.Replace("及", "、");
                message = message.TrimEnd('、');
                if (message.IndexOf('、') > -1)
                    message = message.Substring(0, message.LastIndexOf("、")) + "及" + message.Substring(message.LastIndexOf("、") + 1);
            }
            return message;
        }


        protected BlockMegAndColor[] separateMegColor(BlockMegAndColor megColor)
        {
            List<BlockMegAndColor> rowWordCounts = new List<BlockMegAndColor>();
            string[] megs = megColor.Message.Split('\r');
            int str_row_count = 0;
            int char_count = 0;
            for (int i = 0; i < megs.Length; i++)
            {
                if (string.IsNullOrEmpty(megs[i]) || megs[i].Trim() == "")
                {
                    rowWordCounts.Add(new BlockMegAndColor("", new System.Drawing.Color[0], new System.Drawing.Color[0]));
                    rowWordCounts.Add(new BlockMegAndColor("", new System.Drawing.Color[0], new System.Drawing.Color[0]));
                    str_row_count = +2; ;
                }
                else
                {
                    string[] submegs = megs[i].Split('㊣');
                    for (int j = 0; j < submegs.Length; j++)
                    {
                        if (string.IsNullOrEmpty(submegs[j]))
                        {
                            rowWordCounts.Add(new BlockMegAndColor("", new System.Drawing.Color[0], new System.Drawing.Color[0]));
                            str_row_count++;
                        }
                        else
                        {
                            BlockMegAndColor myMegColor = new BlockMegAndColor(submegs[j],
                                                                               megColor.getSubColors(megColor.ForeColor, char_count, submegs[j].Length),
                                                                               megColor.getSubColors(megColor.BackColor, char_count, submegs[j].Length));
                            if (j % 2 == 0)
                                rowWordCounts.AddRange(separateMegColor(devType, 1, 2, myMegColor));
                            else
                                rowWordCounts.AddRange(separateMegColor(devType, 1, 4, myMegColor));
                            char_count += submegs[j].Length + 1;
                        }
                        if (j == 1) break;
                    }
                    if (submegs.Length == 1)
                    {
                        rowWordCounts.Add(new BlockMegAndColor("", new System.Drawing.Color[0], new System.Drawing.Color[0]));
                        str_row_count++;
                    }
                    char_count--;
                }
                if (i == 3) break;
            }

            return rowWordCounts.ToArray();
        }

        /// <summary>
        /// 分解訊息和顏色
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="colCount"></param>
        /// <param name="megColor"></param>
        /// <returns></returns>
        protected BlockMegAndColor[] separateMegColor(DeviceType type, int rowCount, int colCount, BlockMegAndColor megColor)
        {
            BlockMegAndColor[] rowWordCounts = new BlockMegAndColor[rowCount];
            if (string.IsNullOrEmpty(megColor.Message))
            {
                for (int i = 0; i < rowWordCounts.Length; i++)
                {
                    rowWordCounts[i] = new BlockMegAndColor("", new System.Drawing.Color[0], new System.Drawing.Color[0]);
                }
            }
            else
            {
                int str_row_count = 0;
                int char_count = 0;
                foreach (string meg in megColor.Message.Split('\r'))
                {
                    if (string.IsNullOrEmpty(meg))
                    {
                        rowWordCounts[str_row_count] = new BlockMegAndColor("", new System.Drawing.Color[0], new System.Drawing.Color[0]);
                        str_row_count++;
                    }
                    else
                    {
                        int index = 0;
                        double k = 0;
                        for (int i = 0; i < meg.Length; i++)
                        {
                            if (k == colCount)
                            {
                                string newmeg = meg.Substring(index, i - index);
                                System.Drawing.Color[] foreColor = megColor.getSubColors(megColor.ForeColor, index + char_count, newmeg.Length);
                                System.Drawing.Color[] backColor = megColor.getSubColors(megColor.BackColor, index + char_count, newmeg.Length);
                                byte[] color = megColor.getSubColors(megColor.Color, index + char_count, newmeg.Length);
                                if (type == DeviceType.CMS)
                                    rowWordCounts[str_row_count] = new BlockMegAndColor(newmeg, color);
                                else
                                    rowWordCounts[str_row_count] = new BlockMegAndColor(newmeg, foreColor, backColor);
                                k = 0;
                                index = i;
                                if (++str_row_count == rowCount) break;
                            }
                            if (decisionHolomorphic(meg[i]))
                                k += 1;
                            else
                                k += .5;

                            if (i + 1 < meg.Length && decisionHolomorphic(meg[i + 1]) && ((k + 0.5) == (colCount))) k = colCount; ;

                            if (i == meg.Length - 1)
                            {
                                string newmeg = meg.Substring(index, meg.Length - index);
                                System.Drawing.Color[] foreColor = megColor.getSubColors(megColor.ForeColor, index + char_count, newmeg.Length);
                                System.Drawing.Color[] backColor = megColor.getSubColors(megColor.BackColor, index + char_count, newmeg.Length);
                                byte[] color = megColor.getSubColors(megColor.Color, index + char_count, newmeg.Length);
                                if (type == DeviceType.CMS)
                                    rowWordCounts[str_row_count] = new BlockMegAndColor(newmeg, color);
                                else
                                    rowWordCounts[str_row_count] = new BlockMegAndColor(newmeg, foreColor, backColor);
                                str_row_count++;
                            }
                        }
                        if (str_row_count == rowCount) break;
                        char_count += meg.Length;
                    }
                }
            }
            return rowWordCounts;
        }

        private bool decisionHolomorphic(char word)
        {
            int length = Encoding.GetEncoding("Big5").GetByteCount(word.ToString());
            if (length == 1)
                return false;
            else
                return true;
        }
    }
}
