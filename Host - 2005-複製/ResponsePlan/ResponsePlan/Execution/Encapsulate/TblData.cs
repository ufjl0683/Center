using System;
using System.Collections.Generic;
using System.Text;

namespace Execution
{    
    /// <summary>
    /// 以規則庫下載需要所有的資料表
    /// </summary>
    [Serializable]
    class RSPDataTbl
    {
        private static RSPDataTbl tblData = null;

        private RSPDataTbl()
        {
            setDBData(RenewData.All);
        }

        public void setDBData(RenewData dataType)
        {
        }

        /// <summary>
        /// 獨體建構者
        /// </summary>
        /// <returns>建構元</returns>
        public static RSPDataTbl getBuilder()
        {
            if (tblData == null)
            {
                lock (typeof(RSPDataTbl))
                {
                    if (tblData == null)
                        tblData = new RSPDataTbl();
                }
            }
            return tblData;
        }
    }
}
