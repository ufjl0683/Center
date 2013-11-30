namespace QYClient
{
    partial class FrmRGSStatus
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
            this.dgv = new System.Windows.Forms.DataGridView();
            this.bs = new System.Windows.Forms.BindingSource(this.components);
            this.ds = new QYTask.Ds();
            this.rgsnameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ip = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.connected = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.DeviceErr = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.CabineteOpen = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.LED_ModuleErr = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.DisplayErr = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.DownLinkErr = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.UplinkErr = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ds)).BeginInit();
            this.SuspendLayout();
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.AllowUserToOrderColumns = true;
            this.dgv.AutoGenerateColumns = false;
            this.dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.rgsnameDataGridViewTextBoxColumn,
            this.ip,
            this.connected,
            this.DeviceErr,
            this.CabineteOpen,
            this.LED_ModuleErr,
            this.DisplayErr,
            this.DownLinkErr,
            this.UplinkErr});
            this.dgv.DataSource = this.bs;
            this.dgv.Location = new System.Drawing.Point(2, 1);
            this.dgv.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.dgv.Name = "dgv";
            this.dgv.ReadOnly = true;
            this.dgv.RowTemplate.Height = 24;
            this.dgv.Size = new System.Drawing.Size(1096, 550);
            this.dgv.TabIndex = 0;
            this.dgv.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgv_DataError);
            // 
            // bs
            // 
            this.bs.AllowNew = false;
            this.bs.DataMember = "tblRGSMain";
            this.bs.DataSource = this.ds;
            // 
            // ds
            // 
            this.ds.DataSetName = "Ds";
            this.ds.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // rgsnameDataGridViewTextBoxColumn
            // 
            this.rgsnameDataGridViewTextBoxColumn.DataPropertyName = "rgs_name";
            this.rgsnameDataGridViewTextBoxColumn.HeaderText = "RMS名稱";
            this.rgsnameDataGridViewTextBoxColumn.Name = "rgsnameDataGridViewTextBoxColumn";
            this.rgsnameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // ip
            // 
            this.ip.DataPropertyName = "ip";
            this.ip.FillWeight = 60F;
            this.ip.HeaderText = "IP";
            this.ip.Name = "ip";
            this.ip.ReadOnly = true;
            // 
            // connected
            // 
            this.connected.DataPropertyName = "connected";
            this.connected.FillWeight = 30F;
            this.connected.HeaderText = "已連線";
            this.connected.Name = "connected";
            this.connected.ReadOnly = true;
            // 
            // DeviceErr
            // 
            this.DeviceErr.DataPropertyName = "DeviceErr";
            this.DeviceErr.FillWeight = 30F;
            this.DeviceErr.HeaderText = "設備故障";
            this.DeviceErr.Name = "DeviceErr";
            this.DeviceErr.ReadOnly = true;
            // 
            // CabineteOpen
            // 
            this.CabineteOpen.DataPropertyName = "CabineteOpen";
            this.CabineteOpen.FillWeight = 30F;
            this.CabineteOpen.HeaderText = "箱門開啟";
            this.CabineteOpen.Name = "CabineteOpen";
            this.CabineteOpen.ReadOnly = true;
            // 
            // LED_ModuleErr
            // 
            this.LED_ModuleErr.DataPropertyName = "LED_ModuleErr";
            this.LED_ModuleErr.FillWeight = 30F;
            this.LED_ModuleErr.HeaderText = "LED模組故障";
            this.LED_ModuleErr.Name = "LED_ModuleErr";
            this.LED_ModuleErr.ReadOnly = true;
            // 
            // DisplayErr
            // 
            this.DisplayErr.DataPropertyName = "DisplayErr";
            this.DisplayErr.FillWeight = 30F;
            this.DisplayErr.HeaderText = "顯示設備故障";
            this.DisplayErr.Name = "DisplayErr";
            this.DisplayErr.ReadOnly = true;
            // 
            // DownLinkErr
            // 
            this.DownLinkErr.DataPropertyName = "DownLinkErr";
            this.DownLinkErr.FillWeight = 30F;
            this.DownLinkErr.HeaderText = "與下層連線錯誤";
            this.DownLinkErr.Name = "DownLinkErr";
            this.DownLinkErr.ReadOnly = true;
            // 
            // UplinkErr
            // 
            this.UplinkErr.DataPropertyName = "UplinkErr";
            this.UplinkErr.FillWeight = 30F;
            this.UplinkErr.HeaderText = "與上層連線錯誤";
            this.UplinkErr.Name = "UplinkErr";
            this.UplinkErr.ReadOnly = true;
            // 
            // FrmRGSStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(998, 553);
            this.Controls.Add(this.dgv);
            this.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "FrmRGSStatus";
            this.Text = "RGS 狀態顯示";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmRGSStatus_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ds)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgv;
        private QYTask.Ds ds;
        private System.Windows.Forms.BindingSource bs;
        private System.Windows.Forms.DataGridViewTextBoxColumn rgsnameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ip;
        private System.Windows.Forms.DataGridViewCheckBoxColumn connected;
        private System.Windows.Forms.DataGridViewCheckBoxColumn DeviceErr;
        private System.Windows.Forms.DataGridViewCheckBoxColumn CabineteOpen;
        private System.Windows.Forms.DataGridViewCheckBoxColumn LED_ModuleErr;
        private System.Windows.Forms.DataGridViewCheckBoxColumn DisplayErr;
        private System.Windows.Forms.DataGridViewCheckBoxColumn DownLinkErr;
        private System.Windows.Forms.DataGridViewCheckBoxColumn UplinkErr;
    }
}