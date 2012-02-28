// ================================================================================================
// <summary>
//      Wikipedia翻訳支援ツール主画面デザインソース</summary>
//
// <copyright file="MainForm.Designer.cs" company="honeplusのメモ帳">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs
{
    /// <summary>
    /// Wikipedia翻訳支援ツール主画面のクラスです。
    /// </summary>
    public partial class MainForm
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
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelStopwatch = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripDropDownButtonLanguage = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripMenuItemEnglishUS = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemEnglishGB = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemJapanese = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemAuto = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDropDownButtonConfig = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripMenuItemNew = new System.Windows.Forms.ToolStripMenuItem();
            this.timerStatusStopwatch = new System.Windows.Forms.Timer(this.components);
            this.groupBoxTransfer.SuspendLayout();
            this.groupBoxSaveDirectory.SuspendLayout();
            this.groupBoxRun.SuspendLayout();
            this.statusStrip.SuspendLayout();
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
            resources.ApplyResources(this.groupBoxTransfer, "groupBoxTransfer");
            this.groupBoxTransfer.Name = "groupBoxTransfer";
            this.groupBoxTransfer.TabStop = false;
            this.toolTip.SetToolTip(this.groupBoxTransfer, resources.GetString("groupBoxTransfer.ToolTip"));
            // 
            // buttonConfig
            // 
            resources.ApplyResources(this.buttonConfig, "buttonConfig");
            this.buttonConfig.Name = "buttonConfig";
            this.toolTip.SetToolTip(this.buttonConfig, resources.GetString("buttonConfig.ToolTip"));
            this.buttonConfig.UseVisualStyleBackColor = true;
            this.buttonConfig.Click += new System.EventHandler(this.ButtonConfig_Click);
            // 
            // labelTarget
            // 
            this.labelTarget.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.labelTarget, "labelTarget");
            this.labelTarget.Name = "labelTarget";
            this.toolTip.SetToolTip(this.labelTarget, resources.GetString("labelTarget.ToolTip"));
            // 
            // comboBoxTarget
            // 
            this.comboBoxTarget.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTarget.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxTarget, "comboBoxTarget");
            this.comboBoxTarget.Name = "comboBoxTarget";
            this.comboBoxTarget.Sorted = true;
            this.toolTip.SetToolTip(this.comboBoxTarget, resources.GetString("comboBoxTarget.ToolTip"));
            this.comboBoxTarget.SelectedIndexChanged += new System.EventHandler(this.ComboBoxTarget_SelectedIndexChanged);
            // 
            // labelArrow
            // 
            resources.ApplyResources(this.labelArrow, "labelArrow");
            this.labelArrow.Name = "labelArrow";
            // 
            // linkLabelSourceURL
            // 
            this.linkLabelSourceURL.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.linkLabelSourceURL, "linkLabelSourceURL");
            this.linkLabelSourceURL.Name = "linkLabelSourceURL";
            this.linkLabelSourceURL.TabStop = true;
            this.toolTip.SetToolTip(this.linkLabelSourceURL, resources.GetString("linkLabelSourceURL.ToolTip"));
            this.linkLabelSourceURL.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelSourceURL_LinkClicked);
            // 
            // labelSource
            // 
            this.labelSource.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.labelSource, "labelSource");
            this.labelSource.Name = "labelSource";
            this.toolTip.SetToolTip(this.labelSource, resources.GetString("labelSource.ToolTip"));
            // 
            // comboBoxSource
            // 
            this.comboBoxSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxSource.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxSource, "comboBoxSource");
            this.comboBoxSource.Name = "comboBoxSource";
            this.comboBoxSource.Sorted = true;
            this.toolTip.SetToolTip(this.comboBoxSource, resources.GetString("comboBoxSource.ToolTip"));
            this.comboBoxSource.SelectedIndexChanged += new System.EventHandler(this.ComboBoxSource_SelectedIndexChanged);
            // 
            // groupBoxSaveDirectory
            // 
            this.groupBoxSaveDirectory.Controls.Add(this.textBoxSaveDirectory);
            this.groupBoxSaveDirectory.Controls.Add(this.buttonSaveDirectory);
            resources.ApplyResources(this.groupBoxSaveDirectory, "groupBoxSaveDirectory");
            this.groupBoxSaveDirectory.Name = "groupBoxSaveDirectory";
            this.groupBoxSaveDirectory.TabStop = false;
            this.toolTip.SetToolTip(this.groupBoxSaveDirectory, resources.GetString("groupBoxSaveDirectory.ToolTip"));
            // 
            // textBoxSaveDirectory
            // 
            resources.ApplyResources(this.textBoxSaveDirectory, "textBoxSaveDirectory");
            this.textBoxSaveDirectory.Name = "textBoxSaveDirectory";
            this.toolTip.SetToolTip(this.textBoxSaveDirectory, resources.GetString("textBoxSaveDirectory.ToolTip"));
            this.textBoxSaveDirectory.Leave += new System.EventHandler(this.TextBoxSaveDirectory_Leave);
            // 
            // buttonSaveDirectory
            // 
            resources.ApplyResources(this.buttonSaveDirectory, "buttonSaveDirectory");
            this.buttonSaveDirectory.Name = "buttonSaveDirectory";
            this.toolTip.SetToolTip(this.buttonSaveDirectory, resources.GetString("buttonSaveDirectory.ToolTip"));
            this.buttonSaveDirectory.UseVisualStyleBackColor = true;
            this.buttonSaveDirectory.Click += new System.EventHandler(this.ButtonSaveDirectory_Click);
            // 
            // groupBoxRun
            // 
            resources.ApplyResources(this.groupBoxRun, "groupBoxRun");
            this.groupBoxRun.Controls.Add(this.textBoxLog);
            this.groupBoxRun.Controls.Add(this.buttonStop);
            this.groupBoxRun.Controls.Add(this.buttonRun);
            this.groupBoxRun.Controls.Add(this.textBoxArticle);
            this.groupBoxRun.Controls.Add(this.labelArticle);
            this.groupBoxRun.Name = "groupBoxRun";
            this.groupBoxRun.TabStop = false;
            this.toolTip.SetToolTip(this.groupBoxRun, resources.GetString("groupBoxRun.ToolTip"));
            // 
            // textBoxLog
            // 
            this.textBoxLog.AcceptsReturn = true;
            this.textBoxLog.AcceptsTab = true;
            resources.ApplyResources(this.textBoxLog, "textBoxLog");
            this.textBoxLog.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ReadOnly = true;
            this.textBoxLog.TabStop = false;
            // 
            // buttonStop
            // 
            resources.ApplyResources(this.buttonStop, "buttonStop");
            this.buttonStop.Name = "buttonStop";
            this.toolTip.SetToolTip(this.buttonStop, resources.GetString("buttonStop.ToolTip"));
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.ButtonStop_Click);
            // 
            // buttonRun
            // 
            resources.ApplyResources(this.buttonRun, "buttonRun");
            this.buttonRun.Name = "buttonRun";
            this.toolTip.SetToolTip(this.buttonRun, resources.GetString("buttonRun.ToolTip"));
            this.buttonRun.UseVisualStyleBackColor = true;
            this.buttonRun.Click += new System.EventHandler(this.ButtonRun_Click);
            // 
            // textBoxArticle
            // 
            resources.ApplyResources(this.textBoxArticle, "textBoxArticle");
            this.textBoxArticle.Name = "textBoxArticle";
            this.toolTip.SetToolTip(this.textBoxArticle, resources.GetString("textBoxArticle.ToolTip"));
            // 
            // labelArticle
            // 
            resources.ApplyResources(this.labelArticle, "labelArticle");
            this.labelArticle.Name = "labelArticle";
            this.toolTip.SetToolTip(this.labelArticle, resources.GetString("labelArticle.ToolTip"));
            // 
            // folderBrowserDialogSaveDirectory
            // 
            resources.ApplyResources(this.folderBrowserDialogSaveDirectory, "folderBrowserDialogSaveDirectory");
            // 
            // backgroundWorkerRun
            // 
            this.backgroundWorkerRun.WorkerSupportsCancellation = true;
            this.backgroundWorkerRun.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorkerRun_DoWork);
            this.backgroundWorkerRun.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorkerRun_RunWorkerCompleted);
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 10000;
            this.toolTip.InitialDelay = 500;
            this.toolTip.ReshowDelay = 100;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelStatus,
            this.toolStripStatusLabelStopwatch,
            this.toolStripDropDownButtonLanguage,
            this.toolStripDropDownButtonConfig});
            resources.ApplyResources(this.statusStrip, "statusStrip");
            this.statusStrip.Name = "statusStrip";
            // 
            // toolStripStatusLabelStatus
            // 
            this.toolStripStatusLabelStatus.Name = "toolStripStatusLabelStatus";
            resources.ApplyResources(this.toolStripStatusLabelStatus, "toolStripStatusLabelStatus");
            this.toolStripStatusLabelStatus.Spring = true;
            // 
            // toolStripStatusLabelStopwatch
            // 
            this.toolStripStatusLabelStopwatch.Name = "toolStripStatusLabelStopwatch";
            resources.ApplyResources(this.toolStripStatusLabelStopwatch, "toolStripStatusLabelStopwatch");
            // 
            // toolStripDropDownButtonLanguage
            // 
            this.toolStripDropDownButtonLanguage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButtonLanguage.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemEnglishUS,
            this.toolStripMenuItemEnglishGB,
            this.toolStripMenuItemJapanese,
            this.toolStripMenuItemAuto});
            resources.ApplyResources(this.toolStripDropDownButtonLanguage, "toolStripDropDownButtonLanguage");
            this.toolStripDropDownButtonLanguage.Name = "toolStripDropDownButtonLanguage";
            // 
            // toolStripMenuItemEnglishUS
            // 
            this.toolStripMenuItemEnglishUS.Name = "toolStripMenuItemEnglishUS";
            resources.ApplyResources(this.toolStripMenuItemEnglishUS, "toolStripMenuItemEnglishUS");
            this.toolStripMenuItemEnglishUS.Click += new System.EventHandler(this.ToolStripMenuItemEnglishUS_Click);
            // 
            // toolStripMenuItemEnglishGB
            // 
            this.toolStripMenuItemEnglishGB.Name = "toolStripMenuItemEnglishGB";
            resources.ApplyResources(this.toolStripMenuItemEnglishGB, "toolStripMenuItemEnglishGB");
            this.toolStripMenuItemEnglishGB.Click += new System.EventHandler(this.ToolStripMenuItemEnglishGB_Click);
            // 
            // toolStripMenuItemJapanese
            // 
            this.toolStripMenuItemJapanese.Name = "toolStripMenuItemJapanese";
            resources.ApplyResources(this.toolStripMenuItemJapanese, "toolStripMenuItemJapanese");
            this.toolStripMenuItemJapanese.Click += new System.EventHandler(this.ToolStripMenuItemJapanese_Click);
            // 
            // toolStripMenuItemAuto
            // 
            this.toolStripMenuItemAuto.Name = "toolStripMenuItemAuto";
            resources.ApplyResources(this.toolStripMenuItemAuto, "toolStripMenuItemAuto");
            this.toolStripMenuItemAuto.Click += new System.EventHandler(this.ToolStripMenuItemAuto_Click);
            // 
            // toolStripDropDownButtonConfig
            // 
            this.toolStripDropDownButtonConfig.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButtonConfig.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemNew});
            resources.ApplyResources(this.toolStripDropDownButtonConfig, "toolStripDropDownButtonConfig");
            this.toolStripDropDownButtonConfig.Name = "toolStripDropDownButtonConfig";
            // 
            // toolStripMenuItemNew
            // 
            this.toolStripMenuItemNew.Name = "toolStripMenuItemNew";
            resources.ApplyResources(this.toolStripMenuItemNew, "toolStripMenuItemNew");
            this.toolStripMenuItemNew.Click += new System.EventHandler(this.ToolStripMenuItemNew_Click);
            // 
            // timerStatusStopwatch
            // 
            this.timerStatusStopwatch.Interval = 1000;
            this.timerStatusStopwatch.Tick += new System.EventHandler(this.TimerStatusStopwatch_Tick);
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.groupBoxRun);
            this.Controls.Add(this.groupBoxSaveDirectory);
            this.Controls.Add(this.groupBoxTransfer);
            this.Name = "MainForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBoxTransfer.ResumeLayout(false);
            this.groupBoxTransfer.PerformLayout();
            this.groupBoxSaveDirectory.ResumeLayout(false);
            this.groupBoxSaveDirectory.PerformLayout();
            this.groupBoxRun.ResumeLayout(false);
            this.groupBoxRun.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelStatus;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelStopwatch;
        private System.Windows.Forms.Timer timerStatusStopwatch;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonLanguage;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemEnglishUS;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemJapanese;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonConfig;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAuto;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemEnglishGB;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemNew;
    }
}

