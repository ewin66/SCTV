namespace SCTVJustinTV
{
    partial class JustinTVGuide
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnRefresh = new System.Windows.Forms.Button();
            this.dgStreams = new System.Windows.Forms.DataGridView();
            this.index = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.category = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.subcategory = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Title = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgStreams)).BeginInit();
            this.SuspendLayout();
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(487, 387);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 0;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // dgStreams
            // 
            this.dgStreams.AllowUserToAddRows = false;
            this.dgStreams.AllowUserToDeleteRows = false;
            this.dgStreams.AllowUserToOrderColumns = true;
            this.dgStreams.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgStreams.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgStreams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgStreams.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.index,
            this.category,
            this.subcategory,
            this.Title});
            this.dgStreams.Location = new System.Drawing.Point(12, 12);
            this.dgStreams.MultiSelect = false;
            this.dgStreams.Name = "dgStreams";
            this.dgStreams.ReadOnly = true;
            this.dgStreams.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgStreams.Size = new System.Drawing.Size(1045, 358);
            this.dgStreams.TabIndex = 2;
            this.dgStreams.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.dgStreams_MouseDoubleClick);
            // 
            // index
            // 
            this.index.DataPropertyName = "index";
            this.index.HeaderText = "index";
            this.index.Name = "index";
            this.index.ReadOnly = true;
            this.index.Visible = false;
            this.index.Width = 57;
            // 
            // category
            // 
            this.category.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.category.DataPropertyName = "category";
            this.category.HeaderText = "Category";
            this.category.Name = "category";
            this.category.ReadOnly = true;
            this.category.Width = 74;
            // 
            // subcategory
            // 
            this.subcategory.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.subcategory.DataPropertyName = "subcategory";
            this.subcategory.HeaderText = "Subcategory";
            this.subcategory.Name = "subcategory";
            this.subcategory.ReadOnly = true;
            this.subcategory.Width = 92;
            // 
            // Title
            // 
            this.Title.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Title.DataPropertyName = "title";
            this.Title.HeaderText = "Title";
            this.Title.Name = "Title";
            this.Title.ReadOnly = true;
            // 
            // JustinTVGuide
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1069, 454);
            this.Controls.Add(this.dgStreams);
            this.Controls.Add(this.btnRefresh);
            this.Name = "JustinTVGuide";
            this.Text = "JustinTVGuide";
            ((System.ComponentModel.ISupportInitialize)(this.dgStreams)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.DataGridView dgStreams;
        private System.Windows.Forms.DataGridViewTextBoxColumn index;
        private System.Windows.Forms.DataGridViewTextBoxColumn category;
        private System.Windows.Forms.DataGridViewTextBoxColumn subcategory;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title;
    }
}