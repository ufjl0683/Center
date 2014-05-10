using System;
using System.Collections.Generic;
using System.Text;

namespace Execution.Category
{
    /// <summary>
    /// ¸Ë¹¢ª«¥ó¡GCCTV
    /// </summary>
    internal class CCTV : ADevice 
    {
        public CCTV(AEvent aDevice, CategoryType type, System.Collections.Hashtable ht, DeviceType devType, System.Collections.Hashtable DevRange, Degree degree)
        {
            Initialize(aDevice, type, ht, devType, DevRange, degree);
        }

        public override ExecutionObj produceExecution(ExecutionObj sender)
        {
            sender.Memo += "==>> CCTV";
            sender.CCTVOutputData = getDisplayContent(type);
            aDevice.produceExecution(sender);
            return sender;
        }

        protected override System.Collections.Hashtable setDisplay(RemoteInterface.HC.FetchDeviceData[] devNames, int maxSegId, MegType megType)
        {
            System.Collections.Hashtable displayht = new System.Collections.Hashtable();
            foreach (RemoteInterface.HC.FetchDeviceData dev in devNames)
            {
                if (!displayht.ContainsKey(dev.DevName))
                {
                    displayht.Add(dev.DevName, null);
                }
            }
            return displayht;
        }
    }
}
