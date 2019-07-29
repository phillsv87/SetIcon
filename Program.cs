using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SetIcon
{
	class Program
	{
		static void Main(string[] args)
		{

			string path = null;
			IntPtr? handle = null;

			for(int i = 0;i<args.Length-1;i++) {
				switch(args[i].ToLower()) {
					case "-image":
						path=args[++i];
						break;

					case "-title":
						handle=FindWindow(null,args[++i]);
						if(handle==IntPtr.Zero) {
							Console.WriteLine("No window found by title");
							Environment.ExitCode=1;
							return;
						}
						break;

					case "-handle":
						try {
							var h = args[++i].ToLower();
							if(h.StartsWith("0x")) {
								handle=(IntPtr)long.Parse(h, NumberStyles.HexNumber);
							} else {
								handle=(IntPtr)long.Parse(h);
							}
						} catch {
							Console.WriteLine("Invalid handle");
							Environment.ExitCode=1;
							return;
						}
						break;
				}
			}

			if(path==null) {
				Console.WriteLine("-image argument required");
				Environment.ExitCode=1;
				return;
			}

			if(handle==null) {
				Console.WriteLine("-title or -handle argument required");
				Environment.ExitCode=1;
				return;
			}

			if(!File.Exists(path)) {
				Console.WriteLine($"No image found at path {path}");
				Environment.ExitCode=1;
				return;
			}

			// Create a Bitmap object from an image file.
			using(Bitmap myBitmap = new Bitmap(path)) {

				// Get an Hicon for myBitmap.
				IntPtr Hicon = myBitmap.GetHicon();

				// Create a new icon from the handle. 
				using(Icon icon = Icon.FromHandle(Hicon)) {

					//var icon = new Icon(path);

					SendMessage(handle.Value, WM_SETICON, ICON_BIG, icon.Handle);
					SendMessage(handle.Value, WM_SETICON, ICON_SMALL, icon.Handle);


					GetWindowThreadProcessId(handle.Value, out uint pid);
					var proc = Process.GetProcessById((int)pid);
					if(proc!=null) {
						proc.EnableRaisingEvents=true;
						proc.WaitForExit();
					}
				}
			}
		}


		#region Native

		const int WM_SETICON = 0x80;
		const int ICON_SMALL = 0;
		const int ICON_BIG = 1;

		[DllImport("kernel32.dll")]
		static extern IntPtr GetConsoleWindow();
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);
		[DllImport("user32.dll", SetLastError = true)]
		static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
		[DllImport("user32.dll", SetLastError = true)]
		static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
		#endregion
	}
}
