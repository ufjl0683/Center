using System;
using System.Collections;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using ReportForm.CommClass.Father;

namespace ReportForm.CommClass.Minor
{
    public class NonKindMinor : ReportForm.CommClass.Father.MTable
    {
        #region ==== 建構元 ====
        public NonKindMinor()
        {
           
        }

        public NonKindMinor(System.Collections.Generic.List<int> percentList)
        {
            this.perList = percentList;          
        }

        public NonKindMinor(int width, int height, int titleCnt, int nonShow, int reportColCnt, System.Collections.Generic.List<int> percentList)
        {
            Width = width;
            Height = height;
            TitleCnt = titleCnt;
            NonShow = nonShow;
            ReportColCnt = reportColCnt;
            this.perList = percentList;
        }
        #endregion ==== 建構元 ====

        #region ==== 方法 ====
        override public XRTable GetXRTable(System.Data.DataSet ds)
        {
            cellWidth = GetCellWidth(perList, ReportColCnt);
            XRTable MyTable = new XRTable();
            MyTable.SuspendLayout();
            MyTable.Width = Width;

            XRTableRow MyTableRow = new XRTableRow();
            MyTableRow.SuspendLayout();
            MyTableRow.Width = Width;

            //報表欄位內容
            for (int b = 0; b < ReportColCnt; b++)
            {
                int CellWidth = cellWidth[b];
                XRTableCell MyTableCell = new XRTableCell();
                MyTableCell.SuspendLayout();
                MyTableCell.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
                  new DevExpress.XtraReports.UI.XRBinding("Text", ds, ds.Tables[0].Columns[b].ColumnName, "")});
                MyTableCell.Width = CellWidth;
                MyTableCell.PerformLayout();
                MyTableRow.Cells.Add(MyTableCell);
            }

            MyTable.Borders = DevExpress.XtraPrinting.BorderSide.Bottom;
            MyTable.Dock = DevExpress.XtraReports.UI.XRDockStyle.Fill;
            MyTable.Font = new System.Drawing.Font("標楷體", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(204)));
            MyTable.Location = new System.Drawing.Point(0, 0);
            MyTable.ParentStyleUsing.UseBorders = false;
            MyTable.ParentStyleUsing.UseFont = false;
            MyTableRow.PerformLayout();
            MyTable.Rows.AddRange(new XRTableRow[] { MyTableRow });
            MyTable.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            //MyTableRow.EvenStyleName = "EvenStyle";
            //MyTableRow.OddStyleName = "OddStyle";
            MyTableRow.StyleName = "OddStyle";
            MyTable.PerformLayout();

            return MyTable;
        }

        #endregion ==== 方法 ====
    }
}
