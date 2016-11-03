namespace WebCrawler
{
    partial class Main
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.tbCheck = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.cbSiteConfig = new System.Windows.Forms.ComboBox();
            this.tbstartPage = new System.Windows.Forms.TextBox();
            this.lbPage = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbendpage = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(56, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "站点配置";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(485, 494);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(169, 36);
            this.button1.TabIndex = 26;
            this.button1.Text = "开始采集";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // tbCheck
            // 
            this.tbCheck.Location = new System.Drawing.Point(58, 79);
            this.tbCheck.Multiline = true;
            this.tbCheck.Name = "tbCheck";
            this.tbCheck.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbCheck.Size = new System.Drawing.Size(756, 391);
            this.tbCheck.TabIndex = 31;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(183, 494);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(144, 36);
            this.button2.TabIndex = 32;
            this.button2.Text = "检测";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // cbSiteConfig
            // 
            this.cbSiteConfig.Font = new System.Drawing.Font("宋体", 9F);
            this.cbSiteConfig.FormattingEnabled = true;
            this.cbSiteConfig.ItemHeight = 12;
            this.cbSiteConfig.Location = new System.Drawing.Point(115, 32);
            this.cbSiteConfig.Name = "cbSiteConfig";
            this.cbSiteConfig.Size = new System.Drawing.Size(127, 20);
            this.cbSiteConfig.TabIndex = 33;
            this.cbSiteConfig.SelectedIndexChanged += new System.EventHandler(this.cbSiteConfig_SelectedIndexChanged);
            // 
            // tbstartPage
            // 
            this.tbstartPage.Location = new System.Drawing.Point(374, 32);
            this.tbstartPage.Name = "tbstartPage";
            this.tbstartPage.Size = new System.Drawing.Size(60, 21);
            this.tbstartPage.TabIndex = 34;
            this.tbstartPage.Text = "1";
            this.tbstartPage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbstartPage_KeyPress);
            // 
            // lbPage
            // 
            this.lbPage.AutoSize = true;
            this.lbPage.Location = new System.Drawing.Point(339, 35);
            this.lbPage.Name = "lbPage";
            this.lbPage.Size = new System.Drawing.Size(29, 12);
            this.lbPage.TabIndex = 35;
            this.lbPage.Text = "页码";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(440, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 12);
            this.label2.TabIndex = 36;
            this.label2.Text = "至";
            // 
            // tbendpage
            // 
            this.tbendpage.Location = new System.Drawing.Point(460, 32);
            this.tbendpage.Name = "tbendpage";
            this.tbendpage.Size = new System.Drawing.Size(64, 21);
            this.tbendpage.TabIndex = 37;
            this.tbendpage.Text = "2";
            this.tbendpage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbendpage_KeyPress);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(0, 562);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(873, 23);
            this.progressBar1.TabIndex = 38;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(872, 586);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.tbendpage);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbPage);
            this.Controls.Add(this.tbstartPage);
            this.Controls.Add(this.cbSiteConfig);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.tbCheck);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Name = "Main";
            this.Text = "采集";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox tbCheck;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ComboBox cbSiteConfig;
        private System.Windows.Forms.TextBox tbstartPage;
        private System.Windows.Forms.Label lbPage;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbendpage;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}

