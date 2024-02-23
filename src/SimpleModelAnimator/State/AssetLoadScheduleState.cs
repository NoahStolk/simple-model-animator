using SimpleModelAnimator.Rendering;

namespace SimpleModelAnimator.State;

/// <summary>
/// Workaround due to native file dialog callbacks not always running on the main thread causing OpenGL problems.
/// </summary>
public static class AssetLoadScheduleState
{
	private static bool _needsLoad;

	public static void Schedule()
	{
		_needsLoad = true;
	}

	public static void LoadIfScheduled()
	{
		if (!_needsLoad)
			return;

		bool reloadedSuccessfully;

		try
		{
			ModelContainer.Rebuild();
			reloadedSuccessfully = true;
		}
		catch (Exception ex)
		{
			DebugState.AddWarning($"Failed to reload assets: {ex.Message}");
			reloadedSuccessfully = false;
		}

		_needsLoad = !reloadedSuccessfully;
	}
}
