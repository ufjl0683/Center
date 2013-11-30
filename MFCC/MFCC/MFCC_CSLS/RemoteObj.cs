using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace MFCC_CSLS
{
    public class RemoteObj : Comm.MFCC.RemoteMFCCBase, RemoteInterface.MFCC.I_MFCC_CSLS
    {



        public override object InitializeLifetimeService()
        {
            return null;
        }

        public override Comm.MFCC.MFCC_Base getMFCC_base()
        {
          //  throw new Exception("The method or operation is not implemented.");

            return Program.mfcc_csls;
        }




        #region I_MFCC_CSLS 成員

        public void SetDisplay(string devName, System.Data.DataSet ds)
        {
            try
            {
                if (ds == null)
                    this.SetDisplayOff(devName);
                else
                {
                    if (ds.Tables[0].Rows[0]["func_name"].ToString() != "set_speed")
                        throw new Exception("only support fucn_name=func_name=set_speed");
                    Comm.TC.CSLSTC tc = (Comm.TC.CSLSTC)this.getMFCC_base().getTcManager()[devName];

                    RemoteInterface.ConsoleServer.WriteLine(devName + "spd:" + ds.Tables[0].Rows[0]["speed"]);
                    // return tc.TC_GetBackgroundPic(0xffff, (byte)mode, (byte)g_code_id, ref desc);
                    tc.TC_SetDislay(ds);
                }
            }
            catch (Exception ex)
            {
                throw new RemoteInterface.RemoteException(ex.Message);
            }
        }

        public void SetDisplayOff(string devName)
        {
            try
            {
                Comm.TC.CSLSTC tc = (Comm.TC.CSLSTC)this.getMFCC_base().getTcManager()[devName];
                // return tc.TC_GetBackgroundPic(0xffff, (byte)mode, (byte)g_code_id, ref desc);
                tc.TC_SetDisplayOff(); ;
                ConsoleServer.WriteLine(devName + "熄滅!");

            }
            catch (Exception ex)
            {
                throw new RemoteInterface.RemoteException(ex.Message);
            }
        }

        #endregion
    }
}
