using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface;

namespace Comm.MFCC
{
  public  class TC_Status
    {
      public I_HW_Status_Desc desc;
      public  byte opmode, opstatus;
      public string dispDesc;
      public byte[] hw_status;

      public TC_Status(byte[]hw_status,I_HW_Status_Desc desc, byte opmode, byte opstatus, string dispDesc)
      {
          this.desc = desc;
          this.opmode = opmode;
          this.opstatus = opstatus;
          this.dispDesc = dispDesc;
          this.hw_status = hw_status;
      }


       
    }
}
