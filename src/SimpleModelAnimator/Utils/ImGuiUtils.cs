using Detach.Numerics;
using ImGuiNET;

namespace SimpleModelAnimator.Utils;

public static class ImGuiUtils
{
	public static void TextOptional(string? text)
	{
		ImGui.TextColored(text == null ? Color.Gray(0.5f) : Color.White, text ?? "N/A");
	}
}
