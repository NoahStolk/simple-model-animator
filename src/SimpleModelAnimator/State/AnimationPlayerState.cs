namespace SimpleModelAnimator.State;

public static class AnimationPlayerState
{
	public static float Time;

	public static int FrameIndex => AnimationState.Animation.FramesPerSecond == 0 ? 0 : (int)(Time * AnimationState.Animation.FramesPerSecond);

	public static void Update(float deltaTime)
	{
		Time += deltaTime;
		if (Time > AnimationState.Animation.FrameCount / AnimationState.Animation.FramesPerSecond)
			Time = 0;
	}
}
