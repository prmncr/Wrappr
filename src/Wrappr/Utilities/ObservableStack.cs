using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Wrappr.Utilities;

public class ObservableStack<T> : Stack<T>, INotifyCollectionChanged, INotifyPropertyChanged {
	public event NotifyCollectionChangedEventHandler? CollectionChanged;

	public event PropertyChangedEventHandler? PropertyChanged;

	public new T Pop() {
		var item = base.Pop();
		OnCollectionChanged(NotifyCollectionChangedAction.Remove, item);
		return item;
	}

	public new void Push(T item) {
		base.Push(item);
		OnCollectionChanged(NotifyCollectionChangedAction.Add, item);
	}

	public new void Clear() {
		base.Clear();
		OnCollectionChanged(NotifyCollectionChangedAction.Reset, default);
	}

	private void OnCollectionChanged(NotifyCollectionChangedAction action, T? item) {
		CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, item, item == null ? -1 : 0));
		OnPropertyChanged(nameof(Count));
	}

	private void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}