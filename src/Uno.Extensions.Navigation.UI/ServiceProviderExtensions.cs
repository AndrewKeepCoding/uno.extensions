﻿using Uno.Extensions.Hosting;
using Uno.Extensions.Navigation.Regions;
using Uno.Extensions.Navigation.UI;

namespace Uno.Extensions.Navigation;

public static class ServiceProviderExtensions
{
	public static void AddInstance<T>(this IServiceProvider provider, Func<T> instanceCreator)
	{
		provider.AddInstance(typeof(T), instanceCreator);
	}

	public static void AddInstance<T>(this IServiceProvider provider, Type serviceType, Func<T> instanceCreator)
	{
		provider.GetRequiredService<IInstanceRepository>().Instances[serviceType] = instanceCreator;
	}

	public static T AddInstance<T>(this IServiceProvider provider, T instance)
	{
		provider.AddInstance(typeof(T), instance!);
		return instance;
	}

	public static object AddInstance(this IServiceProvider provider, Type serviceType, object instance)
	{
		provider.GetRequiredService<IInstanceRepository>().Instances[serviceType] = instance;
		return instance;
	}

	public static T? GetInstance<T>(this IServiceProvider provider)
	{
		var value = provider.GetInstance(typeof(T));
		if (value is Func<T> valueCreator)
		{
			var instance = valueCreator();
			provider.AddInstance(instance);
			return instance;
		}

		if (value is T typedValue)
		{
			return typedValue;
		}

		return default;
	}

	public static object? GetInstance(this IServiceProvider provider, Type type)
	{
		return provider.GetRequiredService<IInstanceRepository>().Instances.TryGetValue(type, out var value) ? value : null;
	}

	public static IServiceProvider CloneNavigationScopedServices(this IServiceProvider services)
	{
		var scope = services.CreateScope();
		var scopedServices = scope.ServiceProvider;

		scopedServices.GetRequiredService<RegionControlProvider>().RegionControl = services.GetRequiredService<RegionControlProvider>().RegionControl;
		var instance = services.GetInstance<INavigator>();
		if (instance is not null)
		{
			scopedServices.AddInstance<INavigator>(instance);
		}

		return scopedServices;
	}

	public static FrameworkElement NavigationHost(this IServiceProvider services, string? initialRoute = "", Type? initialView = null, Type? initialViewModel = null)
	{
		var root = new ContentControl
		{
			HorizontalAlignment = HorizontalAlignment.Stretch,
			VerticalAlignment = VerticalAlignment.Stretch,
			HorizontalContentAlignment = HorizontalAlignment.Stretch,
			VerticalContentAlignment = VerticalAlignment.Stretch
		};

		root.SetServiceProvider(services);

		root.Host(initialRoute, initialView, initialViewModel);

		return root;
	}
}
