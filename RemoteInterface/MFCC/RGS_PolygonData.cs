using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteInterface.MFCC
{

    [Serializable]
    public class RGS_PolygonData
    {
        public RGS_Ploygon[] polygons;
        // byte g_code_id;
        public RGS_PolygonData(RGS_Ploygon[] polygons)
        {
            // this.g_code_id = g_code_id;
            this.polygons = polygons;
        }



    }

}
