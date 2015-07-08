namespace QTP.BundleManagerPlugin
{
    partial class BundleManagerView
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panelEx1 = new DevComponents.DotNetBar.PanelEx();
            this.panelEx2 = new DevComponents.DotNetBar.PanelEx();
            this.btn_stop = new DevComponents.DotNetBar.ButtonX();
            this.btn_uninstall = new DevComponents.DotNetBar.ButtonX();
            this.btn_refresh = new DevComponents.DotNetBar.ButtonX();
            this.tbx_current_bundle = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.dataGridViewX1 = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.select = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.version = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lever = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panelEx1.SuspendLayout();
            this.panelEx2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).BeginInit();
            this.SuspendLayout();
            // 
            // panelEx1
            // 
            this.panelEx1.CanvasColor = System.Drawing.SystemColors.Control;
            this.panelEx1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.panelEx1.Controls.Add(this.panelEx2);
            this.panelEx1.Controls.Add(this.tbx_current_bundle);
            this.panelEx1.Controls.Add(this.labelX1);
            this.panelEx1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelEx1.Location = new System.Drawing.Point(0, 0);
            this.panelEx1.Name = "panelEx1";
            this.panelEx1.Size = new System.Drawing.Size(904, 40);
            this.panelEx1.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.panelEx1.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.panelEx1.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.panelEx1.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
            this.panelEx1.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.panelEx1.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.panelEx1.Style.GradientAngle = 90;
            this.panelEx1.TabIndex = 0;
            // 
            // panelEx2
            // 
            this.panelEx2.CanvasColor = System.Drawing.SystemColors.Control;
            this.panelEx2.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.panelEx2.Controls.Add(this.btn_stop);
            this.panelEx2.Controls.Add(this.btn_uninstall);
            this.panelEx2.Controls.Add(this.btn_refresh);
            this.panelEx2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelEx2.Location = new System.Drawing.Point(524, 0);
            this.panelEx2.Name = "panelEx2";
            this.panelEx2.Size = new System.Drawing.Size(380, 40);
            this.panelEx2.Style.Alignment = System.Drawing.StringAlignment.Center;
            this.panelEx2.Style.BackColor1.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.panelEx2.Style.BackColor2.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.panelEx2.Style.Border = DevComponents.DotNetBar.eBorderType.SingleLine;
            this.panelEx2.Style.BorderColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.panelEx2.Style.ForeColor.ColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.panelEx2.Style.GradientAngle = 90;
            this.panelEx2.TabIndex = 5;
            // 
            // btn_stop
            // 
            this.btn_stop.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btn_stop.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btn_stop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_stop.Location = new System.Drawing.Point(127, 0);
            this.btn_stop.Name = "btn_stop";
            this.btn_stop.Size = new System.Drawing.Size(122, 40);
            this.btn_stop.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btn_stop.TabIndex = 0;
            this.btn_stop.Text = "停止";
            this.btn_stop.Click += new System.EventHandler(this.btn_manager_click);
            // 
            // btn_uninstall
            // 
            this.btn_uninstall.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btn_uninstall.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btn_uninstall.Dock = System.Windows.Forms.DockStyle.Left;
            this.btn_uninstall.Location = new System.Drawing.Point(0, 0);
            this.btn_uninstall.Name = "btn_uninstall";
            this.btn_uninstall.Size = new System.Drawing.Size(127, 40);
            this.btn_uninstall.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btn_uninstall.TabIndex = 1;
            this.btn_uninstall.Text = "卸载";
            this.btn_uninstall.Click += new System.EventHandler(this.btn_uninstall_click);
            // 
            // btn_refresh
            // 
            this.btn_refresh.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btn_refresh.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btn_refresh.Dock = System.Windows.Forms.DockStyle.Right;
            this.btn_refresh.Location = new System.Drawing.Point(249, 0);
            this.btn_refresh.Name = "btn_refresh";
            this.btn_refresh.Size = new System.Drawing.Size(131, 40);
            this.btn_refresh.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btn_refresh.TabIndex = 2;
            this.btn_refresh.Text = "刷新";
            this.btn_refresh.Click += new System.EventHandler(this.btn_refresh_click);
            // 
            // tbx_current_bundle
            // 
            this.tbx_current_bundle.BackColor = System.Drawing.Color.WhiteSmoke;
            // 
            // 
            // 
            this.tbx_current_bundle.Border.Class = "TextBoxBorder";
            this.tbx_current_bundle.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.tbx_current_bundle.Location = new System.Drawing.Point(81, 9);
            this.tbx_current_bundle.Name = "tbx_current_bundle";
            this.tbx_current_bundle.ReadOnly = true;
            this.tbx_current_bundle.Size = new System.Drawing.Size(178, 21);
            this.tbx_current_bundle.TabIndex = 4;
            // 
            // labelX1
            // 
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelX1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelX1.Location = new System.Drawing.Point(0, 0);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(113, 40);
            this.labelX1.TabIndex = 3;
            this.labelX1.Text = "当前插件：";
            // 
            // dataGridViewX1
            // 
            this.dataGridViewX1.AllowUserToAddRows = false;
            this.dataGridViewX1.AllowUserToDeleteRows = false;
            this.dataGridViewX1.AllowUserToResizeRows = false;
            this.dataGridViewX1.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.dataGridViewX1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewX1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.select,
            this.name,
            this.label,
            this.version,
            this.status,
            this.lever});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewX1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewX1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewX1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dataGridViewX1.Location = new System.Drawing.Point(0, 40);
            this.dataGridViewX1.Name = "dataGridViewX1";
            this.dataGridViewX1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dataGridViewX1.RowHeadersVisible = false;
            this.dataGridViewX1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridViewX1.RowTemplate.Height = 23;
            this.dataGridViewX1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridViewX1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewX1.Size = new System.Drawing.Size(904, 622);
            this.dataGridViewX1.TabIndex = 0;
            this.dataGridViewX1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // select
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle1.NullValue = false;
            this.select.DefaultCellStyle = dataGridViewCellStyle1;
            this.select.HeaderText = "选择";
            this.select.Name = "select";
            this.select.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.select.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // name
            // 
            this.name.HeaderText = "名称";
            this.name.Name = "name";
            this.name.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.name.Width = 250;
            // 
            // label
            // 
            this.label.HeaderText = "标识";
            this.label.Name = "label";
            this.label.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.label.Width = 300;
            // 
            // version
            // 
            this.version.HeaderText = "版本";
            this.version.Name = "version";
            this.version.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // status
            // 
            this.status.HeaderText = "状态";
            this.status.Name = "status";
            this.status.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // lever
            // 
            this.lever.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.lever.HeaderText = "启动级别";
            this.lever.Name = "lever";
            this.lever.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // BundleManagerView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridViewX1);
            this.Controls.Add(this.panelEx1);
            this.Name = "BundleManagerView";
            this.Size = new System.Drawing.Size(904, 662);
            this.panelEx1.ResumeLayout(false);
            this.panelEx2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.PanelEx panelEx1;
        private DevComponents.DotNetBar.Controls.DataGridViewX dataGridViewX1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn select;
        private System.Windows.Forms.DataGridViewTextBoxColumn name;
        private System.Windows.Forms.DataGridViewTextBoxColumn label;
        private System.Windows.Forms.DataGridViewTextBoxColumn version;
        private System.Windows.Forms.DataGridViewTextBoxColumn status;
        private System.Windows.Forms.DataGridViewTextBoxColumn lever;
        private DevComponents.DotNetBar.ButtonX btn_refresh;
        private DevComponents.DotNetBar.ButtonX btn_uninstall;
        private DevComponents.DotNetBar.ButtonX btn_stop;
        private DevComponents.DotNetBar.Controls.TextBoxX tbx_current_bundle;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.PanelEx panelEx2;
    }
}
