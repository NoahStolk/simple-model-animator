using SimpleModelAnimator.Formats.Animation.Model;

namespace SimpleModelAnimator.Extensions;

public static class AnimationMeshExtensions
{
	public static Matrix4x4 GetModelMatrix(this AnimationMesh animationMesh)
	{
		return Matrix4x4.Identity;
		//return Matrix4x4.CreateScale(animationMesh.Scale) * MathUtils.CreateRotationMatrixFromEulerAngles(MathUtils.ToRadians(animationMesh.Rotation)) * Matrix4x4.CreateTranslation(animationMesh.Position);
	}
}
