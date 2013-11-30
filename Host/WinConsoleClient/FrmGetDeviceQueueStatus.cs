using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using RemoteInterface;

namespace WinConsoleClient
{
    public partial class FrmGetDeviceQueueStatus : Form
    {
        RemoteInterface.HC.I_HC_Comm  robj = (RemoteInterface.HC.I_HC_Comm)RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.HC.I_HC_Comm),
            RemoteInterface.RemoteBuilder.getRemoteUri(RemoteInterface.RemoteBuilder.getHostIP(), (int)RemotingPortEnum.HOST, "Comm"));
        RemoteInterface.HC.I_HC_FWIS hobj = (RemoteInterface.HC.I_HC_FWIS)RemoteInterface.RemoteBuilder.GetRemoteObj(typeof(RemoteInterface.HC.I_HC_FWIS),
           RemoteInterface.RemoteBuilder.getRemoteUri(RemoteInterface.RemoteBuilder.getHostIP(), (int)RemotingPortEnum.HOST_FIWS, "FWIS"));

        public FrmGetDeviceQueueStatus()
        {
            InitializeComponent();
        }

        private void FrmGetDeviceQueueStatus_Load(object sender, EventArgs e)
        {

            ShowCmsQueueStatus();


        }

        void ShowCmsQueueStatus()
        {
            System.Collections.ArrayList cmsArray = robj.getDeviceNames("CMS");
            System.Collections.ArrayList rmsArray = robj.getDeviceNames("RMS");

            foreach (string devicename in cmsArray)
            {
                try
                {
                    string status = robj.getTCCommStatusStr(devicename);
                    string[] items = status.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    if (items.Length < 6) continue;

                    int sendqcnt, timeoutcnt;
                    sendqcnt = int.Parse(items[3].Split(new char[] { '=' })[1]);
                    timeoutcnt = int.Parse(items[5].Split(new char[] { '=' })[1]);
                    if (sendqcnt > 0 || timeoutcnt > 0)
                    {
                        int priorty=0;
                        string mesg =( hobj.GetCurrentOutput(devicename, ref priorty) as RemoteInterface.HC.CMSOutputData).mesg;
                        listBox1.Items.Add(devicename + "," + sendqcnt + "," + timeoutcnt+","+mesg);

                    }
                    Application.DoEvents();
                }
                catch (Exception ex)
                {
                    listBox1.Items.Add(devicename + "," + ex.Message);
                }
            }

            foreach (string devicename in rmsArray)
            {
                try
                {
                    string status = robj.getTCCommStatusStr(devicename);
                    string[] items = status.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    if (items.Length < 6) continue;

                    int sendqcnt, timeoutcnt;
                    sendqcnt = int.Parse(items[3].Split(new char[] { '=' })[1]);
                    timeoutcnt = int.Parse(items[5].Split(new char[] { '=' })[1]);
                    if (sendqcnt > 0 || timeoutcnt > 0)
                    {
                        int priorty = 0;
                        string mesg = (hobj.GetCurrentOutput(devicename, ref priorty) as RemoteInterface.HC.CMSOutputData).mesg;
                        listBox1.Items.Add(devicename + "," + sendqcnt + "," + timeoutcnt + "," + mesg);

                    }
                    Application.DoEvents();
                }
                catch (Exception ex)
                {
                    listBox1.Items.Add(devicename + "," + ex.Message);
                }
            }
        }
    }
}
