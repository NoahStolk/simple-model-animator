using ImGuiNET;

namespace SimpleModelAnimator.Ui;

public static class ControlsWindow
{
	private static readonly Dictionary<string, string> _controls = new()
	{
		["Pan camera"] = "Middle mouse button",
		["Look around"] = "Right mouse button",
		["Move focus point"] = "WASD, space, left shift",
		["Zoom in/out"] = "Scroll wheel",
	};

	public static void Render(ref bool showWindow)
	{
		ImGui.SetNextWindowSize(new(512, 384));
		if (ImGui.Begin("Controls & Shortcuts", ref showWindow, ImGuiWindowFlags.NoResize))
		{
			if (ImGui.BeginTable("Table", 2))
			{
				ImGui.TableSetupColumn("Action", ImGuiTableColumnFlags.WidthFixed, 256);
				ImGui.TableSetupColumn("Input", ImGuiTableColumnFlags.None);
				ImGui.TableHeadersRow();

				foreach (KeyValuePair<string, string> kvp in _controls)
				{
					ImGui.TableNextRow();
					ImGui.TableNextColumn();
					ImGui.Text(kvp.Key);
					ImGui.TableNextColumn();
					ImGui.Text(kvp.Value);
				}

				ImGui.EndTable();
			}

			ImGui.Spacing();

			if (ImGui.BeginTable("Table", 2))
			{
				ImGui.TableSetupColumn("Shortcut", ImGuiTableColumnFlags.WidthFixed, 256);
				ImGui.TableSetupColumn("Keys", ImGuiTableColumnFlags.None);
				ImGui.TableHeadersRow();

				ImGui.TableNextRow();
				for (int i = 0; i < Shortcuts.ShortcutsList.Count; i++)
				{
					Shortcut shortcut = Shortcuts.ShortcutsList[i];

					ImGui.TableNextColumn();
					ImGui.Text(shortcut.Description);

					ImGui.TableNextColumn();
					ImGui.Text(shortcut.KeyDescription);
				}

				ImGui.EndTable();
			}
		}

		ImGui.End(); // End Shortcuts
	}
}
