namespace SimpleModelAnimator.Formats.Animation.Model;

public class AnimationData
{
	public required float FramesPerSecond;
	public required List<string> RelativeModelPaths;
	public required List<string> RelativeTexturePaths;
	public required List<AnimationMesh> Meshes;

	public static AnimationData CreateDefault()
	{
		return new()
		{
			FramesPerSecond = 30,
			RelativeModelPaths = [],
			RelativeTexturePaths = [],
			Meshes = [],
		};
	}

	public AnimationData DeepCopy()
	{
		List<string> newRelativeModelPaths = [];
		for (int i = 0; i < RelativeModelPaths.Count; i++)
			newRelativeModelPaths.Add(RelativeModelPaths[i]);

		List<string> newRelativeTexturePaths = [];
		for (int i = 0; i < RelativeTexturePaths.Count; i++)
			newRelativeTexturePaths.Add(RelativeTexturePaths[i]);

		List<AnimationMesh> newMeshes = [];
		for (int i = 0; i < Meshes.Count; i++)
			newMeshes.Add(Meshes[i].DeepCopy());

		return new()
		{
			FramesPerSecond = FramesPerSecond,
			RelativeModelPaths = newRelativeModelPaths,
			RelativeTexturePaths = newRelativeTexturePaths,
			Meshes = newMeshes,
		};
	}
}
