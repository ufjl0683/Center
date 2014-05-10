using System;
using System.Collections.Generic;
using System.Text;

namespace Execution.Category
{
    class A_DisplayBlock : ADevice
    {
        protected int DevStartMile = 0;    //�]�ƨ��{ for �ö�ƥ�d��
        protected string DevLineID = string.Empty;//�]�Ƹ��u for �ö�ƥ�d��

        /// <summary>
        /// ���oCMS�C��X
        /// </summary>
        /// <param name="forecolor">�e���C��</param>
        /// <param name="backColor">�I���C��</param>
        /// <returns>CMS�C��X</returns>
        protected byte getCMSWordColor(System.Drawing.Color forecolor, System.Drawing.Color backColor)
        {
            return (byte)((ToCmsColor(changeColor(forecolor)) << 4) + ToCmsColor(changeColor(backColor)));
        }

        /// <summary>
        /// �ഫ�@���C��X��"���B��B��B��"
        /// </summary>
        /// <param name="color">���ഫ���C��</param>
        /// <returns>�ഫ�᪺�C��</returns>
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
        /// �P�_�C��O�_���¡B���B���B��A���ഫ�|��������G�i��X 2009-8-2��(���Ѯv)
        /// </summary>
        /// <param name="color">�n�P�_���C��</param>
        /// <returns>CMS�C��X</returns>
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
        /// ���o�u����
        /// </summary>
        /// <returns>�u����</returns>
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
        /// ���o�]�Ʀܨƥ��I���Z��(0:��,1:��,2:��)
        /// </summary>
        /// <param name="segId">���q���s�X</param>
        /// <param name="maxSegId">�̻����q���s�X</param>
        /// <returns>0:��,1:��,2:��,4:�d��</returns>
        protected int getDeviceDistance(int segId, int maxSegId)
        {
            if (type == CategoryType.RMS || type == CategoryType.RGS)
                return 0;
            int distance = 0;
            if (maxSegId == -99 || segId == -99) return 4;
            if (segId == maxSegId && maxSegId > (int)DevRange["NORMAL"])
                distance = 3;       //�t�Υ�y�D
            else if (segId == 0 || segId == 1)
                distance = 0;       //��
            else if (segId == (int)DevRange["NORMAL"])
                distance = 2;       //��
            else
                distance = 1;       //��
            return distance;
        }

        /// <summary>
        /// �զX�s���������r��
        /// </summary>
        /// <param name="no">�s��</param>
        /// <returns>�r��</returns>
        protected string setMessage(int no,string Location)
        {
            string result = "";
            string start ;
            string end;
            switch (no)
            {
                case 1://����
                    result = "\r";
                    break;
                case 2://�_���a�I�W��(���) isNear need modify                 
                    List<object> list = (List<object>)com.select(DBConnect.DataType.EndSectionName, Command.GetSelectCmd.getEndSectionName(ht["INC_LINEID"].ToString(), ht["INC_DIRECTION"].ToString()[0].ToString()
                        , Convert.ToInt32(ht["ORIGINAL_TO_MILEPOST1"])));
                    string startName = ht["SECTIONNAME"].ToString();
                    string endName = list[0].ToString();
                    result = string.Format("{0}-{1}", startName.Split('-')[0], endName.Split('-')[1]);
                    //result = ht["SECTIONNAME"].ToString();

                    break;
                case 3://�_�����{��
                    {                    
                        int Start = Convert.ToInt32(ht["ORIGINAL_FROM_MILEPOST1"]);
                        int End = Convert.ToInt32(ht["ORIGINAL_TO_MILEPOST1"]);

                        if (!string.IsNullOrEmpty(ht["INC_INTERCHANGE"].ToString()))
                        {
                            return ht["INC_INTERCHANGE"].ToString().Replace("��y�D","");
                        }

                        if (!string.IsNullOrEmpty(ht["BLOCKTYPEID"].ToString()) || (ht["INC_NAME"].Equals(43) || ht["INC_NAME"].Equals(133)))
                        {
                            int BlockType = 0;
                            try
                            {
                                BlockType = Convert.ToInt32(ht["BLOCKTYPEID"]);
                            }
                            catch
                            {
                                ;
                            }
                            if ((BlockType > 1 && BlockType < 7) || ht["INC_NAME"].Equals(43) || ht["INC_NAME"].Equals(133)) //�`���D
                            {
                                string LineID = (string)ht["INC_LINEID"];
                                if (ht["INC_NAME"].Equals(43) && ht["INC_LINEID"].Equals("T74")) // �X�fVD ���{�P�ƥ󨽵{���@ ��ΥX�fVD���{
                                {
                                    System.Data.DataTable DT = RSPGlobal.GetDeviceDT();
                                    System.Data.DataRow DR = DT.Rows.Find(ht["INC_NOTIFY_PLANT"]);
                                    if (DR != null)
                                    {
                                        Start = (int)DR[3];
                                    }
                                }
                                foreach (System.Data.DataRow dr in RSPGlobal.GetDivisionDT().Rows)
                                {
                                    if ((string)dr[3] == LineID)
                                    {
                                        if ((string)dr[1] == "I" || (string)dr[1] == "C")
                                        {
                                            int Distance = Start - (int)dr[2];
                                            if (Distance <= 500 && Distance >= -500)
                                            {
                                                if (ht["INC_NAME"].Equals(43) && ht["INC_LINEID"].Equals("T74"))
                                                {
                                                    string cmd = string.Format("select variablevalue from TBLSYSPARAMETER where groupname = 'DIVISIONROADNAME' and  "
                                                        + " variableName = '{0}'", dr[4]);
                                                    System.Data.DataTable DT = com.Select(cmd);
                                                    if (DT != null && DT.Rows.Count == 1)
                                                    {
                                                        return DT.Rows[0][0].ToString();
                                                    }
                                                    else if (DT != null && DT.Rows.Count == 2)
                                                    {
                                                        string[] Road = DT.Rows[0][0].ToString().Split(',');
                                                        if (ht["INC_DIRECTION"].Equals("E"))
                                                        {
                                                            if (Road[0].Equals("�F�V"))
                                                                return Road[1];
                                                            else
                                                                return DT.Rows[1][0].ToString().Split(',')[1];
                                                        }
                                                        else
                                                        {
                                                            if (Road[0].Equals("��V"))
                                                                return Road[1];
                                                            else
                                                                return DT.Rows[1][0].ToString().Split(',')[1];

                                                        }
                                                    }
                                                }
                                                return (string)dr[0];
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (ht["INC_DIRECTION"].Equals("N") || ht["INC_DIRECTION"].Equals("W"))
                        {
                            start = ((double)Math.Ceiling((double)(Start / 1000D))).ToString();
                            end = Math.Ceiling((double)(End / 1000D)) + "K";
                        }
                        else
                        {

                            start = ((double)Math.Round((double)(Start / 1000))).ToString();
                            end = Math.Round((double)(End / 1000)) + "K";
                        }

                        if (type == CategoryType.OBS  && ht["INC_LINEID"].ToString() == DevLineID) //�ö���q�����]�ƻݬ��_�l���{
                        {
                            if (Start > End && Start > DevStartMile && DevStartMile > End) //W or N
                            {
                                    Start = DevStartMile;
                                    start = Math.Round((double)(Start / 1000)) + "K";
                                    if (start.Equals(end) && Location != "L")
                                    {
                                        throw new OBSLastDeviceException();
                                    }
                            }
                            else if (Start < End && Start < DevStartMile && DevStartMile < End) //E or S
                            {
                                Start = DevStartMile;
                                start = Math.Ceiling((double)(Start / 1000F)) + "K";
                                end = Math.Ceiling((double)(End / 1000F)) + "K";
                                if (start.Equals(end) && Location != "L")
                                {
                                    throw new OBSLastDeviceException();
                                }
                            }                          
                        }
                           
                        if ((start + "K").Equals(end))
                        {
                            result = start + "K";
                        }
                        else
                        {
                            result = start + "~" + end;
                        }
                    }
                    break;
                case 4://�ƥ����O
                    result = ht["SHORTNAME"].ToString();
                    break;
                case 5://���_��m����
                    if (!string.IsNullOrEmpty(ht["INC_INTERCHANGE"].ToString()))
                    {
                        return string.Empty;        
                    }
                    result = getBlockageMessage(Convert.ToInt32(ht["LANE_COUNT"]), ht["INC_BLOCKAGE"].ToString());
                    break;
                case 6://�ö����
                    result = Math.Round((Convert.ToDouble(ht["ORIGINAL_TO_MILEPOST1"]) - Convert.ToDouble(ht["ORIGINAL_FROM_MILEPOST1"])) / 1000) + "����";
                    break;
                case 7://���@��
                    result = "�@";
                    break;
                case 8: //���G��
                    result = "��";
                    break;
                case 9: //��V
                    switch (ht["INC_DIRECTION"].ToString())
                    {
                        case "E":
                            result = "�F��";
                            break;
                        case "W":
                            result = "���";
                            break;
                        case "S":
                            result = "�n�U";
                            break;
                        case "N":
                            result = "�_�W";
                            break;
                        case "EW":
                        case "WE":
                            result = "���V";
                            break;
                        case "SN":
                        case "NS":
                            result = "���V";
                            break;
                    }
                    break;
                case 10://�_�����{��(�p��)
                    {
                        int Start = Convert.ToInt32(ht["ORIGINAL_FROM_MILEPOST1"]);
                        int End = Convert.ToInt32(ht["ORIGINAL_TO_MILEPOST1"]);

                        if (!string.IsNullOrEmpty(ht["INC_INTERCHANGE"].ToString()))
                        {
                            return ht["INC_INTERCHANGE"].ToString();
                        }


                        if (type == CategoryType.OBS && ht["INC_LINEID"].ToString() == DevLineID) //�ö���q�����]�ƻݬ��_�l���{
                        {
                            if (Start > End)
                            {
                                if (Start > DevStartMile)
                                {
                                    Start = DevStartMile;
                                }
                            }
                            else
                            {
                                if (Start < DevStartMile)
                                {
                                    Start = DevStartMile;
                                }
                            }
                        }

                        start = Math.Round(Convert.ToDouble(Start) / 1000, 1) + "K";
                        end = Math.Round(Convert.ToDouble(End) / 1000, 1) + "K";
                        if (start.Equals(end))
                        {
                            result = start;
                        }
                        else
                        {
                            result = start + "~" + end;
                        }
                    }
                    break;
                case 11://�_�I���{��(���)
                    if (!string.IsNullOrEmpty(ht["INC_INTERCHANGE"].ToString()))
                    {
                        return ht["INC_INTERCHANGE"].ToString().Replace("��y�D", "");
                    }
                    if (ht["INC_DIRECTION"].Equals("N") || ht["INC_DIRECTION"].Equals("W"))
                    {
                        result = Math.Ceiling(Convert.ToDouble(ht["ORIGINAL_FROM_MILEPOST1"]) / 1000) + "K";
                    }
                    else
                    {
                        result = Math.Round(Convert.ToDouble(ht["ORIGINAL_FROM_MILEPOST1"]) / 1000) + "K";
                    }
                    break;
                case 12://�_�I���{��(�p��)
                    if (!string.IsNullOrEmpty(ht["INC_INTERCHANGE"].ToString()))
                    {
                        return ht["INC_INTERCHANGE"].ToString().Replace("��y�D", "");
                    }
                    if (ht["INC_DIRECTION"].Equals("N") || ht["INC_DIRECTION"].Equals("W"))
                    {
                        int tmpValue = Convert.ToInt32(ht["ORIGINAL_FROM_MILEPOST1"]);
                        if (tmpValue % 100 > 0)
                        {
                            tmpValue += 100;
                        }
                        result = Math.Round(Convert.ToDouble(tmpValue) / 1000,1)  + "K";
                    }
                    else
                    {
                        result = Math.Round(Convert.ToDouble(ht["ORIGINAL_FROM_MILEPOST1"]) / 1000, 1) + "K";
                    }
                    break;
                case 13://�I�u���e
                    result = ht["MC_Memo"].ToString();
                    break;
                case 14://�D�u�W��(��)
                    {
                        System.Data.DataTable DT = RSPGlobal.GetLineNameDT();
                        System.Data.DataRow dr = DT.Rows.Find(ht["INC_LINEID"].ToString());
                        if (dr != null)
                        {
                            result = dr["LineName"].ToString();
                        }
                    }
                    break;
                case 15://�D�u�W��(�u)
                    {
                        string LineID = ht["INC_LINEID"].ToString();
                        if (LineID.Length > 1)
                        {
                            if (LineID[0].Equals('N'))
                            {
                                result = "��" + LineID.Substring(1);
                            }
                            else if (LineID[0].Equals('T'))
                            {
                                result = "�x" + LineID.Substring(1);
                            }
                        }
                    }
                    break;
                case 16: //�_�I���q�W�� need modiry ? ask GoWay                    
                    result = ht["SECTIONNAME"].ToString();
                    break;
                case 0:
                    result = string.Empty;
                    break;
            }
            return result;
        }

        /// <summary>
        /// ���_���D�ƲզX�r��
        /// </summary>
        /// <param name="laneCount">���D��</param>
        /// <param name="blockage">���_���D</param>
        /// <returns>�r��</returns>
        protected string getBlockageMessage(int laneCount, string blockage)
        {
            string lane = "";
            string other = "";
            try
            {
                if (ht["BLOCKTYPEID"].ToString() == "") ht["BLOCKTYPEID"] = "1";//�n�ЦѮv�g�J

                List<object> megs = (List<object>)com.select(DBConnect.DataType.BlockMeg1, Command.GetSelectCmd.getBlockMeg1(ht["BLOCKTYPEID"].ToString()));

                foreach (object obj in megs)
                {
                    BlockMeg1 meg = (BlockMeg1)obj;
                    if (meg.IfChild)
                    {
                        if ((meg.TypeID == 1)  && !string.IsNullOrEmpty(blockage))
                        {
                            if (blockage.Length != 10)
                                throw new Exception("���_���D�r��զX���~!!");
                            else
                            {
                                string laneStr = blockage.Substring(0, 6);
                                string otherStr = blockage.Substring(6, 4);
                                List<object> list1 = (List<object>)com.select(DBConnect.DataType.BlockMeg2, Command.GetSelectCmd.getBlockMeg2(meg.TypeID, laneCount, laneStr));

                                List<object> list2 = (List<object>)com.select(DBConnect.DataType.BlockMeg2, Command.GetSelectCmd.getBlockMeg2(16, 0, otherStr));

                                if (!string.IsNullOrEmpty(list1[0].ToString()))
                                    lane += string.Format("{0}���D", list1[0].ToString()) + "��";
                                lane += list2[0].ToString() + "��";
                            }
                        }
                        if (meg.TypeID == 16 && !string.IsNullOrEmpty(blockage))
                        {
                            if (blockage.Length != 4)
                                throw new Exception();
                            else
                            {
                                List<object> list = (List<object>)com.select(DBConnect.DataType.BlockMeg2, Command.GetSelectCmd.getBlockMeg2(16, 0, blockage));
                                lane += list[0].ToString() + "��";
                            }
                        }
                        else
                        {
                            if (meg.TypeID == 7 && !string.IsNullOrEmpty(blockage))
                            {
                                other += "��";
                                string laneStr = blockage.Substring(0, 10);

                                for (int i = 0; i < laneStr.Length; i++)
                                {
                                    if (laneStr[i] == '1')
                                    {
                                        other += string.Format("{0}��", i + 1);
                                    }
                                }
                                other = other.TrimEnd('��') + "���D";
                            }
                        }
                    }
                    else
                    {
                        if (meg.Message == "�������j�a")
                        {
                            other += "����\f\b���j�a" + "��";
                        }
                        else
                            other += meg.Message + "��";
                    }
                }
            }
            catch
            {
                throw new Exception("���_���D�r��զX���~!!");
            }
            string message = other + lane;
            if (!string.IsNullOrEmpty(message))
            {
                message = message.Replace("��", "�B");
                message = message.TrimEnd('�B');
                if (message.IndexOf('�B') > -1)
                    message = message.Substring(0, message.LastIndexOf("�B")) + "��" + message.Substring(message.LastIndexOf("�B") + 1);
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
                    string[] submegs = new string[] { megs[i] };//.Split('��');
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
                                                                               megColor.getTmpSubColors(megColor.ForeColor, char_count, submegs[j].Length,submegs[j]),
                                                                               megColor.getTmpSubColors(megColor.BackColor, char_count, submegs[j].Length,submegs[j]));
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
        /// ���ѰT���M�C��
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
                        bool cont = false;
                        int index = 0;
                        double k = 0;
                        int BK = 0;
                        for (int i = 0; i < meg.Length; i++)
                        {
                            if (meg[i] == '\b')
                            {
                                cont = true;
                                BK = i;
                                continue;
                            }
                            else if (meg[i] == '\f')
                            {
                                cont = false;
                                if (i < meg.Length -1)
                                    continue;
                            }
                            if (k == colCount)
                            {
                                if (cont && str_row_count + 1 < rowCount && Encoding.GetEncoding("Big5").GetByteCount(meg.Substring(BK+1,i - BK)) <= colCount * 2 )
                                {
                                    if (BK > 0)
                                        i = BK - 1;
                                    else
                                        i = 0;
                                }
                                string newmeg = meg.Substring(index, i - index).Replace("\b", string.Empty).Replace("\f", string.Empty);
                                System.Drawing.Color[] foreColor = megColor.getSubColors(megColor.ForeColor, index + char_count, newmeg.Length, meg.Substring(index, i - index));
                                System.Drawing.Color[] backColor = megColor.getSubColors(megColor.BackColor, index + char_count, newmeg.Length, meg.Substring(index, i - index));
                                byte[] color = megColor.getSubColors(megColor.Color, index + char_count, newmeg.Length, meg.Substring(index, i - index));


                                if ((rowCount == 1 && colCount == 8) || (rowCount == 2 && colCount == 5))
                                {
                                    FixVDevBug(ref newmeg, ref color, ref foreColor, ref backColor);
                                }
                                if (type == DeviceType.CMS)
                                    rowWordCounts[str_row_count] = new BlockMegAndColor(newmeg, color);
                                else
                                    rowWordCounts[str_row_count] = new BlockMegAndColor(newmeg, foreColor, backColor);
                                k = 0;

                                index = i;
                                if (++str_row_count == rowCount) break;
                                if (i == BK -1)
                                {
                                    continue;
                                }
                              
                            }
                            if (decisionHolomorphic(meg[i]))
                                k += 1;
                            else
                                k += .5;

                            if (i + 1 < meg.Length && ((k + 0.5) == colCount) && (decisionHolomorphic(meg[i + 1]) || meg[i+1] == '\b' || meg[i +1] == '\f' ))//�b�νվ�
                            {
                                k = colCount;
                            }

                            if (i == meg.Length - 1)
                            {
                                string newmeg = meg.Substring(index, meg.Length - index).Replace("\b", string.Empty).Replace("\f", string.Empty); ;
                                System.Drawing.Color[] foreColor = megColor.getSubColors(megColor.ForeColor, index + char_count, newmeg.Length, meg.Substring(index, meg.Length - index));
                                System.Drawing.Color[] backColor = megColor.getSubColors(megColor.BackColor, index + char_count, newmeg.Length, meg.Substring(index, meg.Length - index));
                                byte[] color = megColor.getSubColors(megColor.Color, index + char_count, newmeg.Length, meg.Substring(index, meg.Length - index));

                                if ((rowCount == 1 && colCount == 8) || (rowCount == 2 && colCount == 5))
                                {
                                    FixVDevBug(ref newmeg, ref color, ref foreColor, ref backColor);
                                }
                                
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

        private void FixVDevBug(ref string newmeg,ref byte[] color,ref System.Drawing.Color[] foreColor,ref System.Drawing.Color[] backColor)
        {
            List<byte> colorList = new List<byte>(color);
            List<System.Drawing.Color> foreColorList = new List<System.Drawing.Color>(foreColor);
            List<System.Drawing.Color> backColorList = new List<System.Drawing.Color>(backColor);

            int y = 0;
            for (int z = 0; z < newmeg.Length - 1; z++)
            {
                if (decisionHolomorphic(newmeg[z]))
                    y = y + 2;
                else
                    y = y + 1;
                if (y % 2 == 1)
                {
                    if (decisionHolomorphic(newmeg[z + 1]))
                    {
                        newmeg = newmeg.Insert(z + 1, " ");
                        colorList.Insert(z + 1, 32);
                        foreColorList.Insert(z + 1, System.Drawing.Color.Black);
                        backColorList.Insert(z + 1, System.Drawing.Color.Black);
                    }
                }
            }
            color = colorList.ToArray();
            foreColor = foreColorList.ToArray();
            backColor = backColorList.ToArray();
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
