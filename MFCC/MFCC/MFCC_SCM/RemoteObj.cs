using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.MFCC;
using RemoteInterface;

namespace MFCC_SCM
{
   public  class RemoteObj:Comm.MFCC.RemoteMFCCBase,RemoteInterface.MFCC.I_MFCC_SCM
    {

       public override Comm.MFCC.MFCC_Base getMFCC_base()
       {
          // throw new Exception("The method or operation is not implemented.");
           return Program.mfcc_scm;
       }

       public override object InitializeLifetimeService()
       {
           //return base.InitializeLifetimeService();
           return null;
       }

     


     
   }
}
