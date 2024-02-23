using SimpleModelAnimator.Content.Data;

namespace SimpleModelAnimator.Rendering;

public record MeshEntry(Mesh Mesh, uint MeshVao, uint[] LineIndices, uint LineVao, Vector3 BoundingMin, Vector3 BoundingMax);
