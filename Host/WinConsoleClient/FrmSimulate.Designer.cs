namespace WinConsoleClient
{
    partial class FrmSimulate
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnVD = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ds = new WinConsoleClient.DataSet1();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.distanceDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.degreeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.button2 = new System.Windows.Forms.Button();
            this.dataGridView3 = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pluviometricDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.degreeDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.button3 = new System.Windows.Forms.Button();
            this.dataGridView4 = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.averagewindspeedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.averagewinddirectionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.maxwindspeedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.maxwinddirectionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.degreeDataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.button4 = new System.Windows.Forms.Button();
            this.dataGridView5 = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.monvarDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dayvarDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.degreeDataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.button7 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.dataGridView6 = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.slopeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.shiftDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sinkDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.degreeDataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tmrvd = new System.Windows.Forms.Timer(this.components);
            this.tmrvi = new System.Windows.Forms.Timer(this.components);
            this.tmrrd = new System.Windows.Forms.Timer(this.components);
            this.tmrwd = new System.Windows.Forms.Timer(this.components);
            this.tmrls = new System.Windows.Forms.Timer(this.components);
            this.tmrbs = new System.Windows.Forms.Timer(this.components);
            this.button5 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ds)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).BeginInit();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView4)).BeginInit();
            this.tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView5)).BeginInit();
            this.tabPage6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView6)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage6);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(676, 445);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.button8);
            this.tabPage1.Controls.Add(this.button5);
            this.tabPage1.Controls.Add(this.btnVD);
            this.tabPage1.Controls.Add(this.dataGridView1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(668, 419);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "VD";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnVD
            // 
            this.btnVD.Location = new System.Drawing.Point(413, 6);
            this.btnVD.Name = "btnVD";
            this.btnVD.Size = new System.Drawing.Size(75, 23);
            this.btnVD.TabIndex = 1;
            this.btnVD.Text = "開始";
            this.btnVD.UseVisualStyleBackColor = true;
            this.btnVD.Click += new System.EventHandler(this.button1_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewTextBoxColumn5,
            this.dataGridViewTextBoxColumn6});
            this.dataGridView1.DataMember = "tblVD5Min";
            this.dataGridView1.DataSource = this.ds;
            this.dataGridView1.Location = new System.Drawing.Point(6, 38);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(656, 382);
            this.dataGridView1.TabIndex = 0;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "DeviceName";
            this.dataGridViewTextBoxColumn1.HeaderText = "DeviceName";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "Speed";
            this.dataGridViewTextBoxColumn2.HeaderText = "Speed";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "Vol";
            this.dataGridViewTextBoxColumn3.HeaderText = "Vol";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.DataPropertyName = "Occupancy";
            this.dataGridViewTextBoxColumn4.HeaderText = "Occupancy";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.DataPropertyName = "Length";
            this.dataGridViewTextBoxColumn5.HeaderText = "Length";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.DataPropertyName = "Interval";
            this.dataGridViewTextBoxColumn6.HeaderText = "Interval";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            // 
            // ds
            // 
            this.ds.DataSetName = "DataSet1";
            this.ds.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.button1);
            this.tabPage2.Controls.Add(this.dataGridView2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(668, 419);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "VI";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(529, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "開始";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // dataGridView2
            // 
            this.dataGridView2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView2.AutoGenerateColumns = false;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn7,
            this.distanceDataGridViewTextBoxColumn,
            this.degreeDataGridViewTextBoxColumn});
            this.dataGridView2.DataMember = "tblVI5Min";
            this.dataGridView2.DataSource = this.ds;
            this.dataGridView2.Location = new System.Drawing.Point(6, 34);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.RowTemplate.Height = 24;
            this.dataGridView2.Size = new System.Drawing.Size(656, 382);
            this.dataGridView2.TabIndex = 2;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.DataPropertyName = "DeviceName";
            this.dataGridViewTextBoxColumn7.HeaderText = "DeviceName";
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            // 
            // distanceDataGridViewTextBoxColumn
            // 
            this.distanceDataGridViewTextBoxColumn.DataPropertyName = "Distance";
            this.distanceDataGridViewTextBoxColumn.HeaderText = "Distance";
            this.distanceDataGridViewTextBoxColumn.Name = "distanceDataGridViewTextBoxColumn";
            // 
            // degreeDataGridViewTextBoxColumn
            // 
            this.degreeDataGridViewTextBoxColumn.DataPropertyName = "Degree";
            this.degreeDataGridViewTextBoxColumn.HeaderText = "Degree";
            this.degreeDataGridViewTextBoxColumn.Name = "degreeDataGridViewTextBoxColumn";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.button2);
            this.tabPage3.Controls.Add(this.dataGridView3);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(668, 419);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "RD";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(556, 6);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "開始";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // dataGridView3
            // 
            this.dataGridView3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView3.AutoGenerateColumns = false;
            this.dataGridView3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView3.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn8,
            this.pluviometricDataGridViewTextBoxColumn,
            this.degreeDataGridViewTextBoxColumn1});
            this.dataGridView3.DataMember = "tblRD5Min";
            this.dataGridView3.DataSource = this.ds;
            this.dataGridView3.Location = new System.Drawing.Point(4, 35);
            this.dataGridView3.Name = "dataGridView3";
            this.dataGridView3.RowTemplate.Height = 24;
            this.dataGridView3.Size = new System.Drawing.Size(656, 382);
            this.dataGridView3.TabIndex = 1;
            // 
            // dataGridViewTextBoxColumn8
            // 
            this.dataGridViewTextBoxColumn8.DataPropertyName = "DeviceName";
            this.dataGridViewTextBoxColumn8.HeaderText = "DeviceName";
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            // 
            // pluviometricDataGridViewTextBoxColumn
            // 
            this.pluviometricDataGridViewTextBoxColumn.DataPropertyName = "pluviometric";
            this.pluviometricDataGridViewTextBoxColumn.HeaderText = "pluviometric";
            this.pluviometricDataGridViewTextBoxColumn.Name = "pluviometricDataGridViewTextBoxColumn";
            // 
            // degreeDataGridViewTextBoxColumn1
            // 
            this.degreeDataGridViewTextBoxColumn1.DataPropertyName = "Degree";
            this.degreeDataGridViewTextBoxColumn1.HeaderText = "Degree";
            this.degreeDataGridViewTextBoxColumn1.Name = "degreeDataGridViewTextBoxColumn1";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.button3);
            this.tabPage4.Controls.Add(this.dataGridView4);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(668, 419);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "WD";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(558, 5);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 6;
            this.button3.Text = "開始";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // dataGridView4
            // 
            this.dataGridView4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView4.AutoGenerateColumns = false;
            this.dataGridView4.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView4.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn9,
            this.averagewindspeedDataGridViewTextBoxColumn,
            this.averagewinddirectionDataGridViewTextBoxColumn,
            this.maxwindspeedDataGridViewTextBoxColumn,
            this.maxwinddirectionDataGridViewTextBoxColumn,
            this.degreeDataGridViewTextBoxColumn4});
            this.dataGridView4.DataMember = "tblWD5Min";
            this.dataGridView4.DataSource = this.ds;
            this.dataGridView4.Location = new System.Drawing.Point(6, 34);
            this.dataGridView4.Name = "dataGridView4";
            this.dataGridView4.RowTemplate.Height = 24;
            this.dataGridView4.Size = new System.Drawing.Size(656, 382);
            this.dataGridView4.TabIndex = 5;
            // 
            // dataGridViewTextBoxColumn9
            // 
            this.dataGridViewTextBoxColumn9.DataPropertyName = "DeviceName";
            this.dataGridViewTextBoxColumn9.HeaderText = "DeviceName";
            this.dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
            // 
            // averagewindspeedDataGridViewTextBoxColumn
            // 
            this.averagewindspeedDataGridViewTextBoxColumn.DataPropertyName = "average_wind_speed";
            this.averagewindspeedDataGridViewTextBoxColumn.HeaderText = "average_wind_speed";
            this.averagewindspeedDataGridViewTextBoxColumn.Name = "averagewindspeedDataGridViewTextBoxColumn";
            // 
            // averagewinddirectionDataGridViewTextBoxColumn
            // 
            this.averagewinddirectionDataGridViewTextBoxColumn.DataPropertyName = "average_wind_direction";
            this.averagewinddirectionDataGridViewTextBoxColumn.HeaderText = "average_wind_direction";
            this.averagewinddirectionDataGridViewTextBoxColumn.Name = "averagewinddirectionDataGridViewTextBoxColumn";
            // 
            // maxwindspeedDataGridViewTextBoxColumn
            // 
            this.maxwindspeedDataGridViewTextBoxColumn.DataPropertyName = "max_wind_speed";
            this.maxwindspeedDataGridViewTextBoxColumn.HeaderText = "max_wind_speed";
            this.maxwindspeedDataGridViewTextBoxColumn.Name = "maxwindspeedDataGridViewTextBoxColumn";
            // 
            // maxwinddirectionDataGridViewTextBoxColumn
            // 
            this.maxwinddirectionDataGridViewTextBoxColumn.DataPropertyName = "max_wind_direction";
            this.maxwinddirectionDataGridViewTextBoxColumn.HeaderText = "max_wind_direction";
            this.maxwinddirectionDataGridViewTextBoxColumn.Name = "maxwinddirectionDataGridViewTextBoxColumn";
            // 
            // degreeDataGridViewTextBoxColumn4
            // 
            this.degreeDataGridViewTextBoxColumn4.DataPropertyName = "Degree";
            this.degreeDataGridViewTextBoxColumn4.HeaderText = "Degree";
            this.degreeDataGridViewTextBoxColumn4.Name = "degreeDataGridViewTextBoxColumn4";
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.button4);
            this.tabPage5.Controls.Add(this.dataGridView5);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(668, 419);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "LS";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(585, 17);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 29);
            this.button4.TabIndex = 8;
            this.button4.Text = "開始";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // dataGridView5
            // 
            this.dataGridView5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView5.AutoGenerateColumns = false;
            this.dataGridView5.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView5.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn10,
            this.monvarDataGridViewTextBoxColumn,
            this.dayvarDataGridViewTextBoxColumn,
            this.degreeDataGridViewTextBoxColumn2});
            this.dataGridView5.DataMember = "tblLSData";
            this.dataGridView5.DataSource = this.ds;
            this.dataGridView5.Location = new System.Drawing.Point(9, 48);
            this.dataGridView5.Name = "dataGridView5";
            this.dataGridView5.RowTemplate.Height = 24;
            this.dataGridView5.Size = new System.Drawing.Size(656, 363);
            this.dataGridView5.TabIndex = 6;
            // 
            // dataGridViewTextBoxColumn10
            // 
            this.dataGridViewTextBoxColumn10.DataPropertyName = "DeviceName";
            this.dataGridViewTextBoxColumn10.HeaderText = "DeviceName";
            this.dataGridViewTextBoxColumn10.Name = "dataGridViewTextBoxColumn10";
            // 
            // monvarDataGridViewTextBoxColumn
            // 
            this.monvarDataGridViewTextBoxColumn.DataPropertyName = "mon_var";
            this.monvarDataGridViewTextBoxColumn.HeaderText = "mon_var";
            this.monvarDataGridViewTextBoxColumn.Name = "monvarDataGridViewTextBoxColumn";
            // 
            // dayvarDataGridViewTextBoxColumn
            // 
            this.dayvarDataGridViewTextBoxColumn.DataPropertyName = "day_var";
            this.dayvarDataGridViewTextBoxColumn.HeaderText = "day_var";
            this.dayvarDataGridViewTextBoxColumn.Name = "dayvarDataGridViewTextBoxColumn";
            // 
            // degreeDataGridViewTextBoxColumn2
            // 
            this.degreeDataGridViewTextBoxColumn2.DataPropertyName = "Degree";
            this.degreeDataGridViewTextBoxColumn2.HeaderText = "Degree";
            this.degreeDataGridViewTextBoxColumn2.Name = "degreeDataGridViewTextBoxColumn2";
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.button7);
            this.tabPage6.Controls.Add(this.button6);
            this.tabPage6.Controls.Add(this.dataGridView6);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Size = new System.Drawing.Size(668, 419);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "BS";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(565, 19);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(75, 23);
            this.button7.TabIndex = 9;
            this.button7.Text = "開始";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(565, 19);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(75, 23);
            this.button6.TabIndex = 7;
            this.button6.Text = "開始";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // dataGridView6
            // 
            this.dataGridView6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView6.AutoGenerateColumns = false;
            this.dataGridView6.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView6.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn12,
            this.slopeDataGridViewTextBoxColumn,
            this.shiftDataGridViewTextBoxColumn,
            this.sinkDataGridViewTextBoxColumn,
            this.degreeDataGridViewTextBoxColumn3});
            this.dataGridView6.DataMember = "tblBSData";
            this.dataGridView6.DataSource = this.ds;
            this.dataGridView6.Location = new System.Drawing.Point(8, 48);
            this.dataGridView6.Name = "dataGridView6";
            this.dataGridView6.RowTemplate.Height = 24;
            this.dataGridView6.Size = new System.Drawing.Size(656, 395);
            this.dataGridView6.TabIndex = 2;
            // 
            // dataGridViewTextBoxColumn12
            // 
            this.dataGridViewTextBoxColumn12.DataPropertyName = "DeviceName";
            this.dataGridViewTextBoxColumn12.HeaderText = "DeviceName";
            this.dataGridViewTextBoxColumn12.Name = "dataGridViewTextBoxColumn12";
            // 
            // slopeDataGridViewTextBoxColumn
            // 
            this.slopeDataGridViewTextBoxColumn.DataPropertyName = "slope";
            this.slopeDataGridViewTextBoxColumn.HeaderText = "slope";
            this.slopeDataGridViewTextBoxColumn.Name = "slopeDataGridViewTextBoxColumn";
            // 
            // shiftDataGridViewTextBoxColumn
            // 
            this.shiftDataGridViewTextBoxColumn.DataPropertyName = "shift";
            this.shiftDataGridViewTextBoxColumn.HeaderText = "shift";
            this.shiftDataGridViewTextBoxColumn.Name = "shiftDataGridViewTextBoxColumn";
            // 
            // sinkDataGridViewTextBoxColumn
            // 
            this.sinkDataGridViewTextBoxColumn.DataPropertyName = "sink";
            this.sinkDataGridViewTextBoxColumn.HeaderText = "sink";
            this.sinkDataGridViewTextBoxColumn.Name = "sinkDataGridViewTextBoxColumn";
            // 
            // degreeDataGridViewTextBoxColumn3
            // 
            this.degreeDataGridViewTextBoxColumn3.DataPropertyName = "Degree";
            this.degreeDataGridViewTextBoxColumn3.HeaderText = "Degree";
            this.degreeDataGridViewTextBoxColumn3.Name = "degreeDataGridViewTextBoxColumn3";
            // 
            // tmrvd
            // 
            this.tmrvd.Interval = 60000;
            this.tmrvd.Tick += new System.EventHandler(this.tmrvd_Tick);
            // 
            // tmrvi
            // 
            this.tmrvi.Interval = 60000;
            this.tmrvi.Tick += new System.EventHandler(this.tmrvi_Tick);
            // 
            // tmrrd
            // 
            this.tmrrd.Interval = 60000;
            this.tmrrd.Tick += new System.EventHandler(this.tmrrd_Tick);
            // 
            // tmrwd
            // 
            this.tmrwd.Interval = 60000;
            this.tmrwd.Tick += new System.EventHandler(this.tmrwd_Tick);
            // 
            // tmrls
            // 
            this.tmrls.Interval = 60000;
            this.tmrls.Tick += new System.EventHandler(this.tmrls_Tick);
            // 
            // tmrbs
            // 
            this.tmrbs.Interval = 60000;
            this.tmrbs.Tick += new System.EventHandler(this.tmrbs_Tick);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(494, 6);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 2;
            this.button5.Text = "save";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click_1);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(573, 6);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(75, 23);
            this.button8.TabIndex = 3;
            this.button8.Text = "load";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // FrmSimulate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(676, 445);
            this.Controls.Add(this.tabControl1);
            this.Name = "FrmSimulate";
            this.Text = "FrmSimulate";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ds)).EndInit();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).EndInit();
            this.tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView4)).EndInit();
            this.tabPage5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView5)).EndInit();
            this.tabPage6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView6)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnVD;
        private System.Windows.Forms.DataGridViewTextBoxColumn deviceNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn speedDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn volDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn occupancyDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn lengthDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn intervalDataGridViewTextBoxColumn;
        private System.Windows.Forms.Timer tmrvd;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private DataSet1 ds;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn distanceDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn degreeDataGridViewTextBoxColumn;
        private System.Windows.Forms.Timer tmrvi;
        private System.Windows.Forms.DataGridView dataGridView3;
        private System.Windows.Forms.Timer tmrrd;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private System.Windows.Forms.DataGridViewTextBoxColumn pluviometricDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn degreeDataGridViewTextBoxColumn1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.DataGridView dataGridView4;
        private System.Windows.Forms.Timer tmrwd;
        private System.Windows.Forms.DataGridView dataGridView5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn10;
        private System.Windows.Forms.DataGridViewTextBoxColumn monvarDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dayvarDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn degreeDataGridViewTextBoxColumn2;
        private System.Windows.Forms.Timer tmrls;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.DataGridView dataGridView6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
        private System.Windows.Forms.DataGridViewTextBoxColumn averagewindspeedDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn averagewinddirectionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn maxwindspeedDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn maxwinddirectionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn degreeDataGridViewTextBoxColumn4;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn12;
        private System.Windows.Forms.DataGridViewTextBoxColumn slopeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn shiftDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn sinkDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn degreeDataGridViewTextBoxColumn3;
        private System.Windows.Forms.Timer tmrbs;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button8;
    }
}