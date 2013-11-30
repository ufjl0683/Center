using System;
using System.Collections.Generic;
using System.Text;

namespace Host
{
   public  class InterSection:IComparable,I_Positionable
   {

       public string type, line1, line2,dir1,dir2;
       int mileage1, mileage2;
       I_Positionable nextDev;
       I_Positionable preDev;

       string m_breach1Name;
       string m_breach2Name;
       public InterSection branch1;
       public InterSection branch2;
       public InterSection(string type, string line1, string dir1,int mileage1,string line2,string dir2,int mileage2)
       {

           this.type = type;
           this.line1 = line1;
           this.line2 = line2;
           this.mileage1 = mileage1;
           this.mileage2 = mileage2;
           this.dir1 = dir1;
           this.dir2 = dir2;
           if (type == "C")
               m_breach1Name = "INTERSEC-" + line2 + "-" + dir2 + "-" + mileage2;

       }

       #region IComparable 成員

       int IComparable.CompareTo(object obj)
       {
           I_Positionable dev = (I_Positionable)obj;
           int ret = 0;
           if (this.dir1== "S" || this.dir1== "E")
               ret= this.mileage1- dev.getMileage();
           else
               ret= -(this.mileage1 - dev.getMileage());

           //if (this.getLineID() == "T74甲")
           //    ret = -ret;

           return ret;
       }

       #endregion

       #region I_Position 成員

       public string getLineID()
       {
           return this.line1;
           //throw new Exception("The method or operation is not implemented.");
       }

       public string getDirection()
       {
           return this.dir1;
           // throw new Exception("The method or operation is not implemented.");
       }

       public int getMileage()
       {
         return  this.mileage1;
          // throw new Exception("The method or operation is not implemented.");
       }

       #endregion

       #region I_DevicePosition 成員


       public string getDevType()
       {
          // throw new Exception("The method or operation is not implemented.");
           return this.type;
       }

       public string getDevName()
       {
           //throw new Exception("The method or operation is not implemented.");
           return "INTERSEC-" + line1 + "-"+dir1 +"-"+ mileage1;
       }

       public string BranchName1
       {
           get
           {
               return this.m_breach1Name;

           }

           set
           {
               this.m_breach1Name = value;
           }
       }


       public string BranchName2
       {
           get
           {
               return this.m_breach2Name;

           }

           set
           {
               this.m_breach2Name = value;
           }
       }

       //public string getBranch1DevName()
       //{
       //    //throw new Exception("The method or operation is not implemented.");
       //   // return "INTERSEC-" + line2+ "-" + dir2 + "-" + mileage2;
       //    branch1.getDevName();
       //}

       //public string getBranch2DevName()
       //{
       //  return  branch2.getBranch1DevName();
       //    //throw new Exception("The method or operation is not implemented.");
       //   // return "INTERSEC-" + line2 + "-" + dir2 + "-" + mileage2;
       //}

       //public string getBranch1DevName()
       //{
       //    //throw new Exception("The method or operation is not implemented.");
       //    // return "INTERSEC-" + line2+ "-" + dir2 + "-" + mileage2;
       //    branch1.getDevName();
       //}

       //public string getBranch2DevName()
       //{
       //    return branch2.getBranch1DevName();
       //    //throw new Exception("The method or operation is not implemented.");
       //    // return "INTERSEC-" + line2 + "-" + dir2 + "-" + mileage2;
       //}


       

       #endregion

       #region I_Positionable 成員


       public I_Positionable getNextDev()
       {
           return this.nextDev;
          // throw new Exception("The method or operation is not implemented.");
       }

       public I_Positionable getPrevDev()
       {
           return this.preDev;
           //throw new Exception("The method or operation is not implemented.");
       }

       public void setPreDev(I_Positionable dev)
       {
           this.preDev = dev;
           //throw new Exception("The method or operation is not implemented.");
       }

       public void setNextDev(I_Positionable dev)
       {
           this.nextDev = dev;
           //throw new Exception("The method or operation is not implemented.");
       }

       #endregion
   }
}
