using System;
using System.Collections.Generic;
using System.Text;

namespace Execution
{    
    /// <summary>
    /// �H�W�h�w�U���ݭn�Ҧ�����ƪ�
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
        /// �W��غc��
        /// </summary>
        /// <returns>�غc��</returns>
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
