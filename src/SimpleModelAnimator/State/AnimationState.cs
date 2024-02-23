using SimpleModelAnimator.Formats.Animation.BinaryFormat;
using SimpleModelAnimator.Formats.Animation.Model;
using System.Security.Cryptography;

namespace SimpleModelAnimator.State;

public static class AnimationState
{
	private const int _maxHistoryEntries = 100;
	private const string _fileExtension = "sma";

	private static byte[] _memoryMd5Hash;
	private static byte[] _fileMd5Hash;

	private static AnimationData _animation = AnimationData.CreateDefault();

	static AnimationState()
	{
		byte[] fileBytes = GetBytes(_animation);
		_memoryMd5Hash = MD5.HashData(fileBytes);
		_fileMd5Hash = MD5.HashData(fileBytes);

		History = new List<HistoryEntry> { new(_animation, MD5.HashData(fileBytes), "Reset") };
	}

	public static int CurrentHistoryIndex { get; private set; }
	public static string? AnimationFilePath { get; private set; }
	public static bool IsModified { get; private set; }
	public static AnimationData Animation
	{
		get => _animation;
		private set
		{
			_animation = value;

			byte[] fileBytes = GetBytes(_animation);
			_memoryMd5Hash = MD5.HashData(fileBytes);
			IsModified = !_fileMd5Hash.SequenceEqual(_memoryMd5Hash);
		}
	}

	// Note; the history should never be empty.
	public static IReadOnlyList<HistoryEntry> History { get; private set; }

	private static byte[] GetBytes(AnimationData obj)
	{
		using MemoryStream ms = new();
		AnimationBinarySerializer.WriteAnimation(ms, obj);
		return ms.ToArray();
	}

	public static void New()
	{
		AnimationData animation = AnimationData.CreateDefault();
		SetAnimation(null, animation);
		ClearState();
		AssetLoadScheduleState.Schedule();
		Track("Reset");
	}

	public static void Load()
	{
		DialogWrapper.FileOpen(LoadCallback, _fileExtension);
	}

	private static void LoadCallback(string? path)
	{
		if (path == null)
			return;

		using (FileStream fs = new(path, FileMode.Open))
		{
			AnimationData animation = AnimationBinaryDeserializer.ReadAnimation(fs);

			// TEMP DATA
			AnimationMesh baseMesh = animation.Meshes[0];
			AnimationMesh primaryArm = animation.Meshes[1];
			AnimationMesh secondaryArm = animation.Meshes[2];
			AnimationMesh torus = animation.Meshes[3];

			baseMesh.IsRoot = true;
			baseMesh.KeyFrames.Add(new(0, Vector3.Zero, Quaternion.Identity));
			baseMesh.KeyFrames.Add(new(30, new(0, 1, 0), Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathF.PI / 4f)));
			baseMesh.KeyFrames.Add(new(60, Vector3.Zero, Quaternion.Identity));
			baseMesh.Children.Add(primaryArm);

			primaryArm.KeyFrames.Add(new(0, Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 2f)));
			primaryArm.KeyFrames.Add(new(30, new(0, 1, 0), Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 4f)));
			primaryArm.KeyFrames.Add(new(60, Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 2f)));
			primaryArm.Children.Add(secondaryArm);

			secondaryArm.KeyFrames.Add(new(0, Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathF.PI / 2f)));
			secondaryArm.KeyFrames.Add(new(30, new(0, 1, 0), Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathF.PI / 4f)));
			secondaryArm.KeyFrames.Add(new(60, Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitX, MathF.PI / 2f)));
			secondaryArm.Children.Add(torus);

			torus.KeyFrames.Add(new(0, Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 2f)));
			torus.KeyFrames.Add(new(30, new(0, 1, 0), Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 4f)));
			torus.KeyFrames.Add(new(60, Vector3.Zero, Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 2f)));

			SetAnimation(path, animation);
		}

		ClearState();
		ObjState.Load();
		AssetLoadScheduleState.Schedule();
		Track("Reset");
	}

	public static void Save()
	{
		// TODO
		// if (!IsModified)
		// 	action();
		// else
		// 	_savePromptAction(action);

		if (AnimationFilePath != null && File.Exists(AnimationFilePath))
			Save(AnimationFilePath);
		else
			SaveAs();
	}

	public static void SaveAs()
	{
		DialogWrapper.FileSave(SaveAsCallback, _fileExtension);
	}

	private static void SaveAsCallback(string? path)
	{
		if (path == null)
			return;

		Save(Path.ChangeExtension(path, $".{_fileExtension}"));
	}

	public static void SetHistoryIndex(int index)
	{
		if (index < 0 || index >= History.Count)
			return;

		CurrentHistoryIndex = Math.Clamp(index, 0, History.Count - 1);
		Animation = History[CurrentHistoryIndex].AnimationData.DeepCopy();

		// LevelEditorState.ClearHighlight();
		// LevelEditorState.UpdateSelectedWorldObject();
		// LevelEditorState.UpdateSelectedEntity();
	}

	public static void Track(string editDescription)
	{
		AnimationData copy = Animation.DeepCopy();
		byte[] hash = MD5.HashData(GetBytes(copy));

		if (editDescription == "Reset")
		{
			UpdateHistory(new List<HistoryEntry> { new(copy, hash, "Reset") }, 0);
		}
		else
		{
			IReadOnlyList<byte> originalHash = History[CurrentHistoryIndex].Hash;

			if (originalHash.SequenceEqual(hash))
				return;

			HistoryEntry historyEntry = new(copy, hash, editDescription);

			// Clear any newer history.
			List<HistoryEntry> newHistory = History.ToList();
			newHistory = newHistory.Take(CurrentHistoryIndex + 1).Append(historyEntry).ToList();

			// Remove history if there are too many entries.
			int newCurrentIndex = CurrentHistoryIndex + 1;
			if (newHistory.Count > _maxHistoryEntries)
			{
				newHistory.RemoveAt(0);
				newCurrentIndex--;
			}

			UpdateHistory(newHistory, newCurrentIndex);
		}

		void UpdateHistory(IReadOnlyList<HistoryEntry> history, int currentHistoryIndex)
		{
			History = history;
			CurrentHistoryIndex = currentHistoryIndex;
		}
	}

	private static void Save(string path)
	{
		using MemoryStream ms = new();
		AnimationBinarySerializer.WriteAnimation(ms, Animation);
		File.WriteAllBytes(path, ms.ToArray());
		SetAnimation(path, Animation);
	}

	private static void SetAnimation(string? animationFilePath, AnimationData animation)
	{
		AnimationFilePath = animationFilePath;
		Animation = animation;
		_fileMd5Hash = _memoryMd5Hash;
		IsModified = !_fileMd5Hash.SequenceEqual(_memoryMd5Hash);
	}

	private static void ClearState()
	{
		ObjState.Clear();
	}
}
