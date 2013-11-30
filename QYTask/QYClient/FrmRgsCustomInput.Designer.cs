namespace QYClient
{
    partial class FrmRGSCustomInput
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.grdRGS_main = new System.Windows.Forms.DataGridView();
            this.rgsnameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ipDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.direction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bs1 = new System.Windows.Forms.BindingSource(this.components);
            this.ds = new QYTask.Ds();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.bsmainRGSConf = new System.Windows.Forms.BindingSource(this.components);
            this.dataGridViewComboBoxColumn1 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dataGridViewComboBoxColumn2 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dataGridViewComboBoxColumn3 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dataGridViewComboBoxColumn4 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dataGridViewComboBoxColumn5 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.rgsctl = new QYClient.RgsGenericCtl();
            this.displaypartDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mode = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.iconidDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.msgtemp1DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.msgtemp2DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ficon = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.finput1DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.finput2DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.send = new System.Windows.Forms.DataGridViewButtonColumn();
            this.off = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.grdRGS_main)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bs1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsmainRGSConf)).BeginInit();
            this.SuspendLayout();
            // 
            // grdRGS_main
            // 
            this.grdRGS_main.AllowUserToAddRows = false;
            this.grdRGS_main.AllowUserToDeleteRows = false;
            this.grdRGS_main.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.grdRGS_main.AutoGenerateColumns = false;
            this.grdRGS_main.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdRGS_main.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.rgsnameDataGridViewTextBoxColumn,
            this.ipDataGridViewTextBoxColumn,
            this.direction});
            this.grdRGS_main.DataSource = this.bs1;
            this.grdRGS_main.Location = new System.Drawing.Point(2, 13);
            this.grdRGS_main.Margin = new System.Windows.Forms.Padding(4);
            this.grdRGS_main.Name = "grdRGS_main";
            this.grdRGS_main.ReadOnly = true;
            this.grdRGS_main.RowTemplate.Height = 24;
            this.grdRGS_main.Size = new System.Drawing.Size(406, 589);
            this.grdRGS_main.TabIndex = 1;
            this.grdRGS_main.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // rgsnameDataGridViewTextBoxColumn
            // 
            this.rgsnameDataGridViewTextBoxColumn.DataPropertyName = "rgs_name";
            this.rgsnameDataGridViewTextBoxColumn.Frozen = true;
            this.rgsnameDataGridViewTextBoxColumn.HeaderText = "RGS 名稱";
            this.rgsnameDataGridViewTextBoxColumn.Name = "rgsnameDataGridViewTextBoxColumn";
            this.rgsnameDataGridViewTextBoxColumn.ReadOnly = true;
            this.rgsnameDataGridViewTextBoxColumn.Width = 200;
            // 
            // ipDataGridViewTextBoxColumn
            // 
            this.ipDataGridViewTextBoxColumn.DataPropertyName = "ip";
            this.ipDataGridViewTextBoxColumn.HeaderText = "IP";
            this.ipDataGridViewTextBoxColumn.Name = "ipDataGridViewTextBoxColumn";
            this.ipDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // direction
            // 
            this.direction.DataPropertyName = "direction";
            this.direction.HeaderText = "方向";
            this.direction.Name = "direction";
            this.direction.ReadOnly = true;
            this.direction.Width = 50;
            // 
            // bs1
            // 
            this.bs1.AllowNew = false;
            this.bs1.DataMember = "tblRGSMain";
            this.bs1.DataSource = this.ds;
            this.bs1.CurrentChanged += new System.EventHandler(this.bs1_CurrentChanged);
            // 
            // ds
            // 
            this.ds.DataSetName = "Ds";
            this.ds.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.displaypartDataGridViewTextBoxColumn,
            this.mode,
            this.iconidDataGridViewTextBoxColumn,
            this.msgtemp1DataGridViewTextBoxColumn,
            this.msgtemp2DataGridViewTextBoxColumn,
            this.ficon,
            this.finput1DataGridViewTextBoxColumn,
            this.finput2DataGridViewTextBoxColumn,
            this.send,
            this.off});
            this.dataGridView1.DataSource = this.bsmainRGSConf;
            this.dataGridView1.Location = new System.Drawing.Point(416, 265);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(4);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(855, 337);
            this.dataGridView1.TabIndex = 2;
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick_1);
            // 
            // bsmainRGSConf
            // 
            this.bsmainRGSConf.DataMember = "tblRGSMain_tblRGS_Config";
            this.bsmainRGSConf.DataSource = this.bs1;
            this.bsmainRGSConf.CurrentChanged += new System.EventHandler(this.bsmainRGSConf_CurrentChanged);
            this.bsmainRGSConf.CurrentItemChanged += new System.EventHandler(this.bsmainRGSConf_CurrentItemChanged);
            // 
            // dataGridViewComboBoxColumn1
            // 
            this.dataGridViewComboBoxColumn1.DataPropertyName = "mode";
            this.dataGridViewComboBoxColumn1.HeaderText = "顯示模式";
            this.dataGridViewComboBoxColumn1.Name = "dataGridViewComboBoxColumn1";
            // 
            // dataGridViewComboBoxColumn2
            // 
            this.dataGridViewComboBoxColumn2.DataPropertyName = "mode";
            this.dataGridViewComboBoxColumn2.HeaderText = "顯示模式";
            this.dataGridViewComboBoxColumn2.Name = "dataGridViewComboBoxColumn2";
            // 
            // dataGridViewComboBoxColumn3
            // 
            this.dataGridViewComboBoxColumn3.DataPropertyName = "ficon";
            this.dataGridViewComboBoxColumn3.HeaderText = "手動輸入圖示";
            this.dataGridViewComboBoxColumn3.Name = "dataGridViewComboBoxColumn3";
            // 
            // dataGridViewComboBoxColumn4
            // 
            this.dataGridViewComboBoxColumn4.DataPropertyName = "mode";
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.dataGridViewComboBoxColumn4.DefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewComboBoxColumn4.HeaderText = "顯示模式";
            this.dataGridViewComboBoxColumn4.Name = "dataGridViewComboBoxColumn4";
            // 
            // dataGridViewComboBoxColumn5
            // 
            this.dataGridViewComboBoxColumn5.DataPropertyName = "ficon";
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.dataGridViewComboBoxColumn5.DefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridViewComboBoxColumn5.HeaderText = "手動輸入圖示";
            this.dataGridViewComboBoxColumn5.Name = "dataGridViewComboBoxColumn5";
            // 
            // rgsctl
            // 
            this.rgsctl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rgsctl.iconId1 = ((byte)(78));
            this.rgsctl.iconId2 = ((byte)(77));
            this.rgsctl.Location = new System.Drawing.Point(416, 13);
            this.rgsctl.Margin = new System.Windows.Forms.Padding(4);
            this.rgsctl.Name = "rgsctl";
            this.rgsctl.ReadOnly = false;
            this.rgsctl.Size = new System.Drawing.Size(637, 249);
            this.rgsctl.TabIndex = 0;
            // 
            // displaypartDataGridViewTextBoxColumn
            // 
            this.displaypartDataGridViewTextBoxColumn.DataPropertyName = "display_part";
            this.displaypartDataGridViewTextBoxColumn.HeaderText = "顯示區";
            this.displaypartDataGridViewTextBoxColumn.Name = "displaypartDataGridViewTextBoxColumn";
            // 
            // mode
            // 
            this.mode.DataPropertyName = "mode";
            this.mode.DataSource = this.ds;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.mode.DefaultCellStyle = dataGridViewCellStyle1;
            this.mode.DisplayMember = "tblRGSMode.display";
            this.mode.HeaderText = "顯示模式";
            this.mode.Name = "mode";
            this.mode.ValueMember = "tblRGSMode.value";
            // 
            // iconidDataGridViewTextBoxColumn
            // 
            this.iconidDataGridViewTextBoxColumn.DataPropertyName = "iconid";
            this.iconidDataGridViewTextBoxColumn.HeaderText = "旅行時間圖示";
            this.iconidDataGridViewTextBoxColumn.Name = "iconidDataGridViewTextBoxColumn";
            this.iconidDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // msgtemp1DataGridViewTextBoxColumn
            // 
            this.msgtemp1DataGridViewTextBoxColumn.DataPropertyName = "msg_temp1";
            this.msgtemp1DataGridViewTextBoxColumn.HeaderText = "旅行時間樣板(上)";
            this.msgtemp1DataGridViewTextBoxColumn.Name = "msgtemp1DataGridViewTextBoxColumn";
            this.msgtemp1DataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // msgtemp2DataGridViewTextBoxColumn
            // 
            this.msgtemp2DataGridViewTextBoxColumn.DataPropertyName = "msg_temp2";
            this.msgtemp2DataGridViewTextBoxColumn.HeaderText = "旅行時間樣板(下)";
            this.msgtemp2DataGridViewTextBoxColumn.Name = "msgtemp2DataGridViewTextBoxColumn";
            this.msgtemp2DataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // ficon
            // 
            this.ficon.DataPropertyName = "ficon";
            this.ficon.DataSource = this.ds;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.ficon.DefaultCellStyle = dataGridViewCellStyle2;
            this.ficon.DisplayMember = "tblIcons.description";
            this.ficon.HeaderText = "手動輸入圖示";
            this.ficon.Name = "ficon";
            this.ficon.ValueMember = "tblIcons.iconId";
            // 
            // finput1DataGridViewTextBoxColumn
            // 
            this.finput1DataGridViewTextBoxColumn.DataPropertyName = "finput1";
            this.finput1DataGridViewTextBoxColumn.HeaderText = "手動顯示文字(上)";
            this.finput1DataGridViewTextBoxColumn.Name = "finput1DataGridViewTextBoxColumn";
            this.finput1DataGridViewTextBoxColumn.ReadOnly = true;
            this.finput1DataGridViewTextBoxColumn.Visible = false;
            // 
            // finput2DataGridViewTextBoxColumn
            // 
            this.finput2DataGridViewTextBoxColumn.DataPropertyName = "finput2";
            this.finput2DataGridViewTextBoxColumn.HeaderText = "手動顯示文字(下)";
            this.finput2DataGridViewTextBoxColumn.Name = "finput2DataGridViewTextBoxColumn";
            this.finput2DataGridViewTextBoxColumn.ReadOnly = true;
            this.finput2DataGridViewTextBoxColumn.Visible = false;
            // 
            // send
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.Silver;
            this.send.DefaultCellStyle = dataGridViewCellStyle3;
            this.send.HeaderText = "傳送";
            this.send.Name = "send";
            this.send.ReadOnly = true;
            this.send.Text = "傳送";
            this.send.UseColumnTextForButtonValue = true;
            // 
            // off
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.off.DefaultCellStyle = dataGridViewCellStyle4;
            this.off.HeaderText = "熄滅";
            this.off.Name = "off";
            this.off.Text = "熄滅";
            this.off.UseColumnTextForButtonValue = true;
            // 
            // FrmRGSCustomInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1284, 631);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.grdRGS_main);
            this.Controls.Add(this.rgsctl);
            this.Font = new System.Drawing.Font("新細明體", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FrmRGSCustomInput";
            this.Text = "RGS輸入設定";
            ((System.ComponentModel.ISupportInitialize)(this.grdRGS_main)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bs1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsmainRGSConf)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private RgsGenericCtl rgsctl;
        private System.Windows.Forms.DataGridView grdRGS_main;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.BindingSource bs1;
        private QYTask.Ds ds;
        private System.Windows.Forms.BindingSource bsmainRGSConf;
        private System.Windows.Forms.DataGridViewComboBoxColumn dataGridViewComboBoxColumn1;
        private System.Windows.Forms.DataGridViewComboBoxColumn dataGridViewComboBoxColumn2;
        private System.Windows.Forms.DataGridViewComboBoxColumn dataGridViewComboBoxColumn3;
        private System.Windows.Forms.DataGridViewComboBoxColumn dataGridViewComboBoxColumn4;
        private System.Windows.Forms.DataGridViewComboBoxColumn dataGridViewComboBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn rgsnameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ipDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn direction;
        private System.Windows.Forms.DataGridViewTextBoxColumn displaypartDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn mode;
        private System.Windows.Forms.DataGridViewTextBoxColumn iconidDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn msgtemp1DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn msgtemp2DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn ficon;
        private System.Windows.Forms.DataGridViewTextBoxColumn finput1DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn finput2DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewButtonColumn send;
        private System.Windows.Forms.DataGridViewButtonColumn off;
    }
}