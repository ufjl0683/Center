using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.HC
{
    [Serializable]
   public  class CSLSOutputData
    {
        public System.Data.DataSet dataset;
       public CSLSOutputData(System.Data.DataSet ds)
        {
            this.dataset = ds;
        }
    }
}
