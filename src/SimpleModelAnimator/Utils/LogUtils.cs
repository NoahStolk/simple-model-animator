using Serilog;
using Serilog.Core;
using System.Globalization;

namespace SimpleModelAnimator.Utils;

public static class LogUtils
{
	public static Logger Log { get; } = new LoggerConfiguration()
		.WriteTo.File($"simple-model-animator-{AssemblyUtils.VersionString}.log", formatProvider: CultureInfo.InvariantCulture, rollingInterval: RollingInterval.Infinite)
		.CreateLogger();
}
