using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.HC;

namespace Host.TC
{
   public  class RMS_Comparer:System.Collections.IComparer

    {


        #region IComparer 成員

        int System.Collections.IComparer.Compare(object x, object y)
        {
             int[] priorityTable = new int[] { 2, 3, 1, 4, 0 };
            OutputQueueData toCompare = (OutputQueueData)y;
            OutputQueueData thisobj = (OutputQueueData)x;
            if (thisobj.mode != toCompare.mode)
                return thisobj.mode - toCompare.mode;
            else if (thisobj.mode == OutputModeEnum.ResponsePlanMode)
            {
                RMSOutputData thisrmsout = thisobj.data as RMSOutputData;
                RMSOutputData toComparermsout = toCompare.data as RMSOutputData;
                return priorityTable[thisrmsout.mode] - priorityTable[toComparermsout.mode];
            }
            else if (thisobj.priority == toCompare.priority)
                return 0;
            else

                return thisobj.priority - toCompare.priority;
           
        }

        #endregion
    }
}
