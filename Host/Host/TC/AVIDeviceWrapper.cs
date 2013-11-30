using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.MFCC;
using RemoteInterface;

namespace Host.TC
{
  public   class AVIDeviceWrapper:DeviceBaseWrapper
    {
        const int LiveMiniutes = 60;
        System.Timers.Timer tmr30min = new System.Timers.Timer(10 * 60 * 1000);
        System.Collections.Hashtable plates = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable());
       // public string devicename;
       
        // System.Collections.ArrayList UpAVIList = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList());
      public AVIDeviceWrapper(string mfccid, string devicename, string deviceType, string ip, int port, string location, string lineid, int mile_m, byte[] hw_status, byte opmode, byte opstatus, string direction)
          : base(mfccid, devicename, deviceType, ip, port, location, lineid, mile_m, hw_status, opmode, opstatus,direction)
        {
            //this.devicename = devicename;
            //this.mile_m = mile_m;
            tmr30min.Elapsed += new System.Timers.ElapsedEventHandler(tmr30min_Elapsed);
            tmr30min.Start();
        }

        void tmr30min_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //throw new Exception("The method or operation is not implemented.");
            this.ClearDuePlate();
        }


        //public void AddUpStreamAVI(AVIDeviceWrapper avi)
        //{
        //    this.UpAVIList.Add(avi);

        //}

        public void AddPlate(AVIPlateData data)
        {

            if (plates.Contains(data.plate))
                plates.Remove(data.plate);

            plates.Add(data.plate, data);


        }

        public AVIPlateData GetPlateData(string plate)
        {
            return (AVIPlateData)plates[plate];
        }

        public bool Match(string plate)
        {
            return plates.Contains(plate);
        }

        public int CurrDataCnt
        {
            get
            {
                return this.plates.Count;
            }
        }
        void ClearDuePlate()
        {
            try
            {
                AVIPlateData[] datas = new AVIPlateData[plates.Values.Count];
                plates.Values.CopyTo(datas, 0);
              //  System.Collections.ArrayList ary = new System.Collections.ArrayList();
                foreach (AVIPlateData data in datas)
                {
                   
                    if (System.DateTime.Now - data.dt > TimeSpan.FromMinutes(LiveMiniutes))
                    {
                        plates.Remove(data.plate);
                    }
                       // ary.Add(data.plate);
                }

                //foreach (string plate in ary)
                //    plates.Remove(plate);
            }
            catch (Exception ex)
            {
                ConsoleServer.WriteLine(ex.Message + "," + ex.StackTrace);
            }
        }

    }


   
}
