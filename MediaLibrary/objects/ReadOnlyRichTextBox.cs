using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace SCTV
{
	/// <summary>
	/// Zusammenfassung für ReadOnlyRichTextBox.
	/// </summary>
	public class ReadOnlyRichTextBox : RichTextBox
	{
		public ReadOnlyRichTextBox()
		{
			ReadOnly=true;
			TabStop=false;

			SetStyle(ControlStyles.Selectable, false);
		}

		// Font is overridden because assigning the RichTextBox to a new parent sets
		// its Font to the parent's Font and thus loses all formatting of existing RTF content.
		protected Font _font = new Font("Arial", 10);
		public override Font Font
		{
			get 
			{
				return _font;
			}
			set 
			{
				_font = value;
			}
		}

		[ Browsable(false) ]
		public new bool ReadOnly
		{
			get { return true; }
			set { }
		}
		
		[ Browsable(false) ]
		public new bool TabStop
		{
			get { return false; }
			set {  }
		}

		const int WM_SETFOCUS = 0x0007;
		protected override void WndProc(ref Message m)
		{
			switch (m.Msg)
			{
				case WM_SETFOCUS:
					// We don't want the RichTextBox to be able to receive focus, so just
					// pass the focus back to the control it came from.
					IntPtr prevCtl = m.WParam;
					Control c = Control.FromHandle(prevCtl);
					c.Select();
					return;
			}
			base.WndProc (ref m);
		}

        public void AddMedia(Media mediaToAdd)
        {
            FontStyle newFontStyle;
            Font currentFont = this.SelectionFont;

            this.Clear();

            //Title
            newFontStyle = FontStyle.Bold;
            this.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
            this.AppendText("\n" + mediaToAdd.Title);

            //Rating
            newFontStyle = FontStyle.Bold;
            this.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
            this.AppendText(" (" + mediaToAdd.Rating +")");

            //Description
            newFontStyle = FontStyle.Regular;
            this.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
            this.AppendText("\n" + mediaToAdd.Description);

            //Subtitle
            //newFontStyle = FontStyle.Bold;
            //this.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
            //this.AppendText("\tSubTitle");

            //newFontStyle = FontStyle.Regular;
            //this.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
            //this.AppendText(" " + Prog.SubTitle + " ");

            //Channel
            //newFontStyle = FontStyle.Bold;
            //this.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
            //this.AppendText("\tChannel");

            //newFontStyle = FontStyle.Regular;
            //this.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
            //this.AppendText(" " + Prog.Channel.DisplayName + " ");
            //channel = Prog.Channel.DisplayName;

            //Category
            //newFontStyle = FontStyle.Bold;
            //this.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
            //this.AppendText("\tCategory");

            //newFontStyle = FontStyle.Regular;
            //this.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
            //this.AppendText(" " + mediaToAdd.category + " 
        }
	}
}
