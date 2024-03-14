using SimpleModelAnimator.Formats.Animation.Model;
using SimpleModelAnimator.Rendering;

namespace SimpleModelAnimator.State;

public static class AnimationTransformState
{
	public static void CalculateTransformations()
	{
		for (int i = 0; i < ModelContainer.Meshes.Count; i++)
		{
			MeshEntry mesh = ModelContainer.Meshes[i];
			AnimationMesh? animationMesh = AnimationState.Animation.Meshes.Find(m => m.MeshName == mesh.Name);
			if (animationMesh is not { IsRoot: true })
				continue;

			// Only calculate root meshes here. The rest is done recursively.
			mesh.Origin = animationMesh.Origin;
			mesh.Transformation = Matrix4x4.Identity;
			CalculateTransformation(animationMesh, mesh, AnimationPlayerState.FrameIndex);
		}
	}

	private static void CalculateTransformation(AnimationMesh animationMesh, MeshEntry mesh, int frameIndex)
	{
		if (animationMesh.KeyFrames.Count == 0)
			return;

		KeyFrameResult result = GetKeyFrames(animationMesh, frameIndex);
		Vector3 interpolatedPosition = Vector3.Lerp(result.Previous.Position, result.Next.Position, result.Interpolation);
		Quaternion interpolatedRotation = Quaternion.Slerp(result.Previous.Rotation, result.Next.Rotation, result.Interpolation);
		Matrix4x4 transformation = Matrix4x4.CreateFromQuaternion(interpolatedRotation) * Matrix4x4.CreateTranslation(interpolatedPosition);
		mesh.Transformation *= transformation;

		foreach (AnimationMesh child in animationMesh.Children)
		{
			MeshEntry? childMesh = ModelContainer.Meshes.FirstOrDefault(m => m.Name == child.MeshName);
			if (childMesh == null)
				continue;

			mesh.Origin = animationMesh.Origin;
			childMesh.Transformation = transformation;
			CalculateTransformation(child, childMesh, frameIndex);
		}
	}

	private static KeyFrameResult GetKeyFrames(AnimationMesh animationMesh, int frameIndex)
	{
		for (int i = 1; i < animationMesh.KeyFrames.Count; i++)
		{
			AnimationKeyFrame currentFrame = animationMesh.KeyFrames[i];
			if (currentFrame.Index > frameIndex)
			{
				AnimationKeyFrame previousFrame = animationMesh.KeyFrames[i - 1];
				return new(previousFrame, currentFrame, CalculateInterpolation(frameIndex, previousFrame.Index, currentFrame.Index));
			}
		}

		float interpolation = CalculateInterpolation(frameIndex, animationMesh.KeyFrames[^1].Index, AnimationState.Animation.FrameCount);
		return new(animationMesh.KeyFrames[^1], animationMesh.KeyFrames[0], interpolation);
	}

	private static float CalculateInterpolation(int frameIndex, int previousFrameIndex, int nextFrameIndex)
	{
		return (frameIndex - previousFrameIndex) / (float)(nextFrameIndex - previousFrameIndex);
	}

	private sealed record KeyFrameResult(AnimationKeyFrame Previous, AnimationKeyFrame Next, float Interpolation);
}
