using System;
using System.Collections.Generic;
using System.Text;

namespace Host
{
   public interface I_Positionable

    {
       string getLineID();
       string getDirection();
       int getMileage();
       string getDevType();
       string getDevName();
       I_Positionable getNextDev();
       I_Positionable getPrevDev();
       void setPreDev(I_Positionable dev);
       void setNextDev(I_Positionable dev);
    }
}
