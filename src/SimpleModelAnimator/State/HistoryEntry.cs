using SimpleModelAnimator.Formats.Animation.Model;

namespace SimpleModelAnimator.State;

public sealed record HistoryEntry(AnimationData AnimationData, IReadOnlyList<byte> Hash, string EditDescription);
