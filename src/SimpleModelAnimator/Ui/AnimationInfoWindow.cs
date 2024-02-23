using Detach;
using ImGuiNET;
using SimpleModelAnimator.Formats.Animation.Model;
using SimpleModelAnimator.State;
using System.Globalization;

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
		ImGui.Text(Inline.Span($"Frame count: {animation.FrameCount}"));
		ImGui.Text(Inline.Span($"OBJ file: {animation.ObjPath}"));
		ImGui.Text(Inline.Span($"Meshes: {animation.Meshes.Count}"));

		ImGui.Separator();

		if (ImGui.TreeNode("Meshes"))
		{
			if (ImGui.BeginTable("MeshTable", 5))
			{
				ImGui.TableSetupColumn("Name");
				ImGui.TableSetupColumn("Root", ImGuiTableColumnFlags.WidthFixed, 32);
				ImGui.TableSetupColumn("Origin");
				ImGui.TableSetupColumn("Children");
				ImGui.TableSetupColumn("Key frames");

				ImGui.TableHeadersRow();

				for (int i = 0; i < animation.Meshes.Count; i++)
				{
					AnimationMesh mesh = animation.Meshes[i];

					ImGui.TableNextRow();

					ImGui.TableNextColumn();
					ImGui.Text(mesh.MeshName);

					ImGui.TableNextColumn();
					ImGui.Text(mesh.IsRoot ? "Yes" : "No");

					ImGui.TableNextColumn();
					ImGui.Text(Inline.Span(mesh.Origin, "0.00", CultureInfo.InvariantCulture));

					ImGui.TableNextColumn();
					ImGui.Text(Inline.Span(mesh.Children.Count));

					if (ImGui.IsItemHovered() && mesh.Children.Count > 0)
					{
						ImGui.BeginTooltip();
						if (ImGui.BeginTable("ChildTable", 1))
						{
							ImGui.TableSetupColumn("Name");
							ImGui.TableHeadersRow();

							for (int j = 0; j < mesh.Children.Count; j++)
							{
								AnimationMesh child = mesh.Children[j];

								ImGui.TableNextRow();

								ImGui.TableNextColumn();
								ImGui.Text(child.MeshName);
							}

							ImGui.EndTable();
						}

						ImGui.EndTooltip();
					}

					ImGui.TableNextColumn();
					ImGui.Text(Inline.Span(mesh.KeyFrames.Count));

					if (ImGui.IsItemHovered() && mesh.KeyFrames.Count > 0)
					{
						ImGui.BeginTooltip();
						if (ImGui.BeginTable("KeyFrameTable", 3))
						{
							ImGui.TableSetupColumn("Index");
							ImGui.TableSetupColumn("Position");
							ImGui.TableSetupColumn("Rotation");
							ImGui.TableHeadersRow();

							for (int j = 0; j < mesh.KeyFrames.Count; j++)
							{
								AnimationKeyFrame keyFrame = mesh.KeyFrames[j];

								ImGui.TableNextRow();

								ImGui.TableNextColumn();
								ImGui.Text(Inline.Span(keyFrame.Index));

								ImGui.TableNextColumn();
								ImGui.Text(Inline.Span(keyFrame.Position, "0.00", CultureInfo.InvariantCulture));

								ImGui.TableNextColumn();
								ImGui.Text(Inline.Span(keyFrame.Rotation, "0.00", CultureInfo.InvariantCulture));
							}

							ImGui.EndTable();
						}

						ImGui.EndTooltip();
					}
				}

				ImGui.EndTable();
			}

			ImGui.TreePop();
		}
	}
}
