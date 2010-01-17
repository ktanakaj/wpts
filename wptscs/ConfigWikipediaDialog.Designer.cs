// ================================================================================================
// <summary>
//      Wikipedia翻訳支援ツール設定画面デザインソース</summary>
//
// <copyright file="ConfigWikipediaDialog.Designer.cs" company="honeplusのメモ帳">
//      Copyright (C) 2010 Honeplus. All rights reserved.</copyright>
// <author>
//      Honeplus</author>
// ================================================================================================

namespace Honememo.Wptscs
{
    /// <summary>
    /// Wikipedia翻訳支援ツール設定画面のクラスです。
    /// </summary>
    public partial class ConfigWikipediaDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigWikipediaDialog));
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBoxTitleKey = new System.Windows.Forms.GroupBox();
            this.dataGridViewTitleKey = new System.Windows.Forms.DataGridView();
            this.groupBoxLanguage = new System.Windows.Forms.GroupBox();
            this.groupBoxName = new System.Windows.Forms.GroupBox();
            this.dataGridViewName = new System.Windows.Forms.DataGridView();
            this.Code = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ArticleName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ShortName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBoxStyle = new System.Windows.Forms.GroupBox();
            this.textBoxRedirect = new System.Windows.Forms.TextBox();
            this.labelRedirect = new System.Windows.Forms.Label();
            this.textBoxXml = new System.Windows.Forms.TextBox();
            this.labelXml = new System.Windows.Forms.Label();
            this.comboBoxCode = new System.Windows.Forms.ComboBox();
            this.contextMenuStripCode = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemModify = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.labelCode = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.groupBoxTitleKey.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTitleKey)).BeginInit();
            this.groupBoxLanguage.SuspendLayout();
            this.groupBoxName.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewName)).BeginInit();
            this.groupBoxStyle.SuspendLayout();
            this.contextMenuStripCode.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonOK.Location = new System.Drawing.Point(226, 330);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(328, 330);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // groupBoxTitleKey
            // 
            this.groupBoxTitleKey.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxTitleKey.Controls.Add(this.dataGridViewTitleKey);
            this.groupBoxTitleKey.Location = new System.Drawing.Point(12, 12);
            this.groupBoxTitleKey.Name = "groupBoxTitleKey";
            this.groupBoxTitleKey.Size = new System.Drawing.Size(314, 308);
            this.groupBoxTitleKey.TabIndex = 2;
            this.groupBoxTitleKey.TabStop = false;
            this.groupBoxTitleKey.Text = "定型句の設定";
            this.toolTip.SetToolTip(this.groupBoxTitleKey, "見出しの変換を行うための設定です。\r\n各言語で、同じ意味を持つ項目が何であるかのパターンを設定します。\r\n\r\n例、英語版の See Also という見出しを日本語" +
                    "版の 関連項目 と対応させたい場合、\r\n    en の列に See Also を、ja 列の同じ行に 関連項目 をそれぞれ記述します。");
            // 
            // dataGridViewTitleKey
            // 
            this.dataGridViewTitleKey.AllowUserToResizeRows = false;
            this.dataGridViewTitleKey.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewTitleKey.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewTitleKey.Location = new System.Drawing.Point(11, 20);
            this.dataGridViewTitleKey.Name = "dataGridViewTitleKey";
            this.dataGridViewTitleKey.RowHeadersWidth = 20;
            this.dataGridViewTitleKey.RowTemplate.Height = 21;
            this.dataGridViewTitleKey.Size = new System.Drawing.Size(291, 279);
            this.dataGridViewTitleKey.TabIndex = 0;
            // 
            // groupBoxLanguage
            // 
            this.groupBoxLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxLanguage.Controls.Add(this.groupBoxName);
            this.groupBoxLanguage.Controls.Add(this.groupBoxStyle);
            this.groupBoxLanguage.Controls.Add(this.comboBoxCode);
            this.groupBoxLanguage.Controls.Add(this.labelCode);
            this.groupBoxLanguage.Location = new System.Drawing.Point(332, 12);
            this.groupBoxLanguage.Name = "groupBoxLanguage";
            this.groupBoxLanguage.Size = new System.Drawing.Size(280, 308);
            this.groupBoxLanguage.TabIndex = 3;
            this.groupBoxLanguage.TabStop = false;
            this.groupBoxLanguage.Text = "言語のプロパティ";
            this.toolTip.SetToolTip(this.groupBoxLanguage, "各言語に対する設定です。\r\n設定を行う言語の、言語コードを選択してください。");
            // 
            // groupBoxName
            // 
            this.groupBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxName.Controls.Add(this.dataGridViewName);
            this.groupBoxName.Enabled = false;
            this.groupBoxName.Location = new System.Drawing.Point(8, 120);
            this.groupBoxName.Name = "groupBoxName";
            this.groupBoxName.Size = new System.Drawing.Size(266, 182);
            this.groupBoxName.TabIndex = 3;
            this.groupBoxName.TabStop = false;
            this.groupBoxName.Text = "各言語での呼称";
            this.toolTip.SetToolTip(this.groupBoxName, "選択した言語の、各言語での呼称です。");
            // 
            // dataGridViewName
            // 
            this.dataGridViewName.AllowUserToResizeRows = false;
            this.dataGridViewName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewName.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewName.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewName.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewName.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Code,
            this.ArticleName,
            this.ShortName});
            this.dataGridViewName.Location = new System.Drawing.Point(6, 18);
            this.dataGridViewName.Name = "dataGridViewName";
            this.dataGridViewName.RowHeadersWidth = 20;
            this.dataGridViewName.RowTemplate.Height = 21;
            this.dataGridViewName.Size = new System.Drawing.Size(254, 158);
            this.dataGridViewName.TabIndex = 0;
            this.dataGridViewName.Leave += new System.EventHandler(this.dataGridViewName_Leave);
            // 
            // Code
            // 
            this.Code.HeaderText = "コード";
            this.Code.MaxInputLength = 10;
            this.Code.Name = "Code";
            this.Code.ToolTipText = "言語コードです。まず言語コードを入力し、呼称列と略称列に、その言語コードの言語での名称を入力してください。";
            this.Code.Width = 57;
            // 
            // ArticleName
            // 
            this.ArticleName.HeaderText = "記事名";
            this.ArticleName.MaxInputLength = 100;
            this.ArticleName.Name = "ArticleName";
            this.ArticleName.ToolTipText = "その言語のWikipediaでの記事名です。\n言語コード列の言語での記事名を設定してください。\n\n例、日本語 で言語コード列が en の場合、Japanese";
            this.ArticleName.Width = 66;
            // 
            // ShortName
            // 
            this.ShortName.HeaderText = "略称";
            this.ShortName.MaxInputLength = 100;
            this.ShortName.Name = "ShortName";
            this.ShortName.ToolTipText = "言語の略称です。\n言語コード列の言語での略称を設定してください。\n特に存在しない場合は、未入力で問題ありません。\n\n例、ドイツ語 で言語コード列が ja の場合、" +
                "独";
            this.ShortName.Width = 54;
            // 
            // groupBoxStyle
            // 
            this.groupBoxStyle.Controls.Add(this.textBoxRedirect);
            this.groupBoxStyle.Controls.Add(this.labelRedirect);
            this.groupBoxStyle.Controls.Add(this.textBoxXml);
            this.groupBoxStyle.Controls.Add(this.labelXml);
            this.groupBoxStyle.Enabled = false;
            this.groupBoxStyle.Location = new System.Drawing.Point(8, 44);
            this.groupBoxStyle.Name = "groupBoxStyle";
            this.groupBoxStyle.Size = new System.Drawing.Size(266, 70);
            this.groupBoxStyle.TabIndex = 2;
            this.groupBoxStyle.TabStop = false;
            this.groupBoxStyle.Text = "各言語での書式";
            this.toolTip.SetToolTip(this.groupBoxStyle, "各言語での記事XMLデータのパスや、リダイレクトの書式を入力します。\r\nこの値が正しく設定されていない場合、正常な処理は行えません。");
            // 
            // textBoxRedirect
            // 
            this.textBoxRedirect.Location = new System.Drawing.Point(102, 43);
            this.textBoxRedirect.MaxLength = 100;
            this.textBoxRedirect.Name = "textBoxRedirect";
            this.textBoxRedirect.Size = new System.Drawing.Size(129, 19);
            this.textBoxRedirect.TabIndex = 3;
            this.toolTip.SetToolTip(this.textBoxRedirect, "リダイレクトの書式です。\r\n各言語の書式と、英語版の書式でチェックを行います。\r\n\r\n例、#転送");
            // 
            // labelRedirect
            // 
            this.labelRedirect.AutoSize = true;
            this.labelRedirect.Location = new System.Drawing.Point(9, 47);
            this.labelRedirect.Name = "labelRedirect";
            this.labelRedirect.Size = new System.Drawing.Size(61, 12);
            this.labelRedirect.TabIndex = 2;
            this.labelRedirect.Text = "リダイレクト：";
            this.toolTip.SetToolTip(this.labelRedirect, "リダイレクトの書式です。\r\n各言語の書式と、英語版の書式でチェックを行います。\r\n\r\n例、#転送");
            // 
            // textBoxXml
            // 
            this.textBoxXml.Location = new System.Drawing.Point(102, 18);
            this.textBoxXml.MaxLength = 100;
            this.textBoxXml.Name = "textBoxXml";
            this.textBoxXml.Size = new System.Drawing.Size(129, 19);
            this.textBoxXml.TabIndex = 1;
            this.toolTip.SetToolTip(this.textBoxXml, "記事のXMLデータが格納されているパスです。\r\n設定されたパスに記事名を付加して、XMLデータの取得を行います。\r\n\r\n例、wiki/Special:Export" +
                    "/");
            // 
            // labelXml
            // 
            this.labelXml.AutoSize = true;
            this.labelXml.Location = new System.Drawing.Point(6, 21);
            this.labelXml.Name = "labelXml";
            this.labelXml.Size = new System.Drawing.Size(90, 12);
            this.labelXml.TabIndex = 0;
            this.labelXml.Text = "XMLデータのパス：";
            this.toolTip.SetToolTip(this.labelXml, "記事のXMLデータが格納されているパスです。\r\n設定されたパスに記事名を付加して、XMLデータの取得を行います。\r\n\r\n例、wiki/Special:Export" +
                    "/");
            // 
            // comboBoxCode
            // 
            this.comboBoxCode.ContextMenuStrip = this.contextMenuStripCode;
            this.comboBoxCode.FormattingEnabled = true;
            this.comboBoxCode.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.comboBoxCode.Location = new System.Drawing.Point(74, 18);
            this.comboBoxCode.MaxLength = 10;
            this.comboBoxCode.Name = "comboBoxCode";
            this.comboBoxCode.Size = new System.Drawing.Size(85, 20);
            this.comboBoxCode.TabIndex = 1;
            this.toolTip.SetToolTip(this.comboBoxCode, "設定を行う言語の、言語コードを選択します。\r\n言語の追加を行う場合は、言語コード入力後にEnterキーで登録できます。\r\n削除する場合は、右クリックで選択した値を" +
                    "削除できます。\r\nこのコンボボックスに設定する言語コードは、Wikipediaの言語コードと一致させてください。");
            this.comboBoxCode.SelectedIndexChanged += new System.EventHandler(this.comboBoxCode_SelectedIndexChanged);
            this.comboBoxCode.Leave += new System.EventHandler(this.comboBoxCode_Leave);
            this.comboBoxCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.comboBoxCode_KeyDown);
            // 
            // contextMenuStripCode
            // 
            this.contextMenuStripCode.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemModify,
            this.toolStripMenuItemDelete});
            this.contextMenuStripCode.Name = "contextMenuStripCode";
            this.contextMenuStripCode.Size = new System.Drawing.Size(193, 48);
            // 
            // toolStripMenuItemModify
            // 
            this.toolStripMenuItemModify.Name = "toolStripMenuItemModify";
            this.toolStripMenuItemModify.Size = new System.Drawing.Size(192, 22);
            this.toolStripMenuItemModify.Text = "言語コードの変更(&M)";
            this.toolStripMenuItemModify.ToolTipText = "選択された言語の言語コードを変更します。\r\n変更を選ぶと、新しい言語コードを入力するダイアログが開きます。";
            this.toolStripMenuItemModify.Click += new System.EventHandler(this.toolStripMenuItemModify_Click);
            // 
            // toolStripMenuItemDelete
            // 
            this.toolStripMenuItemDelete.Name = "toolStripMenuItemDelete";
            this.toolStripMenuItemDelete.Size = new System.Drawing.Size(192, 22);
            this.toolStripMenuItemDelete.Text = "言語の削除(&D)";
            this.toolStripMenuItemDelete.ToolTipText = "選択された言語を削除します。\r\n削除を選ぶと、その言語に属する設定は全て削除されます。";
            this.toolStripMenuItemDelete.Click += new System.EventHandler(this.toolStripMenuItemDelete_Click);
            // 
            // labelCode
            // 
            this.labelCode.AutoSize = true;
            this.labelCode.Location = new System.Drawing.Point(6, 21);
            this.labelCode.Name = "labelCode";
            this.labelCode.Size = new System.Drawing.Size(62, 12);
            this.labelCode.TabIndex = 0;
            this.labelCode.Text = "言語コード：";
            this.toolTip.SetToolTip(this.labelCode, "設定を行う言語の、言語コードを選択します。\r\n言語の追加を行う場合は、言語コード入力後にEnterキーで登録できます。\r\n削除する場合は、右クリックで選択した値を" +
                    "削除できます。\r\nこのコンボボックスに設定する言語コードは、Wikipediaの言語コードと一致させてください。");
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 10000;
            this.toolTip.InitialDelay = 500;
            this.toolTip.ReshowDelay = 100;
            // 
            // ConfigWikipediaDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 363);
            this.Controls.Add(this.groupBoxLanguage);
            this.Controls.Add(this.groupBoxTitleKey);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(640, 400);
            this.Name = "ConfigWikipediaDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "言語の設定";
            this.Load += new System.EventHandler(this.ConfigWikipediaDialog_Load);
            this.groupBoxTitleKey.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewTitleKey)).EndInit();
            this.groupBoxLanguage.ResumeLayout(false);
            this.groupBoxLanguage.PerformLayout();
            this.groupBoxName.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewName)).EndInit();
            this.groupBoxStyle.ResumeLayout(false);
            this.groupBoxStyle.PerformLayout();
            this.contextMenuStripCode.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBoxTitleKey;
        private System.Windows.Forms.GroupBox groupBoxLanguage;
        private System.Windows.Forms.DataGridView dataGridViewTitleKey;
        private System.Windows.Forms.ComboBox comboBoxCode;
        private System.Windows.Forms.Label labelCode;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripCode;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemModify;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemDelete;
        private System.Windows.Forms.GroupBox groupBoxStyle;
        private System.Windows.Forms.TextBox textBoxXml;
        private System.Windows.Forms.Label labelXml;
        private System.Windows.Forms.TextBox textBoxRedirect;
        private System.Windows.Forms.Label labelRedirect;
        private System.Windows.Forms.GroupBox groupBoxName;
        private System.Windows.Forms.DataGridView dataGridViewName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Code;
        private System.Windows.Forms.DataGridViewTextBoxColumn ArticleName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ShortName;
    }
}