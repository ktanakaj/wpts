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
            this.ColumnArrow = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnToCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnToTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnTimestamp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPageHeadings = new System.Windows.Forms.TabPage();
            this.dataGridViewHeading = new System.Windows.Forms.DataGridView();
            this.tabPageServer = new System.Windows.Forms.TabPage();
            this.tabPageApplication = new System.Windows.Forms.TabPage();
            this.tabControl.SuspendLayout();
            this.tabPageItems.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewItems)).BeginInit();
            this.tabPageHeadings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewHeading)).BeginInit();
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
            resources.ApplyResources(this.tabPageServer, "tabPageServer");
            this.tabPageServer.Name = "tabPageServer";
            this.tabPageServer.UseVisualStyleBackColor = true;
            // 
            // tabPageApplication
            // 
            resources.ApplyResources(this.tabPageApplication, "tabPageApplication");
            this.tabPageApplication.Name = "tabPageApplication";
            this.tabPageApplication.UseVisualStyleBackColor = true;
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
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageItems;
        private System.Windows.Forms.TabPage tabPageHeadings;
        private System.Windows.Forms.DataGridView dataGridViewItems;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnFromCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnFromTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnArrow;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnToCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnToTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnTimestamp;
        private System.Windows.Forms.DataGridView dataGridViewHeading;
        private System.Windows.Forms.TabPage tabPageServer;
        private System.Windows.Forms.TabPage tabPageApplication;
    }
}