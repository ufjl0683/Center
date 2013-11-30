namespace QYClient
{
    partial class FrmRmsModeSetting
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.rmsnameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.intersectionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ipDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.portDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ctl_mode_setting = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ds = new QYTask.Ds();
            this.planno_setting = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Send = new System.Windows.Forms.DataGridViewButtonColumn();
            this.bs = new System.Windows.Forms.BindingSource(this.components);
            this.dataGridViewComboBoxColumn1 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dataGridViewComboBoxColumn2 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dataGridViewComboBoxColumn3 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bs)).BeginInit();
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
            this.portDataGridViewTextBoxColumn,
            this.ctl_mode_setting,
            this.planno_setting,
            this.Send});
            this.dataGridView1.DataSource = this.bs;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(758, 494);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridView1_CellValidating);
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
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
            this.intersectionDataGridViewTextBoxColumn.HeaderText = "所在交流道位置";
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
            // portDataGridViewTextBoxColumn
            // 
            this.portDataGridViewTextBoxColumn.DataPropertyName = "port";
            this.portDataGridViewTextBoxColumn.Frozen = true;
            this.portDataGridViewTextBoxColumn.HeaderText = "Port";
            this.portDataGridViewTextBoxColumn.Name = "portDataGridViewTextBoxColumn";
            this.portDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // ctl_mode_setting
            // 
            this.ctl_mode_setting.DataPropertyName = "ctl_mode_setting";
            this.ctl_mode_setting.DataSource = this.ds;
            this.ctl_mode_setting.DisplayMember = "tblRMSMode.display";
            this.ctl_mode_setting.Frozen = true;
            this.ctl_mode_setting.HeaderText = "模式設定";
            this.ctl_mode_setting.Name = "ctl_mode_setting";
            this.ctl_mode_setting.ValueMember = "tblRMSMode.value";
            // 
            // ds
            // 
            this.ds.DataSetName = "Ds";
            this.ds.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // planno_setting
            // 
            this.planno_setting.DataPropertyName = "planno_setting";
            this.planno_setting.HeaderText = "時制編號";
            this.planno_setting.Name = "planno_setting";
            // 
            // Send
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle2.NullValue = "傳送";
            this.Send.DefaultCellStyle = dataGridViewCellStyle2;
            this.Send.HeaderText = "傳送";
            this.Send.Name = "Send";
            this.Send.ReadOnly = true;
            this.Send.Text = "傳送";
            // 
            // bs
            // 
            this.bs.DataMember = "tblRmsConfig";
            this.bs.DataSource = this.ds;
            this.bs.CurrentItemChanged += new System.EventHandler(this.bs_CurrentItemChanged);
            // 
            // dataGridViewComboBoxColumn1
            // 
            this.dataGridViewComboBoxColumn1.DataPropertyName = "ctl_mode_setting";
            this.dataGridViewComboBoxColumn1.HeaderText = "模式設定";
            this.dataGridViewComboBoxColumn1.Name = "dataGridViewComboBoxColumn1";
            // 
            // dataGridViewComboBoxColumn2
            // 
            this.dataGridViewComboBoxColumn2.DataPropertyName = "ctl_mode_setting";
            this.dataGridViewComboBoxColumn2.HeaderText = "模式設定";
            this.dataGridViewComboBoxColumn2.Name = "dataGridViewComboBoxColumn2";
            // 
            // dataGridViewComboBoxColumn3
            // 
            this.dataGridViewComboBoxColumn3.DataPropertyName = "ctl_mode_setting";
            this.dataGridViewComboBoxColumn3.HeaderText = "模式設定";
            this.dataGridViewComboBoxColumn3.Name = "dataGridViewComboBoxColumn3";
            // 
            // FrmRmsModeSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(758, 494);
            this.Controls.Add(this.dataGridView1);
            this.Font = new System.Drawing.Font("新細明體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Name = "FrmRmsModeSetting";
            this.Text = "RMS輸入設定";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bs)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private QYTask.Ds ds;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.BindingSource bs;
        private System.Windows.Forms.DataGridViewComboBoxColumn dataGridViewComboBoxColumn1;
        private System.Windows.Forms.DataGridViewComboBoxColumn dataGridViewComboBoxColumn2;
        private System.Windows.Forms.DataGridViewComboBoxColumn dataGridViewComboBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn rmsnameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn intersectionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ipDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn portDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn ctl_mode_setting;
        private System.Windows.Forms.DataGridViewComboBoxColumn planno_setting;
        private System.Windows.Forms.DataGridViewButtonColumn Send;
    }
}