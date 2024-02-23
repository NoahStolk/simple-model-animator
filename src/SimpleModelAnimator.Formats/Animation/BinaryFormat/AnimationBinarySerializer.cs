using SimpleModelAnimator.Formats.Animation.Model;
using SimpleModelAnimator.Formats.Extensions;

namespace SimpleModelAnimator.Formats.Animation.BinaryFormat;

public static class AnimationBinarySerializer
{
	private const int _version = 1;

	public static void WriteAnimation(MemoryStream ms, AnimationData animation)
	{
		using BinaryWriter bw = new(ms);

		// Header
		bw.Write(AnimationBinaryConstants.Header);
		bw.Write7BitEncodedInt(_version);
		bw.Write(animation.FramesPerSecond);
		bw.Write(animation.ObjPath != null);
		if (animation.ObjPath != null)
			bw.Write(animation.ObjPath);

		// Sections
		List<Section> sections =
		[
			new(AnimationBinaryConstants.MeshesSectionId, WriteMeshesSection(animation.Meshes)),
		];

		bw.Write7BitEncodedInt(sections.Count);
		foreach (Section section in sections)
			section.Write(bw);
	}

	private static byte[] WriteMeshesSection(IReadOnlyCollection<AnimationMesh> animationMeshes)
	{
		using MemoryStream ms = new();
		using BinaryWriter bw = new(ms);
		bw.Write7BitEncodedInt(animationMeshes.Count);
		foreach (AnimationMesh am in animationMeshes)
		{
			bw.Write(am.MeshName);
			bw.Write(am.Origin);
			bw.WriteOptional(am.ParentMeshName);
			bw.Write7BitEncodedInt(am.KeyFrames.Count);
			foreach (AnimationKeyFrame kf in am.KeyFrames)
			{
				bw.Write7BitEncodedInt(kf.Index);
				bw.Write(kf.Position);
				bw.Write(kf.Rotation);
			}
		}

		return ms.ToArray();
	}
}
