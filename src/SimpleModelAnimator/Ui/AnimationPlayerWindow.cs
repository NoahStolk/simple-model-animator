using Detach;
using ImGuiNET;
using SimpleModelAnimator.State;
using System.Globalization;

namespace SimpleModelAnimator.Ui;

public static class AnimationPlayerWindow
{
	public static void Render()
	{
		if (ImGui.Begin("Animation Player"))
		{
			ImGui.Text(Inline.Span(AnimationPlayerState.Time, "0.00", CultureInfo.InvariantCulture));
			ImGui.SliderFloat("Time", ref AnimationPlayerState.Time, 0, AnimationState.Animation.FrameCount / AnimationState.Animation.FramesPerSecond);

			ImGui.Text(Inline.Span(AnimationPlayerState.FrameIndex));
		}

		ImGui.End();
	}
}
