﻿using Uno.Extensions.Navigation.Regions;

namespace Uno.Extensions.Navigation.Navigators;

public abstract class DialogNavigator : ControlNavigator
{
    public override bool CanGoBack => true;

    private IAsyncInfo? ShowTask { get; set; }

    protected DialogNavigator(
        ILogger<DialogNavigator> logger,
        IRouteResolver routeResolver,
        IRegion region)
        : base(logger, routeResolver,  region)
    {
    }

	protected override bool QualifierIsSupported(Route route) =>
			base.QualifierIsSupported(route) ||
			// "-" (back or close) Add closing 
			route.IsBackOrCloseNavigation();

    protected override async Task<Route?> ExecuteRequestAsync(NavigationRequest request)
    {
        var route = request.Route;
        // If this is back navigation, then make sure it's used to close
        // any of the open dialogs
        if (route.FrameIsBackNavigation() && ShowTask is not null)
        {
            await CloseDialog();
        }
        else
        {
            var mapping = RouteResolver.Find(route);
            var viewModel = (Region.Services is not null && mapping?.ViewModel is not null) ? CreateViewModel(Region.Services, route, mapping) : default(object);
            ShowTask = await DisplayDialog(request, mapping?.View, viewModel);
        }
        var responseRequest = route with { Path = null };
        return responseRequest;
    }

    protected async Task CloseDialog()
    {
        var dialog = ShowTask;
        ShowTask = null;

        dialog?.Cancel();
    }

    protected abstract Task<IAsyncInfo?> DisplayDialog(NavigationRequest request, Type? viewType, object? viewModel);
}
