using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.MFCC;
using RemoteInterface;


namespace MFCC_CMS
{
   public  class RemoteObj:Comm.MFCC.RemoteMFCCBase,RemoteInterface.MFCC.I_MFCC_CMS
    {

       public override Comm.MFCC.MFCC_Base getMFCC_base()
       {
          // throw new Exception("The method or operation is not implemented.");
           return Program.mfcc_cms;
       }

       public override object InitializeLifetimeService()
       {
           //return base.InitializeLifetimeService();
           return null;
       }

       

       public void SendDisplay(string devName,int datatype, int icon_id, int g_code_id, int hor_space, string mesg, byte[] colors)
       {
           try
           {
               int ver_no = 1;
               //   Comm.TC.CMSTC tc =( Comm.TC.CMSTC)Program.mfcc_cms.getTcManager()[devName];
               //   checkAllowConnect(tc);
               for (int i = 0; i < mesg.Length; i++)
               {
                   if (mesg[i] == '\r')
                       ver_no++;
               }
               byte[] vspaces = new byte[ver_no];
               this.SendDisplay(devName,datatype, icon_id, g_code_id, hor_space, mesg, colors, vspaces);

               // ConsoleServer.WriteLine(string.Format(devName + "icon_id:{0} g_code_id:{1} hos_space:{2} mesg:{3}", icon_id, g_code_id, hor_space, mesg));
           }
           catch (Exception ex)
           {
               //   ConsoleServer.WriteLine(devName+","+ex.Message);
               throw new RemoteInterface.RemoteException(ex.Message + "," + ex.StackTrace);
           }
       }

       
       public void SendDisplay(string devName, int icon_id, int g_code_id, int hor_space, string mesg, byte[] colors)
       {
           //throw new Exception("The method or operation is not implemented.");
         
           try
           {
               int ver_no=1;
            //   Comm.TC.CMSTC tc =( Comm.TC.CMSTC)Program.mfcc_cms.getTcManager()[devName];
            //   checkAllowConnect(tc);
               for (int i = 0; i < mesg.Length; i++)
               {
                   if (mesg[i] == '\r')
                       ver_no++;
               }
               byte[] vspaces = new byte[ver_no];
               this.SendDisplay(devName, icon_id, g_code_id, hor_space, mesg, colors, vspaces);
            
              // ConsoleServer.WriteLine(string.Format(devName + "icon_id:{0} g_code_id:{1} hos_space:{2} mesg:{3}", icon_id, g_code_id, hor_space, mesg));
           }
           catch (Exception ex)
           {
            //   ConsoleServer.WriteLine(devName+","+ex.Message);
               throw new RemoteInterface.RemoteException(ex.Message+","+ex.StackTrace);
           }

       }

       public void SendDisplay(string devName,int dataType, int icon_id, int g_code_id, int hor_space, string mesg, byte[] colors, byte[] v_spaces)
       {
           //throw new Exception("The method or operation is not implemented.");
           try
           {
               // ConsoleServer.WriteLine("from host1");
               Comm.TC.CMSTC tc = (Comm.TC.CMSTC)Program.mfcc_cms.getTcManager()[devName];
               checkAllowConnect(tc);
               tc.TC_SendDisplay(dataType, icon_id, g_code_id, hor_space, mesg, colors, v_spaces);
               ConsoleServer.WriteLine(string.Format(devName + "icon_id:{0} g_code_id:{1} hos_space:{2} mesg:{3}", icon_id, g_code_id, hor_space, mesg));
           }
           catch (Exception ex)
           {
               ConsoleServer.WriteLine(devName + "," + ex.Message);
               throw new RemoteInterface.RemoteException(ex.Message + "," + ex.StackTrace);
           }

       }

      
        public void SendDisplay(string devName, int icon_id, int g_code_id, int hor_space, string mesg, byte[] colors,  byte[] v_spaces)
       {
           //throw new Exception("The method or operation is not implemented.");
           try
           {
              // ConsoleServer.WriteLine("from host1");
               Comm.TC.CMSTC tc =( Comm.TC.CMSTC)Program.mfcc_cms.getTcManager()[devName];
               checkAllowConnect(tc);
               tc.TC_SendDisplay(icon_id, g_code_id, hor_space, mesg, colors,v_spaces);
               ConsoleServer.WriteLine(string.Format(devName + "icon_id:{0} g_code_id:{1} hos_space:{2} mesg:{3}", icon_id, g_code_id, hor_space, mesg));
           }
           catch (Exception ex)
           {
               ConsoleServer.WriteLine(devName+","+ex.Message);
               throw new RemoteInterface.RemoteException(ex.Message+","+ex.StackTrace);
           }

       }

      

       public void GetCurrentTCDisplay(string devName, ref int icon_id, ref int g_code_id, ref int hor_space, ref string mesg, ref byte[] colors)
       {
           byte[] vspaces=new byte[0];
           GetCurrentTCDisplay( devName,ref  icon_id,ref  g_code_id,ref  hor_space, ref mesg, ref colors,ref  vspaces );
       }



       public void GetCurrentTCDisplay(string devName,ref int icon_id,ref int g_code_id,ref int hor_space,ref string mesg,ref byte[]colors,ref byte[] vspaces )
       {
        try
           {
               Comm.TC.CMSTC tc =(Comm.TC.CMSTC)Program.mfcc_cms.getTcManager()[devName];
               checkAllowConnect(tc);

               System.Data.DataSet ds = tc.TC_GetDisplay();

               if (ds.Tables[0].Rows[0]["func_name"].ToString() == "get_display_control")
               {
                   icon_id = System.Convert.ToInt32(ds.Tables[0].Rows[0]["icon_code_id"]);
               }
               else if (ds.Tables[0].Rows[0]["func_name"].ToString() == "get_CMS_display")
               {
                   icon_id = 0;



               }
               else
                   throw new RemoteException("unknown Display Command!");
               
               g_code_id = (ds.Tables[0].Rows[0]["g_code_id"]==System.DBNull.Value)?0:  System.Convert.ToInt32(ds.Tables[0].Rows[0]["g_code_id"]);
               hor_space = (ds.Tables[0].Rows[0]["hor_space"] == System.DBNull.Value) ? 0 : System.Convert.ToInt32(ds.Tables[0].Rows[0]["hor_space"]);
               int msgcnt = (ds.Tables[0].Rows[0]["msgcnt"] == System.DBNull.Value) ? 0 : System.Convert.ToInt32(ds.Tables[0].Rows[0]["msgcnt"]);
               byte[] big5_code = new byte[ds.Tables["tblmsgcnt"].Rows.Count];
               for (int i = 0; i < big5_code.Length; i++)
                   big5_code[i] = System.Convert.ToByte(ds.Tables["tblmsgcnt"].Rows[i]["message"]);

               mesg = Util.Big5BytesToString(big5_code).Replace("\n","");

               byte[] color_code = new byte[ds.Tables["tblcolorcnt"].Rows.Count];
               for (int i = 0; i < color_code.Length; i++)
                   color_code[i] = System.Convert.ToByte(ds.Tables["tblcolorcnt"].Rows[i]["color"]);
               colors = color_code;

               byte[] v_spaces = new byte[ds.Tables["tblver_no"].Rows.Count];

               for (int i = 0; i < v_spaces.Length; i++)
                   v_spaces[i] = System.Convert.ToByte(ds.Tables["tblver_no"].Rows[i][0]);

            
            //   tc.TC_SendDisplay(icon_id, g_code_id, hor_space, mesg, colors);
            //   ConsoleServer.WriteLine(string.Format(devName + "icon_id:{0} g_code_id:{1} hos_space:{2} mesg:{3}", icon_id, g_code_id, hor_space, mesg));
           }
           catch (Exception ex)
           {
               ConsoleServer.WriteLine(devName+","+ex.Message);
               throw new RemoteInterface.RemoteException(ex.Message+","+ex.StackTrace);
           }
       }

       public void GetCurrentTCDisplay(string devName,ref int dataType, ref int icon_id, ref int g_code_id, ref int hor_space, ref string mesg, ref byte[] colors, ref byte[] vspaces)
       {
           try
           {
               Comm.TC.CMSTC tc = (Comm.TC.CMSTC)Program.mfcc_cms.getTcManager()[devName];
               checkAllowConnect(tc);

               System.Data.DataSet ds = tc.TC_GetDisplay();

               if (ds.Tables[0].Rows[0]["func_name"].ToString() == "get_display_control")
               {
                   icon_id = System.Convert.ToInt32(ds.Tables[0].Rows[0]["icon_code_id"]);
                   dataType = System.Convert.ToInt32(ds.Tables[0].Rows[0]["data_type"]);
               }
               else if (ds.Tables[0].Rows[0]["func_name"].ToString() == "get_CMS_display")
               {
                   icon_id = 0;
                   dataType = 0;


               }
               else
                   throw new RemoteException("unknown Display Command!");

               g_code_id = (ds.Tables[0].Rows[0]["g_code_id"] == System.DBNull.Value) ? 0 : System.Convert.ToInt32(ds.Tables[0].Rows[0]["g_code_id"]);
               hor_space = (ds.Tables[0].Rows[0]["hor_space"] == System.DBNull.Value) ? 0 : System.Convert.ToInt32(ds.Tables[0].Rows[0]["hor_space"]);
               int msgcnt = (ds.Tables[0].Rows[0]["msgcnt"] == System.DBNull.Value) ? 0 : System.Convert.ToInt32(ds.Tables[0].Rows[0]["msgcnt"]);
               byte[] big5_code = new byte[ds.Tables["tblmsgcnt"].Rows.Count];
               for (int i = 0; i < big5_code.Length; i++)
                   big5_code[i] = System.Convert.ToByte(ds.Tables["tblmsgcnt"].Rows[i]["message"]);

               mesg = Util.Big5BytesToString(big5_code).Replace("\n", "");

               byte[] color_code = new byte[ds.Tables["tblcolorcnt"].Rows.Count];
               for (int i = 0; i < color_code.Length; i++)
                   color_code[i] = System.Convert.ToByte(ds.Tables["tblcolorcnt"].Rows[i]["color"]);
               colors = color_code;

               byte[] v_spaces = new byte[ds.Tables["tblver_no"].Rows.Count];

               for (int i = 0; i < v_spaces.Length; i++)
                   v_spaces[i] = System.Convert.ToByte(ds.Tables["tblver_no"].Rows[i][0]);


               //   tc.TC_SendDisplay(icon_id, g_code_id, hor_space, mesg, colors);
               //   ConsoleServer.WriteLine(string.Format(devName + "icon_id:{0} g_code_id:{1} hos_space:{2} mesg:{3}", icon_id, g_code_id, hor_space, mesg));
           }
           catch (Exception ex)
           {
               ConsoleServer.WriteLine(devName + "," + ex.Message);
               throw new RemoteInterface.RemoteException(ex.Message + "," + ex.StackTrace);
           }
       }


      
      public  void setDisplayOff(string devName)
       {
           try
           {
               Comm.TC.CMSTC tc = (Comm.TC.CMSTC)Program.mfcc_cms.getTcManager()[devName];
               checkAllowConnect(tc);
               tc.TC_SetDisplayOff();
               ConsoleServer.WriteLine(devName + " 熄滅!");
           }
           catch (Exception ex)
           {
               throw new RemoteInterface.RemoteException(ex.Message+","+ex.StackTrace);
           }
       }

       //public void SendDisplay(string ip, int port, int icon_id, int g_code_id, int hor_space, string mesg, byte[] colors)
       //{
       //  //  throw new Exception("The method or operation is not implemented.");
       //    try
       //    {
       //        Comm.TC.CMSTC tc = (Comm.TC.CMSTC)Program.mfcc_cms.getTcManager()[ip,port];
       //        checkAllowConnect(tc);
       //        tc.TC_SendDisplay(icon_id, g_code_id, hor_space, mesg, colors);
       //    }
       //    catch (Exception ex)
       //    {
       //        throw new RemoteInterface.RemoteException(ex.Message);
       //    }
       //}


      
       public void SetIconPic(string DevName,int icon_id, string desc, System.Drawing.Color[][] bmp)
       {
           try
           {
               Comm.TC.CMSTC tc = (Comm.TC.CMSTC)Program.mfcc_cms.getTcManager()[DevName];
               checkAllowConnect(tc);
               System.Drawing.Bitmap pic=RemoteInterface.Util.ColorsToBitMap(bmp);
               tc.TC_SetIconPic(icon_id, desc,pic);
           }
           catch (Exception ex)
           {
               throw new RemoteInterface.RemoteException(ex.Message+","+ex.StackTrace);
           }
       }
     public   System.Drawing.Color[,] GetIconPic(string DevName, int icon_id, ref string desc)
       {
           try
           {
               Comm.TC.CMSTC tc = (Comm.TC.CMSTC)Program.mfcc_cms.getTcManager()[DevName];
               checkAllowConnect(tc);
              // System.Drawing.Bitmap pic = RemoteInterface.Util.ColorsToBitMap(bmp);
             System.Drawing.Bitmap bmp=  tc.TC_GetIconPic(icon_id, ref desc);
             return Util.BitMapToColorsArray(bmp);

           }
           catch (Exception ex)
           {
               throw new RemoteInterface.RemoteException(ex.Message+","+ex.StackTrace);
           }
       }









     public void SendPreStoreDisplay(string devName, int dataType, int icon_id, int g_code_id, int hor_space, string mesg, byte[] colors, byte[] v_spaces)
     {
         try
         {
             // ConsoleServer.WriteLine("from host1");
             Comm.TC.CMSTC tc = (Comm.TC.CMSTC)Program.mfcc_cms.getTcManager()[devName];
             checkAllowConnect(tc);
             tc.TC_SendPrestoreDisplay(dataType, icon_id, g_code_id, hor_space, mesg, colors, v_spaces);
             ConsoleServer.WriteLine(string.Format(devName + "icon_id:{0} g_code_id:{1} hos_space:{2} mesg:{3}", icon_id, g_code_id, hor_space, mesg));
         }
         catch (Exception ex)
         {
             ConsoleServer.WriteLine(devName + "," + ex.Message);
             throw new RemoteInterface.RemoteException(ex.Message + "," + ex.StackTrace);
         }
     }

     public void GetPreStoreTCDisplay(string devName, ref int dataType, ref int icon_id, ref int g_code_id, ref int hor_space, ref string mesg, ref byte[] colors, ref byte[] vspaces)
     {
        
         try
         {
             Comm.TC.CMSTC tc = (Comm.TC.CMSTC)Program.mfcc_cms.getTcManager()[devName];
             checkAllowConnect(tc);

             System.Data.DataSet ds = tc.TC_GetDisplay();

             if (ds.Tables[0].Rows[0]["func_name"].ToString() == "get_prestore_message")
             {
                 icon_id = System.Convert.ToInt32(ds.Tables[0].Rows[0]["icon_code_id"]);
             }
             else if (ds.Tables[0].Rows[0]["func_name"].ToString() == "get_CMS_display")
             {
                 icon_id = 0;



             }
             else
                 throw new RemoteException("unknown Display Command!");

             g_code_id = (ds.Tables[0].Rows[0]["g_code_id"] == System.DBNull.Value) ? 0 : System.Convert.ToInt32(ds.Tables[0].Rows[0]["g_code_id"]);
             hor_space = (ds.Tables[0].Rows[0]["hor_space"] == System.DBNull.Value) ? 0 : System.Convert.ToInt32(ds.Tables[0].Rows[0]["hor_space"]);
             int msgcnt = (ds.Tables[0].Rows[0]["msgcnt"] == System.DBNull.Value) ? 0 : System.Convert.ToInt32(ds.Tables[0].Rows[0]["msgcnt"]);
             byte[] big5_code = new byte[ds.Tables["tblmsgcnt"].Rows.Count];
             for (int i = 0; i < big5_code.Length; i++)
                 big5_code[i] = System.Convert.ToByte(ds.Tables["tblmsgcnt"].Rows[i]["message"]);

             mesg = Util.Big5BytesToString(big5_code).Replace("\n", "");

             byte[] color_code = new byte[ds.Tables["tblcolorcnt"].Rows.Count];
             for (int i = 0; i < color_code.Length; i++)
                 color_code[i] = System.Convert.ToByte(ds.Tables["tblcolorcnt"].Rows[i]["color"]);
             colors = color_code;

             byte[] v_spaces = new byte[ds.Tables["tblver_no"].Rows.Count];

             for (int i = 0; i < v_spaces.Length; i++)
                 v_spaces[i] = System.Convert.ToByte(ds.Tables["tblver_no"].Rows[i][0]);


             //   tc.TC_SendDisplay(icon_id, g_code_id, hor_space, mesg, colors);
             //   ConsoleServer.WriteLine(string.Format(devName + "icon_id:{0} g_code_id:{1} hos_space:{2} mesg:{3}", icon_id, g_code_id, hor_space, mesg));
         }
         catch (Exception ex)
         {
             ConsoleServer.WriteLine(devName + "," + ex.Message);
             throw new RemoteInterface.RemoteException(ex.Message + "," + ex.StackTrace);
         }
     }
    }
}
