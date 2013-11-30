namespace QYClient
{
    partial class FrmRGSConfig
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
            this.dvRGS = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dvRGS)).BeginInit();
            this.SuspendLayout();
            // 
            // dvRGS
            // 
            this.dvRGS.AllowUserToAddRows = false;
            this.dvRGS.AllowUserToDeleteRows = false;
            this.dvRGS.AllowUserToResizeRows = false;
            this.dvRGS.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dvRGS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dvRGS.Location = new System.Drawing.Point(0, 0);
            this.dvRGS.Name = "dvRGS";
            this.dvRGS.ReadOnly = true;
            this.dvRGS.RowTemplate.Height = 24;
            this.dvRGS.Size = new System.Drawing.Size(580, 436);
            this.dvRGS.TabIndex = 0;
            this.dvRGS.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dvRGS_CellContentClick);
            // 
            // FrmRGSConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(580, 436);
            this.Controls.Add(this.dvRGS);
            this.Name = "FrmRGSConfig";
            this.Text = "RGS配置表";
            ((System.ComponentModel.ISupportInitialize)(this.dvRGS)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dvRGS;
    }
}