using System;
using System.Collections.Generic;
using System.Text;
using Comm;
//using System.Drawing;
using RemoteInterface.MFCC;
using RemoteInterface;

namespace MFCC_LCS
{
   public  class RemoteObj:Comm.MFCC.RemoteMFCCBase,RemoteInterface.MFCC.I_MFCC_LCS
    {

       public override object InitializeLifetimeService()
       {
           return null;
       }

        public override Comm.MFCC.MFCC_Base getMFCC_base()
        {
            return Program.mfcc_lcs;
           // throw new Exception("The method or operation is not implemented.");
        }

     


      // #region I_MFCC_RGS 成員

       public void SetDisplay(string devName, System.Data.DataSet ds)
       {
           // throw new Exception("The method or operation is not implemented.");
           try
           {
               if (ds == null)
                   this.SetDisplayOff(devName);
               else
               {
                   if (ds.Tables[0].Rows[0]["func_name"].ToString() != "set_ctl_sign")
                       throw new Exception("only support fucn_name=func_name=set_ctl_sign");
                   Comm.TC.LCSTC tc = (Comm.TC.LCSTC)this.getMFCC_base().getTcManager()[devName];

                   // return tc.TC_GetBackgroundPic(0xffff, (byte)mode, (byte)g_code_id, ref desc);
                   tc.TC_SetDislay(ds);

                   string outstr;
                   outstr=tc.DeviceName;
                   for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                   
                     outstr+=  "["+ ds.Tables[1].Rows[i]["sign_no"] + ":" + ds.Tables[1].Rows[i]["sign_status"] + "] ";
                   

                   ConsoleServer.WriteLine(outstr);
               }
           }
           catch (Exception ex)
           {
               throw new RemoteInterface.RemoteException(ex.Message);
           }
       }

       public void SetDisplayOff(string devName)
       {
         //  throw new RemoteException ("The method or operation is not implemented.");
           try
           {
               Comm.TC.LCSTC tc = (Comm.TC.LCSTC)this.getMFCC_base().getTcManager()[devName];
               // return tc.TC_GetBackgroundPic(0xffff, (byte)mode, (byte)g_code_id, ref desc);
               tc.TC_SetDisplayOff(); ;
               ConsoleServer.WriteLine(devName + "熄滅!");

           }
           catch (Exception ex)
           {
               throw new RemoteInterface.RemoteException(ex.Message);
           }
          
       }

     //  #endregion
   }
}
