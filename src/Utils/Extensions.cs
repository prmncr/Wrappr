namespace Wrappr.Utils;

public static class Extensions {
	public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, TimeSpan timeout) {
		using var timeoutCancellationTokenSource = new CancellationTokenSource();
		var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
		if (completedTask != task) throw new TimeoutException("The operation has timed out.");
		await timeoutCancellationTokenSource.CancelAsync();
		return await task;
	}

	public static async Task TimeoutAfter(this Task task, TimeSpan timeout) {
		using var timeoutCancellationTokenSource = new CancellationTokenSource();
		var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
		if (completedTask != task) throw new TimeoutException("The operation has timed out.");
		await timeoutCancellationTokenSource.CancelAsync();
		await task;
	}
}