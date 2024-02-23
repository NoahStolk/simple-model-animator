namespace SimpleModelAnimator.Formats.Animation.Model;

public class AnimationMesh
{
	public readonly string RelativeModelPath;
	public readonly string MeshName;

	public AnimationMesh(string relativeModelPath, string meshName)
	{
		RelativeModelPath = relativeModelPath;
		MeshName = meshName;
	}

	public AnimationMesh DeepCopy()
	{
		return new(RelativeModelPath, MeshName);
	}
}
