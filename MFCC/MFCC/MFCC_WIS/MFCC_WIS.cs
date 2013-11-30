using System;
using System.Collections.Generic;
using System.Text;
using Comm.MFCC;
using Comm;
namespace MFCC_WIS
{
     public  class MFCC_WIS:MFCC_Base

     {
       //  WIS_Manager wis_manager;

         public MFCC_WIS(string mfccid,string devType, int remotePort, int notifyPort, int consolePort, string regRemoteName, Type regRemoteType)
             : base(mfccid,devType, remotePort, notifyPort, consolePort, regRemoteName, regRemoteType)
         {
         }

        //public override void loadTC_AndBuildManaer()
        //{
        //    System.Collections.ArrayList tcary =System.Collections.ArrayList.Synchronized( new System.Collections.ArrayList());

        //    tcary.Add(new Comm.TC.WISTC(this.protocol, "WIS130", "192.168.3.127", 1001, 0xffff, new byte[] { 0, 0, 0, 0 }));
        //    wis_manager=new WIS_Manager(tcary);


        //   // throw new Exception("The method or operation is not implemented.");
        //}

         public override void BindEvent(object tc)
         {
            // ((Comm.TCBase)tc).OnTCReport += new Comm.OnTCReportHandler(MFCC_WIS_OnTCReport);
             //throw new Exception("The method or operation is not implemented.");
         }

         void MFCC_WIS_OnTCReport(object tc, Comm.TextPackage txt)
         {
            // Comm.TCBase tcc = tc as Comm.TCBase;

             //if (txt.Text[0] == 0x5f && txt.Text[1] = 0xda)
             //{
                
             //}

             //throw new Exception("The method or operation is not implemented.");
         }
         



        //public override TC_Manager getTcManager()
        //{
        //   // throw new Exception("The method or operation is not implemented.");
        //    return this.wis_manager;
        //}
    }
}
