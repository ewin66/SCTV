using System;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace SCTVTelevision
{
	/// <summary>
	/// Summary description for FileNameSelector.
	/// </summary>
	public class FileNameSelector : FileNameEditor
	{
		public FileNameSelector()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		//Allows file that don't exist to be selected
		protected override void InitializeDialog(System.Windows.Forms.OpenFileDialog openFileDialog)
		{
			openFileDialog.CheckFileExists=false;
			base.InitializeDialog (openFileDialog);
		}
	}
}
