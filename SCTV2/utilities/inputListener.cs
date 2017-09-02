using System; 
using System.Runtime.InteropServices; 
using System.Reflection; 
using System.Threading; 
using System.Windows.Forms;

namespace SCTV
{
	/// <summary>
	/// Summary description for inputListener.
	/// </summary>
	public class InputListener 
	{ 
		#region Variables

		private int lastX = 0; 
		private int lastY = 0;  
		public delegate void MouseMoveHandler (object inputListener, MouseMoveEventArgs mouseMoveInfo); 
		public event MouseMoveHandler OnMouseMove; 
		private MouseButtons lastButton = MouseButtons.None; 
		public delegate void MouseButtonHandler (object inputListener, MouseButtonEventArgs mouseButtonInfo); 
		public event MouseButtonHandler OnMouseButton;
		[DllImport("user32.dll")] 
		public static extern int GetAsyncKeyState (long vKey); 
		public delegate void KeyPressHandler (object inputListener, KeyPressEventArgs KeyPressInfo); 
		public event KeyPressHandler OnKeyPress; 
		#endregion

		#region public void Run() 
		public void Run() 
		{ 
			Thread thdMain = new Thread(new ThreadStart(RunThread)); 
			thdMain.Start(); 
		}
		#endregion
 
		#region private void RunThread() 
		private void RunThread() 
		{ 
			while(true) 
			{ 
				Thread.Sleep(10); 
				// Check for Mouse Move Events                        
				if (lastX!=Control.MousePosition.X || 
					lastY!=Control.MousePosition.Y) 
				{ 
					lastX = Control.MousePosition.X; 
					lastY = Control.MousePosition.Y; 
					MouseMoveEventArgs mouseMoveInfo = new 
						MouseMoveEventArgs(lastX,lastY); 
					if (OnMouseMove!=null) 
					{ 
						OnMouseMove(this,mouseMoveInfo); 
					} 
				} 
				
				//check Mouse Button Events
				if (lastButton!=Control.MouseButtons) 
				{                       
					MouseButtonEventArgs mouseButtonInfo; 
					if (Control.MouseButtons==MouseButtons.None) 
					{ 
						mouseButtonInfo = new 
							MouseButtonEventArgs( 
							lastButton, MouseButtonState.Released); 
						if (OnMouseButton!=null) 
						{ 
							OnMouseButton( this, 
								mouseButtonInfo); 
						} 
					}                                         
					else 
					{ 
						mouseButtonInfo = new MouseButtonEventArgs(Control.MouseButtons, MouseButtonState.Pressed); 
						if (OnMouseButton!=null) 
						{ 
							OnMouseButton( this, mouseButtonInfo); 
						}                                   
					} 
					lastButton=Control.MouseButtons;          
				} 

				// check for key presses 
				int i=0; 
				for(i=1;i<Byte.MaxValue;i++) 
				{ 
					if (GetAsyncKeyState(i) ==  Int16.MinValue+1 ) 
					{ 
						KeyPressEventArgs KeyPressInfo = 
							new KeyPressEventArgs( 
							Control.ModifierKeys,i); 
						if (OnKeyPress!=null) 
						{
							OnKeyPress(this,KeyPressInfo); 
						}                                   
					} 

				} 

			} 
		} 
		#endregion
	} 

	#region public class MouseMoveEventArgs : EventArgs 
	public class MouseMoveEventArgs : EventArgs 
	{ 
		public MouseMoveEventArgs(int X,int Y) 
		{ 
			this.X = X; 
			this.Y = Y; 
		} 
		public readonly int X; 
		public readonly int Y; 
	}
	#endregion

	#region public class MouseButtonEventArgs : EventArgs 
	public enum MouseButtonState {Pressed,Released} 
	public class MouseButtonEventArgs : EventArgs 
	{ 
		public MouseButtonEventArgs(MouseButtons  
			Button,MouseButtonState buttonState) 
		{ 
			this.Button = Button; 
			this.buttonState = buttonState; 
		} 
		public readonly MouseButtons Button; 
		public readonly MouseButtonState buttonState; 

	} 
	#endregion

	#region public class KeyPressEventArgs : EventArgs 
	public class KeyPressEventArgs : EventArgs 
	{ 
		public KeyPressEventArgs( Keys ModifierKeys, int KeyCode) 
		{ 
			this.ModifierKeys= ModifierKeys; 
			this.KeyCode = KeyCode; 
		} 
		public readonly Keys ModifierKeys; 
		public readonly int KeyCode; 
	} 
	#endregion
}
