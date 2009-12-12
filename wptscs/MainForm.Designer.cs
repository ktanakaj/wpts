namespace wptscs
{
    partial class MainForm
    {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.groupBoxTransfer = new System.Windows.Forms.GroupBox();
            this.buttonConfig = new System.Windows.Forms.Button();
            this.labelTarget = new System.Windows.Forms.Label();
            this.comboBoxTarget = new System.Windows.Forms.ComboBox();
            this.labelArrow = new System.Windows.Forms.Label();
            this.linkLabelSourceURL = new System.Windows.Forms.LinkLabel();
            this.labelSource = new System.Windows.Forms.Label();
            this.comboBoxSource = new System.Windows.Forms.ComboBox();
            this.groupBoxSaveDirectory = new System.Windows.Forms.GroupBox();
            this.textBoxSaveDirectory = new System.Windows.Forms.TextBox();
            this.buttonSaveDirectory = new System.Windows.Forms.Button();
            this.groupBoxRun = new System.Windows.Forms.GroupBox();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonRun = new System.Windows.Forms.Button();
            this.textBoxArticle = new System.Windows.Forms.TextBox();
            this.labelArticle = new System.Windows.Forms.Label();
            this.folderBrowserDialogSaveDirectory = new System.Windows.Forms.FolderBrowserDialog();
            this.backgroundWorkerRun = new System.ComponentModel.BackgroundWorker();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.groupBoxTransfer.SuspendLayout();
            this.groupBoxSaveDirectory.SuspendLayout();
            this.groupBoxRun.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxTransfer
            // 
            this.groupBoxTransfer.Controls.Add(this.buttonConfig);
            this.groupBoxTransfer.Controls.Add(this.labelTarget);
            this.groupBoxTransfer.Controls.Add(this.comboBoxTarget);
            this.groupBoxTransfer.Controls.Add(this.labelArrow);
            this.groupBoxTransfer.Controls.Add(this.linkLabelSourceURL);
            this.groupBoxTransfer.Controls.Add(this.labelSource);
            this.groupBoxTransfer.Controls.Add(this.comboBoxSource);
            this.groupBoxTransfer.Location = new System.Drawing.Point(12, 12);
            this.groupBoxTransfer.Name = "groupBoxTransfer";
            this.groupBoxTransfer.Size = new System.Drawing.Size(395, 96);
            this.groupBoxTransfer.TabIndex = 0;
            this.groupBoxTransfer.TabStop = false;
            this.groupBoxTransfer.Text = "翻訳元→先の言語を設定";
            this.toolTip.SetToolTip(this.groupBoxTransfer, "翻訳元・先の言語を選択してください。\r\nコンボボックスに目的の言語コードが存在しない場合は、設定画面で登録を行ってください。");
            // 
            // buttonConfig
            // 
            this.buttonConfig.Location = new System.Drawing.Point(191, 61);
            this.buttonConfig.Name = "buttonConfig";
            this.buttonConfig.Size = new System.Drawing.Size(43, 23);
            this.buttonConfig.TabIndex = 5;
            this.buttonConfig.Text = "設定";
            this.toolTip.SetToolTip(this.buttonConfig, "言語に関する設定を行います。");
            this.buttonConfig.UseVisualStyleBackColor = true;
            this.buttonConfig.Click += new System.EventHandler(this.buttonConfig_Click);
            // 
            // labelTarget
            // 
            this.labelTarget.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelTarget.Location = new System.Drawing.Point(74, 64);
            this.labelTarget.Name = "labelTarget";
            this.labelTarget.Size = new System.Drawing.Size(101, 21);
            this.labelTarget.TabIndex = 6;
            this.labelTarget.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolTip.SetToolTip(this.labelTarget, "翻訳先の言語です。");
            // 
            // comboBoxTarget
            // 
            this.comboBoxTarget.FormattingEnabled = true;
            this.comboBoxTarget.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.comboBoxTarget.Location = new System.Drawing.Point(11, 64);
            this.comboBoxTarget.MaxLength = 10;
            this.comboBoxTarget.Name = "comboBoxTarget";
            this.comboBoxTarget.Size = new System.Drawing.Size(57, 20);
            this.comboBoxTarget.Sorted = true;
            this.comboBoxTarget.TabIndex = 2;
            this.toolTip.SetToolTip(this.comboBoxTarget, "翻訳先の言語を選択、または入力します。\r\n入力の場合、Wikipediaで使用されている言語コードを入力してください。");
            this.comboBoxTarget.SelectedIndexChanged += new System.EventHandler(this.comboBoxTarget_SelectedIndexChanged);
            this.comboBoxTarget.Leave += new System.EventHandler(this.comboBoxTarget_Leave);
            // 
            // labelArrow
            // 
            this.labelArrow.AutoSize = true;
            this.labelArrow.Location = new System.Drawing.Point(75, 46);
            this.labelArrow.Name = "labelArrow";
            this.labelArrow.Size = new System.Drawing.Size(17, 12);
            this.labelArrow.TabIndex = 5;
            this.labelArrow.Text = "↓";
            // 
            // linkLabelSourceURL
            // 
            this.linkLabelSourceURL.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.linkLabelSourceURL.Location = new System.Drawing.Point(191, 21);
            this.linkLabelSourceURL.Name = "linkLabelSourceURL";
            this.linkLabelSourceURL.Size = new System.Drawing.Size(191, 20);
            this.linkLabelSourceURL.TabIndex = 4;
            this.linkLabelSourceURL.TabStop = true;
            this.linkLabelSourceURL.Text = "http://";
            this.linkLabelSourceURL.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolTip.SetToolTip(this.linkLabelSourceURL, "翻訳元WikipediaのURLです。");
            this.linkLabelSourceURL.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelSourceURL_LinkClicked);
            // 
            // labelSource
            // 
            this.labelSource.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelSource.Location = new System.Drawing.Point(74, 21);
            this.labelSource.Name = "labelSource";
            this.labelSource.Size = new System.Drawing.Size(101, 21);
            this.labelSource.TabIndex = 1;
            this.labelSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolTip.SetToolTip(this.labelSource, "翻訳元の言語です。");
            // 
            // comboBoxSource
            // 
            this.comboBoxSource.FormattingEnabled = true;
            this.comboBoxSource.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.comboBoxSource.Location = new System.Drawing.Point(11, 21);
            this.comboBoxSource.MaxLength = 10;
            this.comboBoxSource.Name = "comboBoxSource";
            this.comboBoxSource.Size = new System.Drawing.Size(57, 20);
            this.comboBoxSource.Sorted = true;
            this.comboBoxSource.TabIndex = 0;
            this.toolTip.SetToolTip(this.comboBoxSource, "翻訳元の言語を選択、または入力します。\r\n入力の場合、Wikipediaで使用されている言語コードを入力してください。");
            this.comboBoxSource.SelectedIndexChanged += new System.EventHandler(this.comboBoxSource_SelectedIndexChanged);
            this.comboBoxSource.Leave += new System.EventHandler(this.comboBoxSource_Leave);
            // 
            // groupBoxSaveDirectory
            // 
            this.groupBoxSaveDirectory.Controls.Add(this.textBoxSaveDirectory);
            this.groupBoxSaveDirectory.Controls.Add(this.buttonSaveDirectory);
            this.groupBoxSaveDirectory.Location = new System.Drawing.Point(12, 114);
            this.groupBoxSaveDirectory.Name = "groupBoxSaveDirectory";
            this.groupBoxSaveDirectory.Size = new System.Drawing.Size(329, 49);
            this.groupBoxSaveDirectory.TabIndex = 1;
            this.groupBoxSaveDirectory.TabStop = false;
            this.groupBoxSaveDirectory.Text = "処理結果を出力するフォルダの選択";
            this.toolTip.SetToolTip(this.groupBoxSaveDirectory, "処理結果を出力するフォルダを選択してください。\r\n指定されたフォルダに記事テキストとログを出力します。");
            // 
            // textBoxSaveDirectory
            // 
            this.textBoxSaveDirectory.Location = new System.Drawing.Point(60, 18);
            this.textBoxSaveDirectory.Name = "textBoxSaveDirectory";
            this.textBoxSaveDirectory.Size = new System.Drawing.Size(256, 19);
            this.textBoxSaveDirectory.TabIndex = 1;
            this.toolTip.SetToolTip(this.textBoxSaveDirectory, "処理結果の出力先フォルダは、「参照」ボタンから選択する以外に、こちらに直接指定することもできます。");
            this.textBoxSaveDirectory.Leave += new System.EventHandler(this.textBoxSaveDirectory_Leave);
            // 
            // buttonSaveDirectory
            // 
            this.buttonSaveDirectory.Location = new System.Drawing.Point(11, 16);
            this.buttonSaveDirectory.Name = "buttonSaveDirectory";
            this.buttonSaveDirectory.Size = new System.Drawing.Size(43, 23);
            this.buttonSaveDirectory.TabIndex = 0;
            this.buttonSaveDirectory.Text = "参照";
            this.toolTip.SetToolTip(this.buttonSaveDirectory, "処理結果を出力するフォルダを選択します。");
            this.buttonSaveDirectory.UseVisualStyleBackColor = true;
            this.buttonSaveDirectory.Click += new System.EventHandler(this.buttonSaveDirectory_Click);
            // 
            // groupBoxRun
            // 
            this.groupBoxRun.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxRun.Controls.Add(this.textBoxLog);
            this.groupBoxRun.Controls.Add(this.buttonStop);
            this.groupBoxRun.Controls.Add(this.buttonRun);
            this.groupBoxRun.Controls.Add(this.textBoxArticle);
            this.groupBoxRun.Controls.Add(this.labelArticle);
            this.groupBoxRun.Location = new System.Drawing.Point(12, 169);
            this.groupBoxRun.Name = "groupBoxRun";
            this.groupBoxRun.Size = new System.Drawing.Size(440, 262);
            this.groupBoxRun.TabIndex = 2;
            this.groupBoxRun.TabStop = false;
            this.groupBoxRun.Text = "翻訳する記事を指定して、実行";
            this.toolTip.SetToolTip(this.groupBoxRun, "目的の記事の、翻訳元言語のWikipediaでの記事名を入力し、実行ボタンを押してください。");
            // 
            // textBoxLog
            // 
            this.textBoxLog.AcceptsReturn = true;
            this.textBoxLog.AcceptsTab = true;
            this.textBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxLog.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxLog.Location = new System.Drawing.Point(12, 56);
            this.textBoxLog.MaxLength = 0;
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ReadOnly = true;
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLog.Size = new System.Drawing.Size(416, 193);
            this.textBoxLog.TabIndex = 4;
            this.textBoxLog.TabStop = false;
            // 
            // buttonStop
            // 
            this.buttonStop.Enabled = false;
            this.buttonStop.Location = new System.Drawing.Point(241, 18);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(43, 23);
            this.buttonStop.TabIndex = 3;
            this.buttonStop.Text = "中止";
            this.toolTip.SetToolTip(this.buttonStop, "処理を中断します。");
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonRun
            // 
            this.buttonRun.Location = new System.Drawing.Point(183, 18);
            this.buttonRun.Name = "buttonRun";
            this.buttonRun.Size = new System.Drawing.Size(43, 23);
            this.buttonRun.TabIndex = 2;
            this.buttonRun.Text = "実行";
            this.toolTip.SetToolTip(this.buttonRun, "記事にアクセスし、翻訳支援処理を行います。\r\n作成した翻訳支援ファイルとログは、出力先フォルダに出力されます。");
            this.buttonRun.UseVisualStyleBackColor = true;
            this.buttonRun.Click += new System.EventHandler(this.buttonRun_Click);
            // 
            // textBoxArticle
            // 
            this.textBoxArticle.Location = new System.Drawing.Point(62, 20);
            this.textBoxArticle.MaxLength = 255;
            this.textBoxArticle.Name = "textBoxArticle";
            this.textBoxArticle.Size = new System.Drawing.Size(108, 19);
            this.textBoxArticle.TabIndex = 1;
            this.toolTip.SetToolTip(this.textBoxArticle, "翻訳元Wikipediaでの記事名を入力します。");
            // 
            // labelArticle
            // 
            this.labelArticle.AutoSize = true;
            this.labelArticle.Location = new System.Drawing.Point(15, 23);
            this.labelArticle.Name = "labelArticle";
            this.labelArticle.Size = new System.Drawing.Size(41, 12);
            this.labelArticle.TabIndex = 0;
            this.labelArticle.Text = "記事名";
            this.toolTip.SetToolTip(this.labelArticle, "翻訳元Wikipediaでの記事名を入力します。");
            // 
            // folderBrowserDialogSaveDirectory
            // 
            this.folderBrowserDialogSaveDirectory.Description = "処理結果を出力するフォルダを選択してください。";
            // 
            // backgroundWorkerRun
            // 
            this.backgroundWorkerRun.WorkerSupportsCancellation = true;
            this.backgroundWorkerRun.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerRun_DoWork);
            this.backgroundWorkerRun.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerRun_RunWorkerCompleted);
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 10000;
            this.toolTip.InitialDelay = 500;
            this.toolTip.ReshowDelay = 100;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 443);
            this.Controls.Add(this.groupBoxRun);
            this.Controls.Add(this.groupBoxSaveDirectory);
            this.Controls.Add(this.groupBoxTransfer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(480, 480);
            this.Name = "MainForm";
            this.Text = "Wikipedia 翻訳支援ツール C#";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.groupBoxTransfer.ResumeLayout(false);
            this.groupBoxTransfer.PerformLayout();
            this.groupBoxSaveDirectory.ResumeLayout(false);
            this.groupBoxSaveDirectory.PerformLayout();
            this.groupBoxRun.ResumeLayout(false);
            this.groupBoxRun.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxTransfer;
        private System.Windows.Forms.GroupBox groupBoxSaveDirectory;
        private System.Windows.Forms.GroupBox groupBoxRun;
        private System.Windows.Forms.ComboBox comboBoxSource;
        private System.Windows.Forms.Label labelSource;
        private System.Windows.Forms.LinkLabel linkLabelSourceURL;
        private System.Windows.Forms.ComboBox comboBoxTarget;
        private System.Windows.Forms.Label labelArrow;
        private System.Windows.Forms.Label labelTarget;
        private System.Windows.Forms.Button buttonConfig;
        private System.Windows.Forms.Button buttonSaveDirectory;
        private System.Windows.Forms.TextBox textBoxSaveDirectory;
        private System.Windows.Forms.Label labelArticle;
        private System.Windows.Forms.TextBox textBoxArticle;
        private System.Windows.Forms.Button buttonRun;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.TextBox textBoxLog;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogSaveDirectory;
        private System.ComponentModel.BackgroundWorker backgroundWorkerRun;
        private System.Windows.Forms.ToolTip toolTip;
    }
}

