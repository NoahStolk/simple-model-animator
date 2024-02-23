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

		bool reloadedSuccessfully = AnimationState.ReloadAssets();
		_needsLoad = !reloadedSuccessfully;
	}
}
