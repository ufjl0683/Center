using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface
{
    abstract public class RemoteClassBase : MarshalByRefObject
    {
        public static long CallCnt = 0;

        public string HelloWorld()
        {
            CallCnt++;
            return "HelloWorld!";
        }




    }
}
