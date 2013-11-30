namespace QYClient
{
    partial class FrmRmsStatus
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
            this.ds = new QYTask.Ds();
            this.rmsnameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.intersectionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ipDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ctl_mode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.planno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.connectedDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.deviceErrDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.cabineteOpenDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.lEDModuleErrDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.downLinkErrDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.uplinkErrDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.displayErrDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ds)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.rmsnameDataGridViewTextBoxColumn,
            this.intersectionDataGridViewTextBoxColumn,
            this.ipDataGridViewTextBoxColumn,
            this.ctl_mode,
            this.planno,
            this.connectedDataGridViewCheckBoxColumn,
            this.deviceErrDataGridViewCheckBoxColumn,
            this.cabineteOpenDataGridViewCheckBoxColumn,
            this.lEDModuleErrDataGridViewCheckBoxColumn,
            this.downLinkErrDataGridViewCheckBoxColumn,
            this.uplinkErrDataGridViewCheckBoxColumn,
            this.displayErrDataGridViewCheckBoxColumn});
            this.dataGridView1.DataMember = "tblRmsConfig";
            this.dataGridView1.DataSource = this.ds;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(794, 464);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView1_CellFormatting);
            // 
            // ds
            // 
            this.ds.DataSetName = "Ds";
            this.ds.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // rmsnameDataGridViewTextBoxColumn
            // 
            this.rmsnameDataGridViewTextBoxColumn.DataPropertyName = "rms_name";
            this.rmsnameDataGridViewTextBoxColumn.Frozen = true;
            this.rmsnameDataGridViewTextBoxColumn.HeaderText = "RMS名稱";
            this.rmsnameDataGridViewTextBoxColumn.Name = "rmsnameDataGridViewTextBoxColumn";
            this.rmsnameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // intersectionDataGridViewTextBoxColumn
            // 
            this.intersectionDataGridViewTextBoxColumn.DataPropertyName = "intersection";
            this.intersectionDataGridViewTextBoxColumn.Frozen = true;
            this.intersectionDataGridViewTextBoxColumn.HeaderText = "所在交流道";
            this.intersectionDataGridViewTextBoxColumn.Name = "intersectionDataGridViewTextBoxColumn";
            this.intersectionDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // ipDataGridViewTextBoxColumn
            // 
            this.ipDataGridViewTextBoxColumn.DataPropertyName = "ip";
            this.ipDataGridViewTextBoxColumn.Frozen = true;
            this.ipDataGridViewTextBoxColumn.HeaderText = "IP";
            this.ipDataGridViewTextBoxColumn.Name = "ipDataGridViewTextBoxColumn";
            this.ipDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // ctl_mode
            // 
            this.ctl_mode.DataPropertyName = "ctl_mode";
            this.ctl_mode.Frozen = true;
            this.ctl_mode.HeaderText = "模式";
            this.ctl_mode.Name = "ctl_mode";
            this.ctl_mode.ReadOnly = true;
            // 
            // planno
            // 
            this.planno.DataPropertyName = "planno";
            this.planno.Frozen = true;
            this.planno.HeaderText = "時制";
            this.planno.Name = "planno";
            this.planno.ReadOnly = true;
            // 
            // connectedDataGridViewCheckBoxColumn
            // 
            this.connectedDataGridViewCheckBoxColumn.DataPropertyName = "connected";
            this.connectedDataGridViewCheckBoxColumn.HeaderText = "連線";
            this.connectedDataGridViewCheckBoxColumn.Name = "connectedDataGridViewCheckBoxColumn";
            this.connectedDataGridViewCheckBoxColumn.ReadOnly = true;
            // 
            // deviceErrDataGridViewCheckBoxColumn
            // 
            this.deviceErrDataGridViewCheckBoxColumn.DataPropertyName = "DeviceErr";
            this.deviceErrDataGridViewCheckBoxColumn.HeaderText = "設備故障";
            this.deviceErrDataGridViewCheckBoxColumn.Name = "deviceErrDataGridViewCheckBoxColumn";
            this.deviceErrDataGridViewCheckBoxColumn.ReadOnly = true;
            // 
            // cabineteOpenDataGridViewCheckBoxColumn
            // 
            this.cabineteOpenDataGridViewCheckBoxColumn.DataPropertyName = "CabineteOpen";
            this.cabineteOpenDataGridViewCheckBoxColumn.HeaderText = "箱門開啟";
            this.cabineteOpenDataGridViewCheckBoxColumn.Name = "cabineteOpenDataGridViewCheckBoxColumn";
            this.cabineteOpenDataGridViewCheckBoxColumn.ReadOnly = true;
            // 
            // lEDModuleErrDataGridViewCheckBoxColumn
            // 
            this.lEDModuleErrDataGridViewCheckBoxColumn.DataPropertyName = "LED_ModuleErr";
            this.lEDModuleErrDataGridViewCheckBoxColumn.HeaderText = "LED 模組故障";
            this.lEDModuleErrDataGridViewCheckBoxColumn.Name = "lEDModuleErrDataGridViewCheckBoxColumn";
            this.lEDModuleErrDataGridViewCheckBoxColumn.ReadOnly = true;
            // 
            // downLinkErrDataGridViewCheckBoxColumn
            // 
            this.downLinkErrDataGridViewCheckBoxColumn.DataPropertyName = "DownLinkErr";
            this.downLinkErrDataGridViewCheckBoxColumn.HeaderText = "下層連線異常";
            this.downLinkErrDataGridViewCheckBoxColumn.Name = "downLinkErrDataGridViewCheckBoxColumn";
            this.downLinkErrDataGridViewCheckBoxColumn.ReadOnly = true;
            // 
            // uplinkErrDataGridViewCheckBoxColumn
            // 
            this.uplinkErrDataGridViewCheckBoxColumn.DataPropertyName = "UplinkErr";
            this.uplinkErrDataGridViewCheckBoxColumn.HeaderText = "上曾連線異常";
            this.uplinkErrDataGridViewCheckBoxColumn.Name = "uplinkErrDataGridViewCheckBoxColumn";
            this.uplinkErrDataGridViewCheckBoxColumn.ReadOnly = true;
            // 
            // displayErrDataGridViewCheckBoxColumn
            // 
            this.displayErrDataGridViewCheckBoxColumn.DataPropertyName = "DisplayErr";
            this.displayErrDataGridViewCheckBoxColumn.HeaderText = "顯示模組錯誤";
            this.displayErrDataGridViewCheckBoxColumn.Name = "displayErrDataGridViewCheckBoxColumn";
            this.displayErrDataGridViewCheckBoxColumn.ReadOnly = true;
            // 
            // FrmRmsStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(794, 464);
            this.Controls.Add(this.dataGridView1);
            this.Name = "FrmRmsStatus";
            this.Text = "RMS狀態";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmRmsStatus_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ds)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private QYTask.Ds ds;
        private System.Windows.Forms.DataGridViewTextBoxColumn rmsnameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn intersectionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ipDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ctl_mode;
        private System.Windows.Forms.DataGridViewTextBoxColumn planno;
        private System.Windows.Forms.DataGridViewCheckBoxColumn connectedDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn deviceErrDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn cabineteOpenDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn lEDModuleErrDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn downLinkErrDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn uplinkErrDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn displayErrDataGridViewCheckBoxColumn;
    }
}