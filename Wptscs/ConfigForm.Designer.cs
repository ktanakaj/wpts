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
            this.textBoxFileNamespace = new System.Windows.Forms.TextBox();
            this.textBoxCategoryNamespace = new System.Windows.Forms.TextBox();
            this.labelCategoryNamespace = new System.Windows.Forms.Label();
            this.textBoxTemplateNamespace = new System.Windows.Forms.TextBox();
            this.labelTemplateNamespace = new System.Windows.Forms.Label();
            this.textBoxContentApi = new System.Windows.Forms.TextBox();
            this.labelContentApi = new System.Windows.Forms.Label();
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
            this.textBoxRequestInterval = new System.Windows.Forms.TextBox();
            this.labelRequestInterval = new System.Windows.Forms.Label();
            this.textBoxMaxConnectRetries = new System.Windows.Forms.TextBox();
            this.labelMaxConnectRetriesNote = new System.Windows.Forms.Label();
            this.labelMaxConnectRetries = new System.Windows.Forms.Label();
            this.checkBoxIgnoreError = new System.Windows.Forms.CheckBox();
            this.labelRefererNote = new System.Windows.Forms.Label();
            this.labelUserAgentNote = new System.Windows.Forms.Label();
            this.labelCacheNote = new System.Windows.Forms.Label();
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
            this.errorProvider.SetIconAlignment(this.buttonOk, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("buttonOk.IconAlignment"))));
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
            this.errorProvider.SetIconAlignment(this.buttonCancel, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("buttonCancel.IconAlignment"))));
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
            this.errorProvider.SetIconAlignment(this.tabControl, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tabControl.IconAlignment"))));
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            // 
            // tabPageItems
            // 
            this.tabPageItems.Controls.Add(this.labelItemsNote);
            this.tabPageItems.Controls.Add(this.dataGridViewItems);
            this.errorProvider.SetIconAlignment(this.tabPageItems, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tabPageItems.IconAlignment"))));
            resources.ApplyResources(this.tabPageItems, "tabPageItems");
            this.tabPageItems.Name = "tabPageItems";
            this.tabPageItems.UseVisualStyleBackColor = true;
            // 
            // labelItemsNote
            // 
            resources.ApplyResources(this.labelItemsNote, "labelItemsNote");
            this.errorProvider.SetIconAlignment(this.labelItemsNote, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelItemsNote.IconAlignment"))));
            this.labelItemsNote.Name = "labelItemsNote";
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
            this.errorProvider.SetIconAlignment(this.dataGridViewItems, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("dataGridViewItems.IconAlignment"))));
            this.dataGridViewItems.Name = "dataGridViewItems";
            this.dataGridViewItems.RowTemplate.Height = 21;
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
            this.tabPageHeadings.Controls.Add(this.dataGridViewHeading);
            this.errorProvider.SetIconAlignment(this.tabPageHeadings, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tabPageHeadings.IconAlignment"))));
            resources.ApplyResources(this.tabPageHeadings, "tabPageHeadings");
            this.tabPageHeadings.Name = "tabPageHeadings";
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
            this.errorProvider.SetIconAlignment(this.dataGridViewHeading, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("dataGridViewHeading.IconAlignment"))));
            this.dataGridViewHeading.Name = "dataGridViewHeading";
            this.dataGridViewHeading.RowTemplate.Height = 21;
            // 
            // tabPageServer
            // 
            this.tabPageServer.Controls.Add(this.buttonLanguageRemove);
            this.tabPageServer.Controls.Add(this.buttonLunguageAdd);
            this.tabPageServer.Controls.Add(this.groupBoxLanguage);
            this.tabPageServer.Controls.Add(this.groupBoxServer);
            this.tabPageServer.Controls.Add(this.comboBoxLanguage);
            this.tabPageServer.Controls.Add(this.labelLanguage);
            this.errorProvider.SetIconAlignment(this.tabPageServer, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tabPageServer.IconAlignment"))));
            resources.ApplyResources(this.tabPageServer, "tabPageServer");
            this.tabPageServer.Name = "tabPageServer";
            this.tabPageServer.UseVisualStyleBackColor = true;
            // 
            // buttonLanguageRemove
            // 
            resources.ApplyResources(this.buttonLanguageRemove, "buttonLanguageRemove");
            this.errorProvider.SetIconAlignment(this.buttonLanguageRemove, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("buttonLanguageRemove.IconAlignment"))));
            this.buttonLanguageRemove.Name = "buttonLanguageRemove";
            this.toolTip.SetToolTip(this.buttonLanguageRemove, resources.GetString("buttonLanguageRemove.ToolTip"));
            this.buttonLanguageRemove.UseVisualStyleBackColor = true;
            this.buttonLanguageRemove.Click += new System.EventHandler(this.ButtonLanguageRemove_Click);
            // 
            // buttonLunguageAdd
            // 
            this.errorProvider.SetIconAlignment(this.buttonLunguageAdd, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("buttonLunguageAdd.IconAlignment"))));
            resources.ApplyResources(this.buttonLunguageAdd, "buttonLunguageAdd");
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
            this.errorProvider.SetIconAlignment(this.groupBoxLanguage, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBoxLanguage.IconAlignment"))));
            this.groupBoxLanguage.Name = "groupBoxLanguage";
            this.groupBoxLanguage.TabStop = false;
            // 
            // groupBoxLanguageName
            // 
            resources.ApplyResources(this.groupBoxLanguageName, "groupBoxLanguageName");
            this.groupBoxLanguageName.Controls.Add(this.dataGridViewLanguageName);
            this.errorProvider.SetIconAlignment(this.groupBoxLanguageName, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBoxLanguageName.IconAlignment"))));
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
            this.errorProvider.SetIconAlignment(this.dataGridViewLanguageName, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("dataGridViewLanguageName.IconAlignment"))));
            this.dataGridViewLanguageName.Name = "dataGridViewLanguageName";
            this.dataGridViewLanguageName.RowTemplate.Height = 21;
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
            this.errorProvider.SetIconAlignment(this.textBoxBracket, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxBracket.IconAlignment"))));
            resources.ApplyResources(this.textBoxBracket, "textBoxBracket");
            this.textBoxBracket.Name = "textBoxBracket";
            this.toolTip.SetToolTip(this.textBoxBracket, resources.GetString("textBoxBracket.ToolTip"));
            this.textBoxBracket.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxBracket_Validating);
            this.textBoxBracket.Validated += new System.EventHandler(this.ResetErrorProvider_Validated);
            // 
            // labelBracket
            // 
            resources.ApplyResources(this.labelBracket, "labelBracket");
            this.errorProvider.SetIconAlignment(this.labelBracket, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelBracket.IconAlignment"))));
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
            this.groupBoxServer.Controls.Add(this.textBoxFileNamespace);
            this.groupBoxServer.Controls.Add(this.textBoxCategoryNamespace);
            this.groupBoxServer.Controls.Add(this.labelCategoryNamespace);
            this.groupBoxServer.Controls.Add(this.textBoxTemplateNamespace);
            this.groupBoxServer.Controls.Add(this.labelTemplateNamespace);
            this.groupBoxServer.Controls.Add(this.textBoxContentApi);
            this.groupBoxServer.Controls.Add(this.labelContentApi);
            this.groupBoxServer.Controls.Add(this.textBoxMetaApi);
            this.groupBoxServer.Controls.Add(this.labelMetaApi);
            this.groupBoxServer.Controls.Add(this.textBoxLocation);
            this.groupBoxServer.Controls.Add(this.labelLocation);
            this.groupBoxServer.Controls.Add(this.labelFileNamespace);
            this.errorProvider.SetIconAlignment(this.groupBoxServer, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBoxServer.IconAlignment"))));
            this.groupBoxServer.Name = "groupBoxServer";
            this.groupBoxServer.TabStop = false;
            // 
            // textBoxInterlanguageApi
            // 
            this.errorProvider.SetIconAlignment(this.textBoxInterlanguageApi, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxInterlanguageApi.IconAlignment"))));
            resources.ApplyResources(this.textBoxInterlanguageApi, "textBoxInterlanguageApi");
            this.textBoxInterlanguageApi.Name = "textBoxInterlanguageApi";
            this.toolTip.SetToolTip(this.textBoxInterlanguageApi, resources.GetString("textBoxInterlanguageApi.ToolTip"));
            // 
            // labelInterlanguageApi
            // 
            resources.ApplyResources(this.labelInterlanguageApi, "labelInterlanguageApi");
            this.errorProvider.SetIconAlignment(this.labelInterlanguageApi, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelInterlanguageApi.IconAlignment"))));
            this.labelInterlanguageApi.Name = "labelInterlanguageApi";
            this.toolTip.SetToolTip(this.labelInterlanguageApi, resources.GetString("labelInterlanguageApi.ToolTip"));
            // 
            // checkBoxHasLanguagePage
            // 
            resources.ApplyResources(this.checkBoxHasLanguagePage, "checkBoxHasLanguagePage");
            this.errorProvider.SetIconAlignment(this.checkBoxHasLanguagePage, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("checkBoxHasLanguagePage.IconAlignment"))));
            this.checkBoxHasLanguagePage.Name = "checkBoxHasLanguagePage";
            this.toolTip.SetToolTip(this.checkBoxHasLanguagePage, resources.GetString("checkBoxHasLanguagePage.ToolTip"));
            this.checkBoxHasLanguagePage.UseVisualStyleBackColor = true;
            // 
            // textBoxLangFormat
            // 
            this.errorProvider.SetIconAlignment(this.textBoxLangFormat, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxLangFormat.IconAlignment"))));
            resources.ApplyResources(this.textBoxLangFormat, "textBoxLangFormat");
            this.textBoxLangFormat.Name = "textBoxLangFormat";
            this.toolTip.SetToolTip(this.textBoxLangFormat, resources.GetString("textBoxLangFormat.ToolTip"));
            // 
            // labelLangFormat
            // 
            resources.ApplyResources(this.labelLangFormat, "labelLangFormat");
            this.errorProvider.SetIconAlignment(this.labelLangFormat, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelLangFormat.IconAlignment"))));
            this.labelLangFormat.Name = "labelLangFormat";
            this.toolTip.SetToolTip(this.labelLangFormat, resources.GetString("labelLangFormat.ToolTip"));
            // 
            // textBoxLinkInterwikiFormat
            // 
            this.errorProvider.SetIconAlignment(this.textBoxLinkInterwikiFormat, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxLinkInterwikiFormat.IconAlignment"))));
            resources.ApplyResources(this.textBoxLinkInterwikiFormat, "textBoxLinkInterwikiFormat");
            this.textBoxLinkInterwikiFormat.Name = "textBoxLinkInterwikiFormat";
            this.toolTip.SetToolTip(this.textBoxLinkInterwikiFormat, resources.GetString("textBoxLinkInterwikiFormat.ToolTip"));
            // 
            // labelLinkInterwikiFormat
            // 
            resources.ApplyResources(this.labelLinkInterwikiFormat, "labelLinkInterwikiFormat");
            this.errorProvider.SetIconAlignment(this.labelLinkInterwikiFormat, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelLinkInterwikiFormat.IconAlignment"))));
            this.labelLinkInterwikiFormat.Name = "labelLinkInterwikiFormat";
            this.toolTip.SetToolTip(this.labelLinkInterwikiFormat, resources.GetString("labelLinkInterwikiFormat.ToolTip"));
            // 
            // textBoxFileNamespace
            // 
            this.errorProvider.SetIconAlignment(this.textBoxFileNamespace, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxFileNamespace.IconAlignment"))));
            resources.ApplyResources(this.textBoxFileNamespace, "textBoxFileNamespace");
            this.textBoxFileNamespace.Name = "textBoxFileNamespace";
            this.toolTip.SetToolTip(this.textBoxFileNamespace, resources.GetString("textBoxFileNamespace.ToolTip"));
            this.textBoxFileNamespace.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxNamespace_Validating);
            this.textBoxFileNamespace.Validated += new System.EventHandler(this.ResetErrorProvider_Validated);
            // 
            // textBoxCategoryNamespace
            // 
            this.errorProvider.SetIconAlignment(this.textBoxCategoryNamespace, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxCategoryNamespace.IconAlignment"))));
            resources.ApplyResources(this.textBoxCategoryNamespace, "textBoxCategoryNamespace");
            this.textBoxCategoryNamespace.Name = "textBoxCategoryNamespace";
            this.toolTip.SetToolTip(this.textBoxCategoryNamespace, resources.GetString("textBoxCategoryNamespace.ToolTip"));
            this.textBoxCategoryNamespace.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxNamespace_Validating);
            this.textBoxCategoryNamespace.Validated += new System.EventHandler(this.ResetErrorProvider_Validated);
            // 
            // labelCategoryNamespace
            // 
            resources.ApplyResources(this.labelCategoryNamespace, "labelCategoryNamespace");
            this.errorProvider.SetIconAlignment(this.labelCategoryNamespace, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelCategoryNamespace.IconAlignment"))));
            this.labelCategoryNamespace.Name = "labelCategoryNamespace";
            this.toolTip.SetToolTip(this.labelCategoryNamespace, resources.GetString("labelCategoryNamespace.ToolTip"));
            // 
            // textBoxTemplateNamespace
            // 
            this.errorProvider.SetIconAlignment(this.textBoxTemplateNamespace, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxTemplateNamespace.IconAlignment"))));
            resources.ApplyResources(this.textBoxTemplateNamespace, "textBoxTemplateNamespace");
            this.textBoxTemplateNamespace.Name = "textBoxTemplateNamespace";
            this.toolTip.SetToolTip(this.textBoxTemplateNamespace, resources.GetString("textBoxTemplateNamespace.ToolTip"));
            this.textBoxTemplateNamespace.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxNamespace_Validating);
            this.textBoxTemplateNamespace.Validated += new System.EventHandler(this.ResetErrorProvider_Validated);
            // 
            // labelTemplateNamespace
            // 
            resources.ApplyResources(this.labelTemplateNamespace, "labelTemplateNamespace");
            this.errorProvider.SetIconAlignment(this.labelTemplateNamespace, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelTemplateNamespace.IconAlignment"))));
            this.labelTemplateNamespace.Name = "labelTemplateNamespace";
            this.toolTip.SetToolTip(this.labelTemplateNamespace, resources.GetString("labelTemplateNamespace.ToolTip"));
            // 
            // textBoxContentApi
            // 
            this.errorProvider.SetIconAlignment(this.textBoxContentApi, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxContentApi.IconAlignment"))));
            resources.ApplyResources(this.textBoxContentApi, "textBoxContentApi");
            this.textBoxContentApi.Name = "textBoxContentApi";
            this.toolTip.SetToolTip(this.textBoxContentApi, resources.GetString("textBoxContentApi.ToolTip"));
            // 
            // labelContentApi
            // 
            resources.ApplyResources(this.labelContentApi, "labelContentApi");
            this.errorProvider.SetIconAlignment(this.labelContentApi, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelContentApi.IconAlignment"))));
            this.labelContentApi.Name = "labelContentApi";
            this.toolTip.SetToolTip(this.labelContentApi, resources.GetString("labelContentApi.ToolTip"));
            // 
            // textBoxMetaApi
            // 
            this.errorProvider.SetIconAlignment(this.textBoxMetaApi, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxMetaApi.IconAlignment"))));
            resources.ApplyResources(this.textBoxMetaApi, "textBoxMetaApi");
            this.textBoxMetaApi.Name = "textBoxMetaApi";
            this.toolTip.SetToolTip(this.textBoxMetaApi, resources.GetString("textBoxMetaApi.ToolTip"));
            // 
            // labelMetaApi
            // 
            resources.ApplyResources(this.labelMetaApi, "labelMetaApi");
            this.errorProvider.SetIconAlignment(this.labelMetaApi, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelMetaApi.IconAlignment"))));
            this.labelMetaApi.Name = "labelMetaApi";
            this.toolTip.SetToolTip(this.labelMetaApi, resources.GetString("labelMetaApi.ToolTip"));
            // 
            // textBoxLocation
            // 
            this.errorProvider.SetIconAlignment(this.textBoxLocation, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxLocation.IconAlignment"))));
            resources.ApplyResources(this.textBoxLocation, "textBoxLocation");
            this.textBoxLocation.Name = "textBoxLocation";
            this.toolTip.SetToolTip(this.textBoxLocation, resources.GetString("textBoxLocation.ToolTip"));
            // 
            // labelLocation
            // 
            resources.ApplyResources(this.labelLocation, "labelLocation");
            this.errorProvider.SetIconAlignment(this.labelLocation, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelLocation.IconAlignment"))));
            this.labelLocation.Name = "labelLocation";
            this.toolTip.SetToolTip(this.labelLocation, resources.GetString("labelLocation.ToolTip"));
            // 
            // labelFileNamespace
            // 
            resources.ApplyResources(this.labelFileNamespace, "labelFileNamespace");
            this.errorProvider.SetIconAlignment(this.labelFileNamespace, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelFileNamespace.IconAlignment"))));
            this.labelFileNamespace.Name = "labelFileNamespace";
            this.toolTip.SetToolTip(this.labelFileNamespace, resources.GetString("labelFileNamespace.ToolTip"));
            // 
            // comboBoxLanguage
            // 
            this.comboBoxLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLanguage.FormattingEnabled = true;
            this.errorProvider.SetIconAlignment(this.comboBoxLanguage, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("comboBoxLanguage.IconAlignment"))));
            resources.ApplyResources(this.comboBoxLanguage, "comboBoxLanguage");
            this.comboBoxLanguage.Name = "comboBoxLanguage";
            this.comboBoxLanguage.Sorted = true;
            this.toolTip.SetToolTip(this.comboBoxLanguage, resources.GetString("comboBoxLanguage.ToolTip"));
            this.comboBoxLanguage.SelectedIndexChanged += new System.EventHandler(this.ComboBoxLanguuage_SelectedIndexChanged);
            // 
            // labelLanguage
            // 
            resources.ApplyResources(this.labelLanguage, "labelLanguage");
            this.errorProvider.SetIconAlignment(this.labelLanguage, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelLanguage.IconAlignment"))));
            this.labelLanguage.Name = "labelLanguage";
            this.toolTip.SetToolTip(this.labelLanguage, resources.GetString("labelLanguage.ToolTip"));
            // 
            // tabPageApplication
            // 
            this.tabPageApplication.Controls.Add(this.groupBoxInformation);
            this.tabPageApplication.Controls.Add(this.groupBoxApplicationConfig);
            this.errorProvider.SetIconAlignment(this.tabPageApplication, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tabPageApplication.IconAlignment"))));
            resources.ApplyResources(this.tabPageApplication, "tabPageApplication");
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
            this.errorProvider.SetIconAlignment(this.groupBoxInformation, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBoxInformation.IconAlignment"))));
            this.groupBoxInformation.Name = "groupBoxInformation";
            this.groupBoxInformation.TabStop = false;
            // 
            // labelWebsite
            // 
            resources.ApplyResources(this.labelWebsite, "labelWebsite");
            this.errorProvider.SetIconAlignment(this.labelWebsite, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelWebsite.IconAlignment"))));
            this.labelWebsite.Name = "labelWebsite";
            // 
            // linkLabelWebsite
            // 
            resources.ApplyResources(this.linkLabelWebsite, "linkLabelWebsite");
            this.errorProvider.SetIconAlignment(this.linkLabelWebsite, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("linkLabelWebsite.IconAlignment"))));
            this.linkLabelWebsite.Name = "linkLabelWebsite";
            this.linkLabelWebsite.TabStop = true;
            this.linkLabelWebsite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LinkLabelWebsite_LinkClicked);
            // 
            // labelCopyright
            // 
            resources.ApplyResources(this.labelCopyright, "labelCopyright");
            this.errorProvider.SetIconAlignment(this.labelCopyright, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelCopyright.IconAlignment"))));
            this.labelCopyright.Name = "labelCopyright";
            // 
            // labelApplicationName
            // 
            resources.ApplyResources(this.labelApplicationName, "labelApplicationName");
            this.errorProvider.SetIconAlignment(this.labelApplicationName, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelApplicationName.IconAlignment"))));
            this.labelApplicationName.Name = "labelApplicationName";
            // 
            // groupBoxApplicationConfig
            // 
            resources.ApplyResources(this.groupBoxApplicationConfig, "groupBoxApplicationConfig");
            this.groupBoxApplicationConfig.Controls.Add(this.labelApplicationConfigNote);
            this.groupBoxApplicationConfig.Controls.Add(this.textBoxRequestInterval);
            this.groupBoxApplicationConfig.Controls.Add(this.labelRequestInterval);
            this.groupBoxApplicationConfig.Controls.Add(this.textBoxMaxConnectRetries);
            this.groupBoxApplicationConfig.Controls.Add(this.labelMaxConnectRetriesNote);
            this.groupBoxApplicationConfig.Controls.Add(this.labelMaxConnectRetries);
            this.groupBoxApplicationConfig.Controls.Add(this.checkBoxIgnoreError);
            this.groupBoxApplicationConfig.Controls.Add(this.labelRefererNote);
            this.groupBoxApplicationConfig.Controls.Add(this.labelUserAgentNote);
            this.groupBoxApplicationConfig.Controls.Add(this.labelCacheNote);
            this.groupBoxApplicationConfig.Controls.Add(this.textBoxCacheExpire);
            this.groupBoxApplicationConfig.Controls.Add(this.textBoxReferer);
            this.groupBoxApplicationConfig.Controls.Add(this.labelReferer);
            this.groupBoxApplicationConfig.Controls.Add(this.labelCacheExpire);
            this.groupBoxApplicationConfig.Controls.Add(this.textBoxUserAgent);
            this.groupBoxApplicationConfig.Controls.Add(this.labelUserAgent);
            this.errorProvider.SetIconAlignment(this.groupBoxApplicationConfig, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBoxApplicationConfig.IconAlignment"))));
            this.groupBoxApplicationConfig.Name = "groupBoxApplicationConfig";
            this.groupBoxApplicationConfig.TabStop = false;
            // 
            // labelApplicationConfigNote
            // 
            resources.ApplyResources(this.labelApplicationConfigNote, "labelApplicationConfigNote");
            this.errorProvider.SetIconAlignment(this.labelApplicationConfigNote, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelApplicationConfigNote.IconAlignment"))));
            this.labelApplicationConfigNote.Name = "labelApplicationConfigNote";
            // 
            // textBoxRequestInterval
            // 
            this.errorProvider.SetIconAlignment(this.textBoxRequestInterval, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxRequestInterval.IconAlignment"))));
            resources.ApplyResources(this.textBoxRequestInterval, "textBoxRequestInterval");
            this.textBoxRequestInterval.Name = "textBoxRequestInterval";
            this.toolTip.SetToolTip(this.textBoxRequestInterval, resources.GetString("textBoxRequestInterval.ToolTip"));
            this.textBoxRequestInterval.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxConnectRetryTime_Validating);
            this.textBoxRequestInterval.Validated += new System.EventHandler(this.ResetErrorProvider_Validated);
            // 
            // labelRequestInterval
            // 
            resources.ApplyResources(this.labelRequestInterval, "labelRequestInterval");
            this.errorProvider.SetIconAlignment(this.labelRequestInterval, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelRequestInterval.IconAlignment"))));
            this.labelRequestInterval.Name = "labelRequestInterval";
            this.toolTip.SetToolTip(this.labelRequestInterval, resources.GetString("labelRequestInterval.ToolTip"));
            // 
            // textBoxMaxConnectRetries
            // 
            this.errorProvider.SetIconAlignment(this.textBoxMaxConnectRetries, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxMaxConnectRetries.IconAlignment"))));
            resources.ApplyResources(this.textBoxMaxConnectRetries, "textBoxMaxConnectRetries");
            this.textBoxMaxConnectRetries.Name = "textBoxMaxConnectRetries";
            this.toolTip.SetToolTip(this.textBoxMaxConnectRetries, resources.GetString("textBoxMaxConnectRetries.ToolTip"));
            this.textBoxMaxConnectRetries.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxMaxConnectRetries_Validating);
            this.textBoxMaxConnectRetries.Validated += new System.EventHandler(this.ResetErrorProvider_Validated);
            // 
            // labelMaxConnectRetriesNote
            // 
            resources.ApplyResources(this.labelMaxConnectRetriesNote, "labelMaxConnectRetriesNote");
            this.errorProvider.SetIconAlignment(this.labelMaxConnectRetriesNote, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelMaxConnectRetriesNote.IconAlignment"))));
            this.labelMaxConnectRetriesNote.Name = "labelMaxConnectRetriesNote";
            // 
            // labelMaxConnectRetries
            // 
            resources.ApplyResources(this.labelMaxConnectRetries, "labelMaxConnectRetries");
            this.errorProvider.SetIconAlignment(this.labelMaxConnectRetries, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelMaxConnectRetries.IconAlignment"))));
            this.labelMaxConnectRetries.Name = "labelMaxConnectRetries";
            this.toolTip.SetToolTip(this.labelMaxConnectRetries, resources.GetString("labelMaxConnectRetries.ToolTip"));
            // 
            // checkBoxIgnoreError
            // 
            resources.ApplyResources(this.checkBoxIgnoreError, "checkBoxIgnoreError");
            this.errorProvider.SetIconAlignment(this.checkBoxIgnoreError, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("checkBoxIgnoreError.IconAlignment"))));
            this.checkBoxIgnoreError.Name = "checkBoxIgnoreError";
            this.toolTip.SetToolTip(this.checkBoxIgnoreError, resources.GetString("checkBoxIgnoreError.ToolTip"));
            this.checkBoxIgnoreError.UseVisualStyleBackColor = true;
            // 
            // labelRefererNote
            // 
            resources.ApplyResources(this.labelRefererNote, "labelRefererNote");
            this.errorProvider.SetIconAlignment(this.labelRefererNote, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelRefererNote.IconAlignment"))));
            this.labelRefererNote.Name = "labelRefererNote";
            // 
            // labelUserAgentNote
            // 
            resources.ApplyResources(this.labelUserAgentNote, "labelUserAgentNote");
            this.errorProvider.SetIconAlignment(this.labelUserAgentNote, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelUserAgentNote.IconAlignment"))));
            this.labelUserAgentNote.Name = "labelUserAgentNote";
            // 
            // labelCacheNote
            // 
            resources.ApplyResources(this.labelCacheNote, "labelCacheNote");
            this.errorProvider.SetIconAlignment(this.labelCacheNote, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelCacheNote.IconAlignment"))));
            this.labelCacheNote.Name = "labelCacheNote";
            // 
            // textBoxCacheExpire
            // 
            this.errorProvider.SetIconAlignment(this.textBoxCacheExpire, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxCacheExpire.IconAlignment"))));
            resources.ApplyResources(this.textBoxCacheExpire, "textBoxCacheExpire");
            this.textBoxCacheExpire.Name = "textBoxCacheExpire";
            this.toolTip.SetToolTip(this.textBoxCacheExpire, resources.GetString("textBoxCacheExpire.ToolTip"));
            this.textBoxCacheExpire.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxCacheExpire_Validating);
            this.textBoxCacheExpire.Validated += new System.EventHandler(this.ResetErrorProvider_Validated);
            // 
            // textBoxReferer
            // 
            this.errorProvider.SetIconAlignment(this.textBoxReferer, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxReferer.IconAlignment"))));
            resources.ApplyResources(this.textBoxReferer, "textBoxReferer");
            this.textBoxReferer.Name = "textBoxReferer";
            this.toolTip.SetToolTip(this.textBoxReferer, resources.GetString("textBoxReferer.ToolTip"));
            // 
            // labelReferer
            // 
            resources.ApplyResources(this.labelReferer, "labelReferer");
            this.errorProvider.SetIconAlignment(this.labelReferer, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelReferer.IconAlignment"))));
            this.labelReferer.Name = "labelReferer";
            this.toolTip.SetToolTip(this.labelReferer, resources.GetString("labelReferer.ToolTip"));
            // 
            // labelCacheExpire
            // 
            resources.ApplyResources(this.labelCacheExpire, "labelCacheExpire");
            this.errorProvider.SetIconAlignment(this.labelCacheExpire, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelCacheExpire.IconAlignment"))));
            this.labelCacheExpire.Name = "labelCacheExpire";
            this.toolTip.SetToolTip(this.labelCacheExpire, resources.GetString("labelCacheExpire.ToolTip"));
            // 
            // textBoxUserAgent
            // 
            this.errorProvider.SetIconAlignment(this.textBoxUserAgent, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBoxUserAgent.IconAlignment"))));
            resources.ApplyResources(this.textBoxUserAgent, "textBoxUserAgent");
            this.textBoxUserAgent.Name = "textBoxUserAgent";
            this.toolTip.SetToolTip(this.textBoxUserAgent, resources.GetString("textBoxUserAgent.ToolTip"));
            // 
            // labelUserAgent
            // 
            resources.ApplyResources(this.labelUserAgent, "labelUserAgent");
            this.errorProvider.SetIconAlignment(this.labelUserAgent, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("labelUserAgent.IconAlignment"))));
            this.labelUserAgent.Name = "labelUserAgent";
            this.toolTip.SetToolTip(this.labelUserAgent, resources.GetString("labelUserAgent.ToolTip"));
            // 
            // errorProvider
            // 
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
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
        private System.Windows.Forms.Label labelCacheNote;
        private System.Windows.Forms.GroupBox groupBoxServer;
        private System.Windows.Forms.TextBox textBoxLocation;
        private System.Windows.Forms.Label labelLocation;
        private System.Windows.Forms.TextBox textBoxMetaApi;
        private System.Windows.Forms.Label labelMetaApi;
        private System.Windows.Forms.TextBox textBoxContentApi;
        private System.Windows.Forms.Label labelContentApi;
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
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Label labelLinkInterwikiFormat;
        private System.Windows.Forms.TextBox textBoxLinkInterwikiFormat;
        private System.Windows.Forms.Button buttonLanguageRemove;
        private System.Windows.Forms.Button buttonLunguageAdd;
        private System.Windows.Forms.TextBox textBoxLangFormat;
        private System.Windows.Forms.Label labelLangFormat;
        private System.Windows.Forms.Label labelMaxConnectRetries;
        private System.Windows.Forms.Label labelRequestInterval;
        private System.Windows.Forms.TextBox textBoxMaxConnectRetries;
        private System.Windows.Forms.Label labelMaxConnectRetriesNote;
        private System.Windows.Forms.TextBox textBoxRequestInterval;
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