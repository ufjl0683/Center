using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace RemoteInterface.HC
{
    [Serializable]
     public  class TIMCC_RespInstruction
    {

        public int evtid;
        public System.Data.DataSet tbl;
        public TIMCC_RespInstruction(int evtid, System.Data.DataSet tbl)
        {
            this.evtid = evtid;
            this.tbl = tbl;
        }
    }
}
