﻿

namespace Uno.Extensions;

public static class HostBuilderExtensions
{
	public static IHostBuilder UseStorage(
		this IHostBuilder hostBuilder,
		Action<IServiceCollection> configure)
	{
		return hostBuilder.UseStorage((context, builder) => configure.Invoke(builder));
	}

	public static IHostBuilder UseStorage(
		this IHostBuilder builder,
		Action<HostBuilderContext, IServiceCollection>? configure = default)
	{
		return builder
			.UseConfiguration(
				configure: configBuilder =>
				configBuilder
						.Section<KeyValueStorageConfiguration>(nameof(KeyValueStorageConfiguration))
					)
			.ConfigureServices((ctx, services) =>
		{
			_ = services
				.AddFileStorage()
				.AddKeyedStorage();

			configure?.Invoke(ctx, services);
		});
	}
}
