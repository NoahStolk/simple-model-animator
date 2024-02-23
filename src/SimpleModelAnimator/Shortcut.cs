using Silk.NET.GLFW;
using SimpleModelAnimator.Extensions;

namespace SimpleModelAnimator;

public sealed record Shortcut(string Id, Keys Key, bool Ctrl, bool Shift, string Description, Action Action)
{
	public string KeyDescription { get; } = $"{(Ctrl ? "CTRL+" : string.Empty)}{(Shift ? "SHIFT+" : string.Empty)}{Key.GetDisplayString(true)}";
}
