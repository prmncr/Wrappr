namespace Wrappr.Utilities;

using System.Runtime.InteropServices;

internal class WindowsSystemDispatcherQueueHelper
{
	public void EnsureWindowsSystemDispatcherQueueController()
	{
		if (Windows.System.DispatcherQueue.GetForCurrentThread() != null)
		{
			return;
		}

		if (_dispatcherQueueController != null) return;
		DispatcherQueueOptions options;
		options.dwSize = Marshal.SizeOf(typeof(DispatcherQueueOptions));
		options.threadType = 2;
		options.apartmentType = 2;

		CreateDispatcherQueueController(options, ref _dispatcherQueueController);
	}

	private object? _dispatcherQueueController;

	[DllImport("CoreMessaging.dll")]
	private static extern int CreateDispatcherQueueController(
		[In] DispatcherQueueOptions options,
		[In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object? dispatcherQueueController
	);

	[StructLayout(LayoutKind.Sequential)]
	private struct DispatcherQueueOptions
	{
		internal int dwSize;
		internal int threadType;
		internal int apartmentType;
	}
}