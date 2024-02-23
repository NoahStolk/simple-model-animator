namespace SimpleModelAnimator.Formats.Animation.Model;

public class AnimationMesh
{
	public required string RelativeModelPath;
	public required string MeshName;
	public required string TextureName;

	public AnimationMesh DeepCopy()
	{
		return new()
		{
			RelativeModelPath = RelativeModelPath,
			MeshName = MeshName,
			TextureName = TextureName,
		};
	}
}
