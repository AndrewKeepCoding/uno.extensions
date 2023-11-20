﻿#if __WASM__
using Uno.Foundation;
#endif

namespace Uno.Extensions.Hosting;

internal class AppHostingEnvironment : HostingEnvironment, IAppHostEnvironment, IDataFolderProvider
#if __WASM__
	, IHasAddressBar
#endif
{
	public string? AppDataPath { get; init; }

	public Assembly? HostAssembly { get; init; }

#if __WASM__
	public async Task UpdateAddressBar(Uri applicationUri)
	{
		CoreApplication.MainView?.DispatcherQueue.TryEnqueue(() =>
					{
						var href = Imports.GetLocation();
						var appUriBuilder = new UriBuilder(applicationUri);
						var url = new UriBuilder(href);
						url.Query = appUriBuilder.Query;
						url.Path = appUriBuilder.Path;
						var webUri = url.Uri.OriginalString;
						var result = Imports.PushState($"{webUri}", "", $"{webUri}");
					});
	}


#endif
}

#if __WASM__
internal static partial class Imports
{
	[System.Runtime.InteropServices.JavaScript.JSImport("globalThis.Uno.Extensions.Hosting.getLocation")]
	public static partial string GetLocation();


	[System.Runtime.InteropServices.JavaScript.JSImport("globalThis.window.history.pushState")]
	public static partial string PushState(string state, string title, string url);
}
#endif
