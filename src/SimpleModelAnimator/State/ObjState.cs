using Detach.Parsers.Model;
using Detach.Parsers.Model.ObjFormat;

namespace SimpleModelAnimator.State;

public static class ObjState
{
	public static ModelData? ModelData { get; private set; }

	public static void Load()
	{
		string? animationDirectory = Path.GetDirectoryName(AnimationState.AnimationFilePath);
		if (animationDirectory == null || AnimationState.Animation.ObjPath == null)
			return;

		string absolutePath = Path.Combine(animationDirectory, AnimationState.Animation.ObjPath);
		if (!File.Exists(absolutePath))
			return;

		ModelData = ObjParser.Parse(File.ReadAllBytes(absolutePath));
	}
}
