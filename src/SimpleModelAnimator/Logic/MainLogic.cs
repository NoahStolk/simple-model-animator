using SimpleModelAnimator.State;

namespace SimpleModelAnimator.Logic;

public static class MainLogic
{
	public static void Run(float dt)
	{
		AssetLoadScheduleState.LoadIfScheduled();
		AnimationPlayerState.Update(dt);
	}
}
