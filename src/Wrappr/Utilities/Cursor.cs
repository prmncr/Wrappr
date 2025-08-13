using System.Runtime.InteropServices;

namespace Wrappr.Utilities;

public static partial class Cursor
{
	public static System.Drawing.Point Position
	{
		get
		{
			var point = new Point();
			GetCursorPos(ref point);
			return point;
		}
	}

	[LibraryImport("User32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static partial void GetCursorPos(ref Point point);

	[StructLayout(LayoutKind.Sequential)]
	private struct Point
	{
		public int X;
		public int Y;

		public static implicit operator System.Drawing.Point(Point point)
		{
			return new System.Drawing.Point(point.X, point.Y);
		}
	}
}