using SimpleModelAnimator.Formats.Animation.Model;

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

		float framesPerSecond = br.ReadSingle();

		// Sections
		List<string> modelPaths = [];
		List<string> texturePaths = [];
		List<AnimationMesh> animationMeshes = [];

		int sectionCount = br.Read7BitEncodedInt();
		for (int i = 0; i < sectionCount; i++)
		{
			int sectionId = br.Read7BitEncodedInt();
			int sectionLength = br.Read7BitEncodedInt();
			byte[] sectionData = br.ReadBytes(sectionLength);
			switch (sectionId)
			{
				case AnimationBinaryConstants.ModelsSectionId:
					modelPaths = ReadStringListSection(sectionData);
					break;
				case AnimationBinaryConstants.TexturesSectionId:
					texturePaths = ReadStringListSection(sectionData);
					break;
				case AnimationBinaryConstants.MeshesSectionId:
					animationMeshes = ReadMeshesSection(sectionData);
					break;
			}
		}

		return new()
		{
			FramesPerSecond = framesPerSecond,
			RelativeModelPaths = modelPaths,
			RelativeTexturePaths = texturePaths,
			Meshes = animationMeshes,
		};
	}

	private static List<string> ReadStringListSection(byte[] data)
	{
		using MemoryStream ms = new(data);
		using BinaryReader br = new(ms);
		int count = br.Read7BitEncodedInt();
		List<string> result = [];
		for (int i = 0; i < count; i++)
			result.Add(br.ReadString());
		return result;
	}

	private static List<AnimationMesh> ReadMeshesSection(byte[] data)
	{
		using MemoryStream ms = new(data);
		using BinaryReader br = new(ms);
		int count = br.Read7BitEncodedInt();
		List<AnimationMesh> animationMeshes = [];
		for (int i = 0; i < count; i++)
		{
			string relativeModelPath = br.ReadString();
			string meshName = br.ReadString();
			string textureName = br.ReadString();

			AnimationMesh am = new()
			{
				RelativeModelPath = relativeModelPath,
				MeshName = meshName,
				TextureName = textureName,
			};
			animationMeshes.Add(am);
		}

		return animationMeshes;
	}
}
