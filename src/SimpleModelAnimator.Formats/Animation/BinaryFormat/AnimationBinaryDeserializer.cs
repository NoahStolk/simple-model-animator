using SimpleModelAnimator.Formats.Animation.Model;
using SimpleModelAnimator.Formats.Extensions;
using System.Numerics;

namespace SimpleModelAnimator.Formats.Animation.BinaryFormat;

public static class AnimationBinaryDeserializer
{
	private const int _version = 1;

	public static AnimationData ReadAnimation(Stream stream)
	{
		stream.Position = 0;

		// Header
		using BinaryReader br = new(stream);
		Span<byte> header = br.ReadBytes(4).AsSpan();
		if (!header.SequenceEqual(AnimationBinaryConstants.Header))
			throw new InvalidDataException("Invalid header");

		int version = br.Read7BitEncodedInt();
		if (version != _version)
			throw new NotSupportedException("Unsupported version");

		int frameCount = br.Read7BitEncodedInt();
		float framesPerSecond = br.ReadSingle();
		bool hasObjPath = br.ReadBoolean();
		string? objPath = hasObjPath ? br.ReadString() : null;

		// Sections
		List<AnimationMesh> animationMeshes = [];

		int sectionCount = br.Read7BitEncodedInt();
		for (int i = 0; i < sectionCount; i++)
		{
			int sectionId = br.Read7BitEncodedInt();
			int sectionLength = br.Read7BitEncodedInt();
			byte[] sectionData = br.ReadBytes(sectionLength);
			switch (sectionId)
			{
				case AnimationBinaryConstants.MeshesSectionId:
					animationMeshes = ReadMeshesSection(sectionData);
					break;
			}
		}

		return new()
		{
			FrameCount = frameCount,
			FramesPerSecond = framesPerSecond,
			ObjPath = objPath,
			Meshes = animationMeshes,
		};
	}

	private static List<AnimationMesh> ReadMeshesSection(byte[] data)
	{
		using MemoryStream ms = new(data);
		using BinaryReader br = new(ms);
		int count = br.Read7BitEncodedInt();
		List<AnimationMesh> animationMeshes = [];
		for (int i = 0; i < count; i++)
		{
			string meshName = br.ReadString();
			Vector3 origin = br.ReadVector3();
			string? parentMeshName = br.ReadOptionalString();
			int keyFrameCount = br.Read7BitEncodedInt();
			List<AnimationKeyFrame> keyFrames = [];
			for (int j = 0; j < keyFrameCount; j++)
			{
				int index = br.Read7BitEncodedInt();
				Vector3 position = br.ReadVector3();
				Quaternion rotation = br.ReadQuaternion();
				keyFrames.Add(new(index, position, rotation));
			}

			AnimationMesh am = new(meshName, origin, parentMeshName, keyFrames);
			animationMeshes.Add(am);
		}

		return animationMeshes;
	}
}
