using System;
using System.Collections.Generic;
using System.Text;
using System.Management;

namespace RemoteInterface
{


    public class ChangeIP
    {
        /// 
        /// Build of ArLi 2003.6.3 
        /// 
        public static readonly System.Version myVersion = new System.Version(1, 1);
        private ManagementBaseObject iObj = null;
        private ManagementBaseObject oObj = null;
        private ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
        private readonly ManagementObjectCollection moc;
        /// 
        /// example: 
        /// 
        /// ArLi.CommonPrj.ChangeIP o = new ArLi.CommonPrj.ChangeIP(); 
        /// string[] ipList = new string[]{"192.168.0.253","192.168.0.250"}; 
        /// string[] subnetList = new string[]{"255.255.255.0","255.255.255.0"}; 
        /// o.ChangeTo(ipList,subnetList); 
        /// 
        /// 
        public ChangeIP()
        {
            moc = mc.GetInstances();
        }
        /// cortrol 
        /// IPAddr List 
        /// subnetMask List 
        public void ChangeTo(string[] ipAddr, string[] subnetMask)
        {
            foreach (ManagementObject mo in moc)
            {


                if (!(bool)mo["IPEnabled"]) continue;
               
                iObj = mo.GetMethodParameters("EnableStatic");
                iObj["IPAddress"] = ipAddr;
                iObj["SubnetMask"] = subnetMask;
                oObj = mo.InvokeMethod("EnableStatic", iObj, null);
            }
        }
        /// cortrol 
        /// IPAddr List 
        /// subnetMask List 
        /// gateway List 
        /// gateway CostMetric List, example: 1 
        public void ChangeTo(string[] ipAddr, string[] subnetMask, string[] gateways, string[] gatewayCostMetric)
        {
            foreach (ManagementObject mo in moc)
            {
                if (!(bool)mo["IPEnabled"]) continue;
                iObj = mo.GetMethodParameters("EnableStatic");
                iObj["IPAddress"] = ipAddr;
                iObj["SubnetMask"] = subnetMask;
                oObj = mo.InvokeMethod("EnableStatic", iObj, null);
                iObj = mo.GetMethodParameters("SetGateways");
                iObj["DefaultIPGateway"] = gateways;
                iObj["GatewayCostMetric"] = gatewayCostMetric;
                oObj = mo.InvokeMethod("SetGateways", iObj, null);
            }
        }
        /// cortrol 
        /// IPAddr List 
        /// subnetMask List 
        /// gateway List 
        /// gateway CostMetric List, example: 1 
        /// DNSServer List 
        public void ChangeTo(string[] ipAddr, string[] subnetMask, string[] gateways, string[] gatewayCostMetric, string[] dnsServer)
        {
            foreach (ManagementObject mo in moc)
            {
                if (!(bool)mo["IPEnabled"]) continue;
                iObj = mo.GetMethodParameters("EnableStatic");
                iObj["IPAddress"] = ipAddr;
                iObj["SubnetMask"] = subnetMask;
                oObj = mo.InvokeMethod("EnableStatic", iObj, null);
                iObj = mo.GetMethodParameters("SetGateways");
                iObj["DefaultIPGateway"] = gateways;
                iObj["GatewayCostMetric"] = gatewayCostMetric;
                oObj = mo.InvokeMethod("SetGateways", iObj, null);
                iObj = mo.GetMethodParameters("SetDNSServerSearchOrder");
                iObj["DNSServerSearchOrder"] = dnsServer;
                oObj = mo.InvokeMethod("SetDNSServerSearchOrder", iObj, null);
            }
        }
        /// DHCPEnabled 
        public void EnableDHCP()
        {
            foreach (ManagementObject mo in moc)
            {
                if (!(bool)mo["IPEnabled"]) continue;
                if (!(bool)mo["DHCPEnabled"])
                {
                    iObj = mo.GetMethodParameters("EnableDHCP");
                    oObj = mo.InvokeMethod("EnableDHCP", iObj, null);
                }
            }
        }
    }
}
