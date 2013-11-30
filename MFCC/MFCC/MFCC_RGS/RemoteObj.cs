using System;
using System.Collections.Generic;
using System.Text;
using Comm;
using System.Drawing;
using RemoteInterface.MFCC;
using RemoteInterface;

namespace MFCC_RGS
{
   public  class RemoteObj:Comm.MFCC.RemoteMFCCBase,RemoteInterface.MFCC.I_MFCC_RGS
    {

       public override object InitializeLifetimeService()
       {
           return null;
       }

        public override Comm.MFCC.MFCC_Base getMFCC_base()
        {
            return Program.mfcc_rgs;
           // throw new Exception("The method or operation is not implemented.");
        }

       public void setIconPic(string ip, int port, int icon_id, string desc, System.Drawing.Color[][] bmp)
       {

           try
           {
               Comm.TC.RGSTC tc = (Comm.TC.RGSTC)this.getMFCC_base().getTcManager()[ip, port];
               this.checkAllowConnect(tc);  
               System.Drawing.Bitmap pic = RemoteInterface.Util.ColorsToBitMap(bmp);
               tc.TC_SetIconPic(icon_id, desc, pic);
           }
           catch (Exception ex)
           {
               throw new RemoteInterface.RemoteException(ex.Message+","+ex.StackTrace);
           }
       }


       public void setIconPic(string devName, int icon_id, string desc, System.Drawing.Color[][] bmp)
       {

           try
           {
               Comm.TC.RGSTC tc = (Comm.TC.RGSTC)this.getMFCC_base().getTcManager()[devName];
               this.checkAllowConnect(tc);  
               System.Drawing.Bitmap pic = RemoteInterface.Util.ColorsToBitMap(bmp);
               tc.TC_SetIconPic(icon_id, desc, pic);
           }
           catch (Exception ex)
           {
               throw new RemoteInterface.RemoteException(ex.Message+","+ex.StackTrace);
           }
       }

       //public Color[][] getIconPic(string ip, int port, int icon_id, ref string desc)
       //{

       //    try
       //    {
       //        Comm.TC.RGSTC tc = (Comm.TC.RGSTC)this.getMFCC_base().getTcManager()[ip, port];
       //        this.checkAllowConnect(tc);  
       //        return  Util.BitMapToColors(tc.TC_GetIconPic(icon_id, ref desc));
       //    }
       //    catch (Exception ex)
       //    {
       //        throw new RemoteInterface.RemoteException(ex.Message);
       //    }
       //}

       public Color[,] getIconPic(string devName, int icon_id,ref string desc)
       {
         
          // string desc = "";
           try
           {
               Comm.TC.RGSTC tc = (Comm.TC.RGSTC)this.getMFCC_base().getTcManager()[devName];
               this.checkAllowConnect(tc);
               
               return RemoteInterface.Util.BitMapToColorsArray(tc.TC_GetIconPic(icon_id, ref desc));

              // return ret;
               
              
           }
           catch (Exception ex)
           {
               throw new RemoteInterface.RemoteException(ex.Message+","+ex.StackTrace);
           }
       }

        public void setBackGroundPic(string ip, int port, int mode, int g_code_id, string desc, Color[][] bitmap)
        {
         
          try
          {
              Comm.TCBase tc = this.getMFCC_base().getTcManager()[ip, port];
                checkAllowConnect(tc);

              Bitmap pic=  RemoteInterface.Util.ColorsToBitMap(bitmap);
                ((Comm.TC.RGSTC)tc).TC_SetBackGroundPicture(0xffff, mode, g_code_id,desc, pic);

          }
          catch (Exception ex)
          {
              throw new  RemoteInterface.RemoteException(ex.Message+","+ex.StackTrace);
          }
           
        }

        public void setBackGroundPic(string devName, int mode, int g_code_id, string desc, System.Drawing.Color[][] bitmap)
        {
          
            try
            {
                Comm.TCBase tc = this.getMFCC_base().getTcManager()[devName];
                checkAllowConnect(tc);
               System.Drawing.Bitmap pic= RemoteInterface.Util.ColorsToBitMap(bitmap);
                ((Comm.TC.RGSTC)tc).TC_SetBackGroundPicture(0xffff, mode, g_code_id,desc, pic);

            }
            catch (Exception ex)
            {
                throw new RemoteInterface.RemoteException(ex.Message+","+ex.StackTrace);
            }
           
        }



      

        //public Color[][] getBackGroundPic(string ip, int port, int mode, int g_code_id, ref string desc)
        //{
        //   // throw new Exception("The method or operation is not implemented.");
        //    try
        //    {
        //        Comm.TC.RGSTC tc = (Comm.TC.RGSTC)this.getMFCC_base().getTcManager()[ip,port];
        //        checkAllowConnect(tc);
        //        return Util.BitMapToColors(tc.TC_GetBackgroundPic(0xffff, (byte)mode, (byte)g_code_id, ref desc));
        //       // ((Comm.TC.RGSTC)tc).(0xffff,mode,g_code_id,

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new RemoteInterface.RemoteException(ex.Message);
        //    }

        //}

       public void setPolygonData(string devName, byte g_code_id,RGS_PolygonData pdata)
       {

           try
           {
               Comm.TC.RGSTC tc = (Comm.TC.RGSTC)this.getMFCC_base().getTcManager()[devName];
               this.checkAllowConnect(tc);
               tc.TC_SetPolygon(0xffff, g_code_id, pdata);
                
           }
           catch (Exception ex)
           {
               throw new RemoteInterface.RemoteException(ex.Message+","+ex.StackTrace);
           }

       }

       public RGS_PolygonData getPolygonData(string devName, byte g_code_id)
       {

           try
           {
               Comm.TC.RGSTC tc = (Comm.TC.RGSTC)this.getMFCC_base().getTcManager()[devName];
               this.checkAllowConnect(tc);
                return tc.TC_GetPolygon(0xffff, g_code_id);

           }
           catch (Exception ex)
           {
               throw new RemoteInterface.RemoteException(ex.Message+","+ex.StackTrace);
           }

       }


        public Color[,] getBackGroundPic(string devName, int mode, int g_code_id, ref string desc)
        {
          //  throw new Exception("The method or operation is not implemented.");
            try
            {
                Comm.TC.RGSTC tc = (Comm.TC.RGSTC)this.getMFCC_base().getTcManager()[devName];
                this.checkAllowConnect(tc);  
                return RemoteInterface.Util.BitMapToColorsArray(tc.TC_GetBackgroundPic(0xffff, (byte)mode, (byte)g_code_id, ref desc));
            }
            catch (Exception ex)
            {
                throw new RemoteInterface.RemoteException(ex.Message);
            }
        }

       public void setGenericDisplay(string ip, int port, RGS_GenericDisplay_Data data)
       {

           try
           {
               Comm.TC.RGSTC tc = (Comm.TC.RGSTC)this.getMFCC_base().getTcManager()[ip,port];
               this.checkAllowConnect(tc);  
              // return tc.setG
               if (data == null)
               {
                   tc.TC_SetDisplayOff();
                   
               }
               else
                   tc.TC_SetGenericDisplay(data);


           }
           catch (Exception ex)
           {
               throw new RemoteInterface.RemoteException(ex.Message+","+ex.StackTrace);
           }

       }
       public void setGenericDisplay(string devName, RGS_GenericDisplay_Data data)
       {
           try
           {
              
               Comm.TC.RGSTC tc = (Comm.TC.RGSTC)this.getMFCC_base().getTcManager()[devName];
               this.checkAllowConnect(tc);
               if (data == null)
               {
                   tc.TC_SetDisplayOff();
                   ConsoleServer.WriteLine(devName + " 熄滅!");
               }
               else
               // return tc.TC_GetBackgroundPic(0xffff, (byte)mode, (byte)g_code_id, ref desc);
               {
                   tc.TC_SetGenericDisplay(data);
                   ConsoleServer.WriteLine(devName + data.ToString());
               }
              
           }
           catch (Exception ex)
           {
               throw new RemoteInterface.RemoteException(ex.Message+","+ex.StackTrace);
           }
       }

       public RGS_GenericDisplay_Data getCurrentDisplay(string ip, int port)
       {
           try
           {
               Comm.TC.RGSTC tc = (Comm.TC.RGSTC)this.getMFCC_base().getTcManager()[ip,port];
               this.checkAllowConnect(tc);  
               // return tc.TC_GetBackgroundPic(0xffff, (byte)mode, (byte)g_code_id, ref desc);
               return tc.TC_GetCurrDisplayData();

           }
           catch (Exception ex)
           {
               throw new RemoteInterface.RemoteException(ex.Message+","+ex.StackTrace);
           }
       }

       public RGS_GenericDisplay_Data getCurrentDisplay(string devName)
       {
           try
           {
               Comm.TC.RGSTC tc = (Comm.TC.RGSTC)this.getMFCC_base().getTcManager()[devName];
               this.checkAllowConnect(tc);  
               // return tc.TC_GetBackgroundPic(0xffff, (byte)mode, (byte)g_code_id, ref desc);
               return tc.TC_GetCurrDisplayData();

           }
           catch (Exception ex)
           {
               throw new RemoteInterface.RemoteException(ex.Message+","+ex.StackTrace);
           }
       }




      // #region I_MFCC_RGS 成員


       public void setDisplayOff(string devName)
       {
          // throw new Exception("The method or operation is not implemented.");
           try
           {
               Comm.TC.RGSTC tc = (Comm.TC.RGSTC)this.getMFCC_base().getTcManager()[devName];
               this.checkAllowConnect(tc);  
               // return tc.TC_GetBackgroundPic(0xffff, (byte)mode, (byte)g_code_id, ref desc);
                tc.TC_SetDisplayOff(); ;

           }
           catch (Exception ex)
           {
               throw new RemoteInterface.RemoteException(ex.Message+","+ex.StackTrace);
           }
       }

     //  #endregion

  


       public RGS_GenericDisplay_Data getCurrentGenericDisplay(string devName)
       {
           try
           {
               Comm.TC.RGSTC tc = (Comm.TC.RGSTC)this.getMFCC_base().getTcManager()[devName];
               this.checkAllowConnect(tc);
              
              return  tc.TC_GetCurrDisplayData();

           }
           catch (Exception ex)
           {
               throw new RemoteInterface.RemoteException(ex.Message+","+ex.StackTrace);
           }
       }

     
   }
}
