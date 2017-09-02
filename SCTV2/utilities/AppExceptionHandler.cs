using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Microsoft.ApplicationBlocks.ExceptionManagement;

namespace SCTV
{
	/// <summary>
	/// Summary description for AppExceptionHandler.
	/// </summary>
	public class AppExceptionHandler : object
	{
		public AppExceptionHandler()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public void AppException(object sender, ThreadExceptionEventArgs e)
		{
			try
			{
				//First publish the exception to the xml log
				ExceptionManager.Publish(e.Exception);

				DialogResult result=ShowThreadExceptionDialog(e.Exception);
                
				if (result==DialogResult.Abort)
					Application.Exit();            
			}
			catch
			{
				try
				{
					ShowThreadExceptionDialog(e.Exception);					
				}
				finally
				{
					Application.Exit();
				}
			}
		}

		private DialogResult ShowThreadExceptionDialog(Exception ex) 
		{       
			string errorMessage=String.Format("A critical error has occurred, check Exceptions.xml in the TVGuide directory for more information.\n\nError Message: {0}",ex.Message);
    
			return MessageBox.Show(errorMessage,"TV Guide Fatal Error",MessageBoxButtons.AbortRetryIgnore,MessageBoxIcon.Stop);
		}    
	}
}
