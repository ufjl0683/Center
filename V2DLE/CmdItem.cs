using System;
using System.Collections.Generic;
using System.Text;

namespace Comm
{
   public  enum  RangeType
   {
       Rannge,Select,Const
   }
   
  public   class CmdItem
    {

      public RangeType RangeType;
      public  int Min, Max;

      public SelectValue[] SelectValues;
      public int Bytes;
      public string ItemName;

      public System.Collections.ArrayList SubItems = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList());
      

      public CmdItem(string Name,int bytes, int lowvalue, int highValue)
      {
          this.RangeType = RangeType.Rannge;
          Min = lowvalue;
          Max = highValue;
          this.Bytes = bytes;
          this.ItemName = Name;
      }

      public CmdItem(string Name,int bytes, SelectValue[] selectValues)
      {
          this.RangeType = RangeType.Select;
          this.SelectValues = selectValues;
          this.Bytes = bytes;
          this.ItemName = Name;

      }

      public CmdItem(string name, int bytes, int val)
      {
          this.RangeType = RangeType.Const;
          Min = val;
          Max = val;
          this.Bytes = bytes;
          this.ItemName = name;
      }


      public bool HasSubItems

      {
          get
          {
              return (SubItems.Count==0) ? false : true;
          }
      }

      public void AddSubItems(CmdItem item)
      {
          SubItems.Add(item);
      }

      public int SubItemsCnt
      {

          get
          {
              return SubItems.Count;
          }
      }


      public override string ToString()
      {
          //return base.ToString();
          return string.Format("({0},{1},{2},SubItems={3})", ItemName, Bytes, RangeType, SubItemsCnt);
      }

        public Type DataType
       {

           get
           {
               switch (Bytes)
               {
                   case 1:
                       return typeof(byte);
                   case 2:
                       return typeof(ushort);
                   case 3:
                       return typeof(uint);
                   case 4:
                       return typeof(uint);
                   //case 8:
                   //    return typeof(ulong);
                   default:
                       return  typeof(ulong);
                 
               }
           }
       }      

    }
}
