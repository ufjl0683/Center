using System;
using System.Collections.Generic;
using System.Text;

namespace Host.Event.APID
{
    public class APIDManager
    {

        System.Collections.Hashtable lines;

        public APIDManager()
        {


            lines = Program.matrix.jammgr.getLines();
        }

    }
}
