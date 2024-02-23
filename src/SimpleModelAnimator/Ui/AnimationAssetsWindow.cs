using Detach;
using ImGuiNET;
using SimpleModelAnimator.State;
using System.Diagnostics;

namespace SimpleModelAnimator.Ui;

public static class AnimationAssetsWindow
{
	public static void Render()
	{
		if (ImGui.Begin("Animation Assets"))
		{
			if (ImGui.Button("Reload all"))
				AnimationState.ReloadAssets(AnimationState.AnimationFilePath);

			float height = ImGui.GetContentRegionAvail().Y / 2f - 48;

			RenderAssetPaths(height, "Meshes", "obj", AnimationState.Animation.RelativeModelPaths, l => AnimationState.Animation.RelativeModelPaths = l);
			RenderAssetPaths(height, "Textures", "tga", AnimationState.Animation.RelativeTexturePaths, l => AnimationState.Animation.RelativeTexturePaths = l);
		}

		ImGui.End();
	}

	private static void RenderAssetPaths(float windowHeight, ReadOnlySpan<char> name, string dialogFilterList, List<string> list, Action<List<string>> setList)
	{
		ImGui.SeparatorText(name);

		ImGui.BeginDisabled(AnimationState.AnimationFilePath == null);
		if (ImGui.Button(Inline.Span($"Add {name}")))
		{
			Debug.Assert(AnimationState.AnimationFilePath != null, "Cannot click this button because it should be disabled.");

			DialogWrapper.FileOpenMultiple(p => AddAssetsCallback(list, setList, p), dialogFilterList);
		}

		ImGui.EndDisabled();

		if (AnimationState.AnimationFilePath == null)
		{
			ImGui.SameLine();
			ImGui.Text("(?)");
			if (ImGui.IsItemHovered())
				ImGui.SetTooltip("You must save the animation before you can add assets.");
		}

		ImGui.BeginDisabled(AnimationState.AnimationFilePath == null);
		if (ImGui.BeginChild(Inline.Span($"{name}List"), new(0, windowHeight), ImGuiChildFlags.Border))
		{
			string? toRemove = null;
			foreach (string item in list)
			{
				ImGui.PushID(Inline.Span($"button_delete_{name}_{item}"));
				if (ImGui.Button("X"))
					toRemove = item;

				ImGui.PopID();

				ImGui.SameLine();
				ImGui.Text(item);
			}

			if (toRemove != null)
			{
				list.Remove(toRemove);
				AnimationState.ReloadAssets(AnimationState.AnimationFilePath);

				AnimationState.Track("Removed assets");
			}
		}

		ImGui.EndChild();
		ImGui.EndDisabled();
	}

	private static void AddAssetsCallback(List<string> list, Action<List<string>> setList, IReadOnlyList<string>? paths)
	{
		if (paths == null)
			return;

		string? parentDirectory = Path.GetDirectoryName(AnimationState.AnimationFilePath);
		Debug.Assert(parentDirectory != null, "Parent directory should not be null.");

		string[] relativePaths = paths.Select(path => Path.GetRelativePath(parentDirectory, path)).ToArray();

		list.AddRange(relativePaths);
		setList(list.Order().Distinct().ToList());
		AssetLoadScheduleState.Schedule(AnimationState.AnimationFilePath);

		AnimationState.Track("Added assets");
	}
}
