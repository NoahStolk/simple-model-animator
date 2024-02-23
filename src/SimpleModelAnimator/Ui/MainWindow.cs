using ImGuiNET;
using SimpleModelAnimator.State;

namespace SimpleModelAnimator.Ui;

public static class MainWindow
{
	private static bool _showDemoWindow;
	private static bool _showControlsWindow;
	private static bool _showInputDebugWindow;
	private static bool _showDebugWindow;

	public static void Render()
	{
		if (_showDemoWindow)
			ImGui.ShowDemoWindow(ref _showDemoWindow);

		if (_showControlsWindow)
			ControlsWindow.Render(ref _showControlsWindow);

		if (_showInputDebugWindow)
			InputDebugWindow.Render(ref _showInputDebugWindow);

		if (_showDebugWindow)
			DebugWindow.Render(ref _showDebugWindow);

		Vector2 viewportSize = ImGui.GetMainViewport().Size;
		ImGui.SetNextWindowSize(viewportSize);
		ImGui.SetNextWindowPos(Vector2.Zero);

		if (ImGui.BeginMainMenuBar())
		{
			if (ImGui.BeginMenu("File"))
			{
				if (ImGui.MenuItem("New", Shortcuts.GetKeyDescription(Shortcuts.New)))
					AnimationState.New();

				if (ImGui.MenuItem("Open", Shortcuts.GetKeyDescription(Shortcuts.Open)))
					AnimationState.Load();

				if (ImGui.MenuItem("Save", Shortcuts.GetKeyDescription(Shortcuts.Save)))
					AnimationState.Save();

				if (ImGui.MenuItem("Save As", Shortcuts.GetKeyDescription(Shortcuts.SaveAs)))
					AnimationState.SaveAs();

				ImGui.EndMenu();
			}

			if (ImGui.BeginMenu("Debug"))
			{
				if (ImGui.MenuItem("Show ImGui Demo"))
					_showDemoWindow = true;

				if (ImGui.MenuItem("Show Input Debug"))
					_showInputDebugWindow = true;

				if (ImGui.MenuItem("Show Debug"))
					_showDebugWindow = true;

				ImGui.EndMenu();
			}

			if (ImGui.BeginMenu("Help"))
			{
				if (ImGui.MenuItem("Controls & Shortcuts"))
					_showControlsWindow = true;

				ImGui.EndMenu();
			}

			ImGui.EndMainMenuBar();
		}

		AnimationEditorWindow.Render();
		AnimationAssetsWindow.Render();
		AnimationInfoWindow.Render();
		AnimationPlayerWindow.Render();
		WarningsWindow.Render();
	}
}
