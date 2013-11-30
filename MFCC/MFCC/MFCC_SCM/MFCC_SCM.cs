using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;
namespace MFCC_SCM
{
    class MFCC_SCM:Comm.MFCC.MFCC_Base
    {
      //  SCM_Manager SCM_manager;
        public MFCC_SCM(string mfccid,string devType, int remotePort, int notifyPort, int consolePort, string regRemoteName, Type regRemoteTyp)
            : base(mfccid,devType, remotePort, notifyPort, consolePort, regRemoteName, regRemoteTyp)
        {

        
        }

       
        //public override void loadTC_AndBuildManaer()
        //{

        //    this.tcAry.Add(new Comm.TC.SCMTC(protocol, "rms2", "192.168.0.2", 1002, 0xffff, new byte[] { 0, 0, 0, 0 }));
        //    SCM_manager=new SCM_Manager(tcAry);
        //   // throw new Exception("The method or operation is not implemented.");
        //    //this.tcAry.Add(new 
        //}

        public override void BindEvent(object tc)
        {
            ((Comm.TCBase)tc).OnTCReport += new Comm.OnTCReportHandler(MFCC_SCM_OnTCReport);
            //throw new Exception("The method or operation is not implemented.");
        }

        

        void MFCC_SCM_OnTCReport(object tc, Comm.TextPackage txt)
        {
            //throw new Exception("The method or operation is not implemented.");

            Comm.TCBase  dev=(Comm.TCBase)tc;
            if (txt.Text[0] == 0x86 || txt.Text[0] == 0xa7 || txt.Text[0] == 0xae || txt.Text[0] == 0xaf && txt.Text[1] == 0x01)
            {
               // ConsoleServer.WriteLine("In Rms Report:0x" + Util.ToHexString(txt.Text[0]));
                System.Data.DataSet ds = this.protocol.GetSendDsByTextPackage(txt, Comm.CmdType.CmdReport);
                ds.AcceptChanges();
                this.notifier.NotifyAll(new NotifyEventObject(EventEnumType.MFCC_Report_Event,dev.DeviceName,ds));
            }

        }

        //public override Comm.MFCC.TC_Manager getTcManager()
        //{
        //    return SCM_manager;
        //   // throw new Exception("The method or operation is not implemented.");
        //}
    }
}
