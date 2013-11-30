using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.MFCC
{
    [Serializable]
    public  class RD_5Min_Data
    {
        public string devName;
        public  DateTime dt;
         public  int amount, acc_amount, degree;
        public RD_5Min_Data(string devName, DateTime dt, int amount, int acc_amount, int degree)
        {
            this.devName = devName;
            this.dt = dt;
            this.amount = amount;
            this.acc_amount = acc_amount;
            this.degree = degree;
        }
    }
}
