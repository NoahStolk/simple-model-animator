using Detach;
using Detach.Numerics;
using ImGuiNET;
using SimpleModelAnimator.State;

namespace SimpleModelAnimator.Ui;

public static class WarningsWindow
{
	public static void Render()
	{
		if (ImGui.Begin("Warnings"))
		{
			ImGui.BeginDisabled(DebugState.Warnings.Count == 0);
			if (ImGui.Button("Clear"))
				DebugState.ClearWarnings();

			ImGui.EndDisabled();

			if (ImGui.BeginChild("WarningsList"))
			{
				if (DebugState.Warnings.Count > 0)
				{
					foreach (KeyValuePair<string, int> kvp in DebugState.Warnings)
						ImGui.TextWrapped(Inline.Span($"{kvp.Key}: {kvp.Value}"));
				}
				else
				{
					ImGui.TextColored(Color.Green, "No warnings");
				}
			}

			ImGui.EndChild();
		}

		ImGui.End();
	}
}
