using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace QYClient
{
    public partial class FrmRmsConfig : Form
    {
        public FrmRmsConfig()
        {
            DataSet ds1=null;
            InitializeComponent();
            try
            {
                ds1 = Util.robj.get_rms_config();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            this.dataGridView1.DataSource = ds1;
            this.dataGridView1.DataMember = this.dataGridView1.DataMember;
            this.dataGridView1.Refresh();
        }
    }
}