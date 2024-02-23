using Detach.Parsers.Texture;
using Detach.Parsers.Texture.TgaFormat;
using Silk.NET.OpenGL;
using SimpleModelAnimator.State;

namespace SimpleModelAnimator.Rendering;

public static class TextureContainer
{
	private static readonly Dictionary<string, uint> _textures = new();

	public static uint? GetTexture(string path)
	{
		if (_textures.TryGetValue(path, out uint textureId))
			return textureId;

		DebugState.AddWarning($"Cannot find texture '{path}'");
		return null;
	}

	public static void Rebuild(string? animationFilePath)
	{
		_textures.Clear();

		uint defaultTextureId = CreateFromTexture(1, 1, [0xFF, 0xFF, 0xFF, 0xFF]);
		_textures.Add(string.Empty, defaultTextureId);

		string? animationDirectory = Path.GetDirectoryName(animationFilePath);
		if (animationDirectory == null)
			return;

		foreach (string texturePath in AnimationState.Animation.RelativeTexturePaths)
		{
			string absolutePath = Path.Combine(animationDirectory, texturePath);

			if (!File.Exists(absolutePath))
				continue;

			TextureData textureData = TgaParser.Parse(File.ReadAllBytes(absolutePath));
			uint textureId = CreateFromTexture(textureData.Width, textureData.Height, textureData.ColorData);
			_textures.Add(texturePath, textureId);
		}
	}

	private static unsafe uint CreateFromTexture(uint width, uint height, byte[] pixels)
	{
		uint textureId = Graphics.Gl.GenTexture();

		Graphics.Gl.BindTexture(TextureTarget.Texture2D, textureId);

		Graphics.Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
		Graphics.Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
		Graphics.Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Nearest);
		Graphics.Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Nearest);

		fixed (byte* b = pixels)
			Graphics.Gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, width, height, 0, GLEnum.Rgba, PixelType.UnsignedByte, b);

		Graphics.Gl.GenerateMipmap(TextureTarget.Texture2D);

		return textureId;
	}
}
