using System;
using System.Collections;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using ReportForm.CommClass.Father;

namespace ReportForm.CommClass.Main
{
    public class VDReportMaim :MTable
    {
        #region ==== �غc�� ====
        public VDReportMaim()
        {
        }

        public VDReportMaim(int width, int height, int titleCnt, int nonShow, int reportColCnt)
        {
            Width = width;
            Height = height;
            TitleCnt = titleCnt;
            NonShow = nonShow;
            ReportColCnt = reportColCnt;
        }
        #endregion ==== �غc�� ====

        #region ==== ��k ====
        override public XRTable GetXRTable(System.Data.DataSet ds)
        {
            int CellWidth = Width / (TitleCnt - 1);

            XRTable MyTable = new XRTable();
            MyTable.SuspendLayout();
            MyTable.Width = Width;

            XRTableRow MyTableRow = new XRTableRow();
            MyTableRow.SuspendLayout();
            MyTableRow.Width = Width;

            // ���J�ƭ�
            XRTableCell MyTableCell = new XRTableCell();
            MyTableCell.SuspendLayout();

            MyTableCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", ds, ds.Tables[myTable.tblDevice.ToString()].Columns[myColumn.Devicename.ToString()].ColumnName, "")});

            MyTableCell.Width = CellWidth;
            MyTableCell.PerformLayout();
            MyTableRow.Cells.Add(MyTableCell);

            MyTable.Borders = DevExpress.XtraPrinting.BorderSide.Bottom;
            MyTable.Dock = DevExpress.XtraReports.UI.XRDockStyle.Fill;
            MyTable.Font = new System.Drawing.Font("�з���", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(204)));
            MyTable.Location = new System.Drawing.Point(0, 0);
            MyTable.ParentStyleUsing.UseBorders = false;
            MyTable.ParentStyleUsing.UseFont = false;
            MyTableRow.PerformLayout();
            MyTable.Rows.AddRange(new XRTableRow[] { MyTableRow });
            MyTable.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            //MyTableRow.EvenStyleName = "EvenStyle";
            //MyTableRow.OddStyleName = "OddStyle";
            MyTableRow.StyleName = "MasterStyle";

            MyTable.PerformLayout();

            return MyTable;
        }
        #endregion ==== ��k ====
    }
}
