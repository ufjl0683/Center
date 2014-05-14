using System;
using System.Collections.Generic;
using System.Text;
using Execution.Command;

namespace Execution.Category
{
    /// <summary>
    /// �˹�����GCMS
    /// </summary>
    internal class CMS : A_DisplayBlock 
    {
        System.Collections.Hashtable messColorsHT;
        string CMSDevName = string.Empty;

        public CMS(AEvent aDevice, CategoryType type, System.Collections.Hashtable ht, DeviceType devType, System.Collections.Hashtable DevRange, Degree degree)
        {
            Initialize(aDevice, type, ht, devType, DevRange, degree);
            this.GetMessRuleData += new GetMessRuleDataHandler(CMS_GetMessRuleData);
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
            sender.Memo += "==>> CMS";
            sender.CMSOutputData = getDisplayContent(type);
            aDevice.produceExecution(sender);            
            return sender;
        }


        /// <summary>
        /// �]�wCMS���
        /// </summary>
        /// <param name="devNames"></param>
        /// <returns></returns>        
        protected override System.Collections.Hashtable setDisplay(RemoteInterface.HC.FetchDeviceData[] devNames, int maxSegId, MegType megType)
        {
            System.Collections.Hashtable displayht = new System.Collections.Hashtable();

            List<object> outputs = new List<object>();
            if (devNames == null || devNames.Length == 0) return displayht;

            foreach (RemoteInterface.HC.FetchDeviceData devName in devNames)
            {
                int distance = getDeviceDistance(devName.SegId, maxSegId);
                DevStartMile = devName.Mileage;
                DevLineID = devName.LineId;
                CMSDevName = devName.DevName;

                outputs = (List<object>)com.select(DBConnect.DataType.CmsCategory, Command.GetSelectCmd.getCMSCategory(Convert.ToInt32(DevRange["RULEID"]), (int)secType, devType.ToString(), distance, devName.DevName, megType.ToString(), ht["INC_LINEID"].ToString().Trim(),devName.Location,devName.LineId));
                foreach (object obj in outputs)
                {
                    List<object> output=new List<object>();
                    output.AddRange(new object[] { getPriority(), obj });
                    if (!displayht.Contains(devName.DevName))
                        displayht.Add(devName.DevName, output);
                    else if (devName.Location == "L")
                    {
                        displayht[devName.DevName] = output;
                    }
                }
            }
            return displayht;
        }      


        /// <summary>
        /// �̾�CMS���������o��ܤ��e
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dr"></param>
        /// <returns></returns>
        object CMS_GetMessRuleData(DBConnect.DataType type, object reader)
        {
            if (type == DBConnect.DataType.CmsCategory)
            {
                System.Data.Odbc.OdbcDataReader dr = (System.Data.Odbc.OdbcDataReader)reader;
                return SetCMSDislay(dr[5].ToString(), dr[6].ToString(), Convert.ToInt32(dr[7]), Convert.ToInt32(dr[9]),(string)dr[11],(int)dr[12]);
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
            else
                return null;
        }

        /// <summary>
        /// �̾�CMS���������o��ܤ��e
        /// </summary>
        /// <param name="blockType">���O����</param>
        /// <param name="message">�T��</param>
        /// <param name="isIcon">���O�ϧ����(0:�����q,1:�q��D�ϥ�,2:�q�ƬG�ϥ�,3:�q�C��)</param>
        /// <param name="mode">�Ҧ�</param>
        /// <returns></returns>
        private RemoteInterface.HC.CMSOutputData SetCMSDislay(string blockType, string message, int isIcon,int mode,string Location,int Show_icon)
        {
            this.mode = mode;
            List<string> changeTexts = new List<string>();
            string str = "";
            if (CMSDevName == "CMS-N3-S-196.7")
            {
                message = "�x���a�k";
            }
            else if (CMSDevName == "CMS-N3-S-196.8")
            {
                message = "�x���a�k";
            }

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


            try
            {
                foreach (string text in changeTexts)
                {
                    int no = Convert.ToInt32(text.Substring(text.IndexOf('[') + 1, text.IndexOf(']') - 1));
                    string mag = setMessage(no,Location);
                    message = message.Replace(text,'\b' +  mag + '\f');
                    MessColor messColor = (MessColor)messColorsHT[no];
                    //if (blockType == "2X8(���m)")
                    //{
                    //    rspMegColors.Add(new RSPMegColor(mag, messColor.Forecolor, messColor.BackColor));
                    //}
                    //else
                    //{
                        rspMegColors.Add(new RSPMegColor(mag, getCMSWordColor(messColor.Forecolor, messColor.BackColor)));
                    //}
                }
            }
            catch (OBSLastDeviceException)
            {
                message = "���h�p�߾r�p";
                rspMegColors.Clear();
            }
            

            string newMeg = message.Replace("\r", "");
            List<byte> colors = new List<byte>();
            
            //byte[] colors = new byte[message.Replace("\r", "").Length];
            for (int i = 0; i < newMeg.Length; i++)
            {     
                colors.Add(32);
            }

            foreach (RSPMegColor de in rspMegColors)
            {
                for (int i = newMeg.IndexOf(de.Message); i <= newMeg.IndexOf(de.Message) + de.Message.Length - 1; i++)
                {
                    if (i == -1) continue;
                    colors[i] = Convert.ToByte(de.Color);
                    if (newMeg[i] == '��')
                        colors.Insert(i, colors[i]);
                }
            }

            RemoteInterface.HC.CMSOutputData outputData;
            if (Show_icon == 1)
            {
                byte[] vspaces = new byte[colors.Count];
                outputData = new RemoteInterface.HC.CMSOutputData(4, 0, 0, 0, message.Replace("��", "�@�@"), colors.ToArray(), vspaces);
                outputData.AlarmClass = (int)ht["INC_NAME"];
            }
            else
            {
                 outputData = new RemoteInterface.HC.CMSOutputData(0, 0, 0, message.Replace("��", "�@�@"), colors.ToArray());
                 outputData.AlarmClass = (int)ht["INC_NAME"];
            }

            switch (blockType)
            {
                case "1X4"://�]�w1x4���O��ܤ��e
                    return getDisplay(outputData, 1, 4);
                case "2X6"://�]�w2X6���O��ܤ��e
                    return getDisplay(outputData, 2, 6);
                case "2X8(�T��)"://�]�w2x8�T�⭱�O��ܤ��e
                    return getDisplay(outputData, 2, 8);
                case "2X8(���m)":
                    return get2x8Display(outputData, isIcon);
                case "3X6"://�]�w3X6���O��ܤ��e
                    return getDisplay(outputData, 3, 6);
                case "5X2"://�]�w5X2���O��ܤ��e
                    return getDisplay(outputData, 2, 5);
                case "8X1"://�]�w8X1���O��ܤ��e
                    return getDisplay(outputData, 1, 8);
                default:
                    throw new Exception("�ʤ�CMS���O�w�q!!");
            }
        }       

        /// <summary>
        /// �]�w2x8���m���O��ܤ��e
        /// </summary>
        /// <returns>2x8���m���O��ܤ��e</returns>
        private RemoteInterface.HC.CMSOutputData get2x8Display(RemoteInterface.HC.CMSOutputData Mess, int isIcon)
        {
            if (isIcon == 0)
            {
                Mess = getDisplay(Mess, 2, 8);
            }
            else
            {                
                if (isIcon == 1)
                    Mess.icon_id = Convert.ToInt32(ht["LINE_ICON"]);
                else if (isIcon == 2)
                    Mess.icon_id = Convert.ToInt32(ht["ACC_ICON"]);
                if (Mess.dataType == 4)
                    Mess.dataType = 10;
                Mess = getDisplay(Mess, 2, 6);                
            }
            return Mess;
            
        }

        private RemoteInterface.HC.CMSOutputData getDisplay(RemoteInterface.HC.CMSOutputData Mess, int row, int col)
        {
            BlockMegAndColor megColor = new BlockMegAndColor(Mess.mesg, Mess.colors);
            Mess.mesg = "";
            List<byte> cmsColorlist = new List<byte>();
            foreach (BlockMegAndColor meg in separateMegColor(devType, row, col, megColor))
            {
                if (meg == null) break;
                Mess.mesg += meg.Message + "\r";
                cmsColorlist.AddRange(meg.Color);
            }
            Mess.mesg = Mess.mesg.TrimEnd('\r');
            Mess.colors = cmsColorlist.ToArray();
            return Mess;
        }

       
        //internal 
    }

    internal class OBSLastDeviceException : Exception
    {
    }
}
