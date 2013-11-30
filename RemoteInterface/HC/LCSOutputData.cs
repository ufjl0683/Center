using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.HC
{
    [Serializable]
    public class LCSOutputData
    {
        public System.Data.DataSet dataset;
        public LCSOutputData(System.Data.DataSet ds)
        {
            this.dataset = ds;
        }

       
    }
}
