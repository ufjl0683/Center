namespace QYClient
{
    partial class FrmRGSDisplay
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.rgsnameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Connected = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.displaypartDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.curriconDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.currmsg1DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.currmsg2DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ip = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ds = new QYTask.Ds();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ds)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.rgsnameDataGridViewTextBoxColumn,
            this.Connected,
            this.displaypartDataGridViewTextBoxColumn,
            this.curriconDataGridViewTextBoxColumn,
            this.currmsg1DataGridViewTextBoxColumn,
            this.currmsg2DataGridViewTextBoxColumn,
            this.ip});
            this.dataGridView1.DataMember = "tblRGS_Config";
            this.dataGridView1.DataSource = this.ds;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(897, 485);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView1_DataError);
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // rgsnameDataGridViewTextBoxColumn
            // 
            this.rgsnameDataGridViewTextBoxColumn.DataPropertyName = "rgs_name";
            this.rgsnameDataGridViewTextBoxColumn.HeaderText = "RGS名稱";
            this.rgsnameDataGridViewTextBoxColumn.Name = "rgsnameDataGridViewTextBoxColumn";
            this.rgsnameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // Connected
            // 
            this.Connected.DataPropertyName = "connected";
            this.Connected.FillWeight = 20F;
            this.Connected.HeaderText = "連線";
            this.Connected.Name = "Connected";
            this.Connected.ReadOnly = true;
            // 
            // displaypartDataGridViewTextBoxColumn
            // 
            this.displaypartDataGridViewTextBoxColumn.DataPropertyName = "display_part";
            this.displaypartDataGridViewTextBoxColumn.FillWeight = 20F;
            this.displaypartDataGridViewTextBoxColumn.HeaderText = "顯示區";
            this.displaypartDataGridViewTextBoxColumn.Name = "displaypartDataGridViewTextBoxColumn";
            this.displaypartDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // curriconDataGridViewTextBoxColumn
            // 
            this.curriconDataGridViewTextBoxColumn.DataPropertyName = "curr_icon";
            this.curriconDataGridViewTextBoxColumn.FillWeight = 20F;
            this.curriconDataGridViewTextBoxColumn.HeaderText = "ICON";
            this.curriconDataGridViewTextBoxColumn.Name = "curriconDataGridViewTextBoxColumn";
            this.curriconDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // currmsg1DataGridViewTextBoxColumn
            // 
            this.currmsg1DataGridViewTextBoxColumn.DataPropertyName = "curr_msg1";
            this.currmsg1DataGridViewTextBoxColumn.HeaderText = "訊息1";
            this.currmsg1DataGridViewTextBoxColumn.Name = "currmsg1DataGridViewTextBoxColumn";
            this.currmsg1DataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // currmsg2DataGridViewTextBoxColumn
            // 
            this.currmsg2DataGridViewTextBoxColumn.DataPropertyName = "curr_msg2";
            this.currmsg2DataGridViewTextBoxColumn.HeaderText = "訊息2";
            this.currmsg2DataGridViewTextBoxColumn.Name = "currmsg2DataGridViewTextBoxColumn";
            this.currmsg2DataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // ip
            // 
            this.ip.DataPropertyName = "ip";
            this.ip.HeaderText = "ip";
            this.ip.Name = "ip";
            this.ip.ReadOnly = true;
            this.ip.Visible = false;
            // 
            // ds
            // 
            this.ds.DataSetName = "Ds";
            this.ds.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // FrmRGSDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(897, 485);
            this.Controls.Add(this.dataGridView1);
            this.Name = "FrmRGSDisplay";
            this.Text = "RGS 顯示";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmRGSDisplay_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ds)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private QYTask.Ds ds;
        private System.Windows.Forms.DataGridViewTextBoxColumn rgsnameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Connected;
        private System.Windows.Forms.DataGridViewTextBoxColumn displaypartDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn curriconDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn currmsg1DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn currmsg2DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ip;
    }
}