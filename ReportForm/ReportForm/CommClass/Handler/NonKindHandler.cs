using System;
using System.Collections;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using ReportForm.CommClass.Father;

namespace ReportForm.CommClass.Handler
{
    public class NonKindHandler:ReportForm.CommClass.Father.Handler
    {
        #region ==== 建構元 ====
        public NonKindHandler()
        {
            
        }

        public NonKindHandler(System.Collections.Generic.List<int> percentList)
        {
            this.perList = percentList;          
        }

        public NonKindHandler(int width, int height, int titleCnt, int nonShow, int reportColCnt, System.Collections.Generic.List<int> percentList)
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
        override public XRTable GetHandler(ArrayList arrList)
        {
            XRTable MyTable = new XRTable();
            XRTableRow MyTableRow = new XRTableRow();
            MyTable.SuspendLayout();
            MyTable.Width = Width;
            cellWidth = GetCellWidth(perList, ReportColCnt);

            for (int a = 0; a < TitleCnt; a++)
            {
                MyTableRow = new XRTableRow();
                string[] ReportTitle;

                MyTableRow.SuspendLayout();
                MyTableRow.Width = Width;

                for (int i = 0; i < ReportColCnt; i++)
                {
                    int CellWidth = cellWidth[i];

                    // 宣告及定義 MyTableCell 的格式
                    XRTableCell MyTableCell = new XRTableCell();
                    MyTableCell.SuspendLayout();

                    string text = arrList[i].ToString();
                    ReportTitle = text.ToString().Split(',');

                    //MyTableCell.Name = arrList[i].ToString().Substring(0, arrList[i].ToString().IndexOf(","));
                    //MyTableCell.Text = arrList[i].ToString().Substring(arrList[i].ToString().IndexOf(",") + 1);
                    MyTableCell.Text = ReportTitle[a].ToString();

                    MyTableCell.Width = CellWidth;
                    MyTableCell.PerformLayout();
                    MyTableRow.Cells.Add(MyTableCell);
                }

                MyTableRow.PerformLayout();
                MyTable.Rows.AddRange(new XRTableRow[] { MyTableRow });
            }

            MyTable.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(201)), ((System.Byte)(214)), ((System.Byte)(237)));
            //MyTable.BorderColor = System.Drawing.Color.FromArgb(((System.Byte)(175)), ((System.Byte)(190)), ((System.Byte)(216)));
            //MyTable.Borders = DevExpress.XtraPrinting.BorderSide.All;
            MyTable.Dock = DevExpress.XtraReports.UI.XRDockStyle.Fill;
            MyTable.Font = new System.Drawing.Font("標楷體", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(204)));
            MyTable.Location = new System.Drawing.Point(0, 0);
            MyTable.ParentStyleUsing.UseBorders = false;
            MyTable.ParentStyleUsing.UseFont = false;
            MyTable.Rows.AddRange(new XRTableRow[] { MyTableRow });
            //MyTable.Size = new System.Drawing.Size(this.PageWidth - 40, this.ph.Height);
            MyTable.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            MyTableRow.EvenStyleName = "EvenStyle";
            MyTableRow.OddStyleName = "OddStyle";

            MyTable.PerformLayout();

            return MyTable;
        }
        #endregion ==== 方法 ====
    }
}
