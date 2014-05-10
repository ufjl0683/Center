using System;
using System.Collections.Generic;
using System.Text;
using Comm.MFCC;

namespace MFCC_ETAG
{
    public class RemoteObj:RemoteMFCCBase,RemoteInterface.MFCC.I_MFCC_AVI
    {

        public override Comm.MFCC.MFCC_Base getMFCC_base()
        {
            return Program.mfcc_etag;
        }
    }
}
