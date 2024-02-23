using System.Numerics;

namespace SimpleModelAnimator.Formats.Animation.Model;

public class AnimationKeyFrame
{
	public int Index;
	public Vector3 Position;
	public Quaternion Rotation;

	public AnimationKeyFrame(int index, Vector3 position, Quaternion rotation)
	{
		Index = index;
		Position = position;
		Rotation = rotation;
	}

	public AnimationKeyFrame DeepCopy()
	{
		return new(Index, Position, Rotation);
	}
}
