namespace QYClient
{
    partial class FrmTravelTimeDetail
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
            this.ds = new QYTask.Ds();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.freewayIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.expresswayIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.directionIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fromlocationDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.frommilepostDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.endlocationDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.endmilepostDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sectionupperlimitDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sectionlowerlimitDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.traveltimeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.ds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // ds
            // 
            this.ds.DataSetName = "Ds";
            this.ds.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.freewayIdDataGridViewTextBoxColumn,
            this.expresswayIdDataGridViewTextBoxColumn,
            this.directionIdDataGridViewTextBoxColumn,
            this.fromlocationDataGridViewTextBoxColumn,
            this.frommilepostDataGridViewTextBoxColumn,
            this.endlocationDataGridViewTextBoxColumn,
            this.endmilepostDataGridViewTextBoxColumn,
            this.sectionupperlimitDataGridViewTextBoxColumn,
            this.sectionlowerlimitDataGridViewTextBoxColumn,
            this.traveltimeDataGridViewTextBoxColumn});
            this.dataGridView1.DataMember = "tblSecTrafficData";
            this.dataGridView1.DataSource = this.ds;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(832, 281);
            this.dataGridView1.TabIndex = 0;
            // 
            // freewayIdDataGridViewTextBoxColumn
            // 
            this.freewayIdDataGridViewTextBoxColumn.DataPropertyName = "freewayId";
            this.freewayIdDataGridViewTextBoxColumn.HeaderText = "freewayId";
            this.freewayIdDataGridViewTextBoxColumn.Name = "freewayIdDataGridViewTextBoxColumn";
            // 
            // expresswayIdDataGridViewTextBoxColumn
            // 
            this.expresswayIdDataGridViewTextBoxColumn.DataPropertyName = "expresswayId";
            this.expresswayIdDataGridViewTextBoxColumn.HeaderText = "expresswayId";
            this.expresswayIdDataGridViewTextBoxColumn.Name = "expresswayIdDataGridViewTextBoxColumn";
            // 
            // directionIdDataGridViewTextBoxColumn
            // 
            this.directionIdDataGridViewTextBoxColumn.DataPropertyName = "directionId";
            this.directionIdDataGridViewTextBoxColumn.HeaderText = "directionId";
            this.directionIdDataGridViewTextBoxColumn.Name = "directionIdDataGridViewTextBoxColumn";
            // 
            // fromlocationDataGridViewTextBoxColumn
            // 
            this.fromlocationDataGridViewTextBoxColumn.DataPropertyName = "from_location";
            this.fromlocationDataGridViewTextBoxColumn.HeaderText = "from_location";
            this.fromlocationDataGridViewTextBoxColumn.Name = "fromlocationDataGridViewTextBoxColumn";
            // 
            // frommilepostDataGridViewTextBoxColumn
            // 
            this.frommilepostDataGridViewTextBoxColumn.DataPropertyName = "from_milepost";
            this.frommilepostDataGridViewTextBoxColumn.HeaderText = "from_milepost";
            this.frommilepostDataGridViewTextBoxColumn.Name = "frommilepostDataGridViewTextBoxColumn";
            // 
            // endlocationDataGridViewTextBoxColumn
            // 
            this.endlocationDataGridViewTextBoxColumn.DataPropertyName = "end_location";
            this.endlocationDataGridViewTextBoxColumn.HeaderText = "end_location";
            this.endlocationDataGridViewTextBoxColumn.Name = "endlocationDataGridViewTextBoxColumn";
            // 
            // endmilepostDataGridViewTextBoxColumn
            // 
            this.endmilepostDataGridViewTextBoxColumn.DataPropertyName = "end_milepost";
            this.endmilepostDataGridViewTextBoxColumn.HeaderText = "end_milepost";
            this.endmilepostDataGridViewTextBoxColumn.Name = "endmilepostDataGridViewTextBoxColumn";
            // 
            // sectionupperlimitDataGridViewTextBoxColumn
            // 
            this.sectionupperlimitDataGridViewTextBoxColumn.DataPropertyName = "section_upper_limit";
            this.sectionupperlimitDataGridViewTextBoxColumn.HeaderText = "section_upper_limit";
            this.sectionupperlimitDataGridViewTextBoxColumn.Name = "sectionupperlimitDataGridViewTextBoxColumn";
            // 
            // sectionlowerlimitDataGridViewTextBoxColumn
            // 
            this.sectionlowerlimitDataGridViewTextBoxColumn.DataPropertyName = "section_lower_limit";
            this.sectionlowerlimitDataGridViewTextBoxColumn.HeaderText = "section_lower_limit";
            this.sectionlowerlimitDataGridViewTextBoxColumn.Name = "sectionlowerlimitDataGridViewTextBoxColumn";
            // 
            // traveltimeDataGridViewTextBoxColumn
            // 
            this.traveltimeDataGridViewTextBoxColumn.DataPropertyName = "travel_time";
            this.traveltimeDataGridViewTextBoxColumn.HeaderText = "travel_time";
            this.traveltimeDataGridViewTextBoxColumn.Name = "traveltimeDataGridViewTextBoxColumn";
            // 
            // FrmTravelTimeDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(832, 281);
            this.Controls.Add(this.dataGridView1);
            this.Name = "FrmTravelTimeDetail";
            this.Text = "FrmTravelTimeDetail";
            this.Load += new System.EventHandler(this.FrmTravelTimeDetail_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private QYTask.Ds ds;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn freewayIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn expresswayIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn directionIdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fromlocationDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn frommilepostDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn endlocationDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn endmilepostDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn sectionupperlimitDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn sectionlowerlimitDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn traveltimeDataGridViewTextBoxColumn;
    }
}