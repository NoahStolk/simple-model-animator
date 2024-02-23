using System.Numerics;

namespace SimpleModelAnimator.Formats.Animation.Model;

public class AnimationMesh
{
	public readonly string MeshName;
	public Vector3 Origin;
	public string? ParentMeshName;
	public readonly List<AnimationKeyFrame> KeyFrames;

	public AnimationMesh(string meshName, Vector3 origin, string? parentMeshName, List<AnimationKeyFrame> keyFrames)
	{
		MeshName = meshName;
		Origin = origin;
		ParentMeshName = parentMeshName;
		KeyFrames = keyFrames;
	}

	public AnimationMesh DeepCopy()
	{
		List<AnimationKeyFrame> newKeyFrames = KeyFrames.ConvertAll(kf => new AnimationKeyFrame(kf.Index, kf.Position, kf.Rotation));

		return new(MeshName, Origin, ParentMeshName, newKeyFrames);
	}
}
