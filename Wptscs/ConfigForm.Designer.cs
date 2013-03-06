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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageItems = new System.Windows.Forms.TabPage();
            this.labelItemsNote = new System.Windows.Forms.Label();
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
            this.buttonLanguageRemove = new System.Windows.Forms.Button();
            this.buttonLunguageAdd = new System.Windows.Forms.Button();
            this.groupBoxLanguage = new System.Windows.Forms.GroupBox();
            this.groupBoxLanguageName = new System.Windows.Forms.GroupBox();
            this.dataGridViewLanguageName = new System.Windows.Forms.DataGridView();
            this.ColumnCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnShortName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.textBoxBracket = new System.Windows.Forms.TextBox();
            this.labelBracket = new System.Windows.Forms.Label();
            this.groupBoxServer = new System.Windows.Forms.GroupBox();
            this.textBoxInterlanguageApi = new System.Windows.Forms.TextBox();
            this.labelInterlanguageApi = new System.Windows.Forms.Label();
            this.checkBoxHasLanguagePage = new System.Windows.Forms.CheckBox();
            this.textBoxLangFormat = new System.Windows.Forms.TextBox();
            this.labelLangFormat = new System.Windows.Forms.Label();
            this.textBoxLinkInterwikiFormat = new System.Windows.Forms.TextBox();
            this.labelLinkInterwikiFormat = new System.Windows.Forms.Label();
            this.textBoxDocumentationTemplateDefaultPage = new System.Windows.Forms.TextBox();
            this.labelDocumentationTemplateDefaultPage = new System.Windows.Forms.Label();
            this.textBoxDocumentationTemplate = new System.Windows.Forms.TextBox();
            this.labelDocumentationTemplate = new System.Windows.Forms.Label();
            this.textBoxFileNamespace = new System.Windows.Forms.TextBox();
            this.textBoxCategoryNamespace = new System.Windows.Forms.TextBox();
            this.labelCategoryNamespace = new System.Windows.Forms.Label();
            this.textBoxTemplateNamespace = new System.Windows.Forms.TextBox();
            this.labelTemplateNamespace = new System.Windows.Forms.Label();
            this.textBoxExportPath = new System.Windows.Forms.TextBox();
            this.labelExportPath = new System.Windows.Forms.Label();
            this.textBoxMetaApi = new System.Windows.Forms.TextBox();
            this.labelMetaApi = new System.Windows.Forms.Label();
            this.textBoxLocation = new System.Windows.Forms.TextBox();
            this.labelLocation = new System.Windows.Forms.Label();
            this.labelFileNamespace = new System.Windows.Forms.Label();
            this.comboBoxLanguage = new System.Windows.Forms.ComboBox();
            this.labelLanguage = new System.Windows.Forms.Label();
            this.tabPageApplication = new System.Windows.Forms.TabPage();
            this.groupBoxInformation = new System.Windows.Forms.GroupBox();
            this.labelWebsite = new System.Windows.Forms.Label();
            this.linkLabelWebsite = new System.Windows.Forms.LinkLabel();
            this.labelCopyright = new System.Windows.Forms.Label();
            this.labelApplicationName = new System.Windows.Forms.Label();
            this.groupBoxApplicationConfig = new System.Windows.Forms.GroupBox();
            this.labelApplicationConfigNote = new System.Windows.Forms.Label();
            this.textBoxConnectRetryTime = new System.Windows.Forms.TextBox();
            this.labelConnectRetryTimeNote = new System.Windows.Forms.Label();
            this.labelConnectRetryTime = new System.Windows.Forms.Label();
            this.textBoxMaxConnectRetries = new System.Windows.Forms.TextBox();
            this.labelMaxConnectRetriesNote = new System.Windows.Forms.Label();
            this.labelMaxConnectRetries = new System.Windows.Forms.Label();
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
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
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
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            resources.ApplyResources(this.buttonOk, "buttonOk");
            this.errorProvider.SetError(this.buttonOk, resources.GetString("buttonOk.Error"));
            this.errorProvider.SetIconAlignment(this.buttonOk, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("buttonOk.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.buttonOk, ((int)(resources.GetObject("buttonOk.IconPadding"))));
            this.buttonOk.Name = "buttonOk";
            this.toolTip.SetToolTip(this.buttonOk, resources.GetString("buttonOk.ToolTip"));
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.ButtonOk_Click);
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.CausesValidation = false;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.errorProvider.SetError(this.buttonCancel, resources.GetString("buttonCancel.Error"));
            this.errorProvider.SetIconAlignment(this.buttonCancel, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("buttonCancel.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.buttonCancel, ((int)(resources.GetObject("buttonCancel.IconPadding"))));
            this.buttonCancel.Name = "buttonCancel";
            this.toolTip.SetToolTip(this.buttonCancel, resources.GetString("buttonCancel.ToolTip"));
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // tabControl
            // 
            resources.ApplyResources(this.tabControl, "tabControl");
            this.tabControl.Controls.Add(this.tabPageItems);
            this.tabControl.Controls.Add(this.tabPageHeadings);
            this.tabControl.Controls.Add(this.tabPageServer);
            this.tabControl.Controls.Add(this.tabPageApplication);
            this.errorProvider.SetError(this.tabControl, resources.GetString("tabControl.Error"));
            this.errorProvider.SetIconAlignment(this.tabControl, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tabControl.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.tabControl, ((int)(resources.GetObject("tabControl.IconPadding"))));
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.toolTip.SetToolTip(this.tabControl, resources.GetString("tabControl.ToolTip"));
            // 
            // tabPageItems
            // 
            resources.ApplyResources(this.tabPageItems, "tabPageItems");
            this.tabPageItems.Controls.Add(this.labelItemsNote);
            this.tabPageItems.Controls.Add(this.dataGridViewItems);
            this.errorProvider.SetError(this.tabPageItems, resources.GetString("tabPageItems.Error"));
            this.errorProvider.SetIconAlignment(this.tabPageItems, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tabPageItems.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.tabPageItems, ((int)(resources.GetObject("tabPageItems.IconPadding"))));
            this.tabPageItems.Name = "tabPageItems";
            this.toolTip.SetToolTip(this.tabPageItems, resources.GetString("tabPageItems.ToolTip"));
            this.tabPageItems.UseVisualStyleBackColor = true;
            // 
            // labelItemsNote
            // 
            resources.ApplyResources(this.labelItemsNote, "labelItemsNote");
            this.errorProvider.SetError(this.labelItemsNote, resources.GetString("labelItemsNote.Error"));
            this.errorProvider.SetIconAlignment(this.labelItemsNote, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelItemsNote.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.labelItemsNote, ((int)(resources.GetObject("labelItemsNote.IconPadding"))));
            this.labelItemsNote.Name = "labelItemsNote";
            this.toolTip.SetToolTip(this.labelItemsNote, resources.GetString("labelItemsNote.ToolTip"));
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
            this.errorProvider.SetError(this.dataGridViewItems, resources.GetString("dataGridViewItems.Error"));
            this.errorProvider.SetIconAlignment(this.dataGridViewItems, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("dataGridViewItems.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.dataGridViewItems, ((int)(resources.GetObject("dataGridViewItems.IconPadding"))));
            this.dataGridViewItems.Name = "dataGridViewItems";
            this.dataGridViewItems.RowTemplate.Height = 21;
            this.toolTip.SetToolTip(this.dataGridViewItems, resources.GetString("dataGridViewItems.ToolTip"));
            this.dataGridViewItems.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewItems_CellValidated);
            this.dataGridViewItems.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.DataGridViewItems_CellValidating);
            this.dataGridViewItems.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewItems_CellValueChanged);
            this.dataGridViewItems.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.DataGridViewItems_RowsAdded);
            this.dataGridViewItems.RowValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.ResetErrorText_RowValidated);
            this.dataGridViewItems.RowValidating += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.DataGridViewItems_RowValidating);
            // 
            // ColumnFromCode
            // 
            resources.ApplyResources(this.ColumnFromCode, "ColumnFromCode");
            this.ColumnFromCode.MaxInputLength = 16;
            this.ColumnFromCode.Name = "ColumnFromCode";
            // 
            // ColumnFromTitle
            // 
            resources.ApplyResources(this.ColumnFromTitle, "ColumnFromTitle");
            this.ColumnFromTitle.MaxInputLength = 255;
            this.ColumnFromTitle.Name = "ColumnFromTitle";
            // 
            // ColumnAlias
            // 
            resources.ApplyResources(this.ColumnAlias, "ColumnAlias");
            this.ColumnAlias.MaxInputLength = 255;
            this.ColumnAlias.Name = "ColumnAlias";
            // 
            // ColumnArrow
            // 
            resources.ApplyResources(this.ColumnArrow, "ColumnArrow");
            this.ColumnArrow.Name = "ColumnArrow";
            this.ColumnArrow.ReadOnly = true;
            this.ColumnArrow.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColumnArrow.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColumnToCode
            // 
            resources.ApplyResources(this.ColumnToCode, "ColumnToCode");
            this.ColumnToCode.MaxInputLength = 16;
            this.ColumnToCode.Name = "ColumnToCode";
            // 
            // ColumnToTitle
            // 
            resources.ApplyResources(this.ColumnToTitle, "ColumnToTitle");
            this.ColumnToTitle.MaxInputLength = 255;
            this.ColumnToTitle.Name = "ColumnToTitle";
            // 
            // ColumnTimestamp
            // 
            resources.ApplyResources(this.ColumnTimestamp, "ColumnTimestamp");
            this.ColumnTimestamp.Name = "ColumnTimestamp";
            // 
            // tabPageHeadings
            // 
            resources.ApplyResources(this.tabPageHeadings, "tabPageHeadings");
            this.tabPageHeadings.Controls.Add(this.dataGridViewHeading);
            this.errorProvider.SetError(this.tabPageHeadings, resources.GetString("tabPageHeadings.Error"));
            this.errorProvider.SetIconAlignment(this.tabPageHeadings, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tabPageHeadings.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.tabPageHeadings, ((int)(resources.GetObject("tabPageHeadings.IconPadding"))));
            this.tabPageHeadings.Name = "tabPageHeadings";
            this.toolTip.SetToolTip(this.tabPageHeadings, resources.GetString("tabPageHeadings.ToolTip"));
            this.tabPageHeadings.UseVisualStyleBackColor = true;
            // 
            // dataGridViewHeading
            // 
            resources.ApplyResources(this.dataGridViewHeading, "dataGridViewHeading");
            this.dataGridViewHeading.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewHeading.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewHeading.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewHeading.DefaultCellStyle = dataGridViewCellStyle1;
            this.errorProvider.SetError(this.dataGridViewHeading, resources.GetString("dataGridViewHeading.Error"));
            this.errorProvider.SetIconAlignment(this.dataGridViewHeading, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("dataGridViewHeading.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.dataGridViewHeading, ((int)(resources.GetObject("dataGridViewHeading.IconPadding"))));
            this.dataGridViewHeading.Name = "dataGridViewHeading";
            this.dataGridViewHeading.RowTemplate.Height = 21;
            this.toolTip.SetToolTip(this.dataGridViewHeading, resources.GetString("dataGridViewHeading.ToolTip"));
            // 
            // tabPageServer
            // 
            resources.ApplyResources(this.tabPageServer, "tabPageServer");
            this.tabPageServer.Controls.Add(this.buttonLanguageRemove);
            this.tabPageServer.Controls.Add(this.buttonLunguageAdd);
            this.tabPageServer.Controls.Add(this.groupBoxLanguage);
            this.tabPageServer.Controls.Add(this.groupBoxServer);
            this.tabPageServer.Controls.Add(this.comboBoxLanguage);
            this.tabPageServer.Controls.Add(this.labelLanguage);
            this.errorProvider.SetError(this.tabPageServer, resources.GetString("tabPageServer.Error"));
            this.errorProvider.SetIconAlignment(this.tabPageServer, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tabPageServer.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.tabPageServer, ((int)(resources.GetObject("tabPageServer.IconPadding"))));
            this.tabPageServer.Name = "tabPageServer";
            this.toolTip.SetToolTip(this.tabPageServer, resources.GetString("tabPageServer.ToolTip"));
            this.tabPageServer.UseVisualStyleBackColor = true;
            // 
            // buttonLanguageRemove
            // 
            resources.ApplyResources(this.buttonLanguageRemove, "buttonLanguageRemove");
            this.errorProvider.SetError(this.buttonLanguageRemove, resources.GetString("buttonLanguageRemove.Error"));
            this.errorProvider.SetIconAlignment(this.buttonLanguageRemove, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("buttonLanguageRemove.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.buttonLanguageRemove, ((int)(resources.GetObject("buttonLanguageRemove.IconPadding"))));
            this.buttonLanguageRemove.Name = "buttonLanguageRemove";
            this.toolTip.SetToolTip(this.buttonLanguageRemove, resources.GetString("buttonLanguageRemove.ToolTip"));
            this.buttonLanguageRemove.UseVisualStyleBackColor = true;
            this.buttonLanguageRemove.Click += new System.EventHandler(this.ButtonLanguageRemove_Click);
            // 
            // buttonLunguageAdd
            // 
            resources.ApplyResources(this.buttonLunguageAdd, "buttonLunguageAdd");
            this.errorProvider.SetError(this.buttonLunguageAdd, resources.GetString("buttonLunguageAdd.Error"));
            this.errorProvider.SetIconAlignment(this.buttonLunguageAdd, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("buttonLunguageAdd.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.buttonLunguageAdd, ((int)(resources.GetObject("buttonLunguageAdd.IconPadding"))));
            this.buttonLunguageAdd.Name = "buttonLunguageAdd";
            this.toolTip.SetToolTip(this.buttonLunguageAdd, resources.GetString("buttonLunguageAdd.ToolTip"));
            this.buttonLunguageAdd.UseVisualStyleBackColor = true;
            this.buttonLunguageAdd.Click += new System.EventHandler(this.ButtonLunguageAdd_Click);
            // 
            // groupBoxLanguage
            // 
            resources.ApplyResources(this.groupBoxLanguage, "groupBoxLanguage");
            this.groupBoxLanguage.Controls.Add(this.groupBoxLanguageName);
            this.groupBoxLanguage.Controls.Add(this.textBoxBracket);
            this.groupBoxLanguage.Controls.Add(this.labelBracket);
            this.errorProvider.SetError(this.groupBoxLanguage, resources.GetString("groupBoxLanguage.Error"));
            this.errorProvider.SetIconAlignment(this.groupBoxLanguage, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBoxLanguage.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.groupBoxLanguage, ((int)(resources.GetObject("groupBoxLanguage.IconPadding"))));
            this.groupBoxLanguage.Name = "groupBoxLanguage";
            this.groupBoxLanguage.TabStop = false;
            this.toolTip.SetToolTip(this.groupBoxLanguage, resources.GetString("groupBoxLanguage.ToolTip"));
            // 
            // groupBoxLanguageName
            // 
            resources.ApplyResources(this.groupBoxLanguageName, "groupBoxLanguageName");
            this.groupBoxLanguageName.Controls.Add(this.dataGridViewLanguageName);
            this.errorProvider.SetError(this.groupBoxLanguageName, resources.GetString("groupBoxLanguageName.Error"));
            this.errorProvider.SetIconAlignment(this.groupBoxLanguageName, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBoxLanguageName.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.groupBoxLanguageName, ((int)(resources.GetObject("groupBoxLanguageName.IconPadding"))));
            this.groupBoxLanguageName.Name = "groupBoxLanguageName";
            this.groupBoxLanguageName.TabStop = false;
            this.toolTip.SetToolTip(this.groupBoxLanguageName, resources.GetString("groupBoxLanguageName.ToolTip"));
            // 
            // dataGridViewLanguageName
            // 
            resources.ApplyResources(this.dataGridViewLanguageName, "dataGridViewLanguageName");
            this.dataGridViewLanguageName.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewLanguageName.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewLanguageName.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnCode,
            this.ColumnName,
            this.ColumnShortName});
            this.errorProvider.SetError(this.dataGridViewLanguageName, resources.GetString("dataGridViewLanguageName.Error"));
            this.errorProvider.SetIconAlignment(this.dataGridViewLanguageName, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("dataGridViewLanguageName.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.dataGridViewLanguageName, ((int)(resources.GetObject("dataGridViewLanguageName.IconPadding"))));
            this.dataGridViewLanguageName.Name = "dataGridViewLanguageName";
            this.dataGridViewLanguageName.RowTemplate.Height = 21;
            this.toolTip.SetToolTip(this.dataGridViewLanguageName, resources.GetString("dataGridViewLanguageName.ToolTip"));
            this.dataGridViewLanguageName.RowValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.ResetErrorText_RowValidated);
            this.dataGridViewLanguageName.RowValidating += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.DataGridViewLanguageName_RowValidating);
            this.dataGridViewLanguageName.Validating += new System.ComponentModel.CancelEventHandler(this.DataGridViewLanguageName_Validating);
            this.dataGridViewLanguageName.Validated += new System.EventHandler(this.ResetErrorText_Validated);
            // 
            // ColumnCode
            // 
            resources.ApplyResources(this.ColumnCode, "ColumnCode");
            this.ColumnCode.MaxInputLength = 10;
            this.ColumnCode.Name = "ColumnCode";
            // 
            // ColumnName
            // 
            resources.ApplyResources(this.ColumnName, "ColumnName");
            this.ColumnName.MaxInputLength = 255;
            this.ColumnName.Name = "ColumnName";
            // 
            // ColumnShortName
            // 
            resources.ApplyResources(this.ColumnShortName, "ColumnShortName");
            this.ColumnShortName.MaxInputLength = 20;
            this.ColumnShortName.Name = "ColumnShortName";
            // 
            // textBoxBracket
            // 
            resources.ApplyResources(this.textBoxBracket, "textBoxBracket");
            this.errorProvider.SetError(this.textBoxBracket, resources.GetString("textBoxBracket.Error"));
            this.errorProvider.SetIconAlignment(this.textBoxBracket, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxBracket.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.textBoxBracket, ((int)(resources.GetObject("textBoxBracket.IconPadding"))));
            this.textBoxBracket.Name = "textBoxBracket";
            this.toolTip.SetToolTip(this.textBoxBracket, resources.GetString("textBoxBracket.ToolTip"));
            this.textBoxBracket.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxBracket_Validating);
            this.textBoxBracket.Validated += new System.EventHandler(this.ResetErrorProvider_Validated);
            // 
            // labelBracket
            // 
            resources.ApplyResources(this.labelBracket, "labelBracket");
            this.errorProvider.SetError(this.labelBracket, resources.GetString("labelBracket.Error"));
            this.errorProvider.SetIconAlignment(this.labelBracket, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelBracket.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.labelBracket, ((int)(resources.GetObject("labelBracket.IconPadding"))));
            this.labelBracket.Name = "labelBracket";
            this.toolTip.SetToolTip(this.labelBracket, resources.GetString("labelBracket.ToolTip"));
            // 
            // groupBoxServer
            // 
            resources.ApplyResources(this.groupBoxServer, "groupBoxServer");
            this.groupBoxServer.Controls.Add(this.textBoxInterlanguageApi);
            this.groupBoxServer.Controls.Add(this.labelInterlanguageApi);
            this.groupBoxServer.Controls.Add(this.checkBoxHasLanguagePage);
            this.groupBoxServer.Controls.Add(this.textBoxLangFormat);
            this.groupBoxServer.Controls.Add(this.labelLangFormat);
            this.groupBoxServer.Controls.Add(this.textBoxLinkInterwikiFormat);
            this.groupBoxServer.Controls.Add(this.labelLinkInterwikiFormat);
            this.groupBoxServer.Controls.Add(this.textBoxDocumentationTemplateDefaultPage);
            this.groupBoxServer.Controls.Add(this.labelDocumentationTemplateDefaultPage);
            this.groupBoxServer.Controls.Add(this.textBoxDocumentationTemplate);
            this.groupBoxServer.Controls.Add(this.labelDocumentationTemplate);
            this.groupBoxServer.Controls.Add(this.textBoxFileNamespace);
            this.groupBoxServer.Controls.Add(this.textBoxCategoryNamespace);
            this.groupBoxServer.Controls.Add(this.labelCategoryNamespace);
            this.groupBoxServer.Controls.Add(this.textBoxTemplateNamespace);
            this.groupBoxServer.Controls.Add(this.labelTemplateNamespace);
            this.groupBoxServer.Controls.Add(this.textBoxExportPath);
            this.groupBoxServer.Controls.Add(this.labelExportPath);
            this.groupBoxServer.Controls.Add(this.textBoxMetaApi);
            this.groupBoxServer.Controls.Add(this.labelMetaApi);
            this.groupBoxServer.Controls.Add(this.textBoxLocation);
            this.groupBoxServer.Controls.Add(this.labelLocation);
            this.groupBoxServer.Controls.Add(this.labelFileNamespace);
            this.errorProvider.SetError(this.groupBoxServer, resources.GetString("groupBoxServer.Error"));
            this.errorProvider.SetIconAlignment(this.groupBoxServer, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBoxServer.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.groupBoxServer, ((int)(resources.GetObject("groupBoxServer.IconPadding"))));
            this.groupBoxServer.Name = "groupBoxServer";
            this.groupBoxServer.TabStop = false;
            this.toolTip.SetToolTip(this.groupBoxServer, resources.GetString("groupBoxServer.ToolTip"));
            // 
            // textBoxInterlanguageApi
            // 
            resources.ApplyResources(this.textBoxInterlanguageApi, "textBoxInterlanguageApi");
            this.errorProvider.SetError(this.textBoxInterlanguageApi, resources.GetString("textBoxInterlanguageApi.Error"));
            this.errorProvider.SetIconAlignment(this.textBoxInterlanguageApi, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxInterlanguageApi.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.textBoxInterlanguageApi, ((int)(resources.GetObject("textBoxInterlanguageApi.IconPadding"))));
            this.textBoxInterlanguageApi.Name = "textBoxInterlanguageApi";
            this.toolTip.SetToolTip(this.textBoxInterlanguageApi, resources.GetString("textBoxInterlanguageApi.ToolTip"));
            // 
            // labelInterlanguageApi
            // 
            resources.ApplyResources(this.labelInterlanguageApi, "labelInterlanguageApi");
            this.errorProvider.SetError(this.labelInterlanguageApi, resources.GetString("labelInterlanguageApi.Error"));
            this.errorProvider.SetIconAlignment(this.labelInterlanguageApi, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelInterlanguageApi.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.labelInterlanguageApi, ((int)(resources.GetObject("labelInterlanguageApi.IconPadding"))));
            this.labelInterlanguageApi.Name = "labelInterlanguageApi";
            this.toolTip.SetToolTip(this.labelInterlanguageApi, resources.GetString("labelInterlanguageApi.ToolTip"));
            // 
            // checkBoxHasLanguagePage
            // 
            resources.ApplyResources(this.checkBoxHasLanguagePage, "checkBoxHasLanguagePage");
            this.errorProvider.SetError(this.checkBoxHasLanguagePage, resources.GetString("checkBoxHasLanguagePage.Error"));
            this.errorProvider.SetIconAlignment(this.checkBoxHasLanguagePage, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("checkBoxHasLanguagePage.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.checkBoxHasLanguagePage, ((int)(resources.GetObject("checkBoxHasLanguagePage.IconPadding"))));
            this.checkBoxHasLanguagePage.Name = "checkBoxHasLanguagePage";
            this.toolTip.SetToolTip(this.checkBoxHasLanguagePage, resources.GetString("checkBoxHasLanguagePage.ToolTip"));
            this.checkBoxHasLanguagePage.UseVisualStyleBackColor = true;
            // 
            // textBoxLangFormat
            // 
            resources.ApplyResources(this.textBoxLangFormat, "textBoxLangFormat");
            this.errorProvider.SetError(this.textBoxLangFormat, resources.GetString("textBoxLangFormat.Error"));
            this.errorProvider.SetIconAlignment(this.textBoxLangFormat, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxLangFormat.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.textBoxLangFormat, ((int)(resources.GetObject("textBoxLangFormat.IconPadding"))));
            this.textBoxLangFormat.Name = "textBoxLangFormat";
            this.toolTip.SetToolTip(this.textBoxLangFormat, resources.GetString("textBoxLangFormat.ToolTip"));
            // 
            // labelLangFormat
            // 
            resources.ApplyResources(this.labelLangFormat, "labelLangFormat");
            this.errorProvider.SetError(this.labelLangFormat, resources.GetString("labelLangFormat.Error"));
            this.errorProvider.SetIconAlignment(this.labelLangFormat, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelLangFormat.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.labelLangFormat, ((int)(resources.GetObject("labelLangFormat.IconPadding"))));
            this.labelLangFormat.Name = "labelLangFormat";
            this.toolTip.SetToolTip(this.labelLangFormat, resources.GetString("labelLangFormat.ToolTip"));
            // 
            // textBoxLinkInterwikiFormat
            // 
            resources.ApplyResources(this.textBoxLinkInterwikiFormat, "textBoxLinkInterwikiFormat");
            this.errorProvider.SetError(this.textBoxLinkInterwikiFormat, resources.GetString("textBoxLinkInterwikiFormat.Error"));
            this.errorProvider.SetIconAlignment(this.textBoxLinkInterwikiFormat, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxLinkInterwikiFormat.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.textBoxLinkInterwikiFormat, ((int)(resources.GetObject("textBoxLinkInterwikiFormat.IconPadding"))));
            this.textBoxLinkInterwikiFormat.Name = "textBoxLinkInterwikiFormat";
            this.toolTip.SetToolTip(this.textBoxLinkInterwikiFormat, resources.GetString("textBoxLinkInterwikiFormat.ToolTip"));
            // 
            // labelLinkInterwikiFormat
            // 
            resources.ApplyResources(this.labelLinkInterwikiFormat, "labelLinkInterwikiFormat");
            this.errorProvider.SetError(this.labelLinkInterwikiFormat, resources.GetString("labelLinkInterwikiFormat.Error"));
            this.errorProvider.SetIconAlignment(this.labelLinkInterwikiFormat, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelLinkInterwikiFormat.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.labelLinkInterwikiFormat, ((int)(resources.GetObject("labelLinkInterwikiFormat.IconPadding"))));
            this.labelLinkInterwikiFormat.Name = "labelLinkInterwikiFormat";
            this.toolTip.SetToolTip(this.labelLinkInterwikiFormat, resources.GetString("labelLinkInterwikiFormat.ToolTip"));
            // 
            // textBoxDocumentationTemplateDefaultPage
            // 
            resources.ApplyResources(this.textBoxDocumentationTemplateDefaultPage, "textBoxDocumentationTemplateDefaultPage");
            this.errorProvider.SetError(this.textBoxDocumentationTemplateDefaultPage, resources.GetString("textBoxDocumentationTemplateDefaultPage.Error"));
            this.errorProvider.SetIconAlignment(this.textBoxDocumentationTemplateDefaultPage, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxDocumentationTemplateDefaultPage.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.textBoxDocumentationTemplateDefaultPage, ((int)(resources.GetObject("textBoxDocumentationTemplateDefaultPage.IconPadding"))));
            this.textBoxDocumentationTemplateDefaultPage.Name = "textBoxDocumentationTemplateDefaultPage";
            this.toolTip.SetToolTip(this.textBoxDocumentationTemplateDefaultPage, resources.GetString("textBoxDocumentationTemplateDefaultPage.ToolTip"));
            // 
            // labelDocumentationTemplateDefaultPage
            // 
            resources.ApplyResources(this.labelDocumentationTemplateDefaultPage, "labelDocumentationTemplateDefaultPage");
            this.errorProvider.SetError(this.labelDocumentationTemplateDefaultPage, resources.GetString("labelDocumentationTemplateDefaultPage.Error"));
            this.errorProvider.SetIconAlignment(this.labelDocumentationTemplateDefaultPage, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelDocumentationTemplateDefaultPage.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.labelDocumentationTemplateDefaultPage, ((int)(resources.GetObject("labelDocumentationTemplateDefaultPage.IconPadding"))));
            this.labelDocumentationTemplateDefaultPage.Name = "labelDocumentationTemplateDefaultPage";
            this.toolTip.SetToolTip(this.labelDocumentationTemplateDefaultPage, resources.GetString("labelDocumentationTemplateDefaultPage.ToolTip"));
            // 
            // textBoxDocumentationTemplate
            // 
            this.textBoxDocumentationTemplate.AcceptsReturn = true;
            resources.ApplyResources(this.textBoxDocumentationTemplate, "textBoxDocumentationTemplate");
            this.errorProvider.SetError(this.textBoxDocumentationTemplate, resources.GetString("textBoxDocumentationTemplate.Error"));
            this.errorProvider.SetIconAlignment(this.textBoxDocumentationTemplate, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxDocumentationTemplate.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.textBoxDocumentationTemplate, ((int)(resources.GetObject("textBoxDocumentationTemplate.IconPadding"))));
            this.textBoxDocumentationTemplate.Name = "textBoxDocumentationTemplate";
            this.toolTip.SetToolTip(this.textBoxDocumentationTemplate, resources.GetString("textBoxDocumentationTemplate.ToolTip"));
            // 
            // labelDocumentationTemplate
            // 
            resources.ApplyResources(this.labelDocumentationTemplate, "labelDocumentationTemplate");
            this.errorProvider.SetError(this.labelDocumentationTemplate, resources.GetString("labelDocumentationTemplate.Error"));
            this.errorProvider.SetIconAlignment(this.labelDocumentationTemplate, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelDocumentationTemplate.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.labelDocumentationTemplate, ((int)(resources.GetObject("labelDocumentationTemplate.IconPadding"))));
            this.labelDocumentationTemplate.Name = "labelDocumentationTemplate";
            this.toolTip.SetToolTip(this.labelDocumentationTemplate, resources.GetString("labelDocumentationTemplate.ToolTip"));
            // 
            // textBoxFileNamespace
            // 
            resources.ApplyResources(this.textBoxFileNamespace, "textBoxFileNamespace");
            this.errorProvider.SetError(this.textBoxFileNamespace, resources.GetString("textBoxFileNamespace.Error"));
            this.errorProvider.SetIconAlignment(this.textBoxFileNamespace, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxFileNamespace.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.textBoxFileNamespace, ((int)(resources.GetObject("textBoxFileNamespace.IconPadding"))));
            this.textBoxFileNamespace.Name = "textBoxFileNamespace";
            this.toolTip.SetToolTip(this.textBoxFileNamespace, resources.GetString("textBoxFileNamespace.ToolTip"));
            this.textBoxFileNamespace.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxNamespace_Validating);
            this.textBoxFileNamespace.Validated += new System.EventHandler(this.ResetErrorProvider_Validated);
            // 
            // textBoxCategoryNamespace
            // 
            resources.ApplyResources(this.textBoxCategoryNamespace, "textBoxCategoryNamespace");
            this.errorProvider.SetError(this.textBoxCategoryNamespace, resources.GetString("textBoxCategoryNamespace.Error"));
            this.errorProvider.SetIconAlignment(this.textBoxCategoryNamespace, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxCategoryNamespace.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.textBoxCategoryNamespace, ((int)(resources.GetObject("textBoxCategoryNamespace.IconPadding"))));
            this.textBoxCategoryNamespace.Name = "textBoxCategoryNamespace";
            this.toolTip.SetToolTip(this.textBoxCategoryNamespace, resources.GetString("textBoxCategoryNamespace.ToolTip"));
            this.textBoxCategoryNamespace.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxNamespace_Validating);
            this.textBoxCategoryNamespace.Validated += new System.EventHandler(this.ResetErrorProvider_Validated);
            // 
            // labelCategoryNamespace
            // 
            resources.ApplyResources(this.labelCategoryNamespace, "labelCategoryNamespace");
            this.errorProvider.SetError(this.labelCategoryNamespace, resources.GetString("labelCategoryNamespace.Error"));
            this.errorProvider.SetIconAlignment(this.labelCategoryNamespace, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelCategoryNamespace.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.labelCategoryNamespace, ((int)(resources.GetObject("labelCategoryNamespace.IconPadding"))));
            this.labelCategoryNamespace.Name = "labelCategoryNamespace";
            this.toolTip.SetToolTip(this.labelCategoryNamespace, resources.GetString("labelCategoryNamespace.ToolTip"));
            // 
            // textBoxTemplateNamespace
            // 
            resources.ApplyResources(this.textBoxTemplateNamespace, "textBoxTemplateNamespace");
            this.errorProvider.SetError(this.textBoxTemplateNamespace, resources.GetString("textBoxTemplateNamespace.Error"));
            this.errorProvider.SetIconAlignment(this.textBoxTemplateNamespace, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxTemplateNamespace.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.textBoxTemplateNamespace, ((int)(resources.GetObject("textBoxTemplateNamespace.IconPadding"))));
            this.textBoxTemplateNamespace.Name = "textBoxTemplateNamespace";
            this.toolTip.SetToolTip(this.textBoxTemplateNamespace, resources.GetString("textBoxTemplateNamespace.ToolTip"));
            this.textBoxTemplateNamespace.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxNamespace_Validating);
            this.textBoxTemplateNamespace.Validated += new System.EventHandler(this.ResetErrorProvider_Validated);
            // 
            // labelTemplateNamespace
            // 
            resources.ApplyResources(this.labelTemplateNamespace, "labelTemplateNamespace");
            this.errorProvider.SetError(this.labelTemplateNamespace, resources.GetString("labelTemplateNamespace.Error"));
            this.errorProvider.SetIconAlignment(this.labelTemplateNamespace, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelTemplateNamespace.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.labelTemplateNamespace, ((int)(resources.GetObject("labelTemplateNamespace.IconPadding"))));
            this.labelTemplateNamespace.Name = "labelTemplateNamespace";
            this.toolTip.SetToolTip(this.labelTemplateNamespace, resources.GetString("labelTemplateNamespace.ToolTip"));
            // 
            // textBoxExportPath
            // 
            resources.ApplyResources(this.textBoxExportPath, "textBoxExportPath");
            this.errorProvider.SetError(this.textBoxExportPath, resources.GetString("textBoxExportPath.Error"));
            this.errorProvider.SetIconAlignment(this.textBoxExportPath, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxExportPath.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.textBoxExportPath, ((int)(resources.GetObject("textBoxExportPath.IconPadding"))));
            this.textBoxExportPath.Name = "textBoxExportPath";
            this.toolTip.SetToolTip(this.textBoxExportPath, resources.GetString("textBoxExportPath.ToolTip"));
            // 
            // labelExportPath
            // 
            resources.ApplyResources(this.labelExportPath, "labelExportPath");
            this.errorProvider.SetError(this.labelExportPath, resources.GetString("labelExportPath.Error"));
            this.errorProvider.SetIconAlignment(this.labelExportPath, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelExportPath.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.labelExportPath, ((int)(resources.GetObject("labelExportPath.IconPadding"))));
            this.labelExportPath.Name = "labelExportPath";
            this.toolTip.SetToolTip(this.labelExportPath, resources.GetString("labelExportPath.ToolTip"));
            // 
            // textBoxMetaApi
            // 
            resources.ApplyResources(this.textBoxMetaApi, "textBoxMetaApi");
            this.errorProvider.SetError(this.textBoxMetaApi, resources.GetString("textBoxMetaApi.Error"));
            this.errorProvider.SetIconAlignment(this.textBoxMetaApi, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxMetaApi.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.textBoxMetaApi, ((int)(resources.GetObject("textBoxMetaApi.IconPadding"))));
            this.textBoxMetaApi.Name = "textBoxMetaApi";
            this.toolTip.SetToolTip(this.textBoxMetaApi, resources.GetString("textBoxMetaApi.ToolTip"));
            // 
            // labelMetaApi
            // 
            resources.ApplyResources(this.labelMetaApi, "labelMetaApi");
            this.errorProvider.SetError(this.labelMetaApi, resources.GetString("labelMetaApi.Error"));
            this.errorProvider.SetIconAlignment(this.labelMetaApi, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelMetaApi.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.labelMetaApi, ((int)(resources.GetObject("labelMetaApi.IconPadding"))));
            this.labelMetaApi.Name = "labelMetaApi";
            this.toolTip.SetToolTip(this.labelMetaApi, resources.GetString("labelMetaApi.ToolTip"));
            // 
            // textBoxLocation
            // 
            resources.ApplyResources(this.textBoxLocation, "textBoxLocation");
            this.errorProvider.SetError(this.textBoxLocation, resources.GetString("textBoxLocation.Error"));
            this.errorProvider.SetIconAlignment(this.textBoxLocation, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxLocation.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.textBoxLocation, ((int)(resources.GetObject("textBoxLocation.IconPadding"))));
            this.textBoxLocation.Name = "textBoxLocation";
            this.toolTip.SetToolTip(this.textBoxLocation, resources.GetString("textBoxLocation.ToolTip"));
            // 
            // labelLocation
            // 
            resources.ApplyResources(this.labelLocation, "labelLocation");
            this.errorProvider.SetError(this.labelLocation, resources.GetString("labelLocation.Error"));
            this.errorProvider.SetIconAlignment(this.labelLocation, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelLocation.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.labelLocation, ((int)(resources.GetObject("labelLocation.IconPadding"))));
            this.labelLocation.Name = "labelLocation";
            this.toolTip.SetToolTip(this.labelLocation, resources.GetString("labelLocation.ToolTip"));
            // 
            // labelFileNamespace
            // 
            resources.ApplyResources(this.labelFileNamespace, "labelFileNamespace");
            this.errorProvider.SetError(this.labelFileNamespace, resources.GetString("labelFileNamespace.Error"));
            this.errorProvider.SetIconAlignment(this.labelFileNamespace, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelFileNamespace.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.labelFileNamespace, ((int)(resources.GetObject("labelFileNamespace.IconPadding"))));
            this.labelFileNamespace.Name = "labelFileNamespace";
            this.toolTip.SetToolTip(this.labelFileNamespace, resources.GetString("labelFileNamespace.ToolTip"));
            // 
            // comboBoxLanguage
            // 
            resources.ApplyResources(this.comboBoxLanguage, "comboBoxLanguage");
            this.comboBoxLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.errorProvider.SetError(this.comboBoxLanguage, resources.GetString("comboBoxLanguage.Error"));
            this.comboBoxLanguage.FormattingEnabled = true;
            this.errorProvider.SetIconAlignment(this.comboBoxLanguage, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("comboBoxLanguage.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.comboBoxLanguage, ((int)(resources.GetObject("comboBoxLanguage.IconPadding"))));
            this.comboBoxLanguage.Name = "comboBoxLanguage";
            this.comboBoxLanguage.Sorted = true;
            this.toolTip.SetToolTip(this.comboBoxLanguage, resources.GetString("comboBoxLanguage.ToolTip"));
            this.comboBoxLanguage.SelectedIndexChanged += new System.EventHandler(this.ComboBoxLanguuage_SelectedIndexChanged);
            // 
            // labelLanguage
            // 
            resources.ApplyResources(this.labelLanguage, "labelLanguage");
            this.errorProvider.SetError(this.labelLanguage, resources.GetString("labelLanguage.Error"));
            this.errorProvider.SetIconAlignment(this.labelLanguage, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelLanguage.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.labelLanguage, ((int)(resources.GetObject("labelLanguage.IconPadding"))));
            this.labelLanguage.Name = "labelLanguage";
            this.toolTip.SetToolTip(this.labelLanguage, resources.GetString("labelLanguage.ToolTip"));
            // 
            // tabPageApplication
            // 
            resources.ApplyResources(this.tabPageApplication, "tabPageApplication");
            this.tabPageApplication.Controls.Add(this.groupBoxInformation);
            this.tabPageApplication.Controls.Add(this.groupBoxApplicationConfig);
            this.errorProvider.SetError(this.tabPageApplication, resources.GetString("tabPageApplication.Error"));
            this.errorProvider.SetIconAlignment(this.tabPageApplication, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tabPageApplication.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.tabPageApplication, ((int)(resources.GetObject("tabPageApplication.IconPadding"))));
            this.tabPageApplication.Name = "tabPageApplication";
            this.toolTip.SetToolTip(this.tabPageApplication, resources.GetString("tabPageApplication.ToolTip"));
            this.tabPageApplication.UseVisualStyleBackColor = true;
            // 
            // groupBoxInformation
            // 
            resources.ApplyResources(this.groupBoxInformation, "groupBoxInformation");
            this.groupBoxInformation.Controls.Add(this.labelWebsite);
            this.groupBoxInformation.Controls.Add(this.linkLabelWebsite);
            this.groupBoxInformation.Controls.Add(this.labelCopyright);
            this.groupBoxInformation.Controls.Add(this.labelApplicationName);
            this.errorProvider.SetError(this.groupBoxInformation, resources.GetString("groupBoxInformation.Error"));
            this.errorProvider.SetIconAlignment(this.groupBoxInformation, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBoxInformation.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.groupBoxInformation, ((int)(resources.GetObject("groupBoxInformation.IconPadding"))));
            this.groupBoxInformation.Name = "groupBoxInformation";
            this.groupBoxInformation.TabStop = false;
            this.toolTip.SetToolTip(this.groupBoxInformation, resources.GetString("groupBoxInformation.ToolTip"));
            // 
            // labelWebsite
            // 
            resources.ApplyResources(this.labelWebsite, "labelWebsite");
            this.errorProvider.SetError(this.labelWebsite, resources.GetString("labelWebsite.Error"));
            this.errorProvider.SetIconAlignment(this.labelWebsite, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelWebsite.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.labelWebsite, ((int)(resources.GetObject("labelWebsite.IconPadding"))));
            this.labelWebsite.Name = "labelWebsite";
            this.toolTip.SetToolTip(this.labelWebsite, resources.GetString("labelWebsite.ToolTip"));
            // 
            // linkLabelWebsite
            // 
            resources.ApplyResources(this.linkLabelWebsite, "linkLabelWebsite");
            this.errorProvider.SetError(this.linkLabelWebsite, resources.GetString("linkLabelWebsite.Error"));
            this.errorProvider.SetIconAlignment(this.linkLabelWebsite, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("linkLabelWebsite.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.linkLabelWebsite, ((int)(resources.GetObject("linkLabelWebsite.IconPadding"))));
            this.linkLabelWebsite.Name = "linkLabelWebsite";
            this.linkLabelWebsite.TabStop = true;
            this.toolTip.SetToolTip(this.linkLabelWebsite, resources.GetString("linkLabelWebsite.ToolTip"));
            this.linkLabelWebsite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelWebsite_LinkClicked);
            // 
            // labelCopyright
            // 
            resources.ApplyResources(this.labelCopyright, "labelCopyright");
            this.errorProvider.SetError(this.labelCopyright, resources.GetString("labelCopyright.Error"));
            this.errorProvider.SetIconAlignment(this.labelCopyright, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelCopyright.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.labelCopyright, ((int)(resources.GetObject("labelCopyright.IconPadding"))));
            this.labelCopyright.Name = "labelCopyright";
            this.toolTip.SetToolTip(this.labelCopyright, resources.GetString("labelCopyright.ToolTip"));
            // 
            // labelApplicationName
            // 
            resources.ApplyResources(this.labelApplicationName, "labelApplicationName");
            this.errorProvider.SetError(this.labelApplicationName, resources.GetString("labelApplicationName.Error"));
            this.errorProvider.SetIconAlignment(this.labelApplicationName, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelApplicationName.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.labelApplicationName, ((int)(resources.GetObject("labelApplicationName.IconPadding"))));
            this.labelApplicationName.Name = "labelApplicationName";
            this.toolTip.SetToolTip(this.labelApplicationName, resources.GetString("labelApplicationName.ToolTip"));
            // 
            // groupBoxApplicationConfig
            // 
            resources.ApplyResources(this.groupBoxApplicationConfig, "groupBoxApplicationConfig");
            this.groupBoxApplicationConfig.Controls.Add(this.labelApplicationConfigNote);
            this.groupBoxApplicationConfig.Controls.Add(this.textBoxConnectRetryTime);
            this.groupBoxApplicationConfig.Controls.Add(this.labelConnectRetryTimeNote);
            this.groupBoxApplicationConfig.Controls.Add(this.labelConnectRetryTime);
            this.groupBoxApplicationConfig.Controls.Add(this.textBoxMaxConnectRetries);
            this.groupBoxApplicationConfig.Controls.Add(this.labelMaxConnectRetriesNote);
            this.groupBoxApplicationConfig.Controls.Add(this.labelMaxConnectRetries);
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
            this.errorProvider.SetError(this.groupBoxApplicationConfig, resources.GetString("groupBoxApplicationConfig.Error"));
            this.errorProvider.SetIconAlignment(this.groupBoxApplicationConfig, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBoxApplicationConfig.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.groupBoxApplicationConfig, ((int)(resources.GetObject("groupBoxApplicationConfig.IconPadding"))));
            this.groupBoxApplicationConfig.Name = "groupBoxApplicationConfig";
            this.groupBoxApplicationConfig.TabStop = false;
            this.toolTip.SetToolTip(this.groupBoxApplicationConfig, resources.GetString("groupBoxApplicationConfig.ToolTip"));
            // 
            // labelApplicationConfigNote
            // 
            resources.ApplyResources(this.labelApplicationConfigNote, "labelApplicationConfigNote");
            this.errorProvider.SetError(this.labelApplicationConfigNote, resources.GetString("labelApplicationConfigNote.Error"));
            this.errorProvider.SetIconAlignment(this.labelApplicationConfigNote, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelApplicationConfigNote.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.labelApplicationConfigNote, ((int)(resources.GetObject("labelApplicationConfigNote.IconPadding"))));
            this.labelApplicationConfigNote.Name = "labelApplicationConfigNote";
            this.toolTip.SetToolTip(this.labelApplicationConfigNote, resources.GetString("labelApplicationConfigNote.ToolTip"));
            // 
            // textBoxConnectRetryTime
            // 
            resources.ApplyResources(this.textBoxConnectRetryTime, "textBoxConnectRetryTime");
            this.errorProvider.SetError(this.textBoxConnectRetryTime, resources.GetString("textBoxConnectRetryTime.Error"));
            this.errorProvider.SetIconAlignment(this.textBoxConnectRetryTime, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxConnectRetryTime.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.textBoxConnectRetryTime, ((int)(resources.GetObject("textBoxConnectRetryTime.IconPadding"))));
            this.textBoxConnectRetryTime.Name = "textBoxConnectRetryTime";
            this.toolTip.SetToolTip(this.textBoxConnectRetryTime, resources.GetString("textBoxConnectRetryTime.ToolTip"));
            this.textBoxConnectRetryTime.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxConnectRetryTime_Validating);
            this.textBoxConnectRetryTime.Validated += new System.EventHandler(this.ResetErrorProvider_Validated);
            // 
            // labelConnectRetryTimeNote
            // 
            resources.ApplyResources(this.labelConnectRetryTimeNote, "labelConnectRetryTimeNote");
            this.errorProvider.SetError(this.labelConnectRetryTimeNote, resources.GetString("labelConnectRetryTimeNote.Error"));
            this.errorProvider.SetIconAlignment(this.labelConnectRetryTimeNote, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelConnectRetryTimeNote.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.labelConnectRetryTimeNote, ((int)(resources.GetObject("labelConnectRetryTimeNote.IconPadding"))));
            this.labelConnectRetryTimeNote.Name = "labelConnectRetryTimeNote";
            this.toolTip.SetToolTip(this.labelConnectRetryTimeNote, resources.GetString("labelConnectRetryTimeNote.ToolTip"));
            // 
            // labelConnectRetryTime
            // 
            resources.ApplyResources(this.labelConnectRetryTime, "labelConnectRetryTime");
            this.errorProvider.SetError(this.labelConnectRetryTime, resources.GetString("labelConnectRetryTime.Error"));
            this.errorProvider.SetIconAlignment(this.labelConnectRetryTime, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelConnectRetryTime.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.labelConnectRetryTime, ((int)(resources.GetObject("labelConnectRetryTime.IconPadding"))));
            this.labelConnectRetryTime.Name = "labelConnectRetryTime";
            this.toolTip.SetToolTip(this.labelConnectRetryTime, resources.GetString("labelConnectRetryTime.ToolTip"));
            // 
            // textBoxMaxConnectRetries
            // 
            resources.ApplyResources(this.textBoxMaxConnectRetries, "textBoxMaxConnectRetries");
            this.errorProvider.SetError(this.textBoxMaxConnectRetries, resources.GetString("textBoxMaxConnectRetries.Error"));
            this.errorProvider.SetIconAlignment(this.textBoxMaxConnectRetries, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxMaxConnectRetries.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.textBoxMaxConnectRetries, ((int)(resources.GetObject("textBoxMaxConnectRetries.IconPadding"))));
            this.textBoxMaxConnectRetries.Name = "textBoxMaxConnectRetries";
            this.toolTip.SetToolTip(this.textBoxMaxConnectRetries, resources.GetString("textBoxMaxConnectRetries.ToolTip"));
            this.textBoxMaxConnectRetries.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxMaxConnectRetries_Validating);
            this.textBoxMaxConnectRetries.Validated += new System.EventHandler(this.ResetErrorProvider_Validated);
            // 
            // labelMaxConnectRetriesNote
            // 
            resources.ApplyResources(this.labelMaxConnectRetriesNote, "labelMaxConnectRetriesNote");
            this.errorProvider.SetError(this.labelMaxConnectRetriesNote, resources.GetString("labelMaxConnectRetriesNote.Error"));
            this.errorProvider.SetIconAlignment(this.labelMaxConnectRetriesNote, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelMaxConnectRetriesNote.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.labelMaxConnectRetriesNote, ((int)(resources.GetObject("labelMaxConnectRetriesNote.IconPadding"))));
            this.labelMaxConnectRetriesNote.Name = "labelMaxConnectRetriesNote";
            this.toolTip.SetToolTip(this.labelMaxConnectRetriesNote, resources.GetString("labelMaxConnectRetriesNote.ToolTip"));
            // 
            // labelMaxConnectRetries
            // 
            resources.ApplyResources(this.labelMaxConnectRetries, "labelMaxConnectRetries");
            this.errorProvider.SetError(this.labelMaxConnectRetries, resources.GetString("labelMaxConnectRetries.Error"));
            this.errorProvider.SetIconAlignment(this.labelMaxConnectRetries, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelMaxConnectRetries.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.labelMaxConnectRetries, ((int)(resources.GetObject("labelMaxConnectRetries.IconPadding"))));
            this.labelMaxConnectRetries.Name = "labelMaxConnectRetries";
            this.toolTip.SetToolTip(this.labelMaxConnectRetries, resources.GetString("labelMaxConnectRetries.ToolTip"));
            // 
            // checkBoxIgnoreError
            // 
            resources.ApplyResources(this.checkBoxIgnoreError, "checkBoxIgnoreError");
            this.errorProvider.SetError(this.checkBoxIgnoreError, resources.GetString("checkBoxIgnoreError.Error"));
            this.errorProvider.SetIconAlignment(this.checkBoxIgnoreError, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("checkBoxIgnoreError.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.checkBoxIgnoreError, ((int)(resources.GetObject("checkBoxIgnoreError.IconPadding"))));
            this.checkBoxIgnoreError.Name = "checkBoxIgnoreError";
            this.toolTip.SetToolTip(this.checkBoxIgnoreError, resources.GetString("checkBoxIgnoreError.ToolTip"));
            this.checkBoxIgnoreError.UseVisualStyleBackColor = true;
            // 
            // labelRefererNote
            // 
            resources.ApplyResources(this.labelRefererNote, "labelRefererNote");
            this.errorProvider.SetError(this.labelRefererNote, resources.GetString("labelRefererNote.Error"));
            this.errorProvider.SetIconAlignment(this.labelRefererNote, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelRefererNote.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.labelRefererNote, ((int)(resources.GetObject("labelRefererNote.IconPadding"))));
            this.labelRefererNote.Name = "labelRefererNote";
            this.toolTip.SetToolTip(this.labelRefererNote, resources.GetString("labelRefererNote.ToolTip"));
            // 
            // labelUserAgentNote
            // 
            resources.ApplyResources(this.labelUserAgentNote, "labelUserAgentNote");
            this.errorProvider.SetError(this.labelUserAgentNote, resources.GetString("labelUserAgentNote.Error"));
            this.errorProvider.SetIconAlignment(this.labelUserAgentNote, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelUserAgentNote.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.labelUserAgentNote, ((int)(resources.GetObject("labelUserAgentNote.IconPadding"))));
            this.labelUserAgentNote.Name = "labelUserAgentNote";
            this.toolTip.SetToolTip(this.labelUserAgentNote, resources.GetString("labelUserAgentNote.ToolTip"));
            // 
            // labelChaceNote
            // 
            resources.ApplyResources(this.labelChaceNote, "labelChaceNote");
            this.errorProvider.SetError(this.labelChaceNote, resources.GetString("labelChaceNote.Error"));
            this.errorProvider.SetIconAlignment(this.labelChaceNote, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelChaceNote.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.labelChaceNote, ((int)(resources.GetObject("labelChaceNote.IconPadding"))));
            this.labelChaceNote.Name = "labelChaceNote";
            this.toolTip.SetToolTip(this.labelChaceNote, resources.GetString("labelChaceNote.ToolTip"));
            // 
            // textBoxCacheExpire
            // 
            resources.ApplyResources(this.textBoxCacheExpire, "textBoxCacheExpire");
            this.errorProvider.SetError(this.textBoxCacheExpire, resources.GetString("textBoxCacheExpire.Error"));
            this.errorProvider.SetIconAlignment(this.textBoxCacheExpire, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxCacheExpire.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.textBoxCacheExpire, ((int)(resources.GetObject("textBoxCacheExpire.IconPadding"))));
            this.textBoxCacheExpire.Name = "textBoxCacheExpire";
            this.toolTip.SetToolTip(this.textBoxCacheExpire, resources.GetString("textBoxCacheExpire.ToolTip"));
            this.textBoxCacheExpire.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxCacheExpire_Validating);
            this.textBoxCacheExpire.Validated += new System.EventHandler(this.ResetErrorProvider_Validated);
            // 
            // textBoxReferer
            // 
            resources.ApplyResources(this.textBoxReferer, "textBoxReferer");
            this.errorProvider.SetError(this.textBoxReferer, resources.GetString("textBoxReferer.Error"));
            this.errorProvider.SetIconAlignment(this.textBoxReferer, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxReferer.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.textBoxReferer, ((int)(resources.GetObject("textBoxReferer.IconPadding"))));
            this.textBoxReferer.Name = "textBoxReferer";
            this.toolTip.SetToolTip(this.textBoxReferer, resources.GetString("textBoxReferer.ToolTip"));
            // 
            // labelReferer
            // 
            resources.ApplyResources(this.labelReferer, "labelReferer");
            this.errorProvider.SetError(this.labelReferer, resources.GetString("labelReferer.Error"));
            this.errorProvider.SetIconAlignment(this.labelReferer, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelReferer.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.labelReferer, ((int)(resources.GetObject("labelReferer.IconPadding"))));
            this.labelReferer.Name = "labelReferer";
            this.toolTip.SetToolTip(this.labelReferer, resources.GetString("labelReferer.ToolTip"));
            // 
            // labelCacheExpire
            // 
            resources.ApplyResources(this.labelCacheExpire, "labelCacheExpire");
            this.errorProvider.SetError(this.labelCacheExpire, resources.GetString("labelCacheExpire.Error"));
            this.errorProvider.SetIconAlignment(this.labelCacheExpire, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelCacheExpire.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.labelCacheExpire, ((int)(resources.GetObject("labelCacheExpire.IconPadding"))));
            this.labelCacheExpire.Name = "labelCacheExpire";
            this.toolTip.SetToolTip(this.labelCacheExpire, resources.GetString("labelCacheExpire.ToolTip"));
            // 
            // textBoxUserAgent
            // 
            resources.ApplyResources(this.textBoxUserAgent, "textBoxUserAgent");
            this.errorProvider.SetError(this.textBoxUserAgent, resources.GetString("textBoxUserAgent.Error"));
            this.errorProvider.SetIconAlignment(this.textBoxUserAgent, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxUserAgent.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.textBoxUserAgent, ((int)(resources.GetObject("textBoxUserAgent.IconPadding"))));
            this.textBoxUserAgent.Name = "textBoxUserAgent";
            this.toolTip.SetToolTip(this.textBoxUserAgent, resources.GetString("textBoxUserAgent.ToolTip"));
            // 
            // labelUserAgent
            // 
            resources.ApplyResources(this.labelUserAgent, "labelUserAgent");
            this.errorProvider.SetError(this.labelUserAgent, resources.GetString("labelUserAgent.Error"));
            this.errorProvider.SetIconAlignment(this.labelUserAgent, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelUserAgent.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.labelUserAgent, ((int)(resources.GetObject("labelUserAgent.IconPadding"))));
            this.labelUserAgent.Name = "labelUserAgent";
            this.toolTip.SetToolTip(this.labelUserAgent, resources.GetString("labelUserAgent.ToolTip"));
            // 
            // errorProvider
            // 
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            resources.ApplyResources(this.errorProvider, "errorProvider");
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 10000;
            this.toolTip.InitialDelay = 500;
            this.toolTip.ReshowDelay = 100;
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
            this.toolTip.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.Load += new System.EventHandler(this.ConfigForm_Load);
            this.tabControl.ResumeLayout(false);
            this.tabPageItems.ResumeLayout(false);
            this.tabPageItems.PerformLayout();
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
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
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
        private System.Windows.Forms.TextBox textBoxMetaApi;
        private System.Windows.Forms.Label labelMetaApi;
        private System.Windows.Forms.TextBox textBoxExportPath;
        private System.Windows.Forms.Label labelExportPath;
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
        private System.Windows.Forms.Label labelDocumentationTemplate;
        private System.Windows.Forms.TextBox textBoxDocumentationTemplateDefaultPage;
        private System.Windows.Forms.Label labelDocumentationTemplateDefaultPage;
        private System.Windows.Forms.TextBox textBoxDocumentationTemplate;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Label labelLinkInterwikiFormat;
        private System.Windows.Forms.TextBox textBoxLinkInterwikiFormat;
        private System.Windows.Forms.Button buttonLanguageRemove;
        private System.Windows.Forms.Button buttonLunguageAdd;
        private System.Windows.Forms.TextBox textBoxLangFormat;
        private System.Windows.Forms.Label labelLangFormat;
        private System.Windows.Forms.Label labelMaxConnectRetries;
        private System.Windows.Forms.Label labelConnectRetryTime;
        private System.Windows.Forms.TextBox textBoxMaxConnectRetries;
        private System.Windows.Forms.Label labelMaxConnectRetriesNote;
        private System.Windows.Forms.Label labelConnectRetryTimeNote;
        private System.Windows.Forms.TextBox textBoxConnectRetryTime;
        private System.Windows.Forms.Label labelItemsNote;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnFromCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnFromTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnAlias;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnArrow;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnToCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnToTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnTimestamp;
        private System.Windows.Forms.Label labelApplicationConfigNote;
        private System.Windows.Forms.CheckBox checkBoxHasLanguagePage;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnShortName;
        private System.Windows.Forms.TextBox textBoxInterlanguageApi;
        private System.Windows.Forms.Label labelInterlanguageApi;
    }
}