using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
namespace temp
{
    class Util
    {

        public static string CPath(string WinPath)
        {


            if (System.Environment.OSVersion.Platform == PlatformID.Win32NT)
                return WinPath;
            else
            {
              //  Console.WriteLine("Unix");
                return WinPath.Replace(@"\", @"/");
            }
        }

        public static System.Data.DataView get_travel_sections_detail_data(uint from_milepost, uint end_milepost, string freewayId,string direction)
        {
            System.Data.DataView vw = null;
            if (direction == "S")
                vw = new DataView(temp.Program.Curr5minSecDs.tblSecTrafficData,
               string.Format("from_milepost>={0} and  from_milepost < {1} and freewayId='{2}' and directionId='S'", from_milepost-500, end_milepost-500, freewayId)
               , "", DataViewRowState.CurrentRows);
            else if (direction == "N")
                vw = new DataView(temp.Program.Curr5minSecDs.tblSecTrafficData,
                string.Format("from_milepost<={0} and  from_milepost > {1} and freewayId='{2}' and directionId='N'", from_milepost+500,end_milepost+500, freewayId)
            , "", DataViewRowState.CurrentRows);

            return vw;
        }


        public static string ToColorString(System.Drawing.Color[] colors)
        {
            string ret="";
            for (int i = 0; i < colors.Length; i++)
            {
                ret+=colors[i].R.ToString()+","+colors[i].G.ToString()+","+colors[i].B.ToString()+",";
            }
            return ret.TrimEnd(new char[]{','});

        }

        public static System.Drawing.Color[] ToColors(string colorstr)
        {
            string[] colorStr = colorstr.Split(new char[] { ',' });
            System.Drawing.Color[] colors = new System.Drawing.Color[colorStr.Length/3];
            for (int i = 0; i < colors.Length; i++)

                colors[i] = System.Drawing.Color.FromArgb(System.Convert.ToInt32(colorStr[i * 3]), System.Convert.ToInt32(colorStr[i * 3 + 1]), System.Convert.ToInt32(colorStr[i * 3] + 2));


            return colors;
        }

        public static System.Data.DataTable getPureDataTable(System.Data.DataTable srctbl)
        {
            System.Data.DataTable retTable = new DataTable(srctbl.TableName);

            foreach (System.Data.DataColumn c in srctbl.Columns)
            {
                System.Data.DataColumn col = retTable.Columns.Add(c.ColumnName);
                col.DataType = c.DataType;

            }

            foreach (System.Data.DataRow r in srctbl.Rows)
            {
                System.Data.DataRow row = retTable.NewRow();
                for (int i = 0; i < r.ItemArray.Length; i++)
                    row[i] = r[i];
                row.RowError = r.RowError;
                //Console.WriteLine("in get pure table:" + r.RowError);
                retTable.Rows.Add(row);
            }

            return retTable;

        }
       public static  DataSet getPureDataSet(DataSet ds)
       {
           DataSet retDs = new DataSet();
           foreach (System.Data.DataTable tbl in ds.Tables)
           {
               System.Data.DataTable table=retDs.Tables.Add(tbl.TableName);
               foreach (System.Data.DataColumn c in tbl.Columns)
               {
                  System.Data.DataColumn col= table.Columns.Add(c.ColumnName);
                  col.DataType = c.DataType;
                 
               }

               foreach (System.Data.DataRow r in tbl.Rows)
               {
                   System.Data.DataRow row = table.NewRow();
                   for (int i = 0; i < r.ItemArray.Length; i++)
                       row[i] = r[i];
                   row.RowError = r.RowError;
                  
                   table.Rows.Add(row);
               }
           }
           return retDs;

       }

        public static DataTable getPureDataTable(System.Data.DataRow []  rows)
        {
            if (rows.Length == 0)
                throw new Exception("rows 長度為零");
            System.Data.DataTable retTable = new DataTable(rows[0].Table.TableName);

            foreach (System.Data.DataColumn c in rows[0].Table.Columns)
            {
                System.Data.DataColumn col = retTable.Columns.Add(c.ColumnName);
                col.DataType = c.DataType;

            }

            foreach (System.Data.DataRow r in rows)
            {
                System.Data.DataRow row = retTable.NewRow();
                for (int i = 0; i < r.ItemArray.Length; i++)
                    row[i] = r[i];
                row.RowError = r.RowError;
                retTable.Rows.Add(row);
            }

            return retTable;
        }

       
    }
}
