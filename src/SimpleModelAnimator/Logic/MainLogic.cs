using SimpleModelAnimator.State;

namespace SimpleModelAnimator.Logic;

public static class MainLogic
{
	public static void Run()
	{
		AssetLoadScheduleState.LoadIfScheduled();
	}
}
