using Detach.Parsers.Model;
using Silk.NET.OpenGL;
using SimpleModelAnimator.Content.Data;
using SimpleModelAnimator.State;
using SimpleModelAnimator.Utils;

namespace SimpleModelAnimator.Rendering;

/// <summary>
/// Converts and holds OpenGL data from the OBJ model.
/// </summary>
public static class ModelContainer
{
	private static readonly List<MeshEntry> _meshes = [];

	public static IReadOnlyList<MeshEntry> Meshes => _meshes;

	public static void Rebuild()
	{
		_meshes.Clear();

		ModelData? modelData = ObjState.ModelData;
		if (modelData == null || modelData.Meshes.Count == 0)
			return;

		foreach (MeshData meshData in modelData.Meshes)
		{
			Mesh mesh = GetMesh(modelData, meshData);
			uint vao = CreateFromMesh(mesh);

			// Find main mesh edges.
			Dictionary<Edge, List<Vector3>> edges = new();
			for (int i = 0; i < meshData.Faces.Count; i += 3)
			{
				uint a = (ushort)(meshData.Faces[i + 0].Position - 1);
				uint b = (ushort)(meshData.Faces[i + 1].Position - 1);
				uint c = (ushort)(meshData.Faces[i + 2].Position - 1);

				Vector3 positionA = modelData.Positions[(int)a];
				Vector3 positionB = modelData.Positions[(int)b];
				Vector3 positionC = modelData.Positions[(int)c];
				Vector3 normal = Vector3.Normalize(Vector3.Cross(positionB - positionA, positionC - positionA));
				if (float.IsNaN(normal.X) || float.IsNaN(normal.Y) || float.IsNaN(normal.Z))
					continue;

				AddEdge(edges, new(a, b), normal);
				AddEdge(edges, new(b, c), normal);
				AddEdge(edges, new(c, a), normal);
			}

			// Find edges that are only used by one triangle.
			List<uint> lineIndices = [];
			foreach (KeyValuePair<Edge, List<Vector3>> edge in edges)
			{
				int distinctNormals = edge.Value.Distinct(NormalComparer.Instance).Count();
				if (edge.Value.Count > 1 && distinctNormals == 1)
					continue;

				lineIndices.Add(edge.Key.A);
				lineIndices.Add(edge.Key.B);
			}

			MeshEntry entry = new(meshData.ObjectName, mesh, vao, lineIndices.ToArray(), VaoUtils.CreateLineVao(modelData.Positions.ToArray()));
			_meshes.Add(entry);
		}

		void AddEdge(IDictionary<Edge, List<Vector3>> edges, Edge d, Vector3 normal)
		{
			if (!edges.ContainsKey(d))
				edges.Add(d, []);

			edges[d].Add(normal);
		}
	}

	private static Mesh GetMesh(ModelData modelData, MeshData meshData)
	{
		Vertex[] outVertices = new Vertex[meshData.Faces.Count];
		uint[] outFaces = new uint[meshData.Faces.Count];
		for (int i = 0; i < meshData.Faces.Count; i++)
		{
			ushort v = meshData.Faces[i].Position;
			ushort vt = meshData.Faces[i].Texture;
			ushort vn = meshData.Faces[i].Normal;

			Vector3 position = modelData.Positions.Count > v - 1 && v > 0 ? modelData.Positions[v - 1] : default;
			Vector2 texture = modelData.Textures.Count > vt - 1 && vt > 0 ? modelData.Textures[vt - 1] : default;
			Vector3 normal = modelData.Normals.Count > vn - 1 && vn > 0 ? modelData.Normals[vn - 1] : default;

			texture = texture with { Y = 1 - texture.Y };

			outVertices[i] = new(position, texture, normal);
			outFaces[i] = (uint)i;
		}

		return new(outVertices, outFaces);
	}

	private static unsafe uint CreateFromMesh(Mesh mesh)
	{
		uint vao = Graphics.Gl.GenVertexArray();
		uint vbo = Graphics.Gl.GenBuffer();

		Graphics.Gl.BindVertexArray(vao);

		Graphics.Gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
		fixed (Vertex* v = &mesh.Vertices[0])
			Graphics.Gl.BufferData(BufferTargetARB.ArrayBuffer, (uint)(mesh.Vertices.Length * sizeof(Vertex)), v, BufferUsageARB.StaticDraw);

		Graphics.Gl.EnableVertexAttribArray(0);
		Graphics.Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)0);

		Graphics.Gl.EnableVertexAttribArray(1);
		Graphics.Gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)(3 * sizeof(float)));

		Graphics.Gl.EnableVertexAttribArray(2);
		Graphics.Gl.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)(5 * sizeof(float)));

		Graphics.Gl.BindVertexArray(0);
		Graphics.Gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
		Graphics.Gl.DeleteBuffer(vbo);

		return vao;
	}

	private sealed record Edge(uint A, uint B)
	{
		public bool Equals(Edge? other)
		{
			if (ReferenceEquals(null, other))
				return false;
			if (ReferenceEquals(this, other))
				return true;

			return A == other.A && B == other.B || A == other.B && B == other.A;
		}

		public override int GetHashCode()
		{
			return A < B ? HashCode.Combine(A, B) : HashCode.Combine(B, A);
		}
	}

	private sealed class NormalComparer : IEqualityComparer<Vector3>
	{
		public static readonly NormalComparer Instance = new();

		public bool Equals(Vector3 x, Vector3 y)
		{
			const float epsilon = 0.01f;
			return Math.Abs(x.X - y.X) < epsilon && Math.Abs(x.Y - y.Y) < epsilon && Math.Abs(x.Z - y.Z) < epsilon;
		}

		public int GetHashCode(Vector3 obj)
		{
			return obj.GetHashCode();
		}
	}
}
