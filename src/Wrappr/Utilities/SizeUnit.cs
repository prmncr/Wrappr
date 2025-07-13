using System.Numerics;
using Size = Windows.Graphics.SizeInt32;

namespace Wrappr.Utilities;

public readonly struct SizeUnit(Size size)
{
	public float X(float coefficient) => size.Width * coefficient;
	public float Y(float coefficient) => size.Height * coefficient;

	public float D(float coefficient) => MathF.Sqrt(size.Width * size.Width + size.Height * size.Height) * coefficient;

	public static UniversalVector operator *(SizeUnit unit, (float, float) coefficients)
	{
		return new UniversalVector {
			X = unit.X(coefficients.Item1),
			Y = unit.Y(coefficients.Item2)
		};
	}

	public readonly struct UniversalVector
	{
		public float X { get; init; }
		public float Y { get; init; }

		public static implicit operator Vector2(UniversalVector vector) => new(vector.X, vector.Y);
		public static implicit operator Size(UniversalVector vector) => new((int)vector.X, (int)vector.Y);
	}
}