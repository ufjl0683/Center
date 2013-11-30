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
    public partial class clsReport : DevExpress.XtraReports.UI.XtraReport
    {
        #region ==== 是否顯示時間Lable ====
        private bool isShowTime = true;
        public bool IsShowTime
        {
            set 
            {
                isShowTime = value;
                xlblTimeRange.Visible = isShowTime;
            }
            get { return isShowTime; }
        }
        #endregion ==== 是否顯示時間Lable ====

        public clsReport(string sReportName, Handler handler, MTable main, MTable minor, int titleCnt, ArrayList arrList, DataTable dt, DataTable dtDev, string sUser, string sDate, string eDate)
        {
            InitializeComponent();
            Initial(sReportName, sUser, sDate, eDate);
            bool isMainTable = Comm.IsMainTable(sReportName);
            int ReportColCnt = arrList.Count;
            xlblTimeRange.Visible = isShowTime;

            #region ==== 宣告設定等 ====
            DataSet ds = Comm.BuildDataSet(isMainTable, dt, dtDev);

            // 資料來源宣告
            this.DataSource = ds;
            if (isMainTable)
                this.DetailReport.DataMember = ds.Relations[0].ParentTable.TableName + "." + ds.Relations[0].RelationName;
            this.DetailReport.DataSource = ds;
            //this.DetailReport.PageBreak = DevExpress.XtraReports.UI.PageBreak.AfterBand;
            this.DetailReport.PageBreak = DevExpress.XtraReports.UI.PageBreak.BeforeBand;


            //XRBinding MyBinding;
            XRControlStyle xrControlStyle1 = Comm.GetXRControlStyle();
            XRControlStyle xrControlStyle2 = Comm.GetXRControlStyle();
            XRControlStyle xrControlStyle3 = Comm.GetXRControlStyle();

            this.StyleSheet.Add("EvenStyle", xrControlStyle1);
            this.StyleSheet.Add("OddStyle", xrControlStyle2);
            this.StyleSheet.Add("MasterStyle", xrControlStyle3);
            #endregion

            if (isMainTable)
            {
                #region ==== 標題上的資訊 ====
                // 主鍵
                this.xrTableDevCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
                        new DevExpress.XtraReports.UI.XRBinding("Text", ds, ds.Tables[myTable.tblDevice.ToString()].Columns[myColumn.Devicename.ToString()].ColumnName, "")});
                // 道路資訊
                this.xrTableRoadInfoCell1.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
                        new DevExpress.XtraReports.UI.XRBinding("Text", ds, ds.Tables[myTable.tblDevice.ToString()].Columns[myColumn.RoadInfo1.ToString()].ColumnName, "")});

                this.xrTableRoadInfoCell2.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
                        new DevExpress.XtraReports.UI.XRBinding("Text", ds, ds.Tables[myTable.tblDevice.ToString()].Columns[myColumn.RoadInfo2.ToString()].ColumnName, "")});
                #endregion
            }

            //參數宣告            
            int iWidth = this.PageWidth - 40;
            int iKey = -1;
            if (isMainTable) iKey = ds.Tables[myTable.tblList.ToString()].Columns[myColumn.Devicename.ToString().ToUpper()].Ordinal;


            //標題 
            handler.TitleCnt = titleCnt;
            handler.Width = iWidth;
            handler.NonShow = iKey;
            handler.ReportColCnt = ReportColCnt;
            XRTable MyTable = handler.GetHandler(arrList);
            this.ph.Controls.AddRange(new XRControl[] { MyTable });
            
            if (isMainTable)
            {
                //主報表 
                main.Width = iWidth;
                main.TitleCnt = titleCnt;
                main.NonShow = iKey;
                main.ReportColCnt = ReportColCnt;
                MyTable = main.GetXRTable(ds);
                // 將主報表隱藏，不秀出來
                MyTable.Visible = false;
                this.dtl.Controls.AddRange(new XRControl[] { MyTable });
                xlblDev.Visible = false;
                xlblDevice.Visible = true;
                xlblUser.Location = new Point(717, 58);
                xlblTime.Location = new Point(717, 83);
                dtl.Visible = true;
            }
            else
            {
                if (dtDev != null)
                {
                    xlblDevice.Visible = false;
                    xlblDev.Visible = true;
                    xlblDev.Text = string.Format("設備種類：{0}", dtDev.Rows[0][0].ToString());
                    dtl.Visible = false;
                    xlblUser.Location = new Point(717, 83);
                    xlblTime.Location = new Point(717, 108);
                }
                else
                {
                    xlblDevice.Visible = false;
                    xlblDev.Visible = false;
                    dtl.Visible = false;
                    xlblUser.Location = new Point(717, 83);
                    xlblTime.Location = new Point(717, 108);
                }
            }
            //副報表
            minor.Width = iWidth;
            minor.TitleCnt = titleCnt;
            minor.NonShow = iKey;
            minor.ReportColCnt = ReportColCnt;
            MyTable = minor.GetXRTable(ds);
            this.Detail.Controls.AddRange(new XRControl[] { MyTable });

            this.xlblMemo.Text = ReportForm.CommClass.Father.Comm.ChangeMemo(sReportName);

            this.Landscape = true;
            this.PaperKind = System.Drawing.Printing.PaperKind.A4;
            this.PrintingSystem.ShowPrintStatusDialog = false;      //取消列印的訊息視窗
            this.PrintingSystem.ShowMarginsWarning = false;
        }

        #region ==== 自己定義的初始化 ====
        private void Initial(string sReportName, string sUser, string sDate, string eDate)
        {
            //報表名稱
            this.xlblReportHeader.Text = sReportName;
            // 操作者
            this.xlblUser.Text = "操作者：" + sUser;
            // 列印日期
            this.xlblTime.Text = "列印日期：" + DateTime.Now.ToString();
            // 資料時間範圍
            this.xlblTimeRange.Text = "時間：" + sDate + " 至 " + eDate;
        }
        #endregion ==== 自己定義的初始化 ====

        #region ==== 變數宣告 ====
        private System.ComponentModel.Container components = null;

        private DetailBand dtl;
        private PageHeaderBand ph;
        private XRLabel xlblReportHeader;
        private TopMarginBand TopMargin;
        private XRLabel xlblTime;
        private XRLabel xlblUser;
        private XRPageInfo xrPageInfo;
        private XRLabel xrLabel1;
        private XRLabel xlblMemo;
        private XRLabel xlblTimeRange;
        private XRLabel xlblDevice;
        private DetailReportBand DetailReport;
        private DetailBand Detail;
        private XRTable xrTableDev;
        private XRTableRow xrTableRow1;
        private XRTableCell xrTableDevCell;
        private XRTable xrTableRoadInfo;
        private XRTableRow xrTableRow2;
        private XRTableCell xrTableRoadInfoCell1;
        private XRTableCell xrTableRoadInfoCell2;
        private XRLabel xlblDev;
        private PageFooterBand pf;
        #endregion ==== 變數宣告 ====

        #region ==== InitializeComponents 明細 ====
        private void InitializeComponent()
        {
            this.dtl = new DevExpress.XtraReports.UI.DetailBand();
            this.ph = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.xlblReportHeader = new DevExpress.XtraReports.UI.XRLabel();
            this.pf = new DevExpress.XtraReports.UI.PageFooterBand();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrPageInfo = new DevExpress.XtraReports.UI.XRPageInfo();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.xrTableRoadInfo = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow2 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableRoadInfoCell1 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableRoadInfoCell2 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableDev = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow1 = new DevExpress.XtraReports.UI.XRTableRow();
            this.xrTableDevCell = new DevExpress.XtraReports.UI.XRTableCell();
            this.xlblDevice = new DevExpress.XtraReports.UI.XRLabel();
            this.xlblTimeRange = new DevExpress.XtraReports.UI.XRLabel();
            this.xlblMemo = new DevExpress.XtraReports.UI.XRLabel();
            this.xlblUser = new DevExpress.XtraReports.UI.XRLabel();
            this.xlblTime = new DevExpress.XtraReports.UI.XRLabel();
            this.DetailReport = new DevExpress.XtraReports.UI.DetailReportBand();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.xlblDev = new DevExpress.XtraReports.UI.XRLabel();
            ((System.ComponentModel.ISupportInitialize)(this.xrTableRoadInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTableDev)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // dtl
            // 
            this.dtl.Font = new System.Drawing.Font("標楷體", 9.75F);
            this.dtl.Height = 0;
            this.dtl.Name = "dtl";
            this.dtl.ParentStyleUsing.UseFont = false;
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
            this.xlblReportHeader.Location = new System.Drawing.Point(250, 25);
            this.xlblReportHeader.Name = "xlblReportHeader";
            this.xlblReportHeader.ParentStyleUsing.UseBackColor = false;
            this.xlblReportHeader.ParentStyleUsing.UseBorderColor = false;
            this.xlblReportHeader.ParentStyleUsing.UseFont = false;
            this.xlblReportHeader.ParentStyleUsing.UseForeColor = false;
            this.xlblReportHeader.Size = new System.Drawing.Size(600, 33);
            this.xlblReportHeader.Text = "測試報表";
            this.xlblReportHeader.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // pf
            // 
            this.pf.Font = new System.Drawing.Font("標楷體", 9.75F);
            this.pf.Height = 158;
            this.pf.Name = "pf";
            this.pf.ParentStyleUsing.UseFont = false;
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
            this.xlblDev,
            this.xrTableRoadInfo,
            this.xrTableDev,
            this.xlblDevice,
            this.xlblTimeRange,
            this.xrPageInfo,
            this.xrLabel1,
            this.xlblMemo,
            this.xlblUser,
            this.xlblTime,
            this.xlblReportHeader});
            this.TopMargin.Font = new System.Drawing.Font("標楷體", 9.75F);
            this.TopMargin.Height = 142;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.ParentStyleUsing.UseFont = false;
            // 
            // xrTableRoadInfo
            // 
            this.xrTableRoadInfo.Location = new System.Drawing.Point(0, 108);
            this.xrTableRoadInfo.Name = "xrTableRoadInfo";
            this.xrTableRoadInfo.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow2});
            this.xrTableRoadInfo.Size = new System.Drawing.Size(408, 25);
            // 
            // xrTableRow2
            // 
            this.xrTableRow2.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableRoadInfoCell1,
            this.xrTableRoadInfoCell2});
            this.xrTableRow2.Name = "xrTableRow2";
            this.xrTableRow2.Size = new System.Drawing.Size(408, 25);
            // 
            // xrTableRoadInfoCell1
            // 
            this.xrTableRoadInfoCell1.Font = new System.Drawing.Font("標楷體", 9.75F);
            this.xrTableRoadInfoCell1.ForeColor = System.Drawing.SystemColors.Desktop;
            this.xrTableRoadInfoCell1.Location = new System.Drawing.Point(0, 0);
            this.xrTableRoadInfoCell1.Name = "xrTableRoadInfoCell1";
            this.xrTableRoadInfoCell1.ParentStyleUsing.UseFont = false;
            this.xrTableRoadInfoCell1.ParentStyleUsing.UseForeColor = false;
            this.xrTableRoadInfoCell1.Size = new System.Drawing.Size(208, 25);
            this.xrTableRoadInfoCell1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrTableRoadInfoCell2
            // 
            this.xrTableRoadInfoCell2.Font = new System.Drawing.Font("標楷體", 9.75F);
            this.xrTableRoadInfoCell2.ForeColor = System.Drawing.SystemColors.Desktop;
            this.xrTableRoadInfoCell2.Location = new System.Drawing.Point(208, 0);
            this.xrTableRoadInfoCell2.Name = "xrTableRoadInfoCell2";
            this.xrTableRoadInfoCell2.ParentStyleUsing.UseFont = false;
            this.xrTableRoadInfoCell2.ParentStyleUsing.UseForeColor = false;
            this.xrTableRoadInfoCell2.Size = new System.Drawing.Size(200, 25);
            this.xrTableRoadInfoCell2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrTableDev
            // 
            this.xrTableDev.Location = new System.Drawing.Point(808, 108);
            this.xrTableDev.Name = "xrTableDev";
            this.xrTableDev.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow1});
            this.xrTableDev.Size = new System.Drawing.Size(159, 25);
            this.xrTableDev.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrTableRow1
            // 
            this.xrTableRow1.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.xrTableDevCell});
            this.xrTableRow1.Name = "xrTableRow1";
            this.xrTableRow1.Size = new System.Drawing.Size(159, 25);
            // 
            // xrTableDevCell
            // 
            this.xrTableDevCell.Font = new System.Drawing.Font("標楷體", 9.75F);
            this.xrTableDevCell.ForeColor = System.Drawing.SystemColors.Desktop;
            this.xrTableDevCell.Location = new System.Drawing.Point(0, 0);
            this.xrTableDevCell.Name = "xrTableDevCell";
            this.xrTableDevCell.ParentStyleUsing.UseFont = false;
            this.xrTableDevCell.ParentStyleUsing.UseForeColor = false;
            this.xrTableDevCell.Size = new System.Drawing.Size(159, 25);
            this.xrTableDevCell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xlblDevice
            // 
            this.xlblDevice.Font = new System.Drawing.Font("標楷體", 9.75F);
            this.xlblDevice.ForeColor = System.Drawing.SystemColors.Desktop;
            this.xlblDevice.Location = new System.Drawing.Point(717, 108);
            this.xlblDevice.Name = "xlblDevice";
            this.xlblDevice.ParentStyleUsing.UseFont = false;
            this.xlblDevice.ParentStyleUsing.UseForeColor = false;
            this.xlblDevice.Size = new System.Drawing.Size(95, 25);
            this.xlblDevice.Text = "偵測器編號：";
            this.xlblDevice.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xlblTimeRange
            // 
            this.xlblTimeRange.Font = new System.Drawing.Font("標楷體", 9.75F);
            this.xlblTimeRange.ForeColor = System.Drawing.SystemColors.Desktop;
            this.xlblTimeRange.Location = new System.Drawing.Point(0, 83);
            this.xlblTimeRange.Name = "xlblTimeRange";
            this.xlblTimeRange.ParentStyleUsing.UseFont = false;
            this.xlblTimeRange.ParentStyleUsing.UseForeColor = false;
            this.xlblTimeRange.Size = new System.Drawing.Size(408, 25);
            this.xlblTimeRange.Text = "時間：";
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
            // xlblUser
            // 
            this.xlblUser.BackColor = System.Drawing.Color.Empty;
            this.xlblUser.Font = new System.Drawing.Font("標楷體", 9.75F);
            this.xlblUser.ForeColor = System.Drawing.SystemColors.Desktop;
            this.xlblUser.Location = new System.Drawing.Point(717, 58);
            this.xlblUser.Name = "xlblUser";
            this.xlblUser.ParentStyleUsing.UseBackColor = false;
            this.xlblUser.ParentStyleUsing.UseFont = false;
            this.xlblUser.ParentStyleUsing.UseForeColor = false;
            this.xlblUser.Size = new System.Drawing.Size(249, 25);
            this.xlblUser.Text = "操作者：";
            this.xlblUser.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xlblTime
            // 
            this.xlblTime.BackColor = System.Drawing.Color.Empty;
            this.xlblTime.Font = new System.Drawing.Font("標楷體", 9.75F);
            this.xlblTime.ForeColor = System.Drawing.SystemColors.Desktop;
            this.xlblTime.Location = new System.Drawing.Point(717, 83);
            this.xlblTime.Name = "xlblTime";
            this.xlblTime.ParentStyleUsing.UseBackColor = false;
            this.xlblTime.ParentStyleUsing.UseFont = false;
            this.xlblTime.ParentStyleUsing.UseForeColor = false;
            this.xlblTime.Size = new System.Drawing.Size(249, 25);
            this.xlblTime.Text = "列印日期：";
            this.xlblTime.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // DetailReport
            // 
            this.DetailReport.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail});
            this.DetailReport.Font = new System.Drawing.Font("標楷體", 9.75F);
            this.DetailReport.Name = "DetailReport";
            this.DetailReport.ParentStyleUsing.UseFont = false;
            // 
            // Detail
            // 
            this.Detail.Font = new System.Drawing.Font("標楷體", 9.75F);
            this.Detail.Height = 0;
            this.Detail.Name = "Detail";
            this.Detail.ParentStyleUsing.UseFont = false;
            // 
            // xlblDev
            // 
            this.xlblDev.Font = new System.Drawing.Font("標楷體", 9.75F);
            this.xlblDev.ForeColor = System.Drawing.SystemColors.Desktop;
            this.xlblDev.Location = new System.Drawing.Point(0, 108);
            this.xlblDev.Name = "xlblDev";
            this.xlblDev.ParentStyleUsing.UseFont = false;
            this.xlblDev.ParentStyleUsing.UseForeColor = false;
            this.xlblDev.Size = new System.Drawing.Size(408, 25);
            this.xlblDev.Text = "設備種類：";
            this.xlblDev.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // clsReport
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.dtl,
            this.ph,
            this.pf,
            this.TopMargin,
            this.DetailReport});
            this.Margins = new System.Drawing.Printing.Margins(20, 20, 142, 75);
            this.PageHeight = 827;
            this.PageWidth = 1169;
            this.PaperKind = System.Drawing.Printing.PaperKind.A4Rotated;
            ((System.ComponentModel.ISupportInitialize)(this.xrTableRoadInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTableDev)).EndInit();
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
