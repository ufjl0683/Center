/********************************************************************************
 * -----    ----------   ----------  ------------------------------------------ *
 * 版次     修改日期     修改者      修改內容                                   *
 * -----    ----------   ----------  ------------------------------------------ *
 * v0.1     2009.11.12   林俊傑      clsReport_RPT_DATA_06 Initial              *
 * v1.1     2010.01.11   鄭元森      clsReport_RPT_DATA_06 Initial              *
 *                                                                              *
 ********************************************************************************/

using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrintingLinks;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using ReportForm.CommClass.Father;

namespace ReportForm
{
    public partial class clsNewsPaper: DevExpress.XtraReports.UI.XtraReport
    {
        //#region ==== 是否顯示時間Lable ====
        //private bool isShowTime = true;
        //public bool IsShowTime
        //{
        //    set 
        //    {
        //        isShowTime = value;
        //        xlblTimeRange.Visible = isShowTime;
        //    }
        //    get { return isShowTime; }
        //}
        //#endregion ==== 是否顯示時間Lable ====

        public clsNewsPaper(string sReportName,  string Section, DataTable dtDev, DateTime DateTime)
        {
            InitializeComponent();
            Initial();

            xrTableCell5.Text = Section;
            xrTableCell1.Text = "日期:" + DateTime.Date.ToString("yyyy-MM-dd");
            xrTableCell2.Text = "時間:" + DateTime.Hour.ToString() + ":" + DateTime.Minute.ToString();


            //bool isMainTable = Comm.IsMainTable(sReportName);
            //int ReportColCnt = arrList.Count;
            //xlblTimeRange.Visible = isShowTime;

            //#region ==== 宣告設定等 ====
            //DataSet ds = Comm.BuildDataSet(isMainTable, dt, dtDev);

            //// 資料來源宣告
            //this.DataSource = ds;
            //if (isMainTable)
            //    this.DetailReport.DataMember = ds.Relations[0].ParentTable.TableName + "." + ds.Relations[0].RelationName;
            //this.DetailReport.DataSource = ds;
            ////this.DetailReport.PageBreak = DevExpress.XtraReports.UI.PageBreak.AfterBand;
            //this.DetailReport.PageBreak = DevExpress.XtraReports.UI.PageBreak.BeforeBand;


            ////XRBinding MyBinding;
            //XRControlStyle xrControlStyle1 = Comm.GetXRControlStyle();
            //XRControlStyle xrControlStyle2 = Comm.GetXRControlStyle();
            //XRControlStyle xrControlStyle3 = Comm.GetXRControlStyle();

            //this.StyleSheet.Add("EvenStyle", xrControlStyle1);
            //this.StyleSheet.Add("OddStyle", xrControlStyle2);
            //this.StyleSheet.Add("MasterStyle", xrControlStyle3);
            //#endregion

            //if (isMainTable)
            //{
            //    #region ==== 標題上的資訊 ====
            //    // 主鍵
            //    //this.xrTableDevCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            //    //        new DevExpress.XtraReports.UI.XRBinding("Text", ds, ds.Tables[myTable.tblDevice.ToString()].Columns[myColumn.Devicename.ToString()].ColumnName, "")});
            //    //// 道路資訊
            //    //this.xrTableRoadInfoCell1.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            //    //        new DevExpress.XtraReports.UI.XRBinding("Text", ds, ds.Tables[myTable.tblDevice.ToString()].Columns[myColumn.RoadInfo1.ToString()].ColumnName, "")});

            //    //this.xrTableRoadInfoCell2.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            //    //        new DevExpress.XtraReports.UI.XRBinding("Text", ds, ds.Tables[myTable.tblDevice.ToString()].Columns[myColumn.RoadInfo2.ToString()].ColumnName, "")});
            //    #endregion
            //}

            ////參數宣告            
            //int iWidth = this.PageWidth - 40;
            //int iKey = -1;
            //if (isMainTable) iKey = ds.Tables[myTable.tblList.ToString()].Columns[myColumn.Devicename.ToString().ToUpper()].Ordinal;


            ////標題 
            //handler.TitleCnt = titleCnt;
            //handler.Width = iWidth;
            //handler.NonShow = iKey;
            //handler.ReportColCnt = ReportColCnt;
            //XRTable MyTable = handler.GetHandler(arrList);
            //this.ph.Controls.AddRange(new XRControl[] { MyTable });
            
            //if (isMainTable)
            //{
            //    //主報表 
            //    main.Width = iWidth;
            //    main.TitleCnt = titleCnt;
            //    main.NonShow = iKey;
            //    main.ReportColCnt = ReportColCnt;
            //    MyTable = main.GetXRTable(ds);
            //    // 將主報表隱藏，不秀出來
            //    MyTable.Visible = false;
            //    this.dtl.Controls.AddRange(new XRControl[] { MyTable });
            //    xlblDev.Visible = false;
            //    xlblDevice.Visible = true;
            //    xlblUser.Location = new Point(717, 58);
            //    xlblTime.Location = new Point(717, 83);
            //    dtl.Visible = true;
            //}
            //else
            //{
            //    if (dtDev != null)
            //    {
            //        xlblDevice.Visible = false;
            //        xlblDev.Visible = true;
            //        xlblDev.Text = string.Format("設備種類：{0}", dtDev.Rows[0][0].ToString());
            //        dtl.Visible = false;
            //        xlblUser.Location = new Point(717, 83);
            //        xlblTime.Location = new Point(717, 108);
            //    }
            //    else
            //    {
            //        xlblDevice.Visible = false;
            //        xlblDev.Visible = false;
            //        dtl.Visible = false;
            //        xlblUser.Location = new Point(717, 83);
            //        xlblTime.Location = new Point(717, 108);
            //    }
            //}
            ////副報表
            //minor.Width = iWidth;
            //minor.TitleCnt = titleCnt;
            //minor.NonShow = iKey;
            //minor.ReportColCnt = ReportColCnt;
            //MyTable = minor.GetXRTable(ds);
            //this.Detail.Controls.AddRange(new XRControl[] { MyTable });

            //this.xlblMemo.Text = ReportForm.CommClass.Father.Comm.ChangeMemo(sReportName);

            //this.Landscape = true;
            //this.PaperKind = System.Drawing.Printing.PaperKind.A4;
            //this.PrintingSystem.ShowPrintStatusDialog = false;      //取消列印的訊息視窗
            //this.PrintingSystem.ShowMarginsWarning = false;
        }

        #region ==== 自己定義的初始化 ====
        private void Initial()
        {
            //this.xrTableCell5.Size = new System.Drawing.Size(742, 100);
            //this.xrTableCell4.Size = new System.Drawing.Size(742, 200);
        }
        #endregion ==== 自己定義的初始化 ====

        #region ==== 變數宣告 ====
        private System.ComponentModel.Container components = null;

        private DetailBand dtl;
        private PageHeaderBand ph;
        private XRLabel xlblReportHeader;
        private TopMarginBand TopMargin;
        private XRPageInfo xrPageInfo;
        private XRLabel xrLabel1;
        private XRLabel xlblMemo;
        private XRLabel xlblTimeRange;
        private XRTable xrTable1;
        private XRTableRow xrTableRow1;
        private XRTableCell xrTableCell1;
        private XRTableCell xrTableCell2;
        private XRTableRow xrTableRow2;
        private XRTableCell xrTableCell3;
        private XRTableRow xrTableRow3;
        private XRTableCell xrTableCell5;
        private XRTableRow xrTableRow4;
        private XRTableCell xrTableCell4;
        private XRTableRow xrTableRow5;
        private XRTableRow xrTableRow6;
        private XRTableCell xrTableCell7;
        private XRTableCell xrTableCell6;
        #endregion ==== 變數宣告 ====

        #region ==== InitializeComponents 明細 ====
        private void InitializeComponent()
        {
            this.dtl = new DevExpress.XtraReports.UI.DetailBand();
            this.xrTable1 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow1 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell1 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell2 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow2 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell3 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow3 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell5 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow4 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell4 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow6 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell7 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRow5 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableCell6 = new DevExpress.XtraReports.UI.XRTableCell();
            this.ph = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.xlblReportHeader = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrPageInfo = new DevExpress.XtraReports.UI.XRPageInfo();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.xlblTimeRange = new DevExpress.XtraReports.UI.XRLabel();
            this.xlblMemo = new DevExpress.XtraReports.UI.XRLabel();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // dtl
            // 
            this.dtl.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable1});
            this.dtl.Font = new System.Drawing.Font("標楷體", 9.75F);
            this.dtl.Height = 744;
            this.dtl.Name = "dtl";
            this.dtl.ParentStyleUsing.UseFont = false;
            // 
            // xrTable1
            // 
            this.xrTable1.BorderColor = System.Drawing.SystemColors.WindowText;
            this.xrTable1.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top)
                        | DevExpress.XtraPrinting.BorderSide.Right)
                        | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrTable1.Location = new System.Drawing.Point(17, 17);
            this.xrTable1.Name = "xrTable1";
            this.xrTable1.ParentStyleUsing.UseBackColor = false;
            this.xrTable1.ParentStyleUsing.UseBorderColor = false;
            this.xrTable1.ParentStyleUsing.UseBorders = false;
            this.xrTable1.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow1,
            this.xrTableRow2,
            this.xrTableRow3,
            this.xrTableRow4,
            this.xrTableRow6,
            this.xrTableRow5});
            this.xrTable1.Size = new System.Drawing.Size(758, 727);
            // 
            // xrTableRow1
            // 
            this.xrTableRow1.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell1,
            this.xrTableCell2});
            this.xrTableRow1.Name = "xrTableRow1";
            this.xrTableRow1.Size = new System.Drawing.Size(758, 30);
            // 
            // xrTableCell1
            // 
            this.xrTableCell1.Font = new System.Drawing.Font("標楷體", 12F);
            this.xrTableCell1.Location = new System.Drawing.Point(0, 0);
            this.xrTableCell1.Name = "xrTableCell1";
            this.xrTableCell1.ParentStyleUsing.UseFont = false;
            this.xrTableCell1.Size = new System.Drawing.Size(379, 30);
            this.xrTableCell1.Text = "日期:";
            // 
            // xrTableCell2
            // 
            this.xrTableCell2.Font = new System.Drawing.Font("標楷體", 12F);
            this.xrTableCell2.Location = new System.Drawing.Point(379, 0);
            this.xrTableCell2.Name = "xrTableCell2";
            this.xrTableCell2.ParentStyleUsing.UseFont = false;
            this.xrTableCell2.Size = new System.Drawing.Size(379, 30);
            this.xrTableCell2.Text = "時間:";
            // 
            // xrTableRow2
            // 
            this.xrTableRow2.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell3});
            this.xrTableRow2.Name = "xrTableRow2";
            this.xrTableRow2.Size = new System.Drawing.Size(758, 63);
            // 
            // xrTableCell3
            // 
            this.xrTableCell3.Font = new System.Drawing.Font("標楷體", 12F);
            this.xrTableCell3.Location = new System.Drawing.Point(0, 0);
            this.xrTableCell3.Multiline = true;
            this.xrTableCell3.Name = "xrTableCell3";
            this.xrTableCell3.ParentStyleUsing.UseFont = false;
            this.xrTableCell3.Size = new System.Drawing.Size(758, 63);
            this.xrTableCell3.Text = "播報內容:\r\n1.事故";
            // 
            // xrTableRow3
            // 
            this.xrTableRow3.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell5});
            this.xrTableRow3.Name = "xrTableRow3";
            this.xrTableRow3.Size = new System.Drawing.Size(758, 84);
            // 
            // xrTableCell5
            // 
            this.xrTableCell5.Font = new System.Drawing.Font("標楷體", 12F);
            this.xrTableCell5.Location = new System.Drawing.Point(0, 0);
            this.xrTableCell5.Multiline = true;
            this.xrTableCell5.Name = "xrTableCell5";
            this.xrTableCell5.ParentStyleUsing.UseFont = false;
            this.xrTableCell5.Size = new System.Drawing.Size(758, 84);
            this.xrTableCell5.Text = "\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n";
            // 
            // xrTableRow4
            // 
            this.xrTableRow4.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell4});
            this.xrTableRow4.Name = "xrTableRow4";
            this.xrTableRow4.Size = new System.Drawing.Size(758, 398);
            // 
            // xrTableCell4
            // 
            this.xrTableCell4.Font = new System.Drawing.Font("標楷體", 12F);
            this.xrTableCell4.Location = new System.Drawing.Point(0, 0);
            this.xrTableCell4.Name = "xrTableCell4";
            this.xrTableCell4.ParentStyleUsing.UseFont = false;
            this.xrTableCell4.Size = new System.Drawing.Size(758, 300);
            this.xrTableCell4.Text = "3.施工(國道1號 國道3號 國道4號 國道6號)";
            // 
            // xrTableRow6
            // 
            this.xrTableRow6.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell7});
            this.xrTableRow6.Name = "xrTableRow6";
            this.xrTableRow6.Size = new System.Drawing.Size(758, 76);
            // 
            // xrTableCell7
            // 
            this.xrTableCell7.Font = new System.Drawing.Font("標楷體", 12F);
            this.xrTableCell7.Location = new System.Drawing.Point(0, 0);
            this.xrTableCell7.Multiline = true;
            this.xrTableCell7.Name = "xrTableCell7";
            this.xrTableCell7.ParentStyleUsing.UseFont = false;
            this.xrTableCell7.Size = new System.Drawing.Size(758, 76);
            this.xrTableCell7.Text = "4.其他相關宣導\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n";
            // 
            // xrTableRow5
            // 
            this.xrTableRow5.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableCell6});
            this.xrTableRow5.Name = "xrTableRow5";
            this.xrTableRow5.Size = new System.Drawing.Size(758, 76);
            // 
            // xrTableCell6
            // 
            this.xrTableCell6.Font = new System.Drawing.Font("標楷體", 12F);
            this.xrTableCell6.Location = new System.Drawing.Point(0, 0);
            this.xrTableCell6.Multiline = true;
            this.xrTableCell6.Name = "xrTableCell6";
            this.xrTableCell6.ParentStyleUsing.UseFont = false;
            this.xrTableCell6.Size = new System.Drawing.Size(758, 76);
            this.xrTableCell6.Text = "以上路況 資訊，由高速公路局中區工程處交控中心提供，祝您行車平安。";
            // 
            // ph
            // 
            this.ph.Font = new System.Drawing.Font("標楷體", 9.75F);
            this.ph.Height = 0;
            this.ph.Name = "ph";
            this.ph.ParentStyleUsing.UseFont = false;
            // 
            // xlblReportHeader
            // 
            this.xlblReportHeader.BackColor = System.Drawing.Color.Empty;
            this.xlblReportHeader.BorderColor = System.Drawing.SystemColors.Control;
            this.xlblReportHeader.Font = new System.Drawing.Font("標楷體", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xlblReportHeader.ForeColor = System.Drawing.SystemColors.Desktop;
            this.xlblReportHeader.Location = new System.Drawing.Point(83, 42);
            this.xlblReportHeader.Name = "xlblReportHeader";
            this.xlblReportHeader.ParentStyleUsing.UseBackColor = false;
            this.xlblReportHeader.ParentStyleUsing.UseBorderColor = false;
            this.xlblReportHeader.ParentStyleUsing.UseFont = false;
            this.xlblReportHeader.ParentStyleUsing.UseForeColor = false;
            this.xlblReportHeader.Size = new System.Drawing.Size(659, 33);
            this.xlblReportHeader.Text = "交通部台灣區國道高速公路中區工程處交控中心";
            this.xlblReportHeader.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLabel1
            // 
            this.xrLabel1.Font = new System.Drawing.Font("標楷體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel1.ForeColor = System.Drawing.SystemColors.Desktop;
            this.xrLabel1.Location = new System.Drawing.Point(967, 108);
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.ParentStyleUsing.UseFont = false;
            this.xrLabel1.ParentStyleUsing.UseForeColor = false;
            this.xrLabel1.Size = new System.Drawing.Size(108, 25);
            this.xrLabel1.Text = "頁次：";
            this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            // 
            // xrPageInfo
            // 
            this.xrPageInfo.Font = new System.Drawing.Font("標楷體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrPageInfo.ForeColor = System.Drawing.SystemColors.Desktop;
            this.xrPageInfo.Location = new System.Drawing.Point(1075, 108);
            this.xrPageInfo.Name = "xrPageInfo";
            this.xrPageInfo.ParentStyleUsing.UseFont = false;
            this.xrPageInfo.ParentStyleUsing.UseForeColor = false;
            this.xrPageInfo.Size = new System.Drawing.Size(50, 25);
            this.xrPageInfo.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // TopMargin
            // 
            this.TopMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xlblTimeRange,
            this.xrPageInfo,
            this.xrLabel1,
            this.xlblMemo,
            this.xlblReportHeader});
            this.TopMargin.Font = new System.Drawing.Font("標楷體", 9.75F);
            this.TopMargin.Height = 142;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.ParentStyleUsing.UseFont = false;
            // 
            // xlblTimeRange
            // 
            this.xlblTimeRange.Font = new System.Drawing.Font("標楷體", 14F);
            this.xlblTimeRange.ForeColor = System.Drawing.SystemColors.Desktop;
            this.xlblTimeRange.Location = new System.Drawing.Point(175, 92);
            this.xlblTimeRange.Name = "xlblTimeRange";
            this.xlblTimeRange.ParentStyleUsing.UseFont = false;
            this.xlblTimeRange.ParentStyleUsing.UseForeColor = false;
            this.xlblTimeRange.Size = new System.Drawing.Size(408, 25);
            this.xlblTimeRange.Text = "         每日定時路況簡報新聞稿草稿";
            this.xlblTimeRange.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xlblMemo
            // 
            this.xlblMemo.Font = new System.Drawing.Font("標楷體", 9.75F);
            this.xlblMemo.ForeColor = System.Drawing.SystemColors.Desktop;
            this.xlblMemo.Location = new System.Drawing.Point(967, 83);
            this.xlblMemo.Name = "xlblMemo";
            this.xlblMemo.ParentStyleUsing.UseFont = false;
            this.xlblMemo.ParentStyleUsing.UseForeColor = false;
            this.xlblMemo.Size = new System.Drawing.Size(158, 25);
            this.xlblMemo.Text = "中區-RPT_DATA_06";
            this.xlblMemo.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            // 
            // clsNewsPaper
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.dtl,
            this.ph,
            this.TopMargin});
            this.Margins = new System.Drawing.Printing.Margins(20, 20, 142, 75);
            this.PageHeight = 1169;
            this.PageWidth = 827;
            this.PaperKind = System.Drawing.Printing.PaperKind.A4;
            ((System.ComponentModel.ISupportInitialize)(this.xrTable1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion ==== InitializeComponents 明細 ====

        #region ==== Dispose 明細 ====
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        #endregion ==== Dispose 明細 ====
    }


    partial class clsReport
    {        
 
    }
}
