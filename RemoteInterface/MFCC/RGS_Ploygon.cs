using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace RemoteInterface.MFCC
{
    [Serializable]
    public class RGS_Ploygon
    {
        public Point[] points;
        public RGS_Ploygon(int no_points)
        {
            points = new Point[no_points];
        }

    }

}
