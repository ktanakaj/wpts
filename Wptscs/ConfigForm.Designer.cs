namespace Honememo.Wptscs
{
    partial class ConfigForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageItems = new System.Windows.Forms.TabPage();
            this.dataGridViewItems = new System.Windows.Forms.DataGridView();
            this.ColumnFromCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnFromTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnAlias = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnArrow = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnToCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnToTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnTimestamp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPageHeadings = new System.Windows.Forms.TabPage();
            this.dataGridViewHeading = new System.Windows.Forms.DataGridView();
            this.tabPageServer = new System.Windows.Forms.TabPage();
            this.groupBoxLanguage = new System.Windows.Forms.GroupBox();
            this.groupBoxLanguageName = new System.Windows.Forms.GroupBox();
            this.dataGridViewLanguageName = new System.Windows.Forms.DataGridView();
            this.ColumnCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnShortName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.textBoxBracket = new System.Windows.Forms.TextBox();
            this.labelBracket = new System.Windows.Forms.Label();
            this.groupBoxServer = new System.Windows.Forms.GroupBox();
            this.textBoxDocumentationTemplateDefaultPage = new System.Windows.Forms.TextBox();
            this.labelDocumentationTemplateDefaultPage = new System.Windows.Forms.Label();
            this.textBoxDocumentationTemplate = new System.Windows.Forms.TextBox();
            this.labelDocumentationTemplate = new System.Windows.Forms.Label();
            this.textBoxFileNamespace = new System.Windows.Forms.TextBox();
            this.labelFileNamespace = new System.Windows.Forms.Label();
            this.textBoxCategoryNamespace = new System.Windows.Forms.TextBox();
            this.labelCategoryNamespace = new System.Windows.Forms.Label();
            this.textBoxTemplateNamespace = new System.Windows.Forms.TextBox();
            this.labelTemplateNamespace = new System.Windows.Forms.Label();
            this.textBoxRedirect = new System.Windows.Forms.TextBox();
            this.labelRedirect = new System.Windows.Forms.Label();
            this.textBoxExportPath = new System.Windows.Forms.TextBox();
            this.labelExportPath = new System.Windows.Forms.Label();
            this.textBoxNamespacePath = new System.Windows.Forms.TextBox();
            this.labelNamespacePath = new System.Windows.Forms.Label();
            this.textBoxXmlns = new System.Windows.Forms.TextBox();
            this.labelXmlns = new System.Windows.Forms.Label();
            this.textBoxLocation = new System.Windows.Forms.TextBox();
            this.labelLocation = new System.Windows.Forms.Label();
            this.comboBoxLanguage = new System.Windows.Forms.ComboBox();
            this.labelLanguage = new System.Windows.Forms.Label();
            this.tabPageApplication = new System.Windows.Forms.TabPage();
            this.groupBoxInformation = new System.Windows.Forms.GroupBox();
            this.labelWebsite = new System.Windows.Forms.Label();
            this.linkLabelWebsite = new System.Windows.Forms.LinkLabel();
            this.labelCopyright = new System.Windows.Forms.Label();
            this.labelApplicationName = new System.Windows.Forms.Label();
            this.groupBoxApplicationConfig = new System.Windows.Forms.GroupBox();
            this.checkBoxIgnoreError = new System.Windows.Forms.CheckBox();
            this.labelRefererNote = new System.Windows.Forms.Label();
            this.labelUserAgentNote = new System.Windows.Forms.Label();
            this.labelChaceNote = new System.Windows.Forms.Label();
            this.textBoxCacheExpire = new System.Windows.Forms.TextBox();
            this.textBoxReferer = new System.Windows.Forms.TextBox();
            this.labelReferer = new System.Windows.Forms.Label();
            this.labelCacheExpire = new System.Windows.Forms.Label();
            this.textBoxUserAgent = new System.Windows.Forms.TextBox();
            this.labelUserAgent = new System.Windows.Forms.Label();
            this.tabControl.SuspendLayout();
            this.tabPageItems.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewItems)).BeginInit();
            this.tabPageHeadings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewHeading)).BeginInit();
            this.tabPageServer.SuspendLayout();
            this.groupBoxLanguage.SuspendLayout();
            this.groupBoxLanguageName.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLanguageName)).BeginInit();
            this.groupBoxServer.SuspendLayout();
            this.tabPageApplication.SuspendLayout();
            this.groupBoxInformation.SuspendLayout();
            this.groupBoxApplicationConfig.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            resources.ApplyResources(this.buttonOk, "buttonOk");
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.ButtonOk_Click);
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // tabControl
            // 
            resources.ApplyResources(this.tabControl, "tabControl");
            this.tabControl.Controls.Add(this.tabPageItems);
            this.tabControl.Controls.Add(this.tabPageHeadings);
            this.tabControl.Controls.Add(this.tabPageServer);
            this.tabControl.Controls.Add(this.tabPageApplication);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            // 
            // tabPageItems
            // 
            this.tabPageItems.Controls.Add(this.dataGridViewItems);
            resources.ApplyResources(this.tabPageItems, "tabPageItems");
            this.tabPageItems.Name = "tabPageItems";
            this.tabPageItems.UseVisualStyleBackColor = true;
            // 
            // dataGridViewItems
            // 
            resources.ApplyResources(this.dataGridViewItems, "dataGridViewItems");
            this.dataGridViewItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnFromCode,
            this.ColumnFromTitle,
            this.ColumnAlias,
            this.ColumnArrow,
            this.ColumnToCode,
            this.ColumnToTitle,
            this.ColumnTimestamp});
            this.dataGridViewItems.Name = "dataGridViewItems";
            this.dataGridViewItems.RowTemplate.Height = 21;
            this.dataGridViewItems.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.DataGridViewItems_RowsAdded);
            // 
            // ColumnFromCode
            // 
            this.ColumnFromCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ColumnFromCode, "ColumnFromCode");
            this.ColumnFromCode.MaxInputLength = 16;
            this.ColumnFromCode.Name = "ColumnFromCode";
            // 
            // ColumnFromTitle
            // 
            this.ColumnFromTitle.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ColumnFromTitle, "ColumnFromTitle");
            this.ColumnFromTitle.MaxInputLength = 255;
            this.ColumnFromTitle.Name = "ColumnFromTitle";
            // 
            // ColumnAlias
            // 
            this.ColumnAlias.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ColumnAlias, "ColumnAlias");
            this.ColumnAlias.MaxInputLength = 255;
            this.ColumnAlias.Name = "ColumnAlias";
            // 
            // ColumnArrow
            // 
            this.ColumnArrow.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ColumnArrow, "ColumnArrow");
            this.ColumnArrow.Name = "ColumnArrow";
            this.ColumnArrow.ReadOnly = true;
            this.ColumnArrow.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnArrow.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColumnToCode
            // 
            this.ColumnToCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ColumnToCode, "ColumnToCode");
            this.ColumnToCode.MaxInputLength = 16;
            this.ColumnToCode.Name = "ColumnToCode";
            // 
            // ColumnToTitle
            // 
            this.ColumnToTitle.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ColumnToTitle, "ColumnToTitle");
            this.ColumnToTitle.MaxInputLength = 255;
            this.ColumnToTitle.Name = "ColumnToTitle";
            // 
            // ColumnTimestamp
            // 
            this.ColumnTimestamp.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ColumnTimestamp, "ColumnTimestamp");
            this.ColumnTimestamp.Name = "ColumnTimestamp";
            // 
            // tabPageHeadings
            // 
            this.tabPageHeadings.Controls.Add(this.dataGridViewHeading);
            resources.ApplyResources(this.tabPageHeadings, "tabPageHeadings");
            this.tabPageHeadings.Name = "tabPageHeadings";
            this.tabPageHeadings.UseVisualStyleBackColor = true;
            // 
            // dataGridViewHeading
            // 
            resources.ApplyResources(this.dataGridViewHeading, "dataGridViewHeading");
            this.dataGridViewHeading.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewHeading.Name = "dataGridViewHeading";
            this.dataGridViewHeading.RowTemplate.Height = 21;
            // 
            // tabPageServer
            // 
            this.tabPageServer.Controls.Add(this.groupBoxLanguage);
            this.tabPageServer.Controls.Add(this.groupBoxServer);
            this.tabPageServer.Controls.Add(this.comboBoxLanguage);
            this.tabPageServer.Controls.Add(this.labelLanguage);
            resources.ApplyResources(this.tabPageServer, "tabPageServer");
            this.tabPageServer.Name = "tabPageServer";
            this.tabPageServer.UseVisualStyleBackColor = true;
            // 
            // groupBoxLanguage
            // 
            resources.ApplyResources(this.groupBoxLanguage, "groupBoxLanguage");
            this.groupBoxLanguage.Controls.Add(this.groupBoxLanguageName);
            this.groupBoxLanguage.Controls.Add(this.textBoxBracket);
            this.groupBoxLanguage.Controls.Add(this.labelBracket);
            this.groupBoxLanguage.Name = "groupBoxLanguage";
            this.groupBoxLanguage.TabStop = false;
            // 
            // groupBoxLanguageName
            // 
            resources.ApplyResources(this.groupBoxLanguageName, "groupBoxLanguageName");
            this.groupBoxLanguageName.Controls.Add(this.dataGridViewLanguageName);
            this.groupBoxLanguageName.Name = "groupBoxLanguageName";
            this.groupBoxLanguageName.TabStop = false;
            // 
            // dataGridViewLanguageName
            // 
            resources.ApplyResources(this.dataGridViewLanguageName, "dataGridViewLanguageName");
            this.dataGridViewLanguageName.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewLanguageName.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnCode,
            this.ColumnName,
            this.ColumnShortName});
            this.dataGridViewLanguageName.Name = "dataGridViewLanguageName";
            this.dataGridViewLanguageName.RowTemplate.Height = 21;
            this.dataGridViewLanguageName.Leave += new System.EventHandler(this.DataGridViewLanguageName_Leave);
            // 
            // ColumnCode
            // 
            this.ColumnCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ColumnCode, "ColumnCode");
            this.ColumnCode.MaxInputLength = 10;
            this.ColumnCode.Name = "ColumnCode";
            // 
            // ColumnName
            // 
            this.ColumnName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ColumnName, "ColumnName");
            this.ColumnName.MaxInputLength = 255;
            this.ColumnName.Name = "ColumnName";
            // 
            // ColumnShortName
            // 
            this.ColumnShortName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.ColumnShortName, "ColumnShortName");
            this.ColumnShortName.MaxInputLength = 20;
            this.ColumnShortName.Name = "ColumnShortName";
            // 
            // textBoxBracket
            // 
            resources.ApplyResources(this.textBoxBracket, "textBoxBracket");
            this.textBoxBracket.Name = "textBoxBracket";
            // 
            // labelBracket
            // 
            resources.ApplyResources(this.labelBracket, "labelBracket");
            this.labelBracket.Name = "labelBracket";
            // 
            // groupBoxServer
            // 
            resources.ApplyResources(this.groupBoxServer, "groupBoxServer");
            this.groupBoxServer.Controls.Add(this.textBoxDocumentationTemplateDefaultPage);
            this.groupBoxServer.Controls.Add(this.labelDocumentationTemplateDefaultPage);
            this.groupBoxServer.Controls.Add(this.textBoxDocumentationTemplate);
            this.groupBoxServer.Controls.Add(this.labelDocumentationTemplate);
            this.groupBoxServer.Controls.Add(this.textBoxFileNamespace);
            this.groupBoxServer.Controls.Add(this.labelFileNamespace);
            this.groupBoxServer.Controls.Add(this.textBoxCategoryNamespace);
            this.groupBoxServer.Controls.Add(this.labelCategoryNamespace);
            this.groupBoxServer.Controls.Add(this.textBoxTemplateNamespace);
            this.groupBoxServer.Controls.Add(this.labelTemplateNamespace);
            this.groupBoxServer.Controls.Add(this.textBoxRedirect);
            this.groupBoxServer.Controls.Add(this.labelRedirect);
            this.groupBoxServer.Controls.Add(this.textBoxExportPath);
            this.groupBoxServer.Controls.Add(this.labelExportPath);
            this.groupBoxServer.Controls.Add(this.textBoxNamespacePath);
            this.groupBoxServer.Controls.Add(this.labelNamespacePath);
            this.groupBoxServer.Controls.Add(this.textBoxXmlns);
            this.groupBoxServer.Controls.Add(this.labelXmlns);
            this.groupBoxServer.Controls.Add(this.textBoxLocation);
            this.groupBoxServer.Controls.Add(this.labelLocation);
            this.groupBoxServer.Name = "groupBoxServer";
            this.groupBoxServer.TabStop = false;
            // 
            // textBoxDocumentationTemplateDefaultPage
            // 
            resources.ApplyResources(this.textBoxDocumentationTemplateDefaultPage, "textBoxDocumentationTemplateDefaultPage");
            this.textBoxDocumentationTemplateDefaultPage.Name = "textBoxDocumentationTemplateDefaultPage";
            // 
            // labelDocumentationTemplateDefaultPage
            // 
            resources.ApplyResources(this.labelDocumentationTemplateDefaultPage, "labelDocumentationTemplateDefaultPage");
            this.labelDocumentationTemplateDefaultPage.Name = "labelDocumentationTemplateDefaultPage";
            // 
            // textBoxDocumentationTemplate
            // 
            resources.ApplyResources(this.textBoxDocumentationTemplate, "textBoxDocumentationTemplate");
            this.textBoxDocumentationTemplate.Name = "textBoxDocumentationTemplate";
            // 
            // labelDocumentationTemplate
            // 
            resources.ApplyResources(this.labelDocumentationTemplate, "labelDocumentationTemplate");
            this.labelDocumentationTemplate.Name = "labelDocumentationTemplate";
            // 
            // textBoxFileNamespace
            // 
            resources.ApplyResources(this.textBoxFileNamespace, "textBoxFileNamespace");
            this.textBoxFileNamespace.Name = "textBoxFileNamespace";
            this.textBoxFileNamespace.Leave += new System.EventHandler(this.TextBoxFileNamespace_Leave);
            // 
            // labelFileNamespace
            // 
            resources.ApplyResources(this.labelFileNamespace, "labelFileNamespace");
            this.labelFileNamespace.Name = "labelFileNamespace";
            // 
            // textBoxCategoryNamespace
            // 
            resources.ApplyResources(this.textBoxCategoryNamespace, "textBoxCategoryNamespace");
            this.textBoxCategoryNamespace.Name = "textBoxCategoryNamespace";
            this.textBoxCategoryNamespace.Leave += new System.EventHandler(this.TextBoxCategoryNamespace_Leave);
            // 
            // labelCategoryNamespace
            // 
            resources.ApplyResources(this.labelCategoryNamespace, "labelCategoryNamespace");
            this.labelCategoryNamespace.Name = "labelCategoryNamespace";
            // 
            // textBoxTemplateNamespace
            // 
            resources.ApplyResources(this.textBoxTemplateNamespace, "textBoxTemplateNamespace");
            this.textBoxTemplateNamespace.Name = "textBoxTemplateNamespace";
            this.textBoxTemplateNamespace.Leave += new System.EventHandler(this.TextBoxTemplateNamespace_Leave);
            // 
            // labelTemplateNamespace
            // 
            resources.ApplyResources(this.labelTemplateNamespace, "labelTemplateNamespace");
            this.labelTemplateNamespace.Name = "labelTemplateNamespace";
            // 
            // textBoxRedirect
            // 
            resources.ApplyResources(this.textBoxRedirect, "textBoxRedirect");
            this.textBoxRedirect.Name = "textBoxRedirect";
            // 
            // labelRedirect
            // 
            resources.ApplyResources(this.labelRedirect, "labelRedirect");
            this.labelRedirect.Name = "labelRedirect";
            // 
            // textBoxExportPath
            // 
            resources.ApplyResources(this.textBoxExportPath, "textBoxExportPath");
            this.textBoxExportPath.Name = "textBoxExportPath";
            // 
            // labelExportPath
            // 
            resources.ApplyResources(this.labelExportPath, "labelExportPath");
            this.labelExportPath.Name = "labelExportPath";
            // 
            // textBoxNamespacePath
            // 
            resources.ApplyResources(this.textBoxNamespacePath, "textBoxNamespacePath");
            this.textBoxNamespacePath.Name = "textBoxNamespacePath";
            // 
            // labelNamespacePath
            // 
            resources.ApplyResources(this.labelNamespacePath, "labelNamespacePath");
            this.labelNamespacePath.Name = "labelNamespacePath";
            // 
            // textBoxXmlns
            // 
            resources.ApplyResources(this.textBoxXmlns, "textBoxXmlns");
            this.textBoxXmlns.Name = "textBoxXmlns";
            // 
            // labelXmlns
            // 
            resources.ApplyResources(this.labelXmlns, "labelXmlns");
            this.labelXmlns.Name = "labelXmlns";
            // 
            // textBoxLocation
            // 
            resources.ApplyResources(this.textBoxLocation, "textBoxLocation");
            this.textBoxLocation.Name = "textBoxLocation";
            // 
            // labelLocation
            // 
            resources.ApplyResources(this.labelLocation, "labelLocation");
            this.labelLocation.Name = "labelLocation";
            // 
            // comboBoxLanguage
            // 
            this.comboBoxLanguage.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxLanguage, "comboBoxLanguage");
            this.comboBoxLanguage.Name = "comboBoxLanguage";
            this.comboBoxLanguage.SelectedIndexChanged += new System.EventHandler(this.ComboBoxLanguuage_SelectedIndexChanged);
            this.comboBoxLanguage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ComboBoxLanguage_KeyDown);
            this.comboBoxLanguage.Leave += new System.EventHandler(this.ComboBoxLanguage_Leave);
            // 
            // labelLanguage
            // 
            resources.ApplyResources(this.labelLanguage, "labelLanguage");
            this.labelLanguage.Name = "labelLanguage";
            // 
            // tabPageApplication
            // 
            this.tabPageApplication.Controls.Add(this.groupBoxInformation);
            this.tabPageApplication.Controls.Add(this.groupBoxApplicationConfig);
            resources.ApplyResources(this.tabPageApplication, "tabPageApplication");
            this.tabPageApplication.Name = "tabPageApplication";
            this.tabPageApplication.UseVisualStyleBackColor = true;
            // 
            // groupBoxInformation
            // 
            resources.ApplyResources(this.groupBoxInformation, "groupBoxInformation");
            this.groupBoxInformation.Controls.Add(this.labelWebsite);
            this.groupBoxInformation.Controls.Add(this.linkLabelWebsite);
            this.groupBoxInformation.Controls.Add(this.labelCopyright);
            this.groupBoxInformation.Controls.Add(this.labelApplicationName);
            this.groupBoxInformation.Name = "groupBoxInformation";
            this.groupBoxInformation.TabStop = false;
            // 
            // labelWebsite
            // 
            resources.ApplyResources(this.labelWebsite, "labelWebsite");
            this.labelWebsite.Name = "labelWebsite";
            // 
            // linkLabelWebsite
            // 
            resources.ApplyResources(this.linkLabelWebsite, "linkLabelWebsite");
            this.linkLabelWebsite.Name = "linkLabelWebsite";
            this.linkLabelWebsite.TabStop = true;
            this.linkLabelWebsite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelWebsite_LinkClicked);
            // 
            // labelCopyright
            // 
            resources.ApplyResources(this.labelCopyright, "labelCopyright");
            this.labelCopyright.Name = "labelCopyright";
            // 
            // labelApplicationName
            // 
            resources.ApplyResources(this.labelApplicationName, "labelApplicationName");
            this.labelApplicationName.Name = "labelApplicationName";
            // 
            // groupBoxApplicationConfig
            // 
            resources.ApplyResources(this.groupBoxApplicationConfig, "groupBoxApplicationConfig");
            this.groupBoxApplicationConfig.Controls.Add(this.checkBoxIgnoreError);
            this.groupBoxApplicationConfig.Controls.Add(this.labelRefererNote);
            this.groupBoxApplicationConfig.Controls.Add(this.labelUserAgentNote);
            this.groupBoxApplicationConfig.Controls.Add(this.labelChaceNote);
            this.groupBoxApplicationConfig.Controls.Add(this.textBoxCacheExpire);
            this.groupBoxApplicationConfig.Controls.Add(this.textBoxReferer);
            this.groupBoxApplicationConfig.Controls.Add(this.labelReferer);
            this.groupBoxApplicationConfig.Controls.Add(this.labelCacheExpire);
            this.groupBoxApplicationConfig.Controls.Add(this.textBoxUserAgent);
            this.groupBoxApplicationConfig.Controls.Add(this.labelUserAgent);
            this.groupBoxApplicationConfig.Name = "groupBoxApplicationConfig";
            this.groupBoxApplicationConfig.TabStop = false;
            // 
            // checkBoxIgnoreError
            // 
            resources.ApplyResources(this.checkBoxIgnoreError, "checkBoxIgnoreError");
            this.checkBoxIgnoreError.Name = "checkBoxIgnoreError";
            this.checkBoxIgnoreError.UseVisualStyleBackColor = true;
            // 
            // labelRefererNote
            // 
            resources.ApplyResources(this.labelRefererNote, "labelRefererNote");
            this.labelRefererNote.Name = "labelRefererNote";
            // 
            // labelUserAgentNote
            // 
            resources.ApplyResources(this.labelUserAgentNote, "labelUserAgentNote");
            this.labelUserAgentNote.Name = "labelUserAgentNote";
            // 
            // labelChaceNote
            // 
            resources.ApplyResources(this.labelChaceNote, "labelChaceNote");
            this.labelChaceNote.Name = "labelChaceNote";
            // 
            // textBoxCacheExpire
            // 
            resources.ApplyResources(this.textBoxCacheExpire, "textBoxCacheExpire");
            this.textBoxCacheExpire.Name = "textBoxCacheExpire";
            this.textBoxCacheExpire.Leave += new System.EventHandler(this.TextBoxCacheExpire_Leave);
            // 
            // textBoxReferer
            // 
            resources.ApplyResources(this.textBoxReferer, "textBoxReferer");
            this.textBoxReferer.Name = "textBoxReferer";
            // 
            // labelReferer
            // 
            resources.ApplyResources(this.labelReferer, "labelReferer");
            this.labelReferer.Name = "labelReferer";
            // 
            // labelCacheExpire
            // 
            resources.ApplyResources(this.labelCacheExpire, "labelCacheExpire");
            this.labelCacheExpire.Name = "labelCacheExpire";
            // 
            // textBoxUserAgent
            // 
            resources.ApplyResources(this.textBoxUserAgent, "textBoxUserAgent");
            this.textBoxUserAgent.Name = "textBoxUserAgent";
            // 
            // labelUserAgent
            // 
            resources.ApplyResources(this.labelUserAgent, "labelUserAgent");
            this.labelUserAgent.Name = "labelUserAgent";
            // 
            // ConfigForm
            // 
            this.AcceptButton = this.buttonOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.MinimizeBox = false;
            this.Name = "ConfigForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.ConfigForm_Load);
            this.tabControl.ResumeLayout(false);
            this.tabPageItems.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewItems)).EndInit();
            this.tabPageHeadings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewHeading)).EndInit();
            this.tabPageServer.ResumeLayout(false);
            this.tabPageServer.PerformLayout();
            this.groupBoxLanguage.ResumeLayout(false);
            this.groupBoxLanguage.PerformLayout();
            this.groupBoxLanguageName.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewLanguageName)).EndInit();
            this.groupBoxServer.ResumeLayout(false);
            this.groupBoxServer.PerformLayout();
            this.tabPageApplication.ResumeLayout(false);
            this.groupBoxInformation.ResumeLayout(false);
            this.groupBoxInformation.PerformLayout();
            this.groupBoxApplicationConfig.ResumeLayout(false);
            this.groupBoxApplicationConfig.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageItems;
        private System.Windows.Forms.TabPage tabPageHeadings;
        private System.Windows.Forms.DataGridView dataGridViewItems;
        private System.Windows.Forms.DataGridView dataGridViewHeading;
        private System.Windows.Forms.TabPage tabPageServer;
        private System.Windows.Forms.TabPage tabPageApplication;
        private System.Windows.Forms.ComboBox comboBoxLanguage;
        private System.Windows.Forms.Label labelLanguage;
        private System.Windows.Forms.TextBox textBoxUserAgent;
        private System.Windows.Forms.Label labelUserAgent;
        private System.Windows.Forms.TextBox textBoxReferer;
        private System.Windows.Forms.Label labelReferer;
        private System.Windows.Forms.Label labelCacheExpire;
        private System.Windows.Forms.TextBox textBoxCacheExpire;
        private System.Windows.Forms.GroupBox groupBoxApplicationConfig;
        private System.Windows.Forms.GroupBox groupBoxInformation;
        private System.Windows.Forms.Label labelApplicationName;
        private System.Windows.Forms.LinkLabel linkLabelWebsite;
        private System.Windows.Forms.Label labelCopyright;
        private System.Windows.Forms.Label labelWebsite;
        private System.Windows.Forms.Label labelRefererNote;
        private System.Windows.Forms.Label labelUserAgentNote;
        private System.Windows.Forms.Label labelChaceNote;
        private System.Windows.Forms.GroupBox groupBoxServer;
        private System.Windows.Forms.TextBox textBoxLocation;
        private System.Windows.Forms.Label labelLocation;
        private System.Windows.Forms.TextBox textBoxXmlns;
        private System.Windows.Forms.Label labelXmlns;
        private System.Windows.Forms.TextBox textBoxNamespacePath;
        private System.Windows.Forms.Label labelNamespacePath;
        private System.Windows.Forms.TextBox textBoxExportPath;
        private System.Windows.Forms.Label labelExportPath;
        private System.Windows.Forms.TextBox textBoxRedirect;
        private System.Windows.Forms.Label labelRedirect;
        private System.Windows.Forms.Label labelTemplateNamespace;
        private System.Windows.Forms.TextBox textBoxTemplateNamespace;
        private System.Windows.Forms.Label labelFileNamespace;
        private System.Windows.Forms.TextBox textBoxCategoryNamespace;
        private System.Windows.Forms.Label labelCategoryNamespace;
        private System.Windows.Forms.TextBox textBoxFileNamespace;
        private System.Windows.Forms.GroupBox groupBoxLanguage;
        private System.Windows.Forms.Label labelBracket;
        private System.Windows.Forms.TextBox textBoxBracket;
        private System.Windows.Forms.GroupBox groupBoxLanguageName;
        private System.Windows.Forms.DataGridView dataGridViewLanguageName;
        private System.Windows.Forms.CheckBox checkBoxIgnoreError;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnFromCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnFromTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnAlias;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnArrow;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnToCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnToTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnTimestamp;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnShortName;
        private System.Windows.Forms.Label labelDocumentationTemplate;
        private System.Windows.Forms.TextBox textBoxDocumentationTemplateDefaultPage;
        private System.Windows.Forms.Label labelDocumentationTemplateDefaultPage;
        private System.Windows.Forms.TextBox textBoxDocumentationTemplate;
    }
}