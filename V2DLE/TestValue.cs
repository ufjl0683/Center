using System;
using System.Collections.Generic;
using System.Text;

namespace Comm
{
 public    class TestValue
    {
        public string itemName;
      public  int value;
     public System.Collections.ArrayList subValues;
     public CmdItem cmdItem;
   public   TestValue(string itemName, int value)
        {
            this.itemName = itemName;
            this.value = value;
            subValues = new System.Collections.ArrayList(30);
        }

    public  bool hasSubValues
     {
         get
         {
             return  subValues.Count != 0;
         }
     }

     public int SubValueCnt
     {
         get
         {
             return subValues.Count;
         }
     }

     public override string ToString()
     {
        
         return "("+itemName+","+value+",subValCNt="+this.SubValueCnt+")";
     }


    }
}
