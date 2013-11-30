using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.MFCC;
using RemoteInterface;

namespace MFCC_WIS
{
   public  class RemoteObj:Comm.MFCC.RemoteMFCCBase,RemoteInterface.MFCC.I_MFCC_WIS
    {

       public override Comm.MFCC.MFCC_Base getMFCC_base()
       {
          // throw new Exception("The method or operation is not implemented.");
           return Program.mfcc_wis;
       }

       public override object InitializeLifetimeService()
       {
           //return base.InitializeLifetimeService();
           return null;
       }

     

       public void SendDisplay(string devName, int g_code_id, int hor_space, string mesg, byte[] colors)
       {
           //throw new Exception("The method or operation is not implemented.");
           try
           {
               Comm.TC.WISTC tc =( Comm.TC.WISTC)Program.mfcc_wis.getTcManager()[devName];
               checkAllowConnect(tc);
               tc.TC_SendDisplay( g_code_id, hor_space, mesg, colors);
               ConsoleServer.WriteLine(string.Format(tc.DeviceName+" g_code_id:{0} hos_space:{1} mesg:{2}", g_code_id, hor_space, mesg));
           }
           catch (Exception ex)
           {
               throw new RemoteInterface.RemoteException(ex.Message);
           }

       }

       public void SendDisplay(string ip, int port, int g_code_id, int hor_space, string mesg, byte[] colors)
       {
           //  throw new Exception("The method or operation is not implemented.");
           try
           {
               Comm.TC.WISTC tc = (Comm.TC.WISTC)Program.mfcc_wis.getTcManager()[ip, port];
               checkAllowConnect(tc);
               tc.TC_SendDisplay(g_code_id, hor_space, mesg, colors);

           }
           catch (Exception ex)
           {
               throw new RemoteInterface.RemoteException(ex.Message);
           }
       }

       public void GetCurrentTCDisplay(string devName,  ref int g_code_id, ref int hor_space, ref string mesg, ref byte[] colors)
       {
           try
           {
               Comm.TC.WISTC tc = (Comm.TC.WISTC)Program.mfcc_wis.getTcManager()[devName];
               checkAllowConnect(tc);

               System.Data.DataSet ds = tc.TC_GetDisplay();

               //if (ds.Tables[0].Rows[0]["func_name"].ToString() == "get_display_control")
               //{
               //    icon_id = System.Convert.ToInt32(ds.Tables[0].Rows[0]["icon_code_id"]);
               //}
             //  else
               //if (ds.Tables[0].Rows[0]["func_name"].ToString() == "get_CMS_display")
               //{
               //    icon_id = 0;



               //}
               g_code_id = (ds.Tables[0].Rows[0]["g_code_id"] == System.DBNull.Value) ? 0 : System.Convert.ToInt32(ds.Tables[0].Rows[0]["g_code_id"]);
               hor_space = (ds.Tables[0].Rows[0]["hor_space"] == System.DBNull.Value) ? 0 : System.Convert.ToInt32(ds.Tables[0].Rows[0]["hor_space"]);
               int msgcnt = (ds.Tables[0].Rows[0]["msgcnt"] == System.DBNull.Value) ? 0 : System.Convert.ToInt32(ds.Tables[0].Rows[0]["msgcnt"]);
               byte[] big5_code = new byte[ds.Tables["tblmsgcnt"].Rows.Count];
               for (int i = 0; i < big5_code.Length; i++)
                   big5_code[i] =  System.Convert.ToByte(ds.Tables["tblmsgcnt"].Rows[i]["message"]);

               mesg = Util.Big5BytesToString(big5_code).Replace("\n", "");

               byte[] color_code = new byte[ds.Tables["tblcolorcnt"].Rows.Count];
               for (int i = 0; i < color_code.Length; i++)
                   color_code[i] = System.Convert.ToByte(ds.Tables["tblcolorcnt"].Rows[i]["color"]);
               colors = color_code;


               //   tc.TC_SendDisplay(icon_id, g_code_id, hor_space, mesg, colors);
               //   ConsoleServer.WriteLine(string.Format(devName + "icon_id:{0} g_code_id:{1} hos_space:{2} mesg:{3}", icon_id, g_code_id, hor_space, mesg));
           }
           catch (Exception ex)
           {
               ConsoleServer.WriteLine(devName + "," + ex.Message);
               throw new RemoteInterface.RemoteException(ex.Message);
           }
       }
   

       #region I_MFCC_WIS 成員


       public void setDisplayOff(string devName)
       {
           try
           {
               Comm.TC.WISTC tc = (Comm.TC.WISTC)Program.mfcc_wis.getTcManager()[devName];
               checkAllowConnect(tc);
               tc.TC_SetDisplayOff();
               ConsoleServer.WriteLine(devName + " 熄滅!");
           }
           catch (Exception ex)
           {
               throw new RemoteInterface.RemoteException(ex.Message);
           }
          // throw new Exception("The method or operation is not implemented.");
       }

       #endregion
   }
}
