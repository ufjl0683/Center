using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.MFCC;
using Comm.TC;
using RemoteInterface;

namespace MFCC_MAS
{
   public  class RemoteObj : Comm.MFCC.RemoteMFCCBase, I_MFCC_MAS
    {
       // void setTransmitCycle(string devName, int cycle);

       int[] curr_g_code_id=new int[6];
       string[] curr_mesg = new string[] { "", "", "", "", "", "" };
       byte[][] curr_color = new byte[6] []{ new byte[0], new byte[0], new byte[0], new byte[0], new byte[0], new byte[0] };

       public override object InitializeLifetimeService()
       {
           return null; //base.InitializeLifetimeService();
       }





       public override Comm.MFCC.MFCC_Base getMFCC_base()
       {

           return Program.mfcc_mas;

       }




       public void SendDisplay(string devName, int laneid, int g_code_id, int hor_space, string mesgs, byte[] colors, byte[] vspaces)
       {
           //throw new Exception("The method or operation is not implemented.");
           try
           {
               Comm.TC.MASTC tc = (Comm.TC.MASTC)Program.mfcc_mas.getTcManager()[devName];
               checkAllowConnect(tc);
               tc.TC_SendDisplay(laneid, g_code_id, hor_space, mesgs, colors, vspaces);
               if(g_code_id==0)
               ConsoleServer.WriteLine(devName + ",laneid:" + laneid + ",mesg=" + mesgs );
               else
               ConsoleServer.WriteLine(devName + ",laneid:" + laneid + ",g_code_id:" + g_code_id);
           }
           catch (Exception ex)
           {
               throw new RemoteException(devName+","+ex.Message+","+ex.StackTrace);
           }
       }

       public void SendDisplay(string devName, int laneid, int speedlimit)
       {
           //throw new Exception("The method or operation is not implemented.");
           try
           {
               Comm.TC.MASTC tc = (Comm.TC.MASTC)Program.mfcc_mas.getTcManager()[devName];
               checkAllowConnect(tc);
               tc.TC_SendDisplay(laneid, speedlimit);

               ConsoleServer.WriteLine(devName + ",laneid:" + laneid + ",spd limit=" + speedlimit);
             
           }
           catch (Exception ex)
           {
               throw new RemoteException(ex.Message + "," + ex.StackTrace);
           }
       }

       public void SendDisplay(string devName, int laneid, int g_code_id, int hor_space, string mesg, byte[] colors)
       {
           try
           {
               int ver_no = 1;
             //  Comm.TC.MASTC tc = (Comm.TC.MASTC)Program.mfcc_mas.getTcManager()[devName];
             //  checkAllowConnect(tc);
               for (int i = 0; i < mesg.Length; i++)
               {
                   if (mesg[i] == '\r')
                       ver_no++;
               }
               byte[] vspaces = new byte[ver_no];

               this.SendDisplay(devName, laneid, g_code_id, hor_space, mesg, colors, vspaces);
             //  tc.TC_SendDisplay(laneid, g_code_id, hor_space, mesg, colors, vspaces);
            //   ConsoleServer.WriteLine(string.Format(devName + "laneid:{0} g_code_id:{1} hos_space:{2} mesg:{3}", icon_id, g_code_id, hor_space, mesg));
           }
           catch (Exception ex)
           {
              // ConsoleServer.WriteLine(devName + "," + ex.Message+","+ex.StackTrace);
               throw new RemoteInterface.RemoteException(ex.Message+","+ex.StackTrace);
           }
       }


       public void SetDisplayOff(string devName)
       {
            try
           {
               Comm.TC.MASTC tc = (Comm.TC.MASTC)Program.mfcc_mas.getTcManager()[devName];
               checkAllowConnect(tc);
               tc.TC_SetDisplayOff();
               ConsoleServer.WriteLine(devName+"熄滅!");
           }
           catch (Exception ex)
           {
               throw new RemoteException(ex.Message+","+ex.StackTrace);
           }
       }

       public void GetCurrentTCDisplay(string devName, byte laneid,ref int  g_code_id ,ref  string mesg ,ref int speedlimit)
       {
           try
           {
               Comm.TC.MASTC tc = (Comm.TC.MASTC)Program.mfcc_mas.getTcManager()[devName];
               checkAllowConnect(tc);
               tc.GetCurrentDisplay(laneid,ref g_code_id, ref mesg,ref speedlimit);
              // 
           }
           catch (Exception ex)
           {
               throw new RemoteException(ex.Message+","+ex.StackTrace);
           }
       }



      


       public void SetDisplayOff(string devName, byte laneid)
       {
           try
           {
               Comm.TC.MASTC tc = (Comm.TC.MASTC)Program.mfcc_mas.getTcManager()[devName];
               checkAllowConnect(tc);
               tc.TC_SetDisplayOff(laneid);
               ConsoleServer.WriteLine(devName + ",laneid=" + laneid + "熄滅");
           }
           catch (Exception ex)
           {
               throw new RemoteException(ex.Message+","+ex.StackTrace);
           }
         
           // throw new Exception("The method or operation is not implemented.");
       }



      


     
      
   }
}
