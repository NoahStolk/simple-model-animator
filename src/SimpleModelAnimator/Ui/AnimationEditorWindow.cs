using ImGuiNET;
using SimpleModelAnimator.Content;
using SimpleModelAnimator.Rendering;
using SimpleModelAnimator.State;

namespace SimpleModelAnimator.Ui;

public static class AnimationEditorWindow
{
	public static void Render()
	{
		if (ImGui.Begin("Animation Editor"))
		{
			Vector2 framebufferSize = ImGui.GetContentRegionAvail();

			SceneFramebuffer.Initialize(framebufferSize);
			Camera3d.AspectRatio = framebufferSize.X / framebufferSize.Y;

			SceneFramebuffer.RenderFramebuffer(framebufferSize);

			ImDrawListPtr drawList = ImGui.GetWindowDrawList();
			Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
			drawList.AddImage((IntPtr)SceneFramebuffer.FramebufferTextureId, cursorScreenPos, cursorScreenPos + framebufferSize, Vector2.UnitY, Vector2.UnitX);

			Vector2 focusPointIconSize = new(16, 16);
			Vector2 focusPointIconPosition = cursorScreenPos + Camera3d.GetScreenPositionFrom3dPoint(Camera3d.FocusPointTarget, framebufferSize) - focusPointIconSize / 2;
			drawList.AddImage((IntPtr)InternalContent.Textures["FocusPoint"], focusPointIconPosition, focusPointIconPosition + focusPointIconSize);

			Vector2 cursorPosition = ImGui.GetCursorPos();

			ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0, 0, 0, 0.2f));
			if (ImGui.BeginChild("Animation Editor Menu", new(280, 192), ImGuiChildFlags.Border))
			{
				const int itemWidth = 160;
				if (ImGui.BeginTabBar("AnimationEditorMenus"))
				{
					ImGui.PushItemWidth(itemWidth);

					if (ImGui.BeginTabItem("Display"))
					{
						ImGui.SliderInt("Cells per side", ref AnimationEditorState.GridCellCount, 1, 64);
						ImGui.SliderInt("Cell size", ref AnimationEditorState.GridCellSize, 1, 4);

						ImGui.EndTabItem();
					}

					ImGui.PopItemWidth();

					ImGui.EndTabBar();
				}
			}

			ImGui.EndChild(); // End Animation Editor Menu

			ImGui.PopStyleColor();

			ImGui.SetCursorPos(cursorPosition);
			ImGui.InvisibleButton("3d_view", framebufferSize);
			bool isFocused = ImGui.IsItemHovered();
			Camera3d.Update(App.Instance.FrameTime, isFocused);
		}

		ImGui.End();
	}
}
