using System;
using System.Collections.Generic;
using System.Text;
using RemoteInterface.HC;

namespace Host.TC
{
   public class CSLS_Comparer:System.Collections.IComparer

    {
       

        //int IComparer<OutputQueueData>.Compare(OutputQueueData x, OutputQueueData y)
        //{
        //    //throw new NotImplementedException();
        //    if (x.mode != y.mode)
        //        return x.mode - y.mode;
        //    else if (x.mode == OutputModeEnum.ResponsePlanMode)
        //    {
        //        System.Data.DataSet thisds = x.data as System.Data.DataSet;
        //        System.Data.DataSet toCompareds = y.data as System.Data.DataSet;
        //        //速度越小優先順序越大
        //        return -(System.Convert.ToInt32(thisds.Tables[0].Rows[0]["speed"]) - System.Convert.ToInt32(toCompareds.Tables[0].Rows[0]["speed"]));
               
        //    }
        //    else
        //        return x.priority - y.priority;
   //}
            
        #region IComparer 成員

public int  Compare(object x, object y)
{
    OutputQueueData xx = x as OutputQueueData;
    OutputQueueData yy = y as OutputQueueData;
 	        if (xx.mode != yy.mode)
                return xx.mode - yy.mode;
            else if (xx.mode == OutputModeEnum.ResponsePlanMode)
            {
                System.Data.DataSet thisds = (xx.data as CSLSOutputData).dataset;
                System.Data.DataSet toCompareds = (yy.data as CSLSOutputData).dataset;
                //速度越小優先順序越大

                int speed1 = 0, speed2 = 0;
                speed1 = System.Convert.ToInt32(thisds.Tables[0].Rows[0]["speed"]);
                speed2 = System.Convert.ToInt32(toCompareds.Tables[0].Rows[0]["speed"]);
                //反映計畫層級，熄滅優先順序最高
                if (speed1 == 255)
                    speed1 = -255;
                if (speed2 == 255)
                    speed2 = -255;
                return -(speed1 - speed2);
               
            }
            else
                return xx.priority - yy.priority;
}

#endregion
}

     
    
}
