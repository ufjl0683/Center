using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace QYClient
{
    public partial class FrmRGSConfig : Form
    {
        public FrmRGSConfig()
        {
            InitializeComponent();
            DataSet ds = Util.robj.get_rgs_conf_table();
            ds.Tables["tblRGS_Config"].DefaultView.Sort = "ip,display_part";
            this.dvRGS.DataSource = ds;
            this.dvRGS.DataMember = "tblRGS_Config";
            for (int i = 0; i < dvRGS.Columns.Count; i++)
                if (i > 12)
                    dvRGS.Columns[i].Visible = false;

            dvRGS.Refresh();
        }

        private void dvRGS_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}