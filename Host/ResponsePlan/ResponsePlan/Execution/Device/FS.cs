using System;
using System.Collections.Generic;
using System.Text;

namespace Execution.Category
{
    /// <summary>
    /// �˹�����GFS
    /// </summary>
    internal class FS : ADevice 
    {
        public FS(AEvent aDevice, CategoryType type, System.Collections.Hashtable ht, DeviceType devType, System.Collections.Hashtable DevRange, Degree degree)
        {
            Initialize(aDevice, type, ht, devType, DevRange, degree);   
        }

        public override ExecutionObj produceExecution(ExecutionObj sender)
        {
            sender.Memo += "==>> FS";
            sender.FSOutputData = getDisplayContent(type);
            aDevice.produceExecution(sender);
            return sender;
        }
        protected override System.Collections.Hashtable setWEADisplay(RemoteInterface.HC.FetchDeviceData[] devNames, int maxSegId)
        {
            System.Collections.Hashtable outputs = new System.Collections.Hashtable();

            byte display = 1;
            switch (secType)
            {
                case 45:    //�@��
                    display = 1;
                    break;
                case 46:    //�j��
                    display = 2;
                    break;
                case 47:    //���B
                    display = 3;
                    break;
                default:    //��L
                    display = 0;
                    break;
            }

            if (devNames == null) return null;
            foreach (RemoteInterface.HC.FetchDeviceData devName in devNames)
            {
                RemoteInterface.HC.FSOutputData output = new RemoteInterface.HC.FSOutputData(display);
                outputs.Add(devName.DevName, output);
            }
            return outputs;
        }
    }
}
