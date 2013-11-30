using System;
using System.Collections.Generic;
using System.Text;

namespace Host.Event.MetroNetwork
{
   public    class MetroNetworkManager
    {

       System.Collections.Generic.Dictionary<string, MetroNetworkRange> hsEvents = new Dictionary<string, MetroNetworkRange>();

       
       public MetroNetworkManager()
       {

           RegistRampDataEvent();
           Program.matrix.rgs_polygon_section_mapping.OnMetroJamChangeEvent += new OnRGS_Metro_Jam_Change_Handler(OnMetroMainEvent);
       }

       public int GetEventCnt()
       {


           return this.hsEvents.Count;
       }
       void OnMetroMainEvent(string rgsname ,int jamlevel,int g_code_id)
       {
           try
           {
              // int jamlevel = 0;
             //  Host.Event.Jam.RampVDData data = sender as Host.Event.Jam.RampVDData;
               if (rgsname == null)
                   return;

               


               if (jamlevel <= 0)
               {

                   if (hsEvents.ContainsKey(rgsname))
                   {

                       hsEvents[rgsname].IsMainVDJamp = false;
                       if (hsEvents[rgsname].CanStop())
                       {


                           hsEvents[rgsname].invokeStop();
                           hsEvents.Remove(rgsname);
                       }

                   }


               }
               else if (jamlevel > 0)
               {
                   if (hsEvents.ContainsKey(rgsname))
                       hsEvents[rgsname].IsMainVDJamp = true;
                   else
                   {
                       MetroNetworkRange range = new MetroNetworkRange(Program.matrix.device_mgr[rgsname] as TC.RGSDeviceWrapper,
                           g_code_id) { IsMainVDJamp = true };

                       hsEvents.Add(rgsname, range);
                       Program.matrix.event_mgr.AddEvent(range);
                   }



               }

           }
           catch (Exception ex)
           {
               RemoteInterface.ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
           }

       }
     void   RegistRampDataEvent()
       {

         System.Collections.IEnumerator ie=  Program.matrix.jammgr.getAllRamData();

         while (ie.MoveNext())
         {
             Host.Event.Jam.RampVDData vddata = ie.Current as Host.Event.Jam.RampVDData;
             if(vddata.RGS_DeviceName!=null)
                  vddata.OnEvent += new EventHandler(vddata_OnEvent);
         }
        
       }

     void vddata_OnEvent(object sender, EventArgs e)
     {
         try
         {
             int jamlevel=0;
             Host.Event.Jam.RampVDData data = sender as Host.Event.Jam.RampVDData;
             if (data.RGS_DeviceName == null)
                 return;

             if (data.laneid == -1)
                 jamlevel = data.vd.jamLevel;
             else
                 jamlevel = data.laneJamLevel;


             if (jamlevel <= 0)
             {

                 if (hsEvents.ContainsKey(data.RGS_DeviceName))
                 {

                     hsEvents[data.RGS_DeviceName].IsRampJam = false;
                     if (hsEvents[data.RGS_DeviceName].CanStop())
                     {

                         
                         hsEvents[data.RGS_DeviceName].invokeStop();
                         hsEvents.Remove(data.RGS_DeviceName);
                     }

                 }


             }
             else if (jamlevel > 0)
             {
                 if (hsEvents.ContainsKey(data.RGS_DeviceName))
                     hsEvents[data.RGS_DeviceName].IsRampJam = true;
                 else
                 {
                     MetroNetworkRange range=  new MetroNetworkRange(Program.matrix.device_mgr[data.RGS_DeviceName] as TC.RGSDeviceWrapper, data.metro_g_code_id) { IsRampJam = true };

                     hsEvents.Add(data.RGS_DeviceName,range);
                     Program.matrix.event_mgr.AddEvent(range);
                 }



             }

         }
         catch (Exception ex)
         {
             RemoteInterface.ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
         }
        
     }


   
    }
}
