﻿---
uid: Overview.Mvux.Overview
---

# MVUX Overview

**M**odel, **V**iew, **U**pdate, e**X**tended (**MVUX**) is a variation of the MVU design pattern that encourages unidirectional flow of immutable data, whilst leveraging the data binding capabilities that makes the MVVM pattern so powerful.

To better understand MVUX, let us consider a weather application that will display the current temperature, obtained from an external weather service. At face value, this seems simple enough: call service to retrieve latest temperature and display the returned value.  
  
Although this seems like an easy problem, as is often the case, there are more details to consider than may be immediately apparent.

- What if the external service isn't immediately available when starting the app?
- How does the app show that data is being loaded? Or being updated?
- What if no data is returned from the external service?
- What if an error occurs while obtaining or processing the data?
- How to keep the app responsive while loading or updating the UI?
- How do we refresh the current data?
- How do we avoid threading or concurrency issues when handling new data in the background?
- How do we make sure the code is testable?

Individually, these questions are simple enough to handle, but hopefully, they highlight that there is more to consider in even a very trivial application. Now imagine an application that has more complex data and user interface, the potential for complexity and the amount of required code can grow enormously.

MVUX is a response to such situations and makes it easier to handle the above scenarios.  

## What is MVUX?

MVUX is an extension to the MVU design pattern, and leverages code generation in order to take advantage of the uniuqe data-binding engine of WinUI and the Uno Platform.

### Model

The **Model** in MVUX is similar in many ways to the ViewModel in MVVM in that it defines the properties that will be available for data binding and methods that include any business logic. In MVUX this is referred to as the Model, highlighting that it is immutable by design.

For our weather app, `WeatherModel` is the **Model**, and defines a property named `CurrentWeather`.

```csharp
public partial record WeatherModel(IWeatherService WeatherService)
{
    public IFeed<WeatherInfo> CurrentWeather => Feed.Async(this.WeatherService.GetCurrentWeather);
}
```

The `CurrentWeather` property represents a feed (`IFeed`) of `WeatherInfo` entities (for those familiar with [Reactive](https://reactivex.io/) this is similar in many ways to an `IObservable`). When the `CurrentWeather` property is accessed an `IFeed` is created via the `Feed.Async` factory method, which will asynchronously call the `GetcurrentWeather` service.  

### View

The **View** is the UI, which can be written in XAML, C#, or a combination of the two, much as you would when using another design pattern. For example, the following can be used to data bind to the `CurrentWeather.Temperature` property.

```xml
<Page x:Class="WeatherApp.MainPage"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <StackPanel>
        <TextBlock Text="{Binding CurrentWeather.Temperature}" />
    </StackPanel>
</Page>
```  

If you're familiar with MVVM, the above XAML would look familiar, as it's the same XAML you would write if you had a ViewModel that exposed a `CurrentWeather` property that returns a `WeatherInfo` entity with a `Temperature` property. What's unique to MVUX is the additional information that `IFeed` exposes, such as when data is being loaded. For this, we can leverage the `FeedView` control which is part of MVUX.

```xml
<Page x:Class="WeatherApp.MainPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:mvux="using:Uno.Extensions.Reactive.UI">
    
    <mvux:FeedView Source="{Binding CurrentWeather}">
        <DataTemplate>
            <StackPanel>
                <TextBlock Text="{Binding Data.Temperature}" />
            </StackPanel>
        </DataTemplate>
    </mvux:FeedView>
    
</Page>
```

The `FeedView` control is designed to work with an `IFeed`, and has different visual states that align with the different states that an `IFeed` can be in (e.g. loading, refreshing, error, etc.). The above XAML defines the `ValueTemplate`, which is required in order to display the `Data` from the `IFeed`. Other templates include `ProgressTemplate`, `ErrorTemplate` and `NoneTemplate`, which can be defined in order to control what's displayed depending on the state of the `IFeed`.

### Update

An **Update** is any action that will result in a change to the **Model**. Whilst an **Update** is often triggered via an interaction by the user with the **View**, such as editing text or clicking a button, an **Update** can also be triggered from background processes (for example a data sync operation, or perhaps a notification triggered by a hardware sensor, such as a GPS).

In the weather example, if we wanted to refresh the current weather data, a `Refresh` method can be added to the `WeatherModel`.

```csharp
public partial record WeatherModel(IWeatherService WeatherService)
{
    public IFeed<WeatherInfo> CurrentWeather => Feed.Async(this.WeatherService.GetCurrentWeather);
    public async ValueTask Refresh() { ... }
}
```  

In the `View` this can be data bound to a `Command` property on a `Button`. Public methods in the Model are automatically exposed as commands to the View, though this behavior is completely configurable.

```xml
<Page x:Class="WeatherApp.MainPage"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <StackPanel>
        <TextBlock Text="{Binding CurrentWeather.Temperature}" />
        <Button Content="Refresh" Command="{Binding Refresh}" />
    </StackPanel>
</Page>
```  

As refreshing a feed is such a common scenario, the `FeedView` control exposes a `Refresh` command that removes the requirement to have a `Refresh` method on the `WeatherModel` and can be data bound, again to the Command property, of a Button, as follows:  

```xml
<Page x:Class="WeatherApp.MainPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:mvux="using:Uno.Extensions.Reactive.UI">
    
    <mvux:FeedView Source="{Binding CurrentWeather}">
        <DataTemplate>
            <StackPanel>
                <TextBlock Text="{Binding Data.Temperature}" />
                <Button Content="Refresh" Command="{Binding Refresh}" />
            </StackPanel>
        </DataTemplate>
    </mvux:FeedView>
    
</Page>
```

Clicking the button will execute the `Refresh` command on the `FeedView` which will signal the `IFeed` to reload. In the case of the weather app it would invoke the `GetCurrentWeather` method of the service again.

### eXtended

At this point you might be wondering how we're able to data bind to `CurrentWeather.Temperature`, as if it were a property that returns a single value, and then also bind the `CurrentWeather` property to the `Source` property of the `FeedView` to access a much richer set of information about the `IFeed`.  This is possible because of the bindable proxies that are being generated by the MVUX source code generators.

The **eXtended** part of MVUX includes the generation of these bindable proxies, that bridge the gap between the **Model** that exposes asynchronous feeds of immutable data and the synchronous data binding capability of WinUI and the Uno Platform. Instead of an instance of `WeatherModel`, the `DataContext` on the **View** is set to be an instance of the generated `BindableWeatherModel` which exposes a property, `CurrentWeather`, the same as the original `WeatherModel`.

For the purpose of this example, the `DataContext` property can be set in the page's code-behind file:

```csharp
public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();

        DataContext = new BindableWeatherModel(new WeatherService());
    }
}
```

### Result

When the app is lanuched, a waiting progress ring appears while the service loads the current temperature:

![Video showing a progress-ring running in the app while waiting for data](Assets/SimpleFeed-3.gif)

It is thereafter replaced with the temperature as soon as it's received from the service:

![A screenshot of the app showing a refresh button](Assets/SimpleFeed-5.jpg)

When the 'Refresh' button is pressed, the progress ring shows again for a short time, until the new temperature is received from the server. The Refresh button is automatically disabled while a refresh request is in progress:

![A screenshot showing the refresh button disabled and temperature updated to 24](Assets/SimpleFeed-6.jpg)

For the full weather app tutorial see [How to create a feed](xref:Overview.Mvux.HowToSimpleFeed).

## Recap

In order to summarize what we've just seen, let's return to the list of challenges posed by our simple application.

- What if the external service isn't immediately available when starting the app?  
**The `FeedView` has an error template that can be used to control what's displayed when data can't be retrieved via the `ErrorTemplate`.**

- How does the app show that data is being loaded? Or being updated?  
**The `FeedView` has a progress template that defaults to a `ProgressRing` but can be overwritten via the `ProgressTemplate` property.**  

- What if no data is returned from the external service?  
**The `FeedView` has a no-data template that can be defined via the `NoneTemplate` property.**  

- What if an error occurs while obtaining or processing the data?  
**The `FeedView` has both a error and undefined templates that can be used to control what's displayed when data can't be retrieved.**  

- How to keep the app responsive while loading or updating the UI?  
**MVUX is inherently asynchronous and all operations are dispatched to background threads to avoid congestion on the UI thread.**

- How do we refresh the current data?  
**Feeds support re-querying the source data and the `FeedView` exposes a Refresh property that can be bound to Command properties on UI elements such as Button.**

- How do we avoid threading or concurrency issues when handling new data in the background?  
**The `IFeed` handles dispatching actions to background threads and then marshalling responses back to the UI thread as required.**

- How do we make sure the code is testable?  
**The **Model** doesn't depend on any UI elements, so can be unit tested, along with the generated bindable proxies.**


## Key points to note  

- Feeds are reactive in nature, similar in many ways to Observables.
- Models and associated entities are immutable.
- Operations are asynchronous by default.
- Feeds include various dimensions such as loading, if there's data or if an error occurred.
- Feeds borrow from Option concept in functional programming where no data is a valid state for the feed.
- MVUX combines the unidirectional flow of data, and immutability of MVU, with the data binding capabilities of MVVM.
