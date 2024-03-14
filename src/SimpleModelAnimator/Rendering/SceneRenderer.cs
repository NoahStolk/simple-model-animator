using Silk.NET.OpenGL;
using SimpleModelAnimator.Content;
using SimpleModelAnimator.Extensions;
using SimpleModelAnimator.State;
using SimpleModelAnimator.Utils;
using static SimpleModelAnimator.Graphics;

namespace SimpleModelAnimator.Rendering;

public static class SceneRenderer
{
	private static readonly uint _lineVao = VaoUtils.CreateLineVao([Vector3.Zero, Vector3.UnitZ]);

	public static void RenderScene()
	{
		ShaderCacheEntry lineShader = InternalContent.Shaders["Line"];
		Gl.UseProgram(lineShader.Id);

		Gl.UniformMatrix4x4(lineShader.GetUniformLocation("view"), Camera3d.ViewMatrix);
		Gl.UniformMatrix4x4(lineShader.GetUniformLocation("projection"), Camera3d.Projection);

		RenderOrigin(lineShader);
		RenderGrid(lineShader);
		RenderMeshOrigins(lineShader);

		ShaderCacheEntry meshShader = InternalContent.Shaders["Mesh"];
		Gl.UseProgram(meshShader.Id);

		Gl.UniformMatrix4x4(meshShader.GetUniformLocation("view"), Camera3d.ViewMatrix);
		Gl.UniformMatrix4x4(meshShader.GetUniformLocation("projection"), Camera3d.Projection);

		RenderMeshes(meshShader);
	}

	private static void RenderOrigin(ShaderCacheEntry lineShader)
	{
		int lineModelUniform = lineShader.GetUniformLocation("model");
		int lineColorUniform = lineShader.GetUniformLocation("color");

		Gl.BindVertexArray(_lineVao);

		Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(1, 1, 256);

		Gl.LineWidth(4);

		// X axis
		Gl.UniformMatrix4x4(lineModelUniform, scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 2));
		Gl.Uniform4(lineColorUniform, new Vector4(1, 0, 0, 1));
		Gl.DrawArrays(PrimitiveType.Lines, 0, 2);

		// Y axis
		Gl.UniformMatrix4x4(lineModelUniform, scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, MathF.PI * 1.5f));
		Gl.Uniform4(lineColorUniform, new Vector4(0, 1, 0, 1));
		Gl.DrawArrays(PrimitiveType.Lines, 0, 2);

		// Z axis
		Gl.UniformMatrix4x4(lineModelUniform, scaleMatrix);
		Gl.Uniform4(lineColorUniform, new Vector4(0, 0, 1, 1));
		Gl.DrawArrays(PrimitiveType.Lines, 0, 2);

		Gl.LineWidth(2);

		// X axis (negative)
		Gl.UniformMatrix4x4(lineModelUniform, scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, -MathF.PI / 2));
		Gl.Uniform4(lineColorUniform, new Vector4(1, 0, 0, 0.5f));
		Gl.DrawArrays(PrimitiveType.Lines, 0, 2);

		// Y axis (negative)
		Gl.UniformMatrix4x4(lineModelUniform, scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, MathF.PI / 2));
		Gl.Uniform4(lineColorUniform, new Vector4(0, 1, 0, 0.5f));
		Gl.DrawArrays(PrimitiveType.Lines, 0, 2);

		// Z axis (negative)
		Gl.UniformMatrix4x4(lineModelUniform, scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathF.PI));
		Gl.Uniform4(lineColorUniform, new Vector4(0, 0, 1, 0.5f));
		Gl.DrawArrays(PrimitiveType.Lines, 0, 2);
	}

	private static void RenderGrid(ShaderCacheEntry lineShader)
	{
		int lineModelUniform = lineShader.GetUniformLocation("model");
		int lineColorUniform = lineShader.GetUniformLocation("color");

		Gl.Uniform4(lineColorUniform, new Vector4(0.5f, 0.5f, 0.5f, 1));
		Gl.LineWidth(1);

		int min = -AnimationEditorState.GridCellCount;
		int max = AnimationEditorState.GridCellCount;
		Vector3 scale = new(1, 1, (max - min) * AnimationEditorState.GridCellSize);
		Matrix4x4 scaleMat = Matrix4x4.CreateScale(scale);
		Vector3 offset = new(MathF.Round(Camera3d.Position.X), 0, MathF.Round(Camera3d.Position.Z));
		offset.X = MathF.Round(offset.X / AnimationEditorState.GridCellSize) * AnimationEditorState.GridCellSize;
		offset.Z = MathF.Round(offset.Z / AnimationEditorState.GridCellSize) * AnimationEditorState.GridCellSize;

		for (int i = min; i <= max; i++)
		{
			// Prevent rendering grid lines on top of origin lines (Z-fighting).
			if (i * AnimationEditorState.GridCellSize + offset.X != 0)
			{
				Gl.UniformMatrix4x4(lineModelUniform, scaleMat * Matrix4x4.CreateTranslation(new Vector3(i * AnimationEditorState.GridCellSize, 0, min * AnimationEditorState.GridCellSize) + offset));
				Gl.DrawArrays(PrimitiveType.Lines, 0, 2);
			}

			if (i * AnimationEditorState.GridCellSize + offset.Z != 0)
			{
				Gl.UniformMatrix4x4(lineModelUniform, scaleMat * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 2) * Matrix4x4.CreateTranslation(new Vector3(min * AnimationEditorState.GridCellSize, 0, i * AnimationEditorState.GridCellSize) + offset));
				Gl.DrawArrays(PrimitiveType.Lines, 0, 2);
			}
		}
	}

	private static void RenderMeshOrigins(ShaderCacheEntry lineShader)
	{
		int lineModelUniform = lineShader.GetUniformLocation("model");
		int lineColorUniform = lineShader.GetUniformLocation("color");

		Gl.Uniform4(lineColorUniform, new Vector4(1, 0, 1, 1));
		Gl.LineWidth(2);

		foreach (MeshEntry mesh in ModelContainer.Meshes)
		{
			Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(0.25f);
			Matrix4x4 translationMatrix = Matrix4x4.CreateTranslation(mesh.Origin);

			Gl.UniformMatrix4x4(lineModelUniform, scaleMatrix * translationMatrix);
			Gl.DrawArrays(PrimitiveType.Lines, 0, 6);
		}
	}

	private static unsafe void RenderMeshes(ShaderCacheEntry meshShader)
	{
		int modelUniform = meshShader.GetUniformLocation("model");

		Gl.BindTexture(TextureTarget.Texture2D, InternalContent.Textures["Blank"]);

		foreach (MeshEntry mesh in ModelContainer.Meshes)
		{
			Gl.UniformMatrix4x4(modelUniform, mesh.Transformation);

			Gl.BindVertexArray(mesh.MeshVao);
			fixed (uint* index = &mesh.Mesh.Indices[0])
				Gl.DrawElements(PrimitiveType.Triangles, (uint)mesh.Mesh.Indices.Length, DrawElementsType.UnsignedInt, index);
		}
	}
}
