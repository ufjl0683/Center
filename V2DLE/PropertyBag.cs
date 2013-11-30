using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
namespace Comm
{
   public abstract class PropertyBag
    {
       public event EventHandler OnHWStatus_Changed;
        private byte _TransmitCycle;
        public HWStatus HWtatus;
        private byte _TransmitMode;
        private byte _HWCycle;
        private byte _OpSTatus;
        private byte _OpMode;
        private byte _ComState;   
        protected bool IsLoading = false;
        protected string savepathfile;
        private bool _IsAllowManualMode;
       
        public PropertyBag()
        {
            HWtatus=new HWStatus();
           this.HWtatus.HWChanged += new EventHandler(HWtatus_HWChanged);
            IsLoading = true;
            ComState = 1;
        }


       public  void SetHasLoaded()
       {
           IsLoading=false;
       }
        void HWtatus_HWChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            Serialize();
            if (this.OnHWStatus_Changed != null)
                this.OnHWStatus_Changed(this, null);
        }

       

        public bool IsAllowManualMode
        {
            get
            {
                return _IsAllowManualMode;
            }
            set
            {
                if (value != _IsAllowManualMode)
                {
                    _IsAllowManualMode = value;
                    Serialize();
                }
            }
        }
        public byte ComState
        {
            get
            {
                return _ComState;
            }
            set
            {
                if (value != _ComState)
                {
                    _ComState = value;
                    Serialize();
                }
            }
        }
        public byte OPStatus
        {
            get
            {
                return _OpSTatus;
            }
            set
            {
                if (_OpSTatus != value)
                {
                    _OpSTatus = value;
                    Serialize();
                }
            }
        }
        public byte OPMode
        {
            get
            {
                return _OpMode;
            }
            set
            {
                if (_OpMode != value)
                {
                    _OpMode = value;
                    Serialize();
                }
            }
        }
      public byte TransmitMode
      {
          get
          {
              return _TransmitMode;
          }
          set{
              if (_TransmitMode != value)
              {
                  _TransmitMode = value;
                  Serialize();
              }
          }
      }
      public byte TransmitCycle
      {
          get
          {
              return _TransmitCycle;
          }
          set
          {

              if (value != _TransmitCycle)
              {
                  _TransmitCycle = value;
                  Serialize();

              }
          }
      }
      public byte HWCycle
      {
          get
          {
              return _HWCycle;
          }
          set
          {
              if (value != _HWCycle)
              {
                  _HWCycle = value;
                  Serialize();
              }
          }
      }
      

   public  void Serialize()
   {
       if (IsLoading)
           return;
       lock (this)
       {
           System.Xml.Serialization.XmlSerializer ser = new XmlSerializer(GetPropertyType());
           System.IO.Stream fs = System.IO.File.Create(AppDomain.CurrentDomain.BaseDirectory + "PropertyBag.xml");
           ser.Serialize(fs, this);
           fs.Close();
           fs.Dispose();
       }
         // Serialize(this.savepathfile);
    }

   protected abstract Type GetPropertyType();
  
      //public void Serialize(string pathfile)
      //{
         
         
      //}
    }


    public class HWStatus
    {

        public event EventHandler HWChanged;

        public byte hwstatus1;
        public byte hwstatus2;
        public byte hwstatus3;
        public byte hwstatus4;
       // private byte[] hwstatus=new byte[4];// = new byte[] { hwstatus1, hwstatus2, hwstatus3, hwstatus4 };
        public HWStatus()
        {
           // hwstatus = hwstatus;
        }
        public byte this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0:
                        return hwstatus1;
                        break;
                    case 1:
                        return hwstatus2;
                        break;
                    case 2:
                        return hwstatus3;
                        break;
                    case 3:
                        return hwstatus4;
                        break;
                }
                return hwstatus1;
            }
            set 
            {

                switch (i)
                {
                    case 0:
                        if (value != hwstatus1)
                        {
                            hwstatus1 = value;
                             if (HWChanged != null)
                               HWChanged(this, null);
                        }
                        break;
                    case 1:

                        if (value != hwstatus2)
                        {
                            hwstatus2 = value;
                            if (HWChanged != null)
                                HWChanged(this, null);
                        }
                        break;
                    case 2:
                        if (value != hwstatus3)
                        {
                            hwstatus3 = value;
                            if (HWChanged != null)
                                HWChanged(this, null);
                        }
                        break;
                    case 3:
                        if (value != hwstatus4)
                        {
                            hwstatus4 = value;
                            if (HWChanged != null)
                                HWChanged(this, null);
                        }
                        break;
                    default:
                        break;
                      
                }
                //if (value != hwstatus[i])
                //{
                //    hwstatus[i] = value;
                //    if (HWChanged != null)
                //        HWChanged(this, null);
                //}

            }
        }
      //public byte[] GetByte()
       //{
       //    return new byte[] { hwstatus1, hwstatus2, hwstatus3, hwstatus4 };
       //}
    }
}
