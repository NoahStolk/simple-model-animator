namespace SimpleModelAnimator.Content;

public class ShaderCacheEntry
{
	private readonly Dictionary<string, int> _uniformLocations = new();

	public ShaderCacheEntry(uint id)
	{
		Id = id;
	}

	public uint Id { get; }

	public int GetUniformLocation(string name)
	{
		if (_uniformLocations.TryGetValue(name, out int location))
			return location;

		location = Graphics.Gl.GetUniformLocation(Id, name);
		_uniformLocations.Add(name, location);

		return location;
	}
}
