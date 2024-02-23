namespace SimpleModelAnimator.Formats.Animation.Model;

public class AnimationMesh
{
	public readonly string MeshName;

	public AnimationMesh(string meshName)
	{
		MeshName = meshName;
	}

	public AnimationMesh DeepCopy()
	{
		return new(MeshName);
	}
}
