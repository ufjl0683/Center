namespace QYClient
{
    partial class FrmCurrentSectionData
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該公開 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改這個方法的內容。
        ///
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cm = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.檢視ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.rgs_name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.display_part = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iconid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.message1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.message2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lowerlimit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.traveltime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.upperlimit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ip = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.from_milepost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.end_milepost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ds1 = new QYTask.Ds();
            this.cm.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ds1)).BeginInit();
            this.SuspendLayout();
            // 
            // cm
            // 
            this.cm.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.檢視ToolStripMenuItem});
            this.cm.Name = "cm";
            this.cm.Size = new System.Drawing.Size(95, 26);
            this.cm.Opening += new System.ComponentModel.CancelEventHandler(this.cm_Opening);
            // 
            // 檢視ToolStripMenuItem
            // 
            this.檢視ToolStripMenuItem.Name = "檢視ToolStripMenuItem";
            this.檢視ToolStripMenuItem.Size = new System.Drawing.Size(94, 22);
            this.檢視ToolStripMenuItem.Text = "檢視";
            this.檢視ToolStripMenuItem.Click += new System.EventHandler(this.檢視ToolStripMenuItem_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.rgs_name,
            this.display_part,
            this.iconid,
            this.message1,
            this.message2,
            this.lowerlimit,
            this.traveltime,
            this.upperlimit,
            this.ip,
            this.from_milepost,
            this.end_milepost});
            this.dataGridView1.ContextMenuStrip = this.cm;
            this.dataGridView1.Cursor = System.Windows.Forms.Cursors.Default;
            this.dataGridView1.DataMember = "tblRGS_Config";
            this.dataGridView1.DataSource = this.ds1;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(981, 577);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_RowEnter);
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // rgs_name
            // 
            this.rgs_name.DataPropertyName = "rgs_name";
            this.rgs_name.HeaderText = "RGS_名稱";
            this.rgs_name.Name = "rgs_name";
            this.rgs_name.ReadOnly = true;
            // 
            // display_part
            // 
            this.display_part.DataPropertyName = "display_part";
            this.display_part.HeaderText = "顯示區";
            this.display_part.Name = "display_part";
            this.display_part.ReadOnly = true;
            // 
            // iconid
            // 
            this.iconid.DataPropertyName = "iconid";
            this.iconid.HeaderText = "圖示";
            this.iconid.Name = "iconid";
            this.iconid.ReadOnly = true;
            // 
            // message1
            // 
            this.message1.DataPropertyName = "message1";
            this.message1.HeaderText = "顯示文字1";
            this.message1.Name = "message1";
            this.message1.ReadOnly = true;
            // 
            // message2
            // 
            this.message2.DataPropertyName = "message2";
            this.message2.HeaderText = "顯示文字2";
            this.message2.Name = "message2";
            this.message2.ReadOnly = true;
            // 
            // lowerlimit
            // 
            this.lowerlimit.DataPropertyName = "lowerlimit";
            this.lowerlimit.HeaderText = "下限值";
            this.lowerlimit.Name = "lowerlimit";
            this.lowerlimit.ReadOnly = true;
            // 
            // traveltime
            // 
            this.traveltime.DataPropertyName = "traveltime";
            this.traveltime.HeaderText = "旅行時間";
            this.traveltime.Name = "traveltime";
            this.traveltime.ReadOnly = true;
            // 
            // upperlimit
            // 
            this.upperlimit.DataPropertyName = "upperlimit";
            this.upperlimit.HeaderText = "上限值";
            this.upperlimit.Name = "upperlimit";
            this.upperlimit.ReadOnly = true;
            // 
            // ip
            // 
            this.ip.DataPropertyName = "ip";
            this.ip.HeaderText = "ip";
            this.ip.Name = "ip";
            this.ip.ReadOnly = true;
            // 
            // from_milepost
            // 
            this.from_milepost.DataPropertyName = "from_milepost";
            this.from_milepost.HeaderText = "from_milepost";
            this.from_milepost.Name = "from_milepost";
            this.from_milepost.ReadOnly = true;
            // 
            // end_milepost
            // 
            this.end_milepost.DataPropertyName = "end_milepost";
            this.end_milepost.HeaderText = "end_milepost";
            this.end_milepost.Name = "end_milepost";
            this.end_milepost.ReadOnly = true;
            // 
            // ds1
            // 
            this.ds1.DataSetName = "Ds";
            this.ds1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // FrmCurrentSectionData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(981, 577);
            this.Controls.Add(this.dataGridView1);
            this.Font = new System.Drawing.Font("新細明體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FrmCurrentSectionData";
            this.Text = "FrmCurrentSectionData";
            this.Load += new System.EventHandler(this.FrmCurrentSectionData_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmCurrentSectionData_FormClosing);
            this.cm.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ds1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private QYTask.Ds ds1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ContextMenuStrip cm;
        private System.Windows.Forms.ToolStripMenuItem 檢視ToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn rgs_name;
        private System.Windows.Forms.DataGridViewTextBoxColumn display_part;
        private System.Windows.Forms.DataGridViewTextBoxColumn iconid;
        private System.Windows.Forms.DataGridViewTextBoxColumn message1;
        private System.Windows.Forms.DataGridViewTextBoxColumn message2;
        private System.Windows.Forms.DataGridViewTextBoxColumn lowerlimit;
        private System.Windows.Forms.DataGridViewTextBoxColumn traveltime;
        private System.Windows.Forms.DataGridViewTextBoxColumn upperlimit;
        private System.Windows.Forms.DataGridViewTextBoxColumn ip;
        private System.Windows.Forms.DataGridViewTextBoxColumn from_milepost;
        private System.Windows.Forms.DataGridViewTextBoxColumn end_milepost;
    }
}