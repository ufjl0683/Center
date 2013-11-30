using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace QYClient
{
    public partial class FrmIcons : Form
    {
        public FrmIcons()
        {
            InitializeComponent();

            ds.Merge(Util.robj.get_tblIcons().Tables["tblIcons"]);

            this.dataGridView1.DataSource = ds;
            this.dataGridView1.DataMember = this.dataGridView1.DataMember;
            this.dataGridView1.Refresh();

           
           
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
          

        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            //if (dataGridView1.Columns[e.ColumnIndex].Name != "pic")
            //    return;
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                DataGridViewImageCell cell = (DataGridViewImageCell)dataGridView1.Rows[i].Cells[0];
                cell.Value = QYClient.Properties.Resources.ResourceManager.GetObject("icon" + dataGridView1.Rows[i].Cells["iconId"].Value.ToString());
                cell.OwningRow.Height = 120;
            }
        }
          
    }
}