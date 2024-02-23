namespace SimpleModelAnimator.State;

/// <summary>
/// Workaround due to native file dialog callbacks not always running on the main thread causing OpenGL problems.
/// </summary>
public static class AssetLoadScheduleState
{
	private static bool _needsLoad;
	private static string? _path;

	public static void Schedule(string? path)
	{
		_needsLoad = true;
		_path = path;
	}

	public static void LoadIfScheduled()
	{
		if (!_needsLoad)
			return;

		bool reloadedSuccessfully = AnimationState.ReloadAssets(_path);
		_needsLoad = !reloadedSuccessfully;
	}
}
