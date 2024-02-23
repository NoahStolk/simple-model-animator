using Detach;
using ImGuiNET;
using SimpleModelAnimator.Formats.Animation.Model;
using SimpleModelAnimator.State;

namespace SimpleModelAnimator.Ui;

public static class AnimationInfoWindow
{
	public static void Render()
	{
		if (ImGui.Begin("Animation Info"))
		{
			ImGui.TextWrapped(AnimationState.AnimationFilePath ?? "<No animation loaded>");
			if (AnimationState.AnimationFilePath != null)
			{
				ImGui.Text(AnimationState.IsModified ? "(unsaved changes)" : "(saved)");
				ImGui.SeparatorText("Animation");
				RenderAnimation(AnimationState.Animation);
			}
		}

		ImGui.End();
	}

	private static void RenderAnimation(AnimationData animation)
	{
		ImGui.Text(Inline.Span($"FPS: {animation.FramesPerSecond}"));
		ImGui.Text(Inline.Span($"OBJ file: {animation.ObjPath}"));
		ImGui.Text(Inline.Span($"Meshes: {animation.Meshes.Count}"));

		ImGui.Separator();

		if (ImGui.TreeNode("Meshes"))
		{
			for (int i = 0; i < animation.Meshes.Count; i++)
			{
				AnimationMesh mesh = animation.Meshes[i];
				ImGui.Text(mesh.MeshName);
			}

			ImGui.TreePop();
		}
	}
}
