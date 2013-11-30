using System;
using System.Collections;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;


namespace ReportForm.CommClass.Father
{
    public enum RptHander
    {
        //有主重報表
        一天交通資料, //ctrlRPT_DATA_00
        五分鐘交通資料記錄報表, //ctrlRPT_DATA_01
        五分鐘車道使用率及車間距報表,//ctrlRPT_DATA_02
        一分鐘交通資料記錄報表,//ctrlRPT_DATA_03
        一分鐘車道使用率及車間距報表,//ctrlRPT_DATA_04
        現點速率調查交通資料記錄報表,//ctrlRPT_DATA_06
        VD詳細記錄報表,//ctrlRPT_DATA_24
        VD總計記錄報表,//ctrlRPT_DATA_25
        小時交通資料記錄報表,//ctrlRPT_HDA_01
        小時交通流量統計報表依日期時段彙整,//ctrlRPT_HDA_03
        小時交通平均速度統計報表,//ctrlRPT_HDA_05
        匝道平均每日交通量統計報表,//ctrlRPT_HDA_11

        //無主重報表
        主線平均每日交通量統計報表,//ctrlRPT_HDA_10
        全區匝道全日交通量統計報表,//ctrlRPT_HDA_12
        全區主線全日交通量統計報表,//ctrlRPT_HDA_13
        全區主線小時路段平均速度統計報表,//ctrlRPT_HDA_14
        資訊可變標誌即時資料報表,//ctrlRPT_MON_01
        設備狀態即時監視報表,//ctrlRPT_MON_07
        路段壅塞狀況一分鐘報表,//ctrlRPT_HDA_35



        //無主重報表,但有種類
        操作記錄報表,//ctrlRPT_OPR1_01
        現場終端設備運作記錄報表,//ctrlRPT_OPR2_06
        定時比對記錄報表,//ctrlRPT_OPR2_07
        現場終端設備狀態記錄報表,//ctrlRPT_STA_01
    }

    internal enum EngRptHander
    {
        //有主重報表
        RPT_DATA_00,
        RPT_DATA_01,
        RPT_DATA_02,
        RPT_DATA_03,
        RPT_DATA_04,
        RPT_DATA_06,
        RPT_DATA_24,
        RPT_DATA_25,
        RPT_HDA_01,
        RPT_HDA_03,
        RPT_HDA_05,
        RPT_HDA_11,

        //無主重報表
        RPT_HDA_10,
        RPT_HDA_12,
        RPT_HDA_13,
        RPT_HDA_14,
        RPT_MON_01,
        RPT_MON_07,

        //無主重報表,但有種類
        RPT_OPR1_01,
        RPT_OPR2_06,
        RPT_OPR2_07,
        RPT_STA_01,
        RPT_HDA_35
    }

    public enum HasMain
    {
        //有主重報表
        一天交通資料, //ctrlRPT_DATA_00
        五分鐘交通資料記錄報表, //ctrlRPT_DATA_01
        五分鐘車道使用率及車間距報表,//ctrlRPT_DATA_02
        一分鐘交通資料記錄報表,//ctrlRPT_DATA_03
        一分鐘車道使用率及車間距報表,//ctrlRPT_DATA_04
        現點速率調查交通資料記錄報表,//ctrlRPT_DATA_06
        VD詳細記錄報表,//ctrlRPT_DATA_24
        VD總計記錄報表,//ctrlRPT_DATA_25
        小時交通資料記錄報表,//ctrlRPT_HDA_01
        小時交通流量統計報表依日期時段彙整,//ctrlRPT_HDA_03
        小時交通平均速度統計報表,//ctrlRPT_HDA_05
        匝道平均每日交通量統計報表,//ctrlRPT_HDA_11
    }

    public enum HasKind
    {
        //無主重報表,但有種類
        操作記錄報表,//ctrlRPT_OPR1_01
        現場終端設備運作記錄報表,//ctrlRPT_OPR2_06
        定時比對記錄報表,//ctrlRPT_OPR2_07
        現場終端設備狀態記錄報表//ctrlRPT_STA_01
    }

    public enum NonKind
    {
        //無主重報表
        主線平均每日交通量統計報表,//ctrlRPT_HDA_10
        全區匝道全日交通量統計報表,//ctrlRPT_HDA_12
        全區主線全日交通量統計報表,//ctrlRPT_HDA_13
        全區主線小時路段平均速度統計報表,//ctrlRPT_HDA_14
        資訊可變標誌即時資料報表,//ctrlRPT_MON_01
        設備狀態即時監視報表,//ctrlRPT_MON_07
        //每日定時路況新聞稿,
        路段壅塞狀況一分鐘報表,//ctrlRPT_HDA_35
    }
    internal enum myTable
    {
        tblDevice,
        tblList
    }
    internal enum myColumn
    {
        Devicename,
        RoadInfo1,
        RoadInfo2
    }

    internal static class Comm
    {
        static public XRControlStyle GetXRControlStyle()
        {
            XRControlStyle xrControlStyle = new DevExpress.XtraReports.UI.XRControlStyle();

            // 畫面初始化
            xrControlStyle.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(239)), ((System.Byte)(243)), ((System.Byte)(250)));
            xrControlStyle.BorderColor = System.Drawing.Color.FromArgb(((System.Byte)(199)), ((System.Byte)(209)), ((System.Byte)(228)));
            xrControlStyle.Borders = ((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right)
                | DevExpress.XtraPrinting.BorderSide.Bottom);
            xrControlStyle.Font = new System.Drawing.Font("標楷體", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(204)));
            xrControlStyle.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;

            return xrControlStyle;
        }

        #region ==== 建立關聯性 DataSet ====
        static public System.Data.DataSet BuildDataSet(bool isMainTable, System.Data.DataTable dt, System.Data.DataTable dtDev)
        {
            System.Data.DataSet ds = new System.Data.DataSet();
            if (isMainTable)
            {
                System.Data.DataTable mydt = new System.Data.DataTable();
                System.Data.DataTable mydtDev = new System.Data.DataTable();

                mydt = dt.Copy();
                mydt.TableName = myTable.tblList.ToString();

                mydtDev = dtDev.Copy();
                mydtDev.TableName = myTable.tblDevice.ToString();

                ds.Tables.Add(mydtDev); // 第一個 DataTable，是爸爸
                ds.Tables.Add(mydt);    // 第二個 DataTable，是孩子

                // 關聯性 DataSet
                System.Data.DataRelation Rel = new System.Data.DataRelation("REL", ds.Tables[myTable.tblDevice.ToString()].Columns[myColumn.Devicename.ToString()], ds.Tables[myTable.tblList.ToString()].Columns[myColumn.Devicename.ToString()]);
                ds.Relations.Add(Rel);
            }
            else
            {
                ds.Tables.Add(dt.Copy());
            }
            return ds;
        }
        #endregion ==== 建立關聯性 DataSet ====

        #region ==== 是否有主重報表 ====
        /// <summary>
        /// 是否有主重報表
        /// </summary>
        /// <param name="sReportName">報表名稱</param>
        /// <returns>true:是有主重報表</returns>
        static public bool IsMainTable(string sReportName)
        {
            System.Collections.Generic.List<string> reportNameList = new System.Collections.Generic.List<string>();

            reportNameList.AddRange(Enum.GetNames(typeof(HasMain)));
            foreach (string s in reportNameList)
            {
                if (sReportName == s) return true;
            }

            return false;
        }
        #endregion ==== 是否有主重報表 ====


        //xlblMemo  ChangeMemo
        static public string ChangeMemo(string RptName)
        {
            Hashtable Ht = new Hashtable();
            string[] ChgRpt = Enum.GetNames(typeof(RptHander));
            string[] EngRpt = Enum.GetNames(typeof(EngRptHander));
            for (int i = 0; i < ChgRpt.Length; i++)
            {
                Ht.Add(ChgRpt[i], EngRpt[i]);
            }

            return (string)Ht[RptName];
        }
    }
}
