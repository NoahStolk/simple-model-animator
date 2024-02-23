﻿using Detach.Parsers.Model;
using ImGuiNET;
using SimpleModelAnimator.Formats.Animation.Model;
using SimpleModelAnimator.State;
using System.Diagnostics;

namespace SimpleModelAnimator.Ui;

public static class AnimationAssetsWindow
{
	public static void Render()
	{
		if (ImGui.Begin("Animation Assets"))
		{
			if (ImGui.Button("Reload"))
				AnimationState.ReloadAssets();

			RenderModelSource();
		}

		ImGui.End();
	}

	private static void RenderModelSource()
	{
		ImGui.SeparatorText("Model");

		ImGui.BeginDisabled(AnimationState.AnimationFilePath == null);
		if (ImGui.Button("Set model"))
		{
			Debug.Assert(AnimationState.AnimationFilePath != null, "Cannot click this button because it should be disabled.");

			DialogWrapper.FileOpen(SetModelCallback, "obj");
		}

		ImGui.EndDisabled();

		if (AnimationState.AnimationFilePath == null)
		{
			ImGui.SameLine();
			ImGui.Text("(?)");
			if (ImGui.IsItemHovered())
				ImGui.SetTooltip("You must save the animation before you can set the model.");
		}

		ImGui.Text(AnimationState.Animation.ObjPath ?? "<No model set>");
	}

	private static void SetModelCallback(string? path)
	{
		if (path == null)
			return;

		string? parentDirectory = Path.GetDirectoryName(AnimationState.AnimationFilePath);
		Debug.Assert(parentDirectory != null, "Parent directory should not be null.");

		string relativePath = Path.GetRelativePath(parentDirectory, path);

		AnimationState.Animation.ObjPath = relativePath;

		ObjState.Load();
		AssetLoadScheduleState.Schedule();

		AnimationState.Animation.Meshes.Clear();
		if (ObjState.ModelData != null)
			AnimationState.Animation.Meshes.AddRange(ObjState.ModelData.Meshes.Select(m => Create(ObjState.ModelData, m)));
		else
			DebugState.AddWarning("Model data is null. Failed to add meshes.");

		AnimationState.Track("Added assets");
	}

	private static AnimationMesh Create(ModelData modelData, MeshData meshData)
	{
		Vector3 origin = Vector3.Zero;
		foreach (ushort vertex in meshData.Faces.Select(face => face.Position))
			origin += modelData.Positions[vertex - 1];
		origin /= meshData.Faces.Count;

		return new(meshData.ObjectName, origin, null, []);
	}
}
