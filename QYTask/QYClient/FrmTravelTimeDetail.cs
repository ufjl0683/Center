using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace QYClient
{
    public partial class FrmTravelTimeDetail : Form
    {
        public FrmTravelTimeDetail(DataSet ds)
        {
            InitializeComponent();
            this.dataGridView1.DataSource = ds;
            this.dataGridView1.DataMember = "tblSecTrafficData";
            this.dataGridView1.Refresh();
        }

        private void FrmTravelTimeDetail_Load(object sender, EventArgs e)
        {

        }
    }
}