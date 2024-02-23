namespace SimpleModelAnimator.Formats.Animation.Model;

public class AnimationData
{
	public required float FramesPerSecond;
	public required string? ObjPath;
	public required List<AnimationMesh> Meshes;

	public static AnimationData CreateDefault()
	{
		return new()
		{
			FramesPerSecond = 30,
			ObjPath = string.Empty,
			Meshes = [],
		};
	}

	public AnimationData DeepCopy()
	{
		List<AnimationMesh> newMeshes = [];
		for (int i = 0; i < Meshes.Count; i++)
			newMeshes.Add(Meshes[i].DeepCopy());

		return new()
		{
			FramesPerSecond = FramesPerSecond,
			ObjPath = ObjPath,
			Meshes = newMeshes,
		};
	}
}
