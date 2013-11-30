namespace QYClient
{
    partial class RgsGenericCtl
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

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改這個方法的內容。
        ///
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.rtxtmsg4 = new System.Windows.Forms.RichTextBox();
            this.cm = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.設定顏色ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rtxtmsg3 = new System.Windows.Forms.RichTextBox();
            this.rtxtnsg2 = new System.Windows.Forms.RichTextBox();
            this.rtxtmsg1 = new System.Windows.Forms.RichTextBox();
            this.colorChoice = new System.Windows.Forms.ColorDialog();
            this.picIcon1 = new System.Windows.Forms.PictureBox();
            this.picIcon2 = new System.Windows.Forms.PictureBox();
            this.cm.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picIcon1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIcon2)).BeginInit();
            this.SuspendLayout();
            // 
            // rtxtmsg4
            // 
            this.rtxtmsg4.BackColor = System.Drawing.Color.Black;
            this.rtxtmsg4.ContextMenuStrip = this.cm;
            this.rtxtmsg4.Font = new System.Drawing.Font("新細明體", 32.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rtxtmsg4.ForeColor = System.Drawing.Color.Red;
            this.rtxtmsg4.Location = new System.Drawing.Point(147, 147);
            this.rtxtmsg4.Multiline = false;
            this.rtxtmsg4.Name = "rtxtmsg4";
            this.rtxtmsg4.Size = new System.Drawing.Size(316, 47);
            this.rtxtmsg4.TabIndex = 22;
            this.rtxtmsg4.Text = "";
            this.rtxtmsg4.Enter += new System.EventHandler(this.rtxtmsg1_Enter);
            this.rtxtmsg4.TextChanged += new System.EventHandler(this.rtxtmsg1_TextChanged);
            // 
            // cm
            // 
            this.cm.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.設定顏色ToolStripMenuItem});
            this.cm.Name = "cm";
            this.cm.Size = new System.Drawing.Size(119, 26);
            // 
            // 設定顏色ToolStripMenuItem
            // 
            this.設定顏色ToolStripMenuItem.Name = "設定顏色ToolStripMenuItem";
            this.設定顏色ToolStripMenuItem.Size = new System.Drawing.Size(118, 22);
            this.設定顏色ToolStripMenuItem.Text = "設定顏色";
            this.設定顏色ToolStripMenuItem.Click += new System.EventHandler(this.設定顏色ToolStripMenuItem_Click);
            // 
            // rtxtmsg3
            // 
            this.rtxtmsg3.BackColor = System.Drawing.Color.Black;
            this.rtxtmsg3.ContextMenuStrip = this.cm;
            this.rtxtmsg3.Font = new System.Drawing.Font("新細明體", 32.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rtxtmsg3.ForeColor = System.Drawing.Color.Red;
            this.rtxtmsg3.Location = new System.Drawing.Point(147, 100);
            this.rtxtmsg3.Multiline = false;
            this.rtxtmsg3.Name = "rtxtmsg3";
            this.rtxtmsg3.Size = new System.Drawing.Size(316, 47);
            this.rtxtmsg3.TabIndex = 21;
            this.rtxtmsg3.Text = "";
            this.rtxtmsg3.Enter += new System.EventHandler(this.rtxtmsg1_Enter);
            this.rtxtmsg3.TextChanged += new System.EventHandler(this.rtxtmsg1_TextChanged);
            // 
            // rtxtnsg2
            // 
            this.rtxtnsg2.BackColor = System.Drawing.Color.Black;
            this.rtxtnsg2.ContextMenuStrip = this.cm;
            this.rtxtnsg2.Font = new System.Drawing.Font("新細明體", 32.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rtxtnsg2.ForeColor = System.Drawing.Color.Red;
            this.rtxtnsg2.Location = new System.Drawing.Point(147, 53);
            this.rtxtnsg2.Multiline = false;
            this.rtxtnsg2.Name = "rtxtnsg2";
            this.rtxtnsg2.Size = new System.Drawing.Size(316, 47);
            this.rtxtnsg2.TabIndex = 20;
            this.rtxtnsg2.Text = "";
            this.rtxtnsg2.Enter += new System.EventHandler(this.rtxtmsg1_Enter);
            this.rtxtnsg2.TextChanged += new System.EventHandler(this.rtxtmsg1_TextChanged);
            // 
            // rtxtmsg1
            // 
            this.rtxtmsg1.BackColor = System.Drawing.Color.Black;
            this.rtxtmsg1.ContextMenuStrip = this.cm;
            this.rtxtmsg1.Font = new System.Drawing.Font("新細明體", 32.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.rtxtmsg1.ForeColor = System.Drawing.Color.Red;
            this.rtxtmsg1.Location = new System.Drawing.Point(147, 6);
            this.rtxtmsg1.Multiline = false;
            this.rtxtmsg1.Name = "rtxtmsg1";
            this.rtxtmsg1.Size = new System.Drawing.Size(316, 47);
            this.rtxtmsg1.TabIndex = 19;
            this.rtxtmsg1.Text = "";
            this.rtxtmsg1.Enter += new System.EventHandler(this.rtxtmsg1_Enter);
            this.rtxtmsg1.TextChanged += new System.EventHandler(this.rtxtmsg1_TextChanged);
            // 
            // picIcon1
            // 
            this.picIcon1.BackColor = System.Drawing.Color.Black;
            this.picIcon1.Location = new System.Drawing.Point(23, 6);
            this.picIcon1.Name = "picIcon1";
            this.picIcon1.Size = new System.Drawing.Size(118, 94);
            this.picIcon1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picIcon1.TabIndex = 23;
            this.picIcon1.TabStop = false;
            // 
            // picIcon2
            // 
            this.picIcon2.BackColor = System.Drawing.Color.Black;
            this.picIcon2.Location = new System.Drawing.Point(23, 100);
            this.picIcon2.Name = "picIcon2";
            this.picIcon2.Size = new System.Drawing.Size(118, 94);
            this.picIcon2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picIcon2.TabIndex = 24;
            this.picIcon2.TabStop = false;
            // 
            // RgsGenericCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.picIcon2);
            this.Controls.Add(this.rtxtmsg4);
            this.Controls.Add(this.rtxtmsg3);
            this.Controls.Add(this.picIcon1);
            this.Controls.Add(this.rtxtnsg2);
            this.Controls.Add(this.rtxtmsg1);
            this.Name = "RgsGenericCtl";
            this.Size = new System.Drawing.Size(466, 199);
            this.cm.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picIcon1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picIcon2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtxtmsg4;
        private System.Windows.Forms.RichTextBox rtxtmsg3;
        private System.Windows.Forms.RichTextBox rtxtnsg2;
        private System.Windows.Forms.RichTextBox rtxtmsg1;
        private System.Windows.Forms.ContextMenuStrip cm;
        private System.Windows.Forms.ToolStripMenuItem 設定顏色ToolStripMenuItem;
        private System.Windows.Forms.ColorDialog colorChoice;
        private System.Windows.Forms.PictureBox picIcon1;
        private System.Windows.Forms.PictureBox picIcon2;
    }
}
