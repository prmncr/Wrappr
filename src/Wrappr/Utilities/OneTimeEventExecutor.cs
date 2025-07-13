using Windows.Foundation;

namespace Wrappr.Utilities;

public class OneTimeEventExecutor<T>
{
	private readonly Action<TypedEventHandler<object, T>> _remove;
	private readonly TypedEventHandler<object, T> _eventHandler;

	private OneTimeEventExecutor(
		Action<TypedEventHandler<object, T>> add,
		Action<TypedEventHandler<object, T>> remove,
		TypedEventHandler<object, T> eventHandler
	)
	{
		_remove = remove;
		_eventHandler = eventHandler;
		add(Proxy);
	}

	private void Proxy(object o, T args)
	{
		_eventHandler.Invoke(o, args);
		_remove.Invoke(Proxy);
	}

	public static void Create(
		Action<TypedEventHandler<object, T>> add,
		Action<TypedEventHandler<object, T>> remove,
		TypedEventHandler<object, T> eventHandler
	)
	{
		_ = new OneTimeEventExecutor<T>(add, remove, eventHandler);
	}
}