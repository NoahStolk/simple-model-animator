using System.Numerics;

namespace SimpleModelAnimator.Formats.Animation.Model;

public class AnimationMesh
{
	public readonly string MeshName;
	public bool IsRoot;
	public Vector3 Origin;
	public readonly List<AnimationMesh> Children;
	public readonly List<AnimationKeyFrame> KeyFrames;

	public AnimationMesh(string meshName, bool isRoot, Vector3 origin, List<AnimationMesh> children, List<AnimationKeyFrame> keyFrames)
	{
		MeshName = meshName;
		IsRoot = isRoot;
		Origin = origin;
		Children = children;
		KeyFrames = keyFrames;
	}

	public AnimationMesh DeepCopy()
	{
		List<AnimationKeyFrame> newKeyFrames = KeyFrames.ConvertAll(kf => kf.DeepCopy());
		List<AnimationMesh> newChildren = Children.ConvertAll(c => c.DeepCopy());

		return new(MeshName, IsRoot, Origin, newChildren, newKeyFrames);
	}
}
