using System;
using System.Collections.Generic;
using System.Text;

namespace Execution.Category
{
    class A_DisplayBlock : ADevice
    {
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
            int distance = 0;
            if (maxSegId == -99) return 4;
            if (segId == maxSegId)
                distance = 2;       //��
            else if (segId == 0)
                distance = 0;       //��
            else
                distance = 1;       //��
            return distance;
        }

        /// <summary>
        /// �զX�s���������r��
        /// </summary>
        /// <param name="no">�s��</param>
        /// <returns>�r��</returns>
        protected string setMessage(int no)
        {
            string result = "";
            string start ;
            string end;
            switch (no)
            {
                case 1://����
                    result = "\r";
                    break;
                case 2://�_���a�I�W��(���)
                    List<object> list = (List<object>)com.select(DBConnect.DataType.EndSectionName, Command.GetSelectCmd.getEndSectionName(ht["INC_LINEID"].ToString(), ht["INC_DIRECTION"].ToString(), Convert.ToInt32(ht["TO_MILEPOST1"])));
                    string startName = ht["SECTIONNAME"].ToString();
                    string endName = list[0].ToString();
                    result = string.Format("{0}-{1}", startName.Split('-')[0], endName.Split('-')[1]);
                    //result = ht["SECTIONNAME"].ToString();

                    break;
                case 3://�_�����{��
                    start = Math.Round(Convert.ToDouble(ht["FROM_MILEPOST1"]) / 1000) + "K";
                    end = Math.Round(Convert.ToDouble(ht["TO_MILEPOST1"]) / 1000) + "K";
                    result = start + "~" + end;
                    break;
                case 4://�ƥ����O
                    result = ht["SHORTNAME"].ToString();
                    break;
                case 5://���_��m����
                    result = getBlockageMessage(Convert.ToInt32(ht["LANE_COUNT"]), ht["INC_BLOCKAGE"].ToString());
                    break;
                case 6://�ö����
                    result = Math.Round((Convert.ToDouble(ht["TO_MILEPOST1"]) - Convert.ToDouble(ht["FROM_MILEPOST1"])) / 1000) + "����";
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
                            result = "�F";
                            break;
                        case "W":
                            result = "��";
                            break;
                        case "S":
                            result = "�n";
                            break;
                        case "N":
                            result = "�_";
                            break;
                    }
                    break;
                case 10://�_�����{��(�p��)
                    start = Math.Round(Convert.ToDouble(ht["FROM_MILEPOST1"]) / 1000, 1) + "K";
                    end = Math.Round(Convert.ToDouble(ht["TO_MILEPOST1"]) / 1000, 1) + "K";
                    result = start + "~" + end;
                    break;
                case 11://�_�I���{��(���)
                    result = Math.Round(Convert.ToDouble(ht["FROM_MILEPOST1"]) / 1000) + "K";
                    break;
                case 12://�_�I���{��(�p��)
                    result = Math.Round(Convert.ToDouble(ht["FROM_MILEPOST1"]) / 1000, 1) + "K";
                    break;
                case 13://�I�u���e
                    result = ht["MC_Memo"].ToString();
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
                        if ((meg.TypeID == 1 || meg.TypeID == 16) && !string.IsNullOrEmpty(blockage))
                        {
                            if (blockage.Length != 10)
                                throw new Exception("���_���D�r��զX���~!!");
                            else
                            {
                                string laneStr = blockage.Substring(0, 6);
                                string otherStr = blockage.Substring(5, 4);
                                List<object> list1 = (List<object>)com.select(DBConnect.DataType.BlockMeg2, Command.GetSelectCmd.getBlockMeg2(meg.TypeID, laneCount, laneStr));

                                List<object> list2 = (List<object>)com.select(DBConnect.DataType.BlockMeg2, Command.GetSelectCmd.getBlockMeg2(16, 0, otherStr));

                                if (!string.IsNullOrEmpty(list1[0].ToString()))
                                    lane += string.Format("{0}���D", list1[0].ToString()) + "��";
                                lane += list2[0].ToString() + "��";
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
                    string[] submegs = megs[i].Split('��');
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
