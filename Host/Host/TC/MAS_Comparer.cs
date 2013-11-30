using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.HC;

namespace Host.TC
{
    public  class MAS_Comparer : System.Collections.IComparer
    {
        #region IComparer 成員

        int System.Collections.IComparer.Compare(object x, object y)
        {
            OutputQueueData xx = x as OutputQueueData;
            OutputQueueData yy = y as OutputQueueData;
            if (xx.mode != yy.mode)
                return xx.mode - yy.mode;
            else if (xx.mode == OutputModeEnum.ResponsePlanMode)
            {
                MASOutputData thisout = xx.data as MASOutputData;
                MASOutputData toCompareout = yy.data as MASOutputData;
                //速度越小優先順序越大
                int thisspd=120, toCompareSpd=120;
                for (int i = 0; i < thisout.displays.Length; i++)
                {
                    if (!(thisout.displays[i] is CMSOutputData))
                    {
                        thisspd = System.Convert.ToInt32(thisout.displays[i]);
                        break;
                    }
                }
                for (int i = 0; i < toCompareout.displays.Length; i++)
                {
                    if (!(toCompareout.displays[i] is CMSOutputData))
                    {
                        toCompareSpd = System.Convert.ToInt32(toCompareout.displays[i]);
                        break;
                    }
                }

                return -(thisspd - toCompareSpd);


               
                //  return -(System.Convert.ToInt32(thisds.Tables[0].Rows[0]["speed"]) - System.Convert.ToInt32(toCompareds.Tables[0].Rows[0]["speed"]));

            }
            else
                return xx.priority - yy.priority;

        }

        #endregion
    }
}
