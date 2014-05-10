using System;
using System.Collections.Generic;
using System.Text;

namespace Execution.Category
{
    /// <summary>
    /// �˹�����GWIS
    /// </summary>
    internal class WIS : A_DisplayBlock 
    {
        public WIS(AEvent aDevice, CategoryType type, System.Collections.Hashtable ht, DeviceType devType, System.Collections.Hashtable DevRange, Degree degree)
        {
            Initialize(aDevice, type, ht, devType, DevRange, degree); 
        }
    
        public override ExecutionObj produceExecution(ExecutionObj sender)
        {
            sender.Memo += "==>> WIS";
            sender.WISOutputData = getDisplayContent(type);
            aDevice.produceExecution(sender);
            return sender;
        }

        protected override System.Collections.Hashtable setWEADisplay(RemoteInterface.HC.FetchDeviceData[] devNames, int maxSegId)
        {
            //serMeg.setAlarmMeg("WIS�T����ƪ��������g��!!");
            System.Collections.Hashtable outputs = new System.Collections.Hashtable();
            string meg = "";
            switch (secType)
            {
                case 45:    //�@��
                    meg = "�@���C��";
                    break;
                case 46:    //�j��
                    meg = "�j���C��";
                    break;
                case 47:    //���B
                    meg = "���B�C��";
                    break;
                default:    //��L
                    meg = "�t�C����";
                    break;
            }
            byte[] colors = new byte[4] { 32, 32, 32, 32 };
            byte[] vespaces = new byte[4] {0,0,0,0};

            if (devNames == null) return null;
            foreach (RemoteInterface.HC.FetchDeviceData devName in devNames)
            {
                //RemoteInterface.HC.WISOutputData output = new RemoteInterface.HC.CMSOutputData(0, 0, 0, meg, colors);
                RemoteInterface.HC.WISOutputData output = new RemoteInterface.HC.WISOutputData(0, 0, 0, 0, 0, meg, colors, vespaces);
                outputs.Add(devName.DevName, output);
            }
            return outputs;
        }
    }
}
