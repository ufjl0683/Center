﻿using System;
using System.Collections.Generic;
using System.Text;
using Host.TC;
using Host.Event;

namespace Host.Event.Jam
{
   public class JamRange:Range
    {
    
      
       public bool DelMark = false;

     

       public JamRange(VDDeviceWrapper vd):base(vd)
       {

        //   this.m_eventid = Global.getEventId();
           this.m_level = vd.jamLevel;

           this.EventId = Global.getEventId();

           this.m_alarm_type = AlarmType.TRAFFIC;
           this.m_class = 41;  //一般道路壅塞
           try
           {
               this.m_eventmode = Global.getEventMode(this.m_class);
           }
           catch
           {
               this.m_eventmode = EventMode.DontCare;
           }

       }

       //protected override void loadEventIdAndMode()
       //{
       //    //throw new Exception("The method or operation is not implemented.");
          
       //}




       public string DeviceName
       {
           get
           {
               return (this.devlist[0] as DeviceBaseWrapper).deviceName;
           }
       }

       public override int StartMile
       {
           get
           {
               return (devlist[0] as VDDeviceWrapper).start_mileage;
           }
       }

       public override int EndMile
       {
           get
           {
               return (devlist[devlist.Count-1] as VDDeviceWrapper).end_mileage;
           }
       }

       public override bool IsInRange(DeviceBaseWrapper dev)
       {
           if (dev == null)
               return false;

           VDDeviceWrapper vddev = dev as VDDeviceWrapper;

           return vddev.start_mileage >= StartMile && vddev.end_mileage < EndMile;
       }

     

       public bool IsInRange(int inx)
       {
           return inx >= StartIndex && inx <= EndIndex;
       }

       public override string ToString()
       {
           if (this.devlist.Count > 0)

               return "EventID:" + this.EventId + "," + "EventMode:" + this.EventMode + "," + this.LineId + "," + this.Direction + "," + StartMile + "~" + EndMile + "壅塞!," + getEventStatus();
           
           else
               return this.EventId + ",clear";
       }

       public int Distance(JamRange range)
       {

           if (range.StartIndex > EndIndex)
               return range.StartIndex - EndIndex-1;  //在後
           else
               return -(this.StartIndex - range.EndIndex-1);  //在前

       }

       public bool Shrink()
       {
           bool isChange = false;
           System.Collections.ArrayList rmlst = new System.Collections.ArrayList();

           if (devlist.Count == 0)
           {
               this.DelMark = true;
               return false;
           }


           VDDeviceWrapper vd = this.devlist[0] as VDDeviceWrapper;
           VDDeviceWrapper lastVD = this.devlist[devlist.Count - 1] as VDDeviceWrapper;
           do
           {
               if (vd.jamLevel < 3)
               {
                   isChange = true;
                   rmlst.Add(vd);
               }
               else
                   break;
               if (vd == lastVD)
                   break;
             
               vd = vd.NextDevice as VDDeviceWrapper;

           } while (true);

           foreach (VDDeviceWrapper dev in rmlst)
               devlist.Remove(dev);

           if (devlist.Count == 0)
           {
               this.DelMark = true;
               return false;
           }

           rmlst.Clear();
           vd = this.devlist[devlist.Count-1] as VDDeviceWrapper;
           lastVD = this.devlist[0] as VDDeviceWrapper;
           do
           {
               if (vd.jamLevel < 3)
               {
                   rmlst.Add(vd);
                   isChange = true;
               }
               else
                   break;
               if (vd == lastVD)
                   break;
               vd = vd.PreDevice as VDDeviceWrapper;

           } while (true);

           foreach (VDDeviceWrapper dev in rmlst)
               devlist.Remove(dev);

           if (devlist.Count == 0)
           {
               this.DelMark = true;
               return false;
           }

        

           devlist.Sort();
           return isChange;
       }

       public bool IsEmpty()
       {
           return devlist.Count == 0;
       }


       public bool Merge( JamRange range)
       {
           int distance=Distance(range);

           //相鄰超過兩座以上
          if (Math.Abs(distance)>=2) 
             return false;
           //相鄰一座

         if (distance == 1)
         {
             VDDeviceWrapper vd = (VDDeviceWrapper)range.devlist[0];
             if (((VDDeviceWrapper)vd.PreDevice).jamLevel < 2)
                 return false;

         }

         if (distance == -1)
         {

             VDDeviceWrapper vd = (VDDeviceWrapper)this.devlist[0];
             if (((VDDeviceWrapper)vd.PreDevice).jamLevel < 2)
                 return false;
         }

         foreach (VDDeviceWrapper dev in range.devlist)
         
             this.devlist.Add(dev);

          
        
           this.devlist.Sort();
           range.DelMark = true;
         return true;
       }


       public JamRange[] Split()
       {

           //chek 22

           for (int i = 1; i < devlist.Count - 1; i++)
           {

               VDDeviceWrapper vd =devlist[i] as VDDeviceWrapper;
               if ((vd.jamLevel == 2 && ((VDDeviceWrapper)vd.NextDevice).jamLevel == 2) || vd.jamLevel < 2)
               {
                   JamRange[] splitrang = new JamRange[2];

                   splitrang[0] = new JamRange(devlist[0] as VDDeviceWrapper);
                   splitrang[1]=new JamRange(devlist[devlist.Count-1]  as VDDeviceWrapper);


                   for (int j = 1; j < i; j++)
                       splitrang[0].Merge(new JamRange(devlist[j]  as VDDeviceWrapper));



                  for (int j = (vd.jamLevel==2)?i + 2:i+1; j < devlist.Count-1; j++)
                       splitrang[1].Merge(new JamRange(devlist[j] as VDDeviceWrapper));

                   return splitrang;
               }


           }

           return null;
             
       }


       #region I_Event 成員









       public override int getDegree()
       {
           //throw new Exception("The method or operation is not implemented.");
           return this.m_level;
       }

       public override string getLineId()
       {
          // throw new Exception("The method or operation is not implemented.");
           return LineId;
       }

       public override string getDir()
       {
         //  throw new Exception("The method or operation is not implemented.");
           return Direction;
       }

       public override int getStartMileM()
       {
         //  throw new Exception("The method or operation is not implemented.");
           return StartMile;
       }

       public override int getEndMileM()
       {
           //throw new Exception("The method or operation is not implemented.");
           return this.EndMile;
       }

       #endregion
   }
}
