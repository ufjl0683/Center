using System;
using System.Collections.Generic;
using System.Text;

namespace Execution.Category
{
    /// <summary>
    /// 裝飾物件：WIS
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
            //serMeg.setAlarmMeg("WIS訊息資料表未完成先寫死!!");
            System.Collections.Hashtable outputs = new System.Collections.Hashtable();
            string meg = "";
            switch (secType)
            {
                case 45:    //濃霧
                    meg = "濃霧慢行";
                    break;
                case 46:    //強風
                    meg = "強風慢行";
                    break;
                case 47:    //豪雨
                    meg = "豪雨慢行";
                    break;
                default:    //其他
                    meg = "系列測試";
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
