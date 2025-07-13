namespace Wrappr.Utilities;

public interface INavigable
{
	public string LocalizedName { get; }

	public static string GetTag<T>() where T : INavigable
	{
		return GetTag(typeof(T));
	}

	private static string GetTag(Type type)
	{
		return type.Name;
	}

	public class RootInfoBuilder
	{
		private readonly Dictionary<string, Type> _roots = new();

		public RootInfoBuilder Add<T>() where T : INavigable
		{
			var type = typeof(T);
			_roots.Add(GetTag(type), type);
			return this;
		}

		public static implicit operator Dictionary<string, Type>(RootInfoBuilder builder)
		{
			return builder._roots;
		}
	}
}