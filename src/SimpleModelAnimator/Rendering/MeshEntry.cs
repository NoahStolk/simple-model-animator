using SimpleModelAnimator.Content.Data;

namespace SimpleModelAnimator.Rendering;

public record MeshEntry(string Name, Mesh Mesh, uint MeshVao, uint[] LineIndices, uint LineVao)
{
	public Vector3 Origin { get; set; }
	public Matrix4x4 Transformation { get; set; }
}
