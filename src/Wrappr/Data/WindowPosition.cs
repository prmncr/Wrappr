using System.Text.Json.Serialization;

namespace Wrappr.Data;

public record WindowPosition {
	[JsonInclude] public int PosX;
	[JsonInclude] public int PosY;
	[JsonInclude] public int Width = 490;
	[JsonInclude] public int Height = 520;
	[JsonInclude] public bool IsMaximized;
}