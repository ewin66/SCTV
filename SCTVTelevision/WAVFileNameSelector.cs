using System;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace SCTVTelevision
{
	/// <summary>
	/// Summary description for WAVFilenameSelector.
	/// </summary>
	public class WAVFileNameSelector : FileNameEditor
	{
		public WAVFileNameSelector()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		//Sets the extension filter to .wav
		protected override void InitializeDialog(System.Windows.Forms.OpenFileDialog openFileDialog)
		{
			openFileDialog.Filter=@"wav Files (*.wav)|*.wav|All files (*.*)|*.*";
		}
	}
}
