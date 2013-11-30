using System;
using System.Collections.Generic;
using System.Text;
using Comm.MFCC;

namespace MFCC_CMS
{
     public  class MFCC_CMS:MFCC_Base

     {
        // CMS_Manager cms_manager;

         public MFCC_CMS(string mfccid,string devType, int remotePort, int notifyPort, int consolePort, string regRemoteName, Type regRemoteType)
             : base(mfccid,devType, remotePort, notifyPort, consolePort, regRemoteName, regRemoteType)
         {

             
         }

         //public override void loadTC_AndBuildManaer()
         //{
         //    System.Collections.ArrayList tcary = new System.Collections.ArrayList();

         //    tcary.Add(new Comm.TC.CMSTC(this.protocol, "cms101", "10.21.42.101", 1001, 0xffff, new byte[] { 0, 0, 0, 0 }));
         //    cms_manager = new CMS_Manager(tcary);


         //    // throw new Exception("The method or operation is not implemented.");
         //}

         public override void BindEvent(object tc)
         {
             //throw new Exception("The method or operation is not implemented.");
         }

        

    }
}
