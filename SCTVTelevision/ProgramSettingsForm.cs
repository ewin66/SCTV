using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SCTVTelevision
{
	/// <summary>
	/// Summary description for ProgramSettingsForm.
	/// </summary>
	public class ProgramSettingsForm : System.Windows.Forms.Form
	{
        private System.Windows.Forms.PropertyGrid SettingsPropertyGrid;
        private System.Windows.Forms.Button OKButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ProgramSettingsForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
            SettingsPropertyGrid.SelectedObject=Form1.ProgramConfiguration;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.SettingsPropertyGrid = new System.Windows.Forms.PropertyGrid();
			this.OKButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// SettingsPropertyGrid
			// 
			this.SettingsPropertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.SettingsPropertyGrid.CommandsVisibleIfAvailable = true;
			this.SettingsPropertyGrid.LargeButtons = false;
			this.SettingsPropertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.SettingsPropertyGrid.Location = new System.Drawing.Point(0, 0);
			this.SettingsPropertyGrid.Name = "SettingsPropertyGrid";
			this.SettingsPropertyGrid.Size = new System.Drawing.Size(596, 476);
			this.SettingsPropertyGrid.TabIndex = 0;
			this.SettingsPropertyGrid.Text = "TVGuide Settings";
			this.SettingsPropertyGrid.ViewBackColor = System.Drawing.SystemColors.Window;
			this.SettingsPropertyGrid.ViewForeColor = System.Drawing.SystemColors.WindowText;
			// 
			// OKButton
			// 
			this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.OKButton.Location = new System.Drawing.Point(516, 484);
			this.OKButton.Name = "OKButton";
			this.OKButton.TabIndex = 1;
			this.OKButton.Text = "OK";
			this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
			// 
			// ProgramSettingsForm
			// 
			this.AcceptButton = this.OKButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(592, 509);
			this.ControlBox = false;
			this.Controls.Add(this.OKButton);
			this.Controls.Add(this.SettingsPropertyGrid);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ProgramSettingsForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "SCTV Settings";
			this.TopMost = true;
			this.ResumeLayout(false);

		}
		#endregion

        private void OKButton_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
	}
}
