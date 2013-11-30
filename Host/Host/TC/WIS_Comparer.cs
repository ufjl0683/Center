﻿using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.HC;

namespace Host.TC
{
    public  class WIS_Comparer: System.Collections.IComparer
    {
        public int Compare(object x, object y)
        {
            int[] priorityTable = new int[] { 0, 3, 1, 2 };

                                  //霧，雨，風
            OutputQueueData xx = x as OutputQueueData;
            OutputQueueData yy = y as OutputQueueData;
            if (xx.mode != yy.mode)
                return xx.mode - yy.mode;
            else if (xx.mode == OutputModeEnum.ResponsePlanMode)
            {
               WISOutputData thisout  = xx.data as WISOutputData;
               WISOutputData toCompareout = yy.data as WISOutputData;
                //速度越小優先順序越大

               return priorityTable[thisout.WisMesgType] - priorityTable[toCompareout.WisMesgType];
              //  return -(System.Convert.ToInt32(thisds.Tables[0].Rows[0]["speed"]) - System.Convert.ToInt32(toCompareds.Tables[0].Rows[0]["speed"]));

            }
            else
                return xx.priority - yy.priority;
        }
    }
}
