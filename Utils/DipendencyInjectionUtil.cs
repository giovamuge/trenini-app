using System;

namespace TreniniApp.Utils;

public static class DipendencyInjectionUtil
{
    private static IServiceProvider? _serviceProvider;

    public static void Initialize(IServiceProvider serviceProvider)
    {
        _serviceProvider ??= serviceProvider;
    }

    public static T GetService<T>()
        where T : notnull
    {
        if (_serviceProvider is null)
        {
            throw new InvalidOperationException("ServiceProvider not initialized.");
        }

        return _serviceProvider.GetRequiredService<T>();
    }

    public static T Resolve<T>()
        where T : notnull
    {
        var services =
            (Application.Current?.Windows[0]?.Page?.Handler?.MauiContext?.Services)
            ?? throw new InvalidOperationException(
                "Unable to resolve service provider from the current application context."
            );

        var service =
            services.GetService<T>()
            ?? throw new InvalidOperationException($"Service of type {typeof(T)} not found.");

        return service;
    }

    public static async Task<T> ResolveAsync<T>()
        where T : notnull
    {
        await Task.Yield(); // Ensures method is asynchronous and does not block UI thread

        var services =
            (Application.Current?.Windows[0]?.Page?.Handler?.MauiContext?.Services)
            ?? throw new InvalidOperationException(
                "Unable to resolve service provider from the current application context."
            );

        var service =
            services.GetService<T>()
            ?? throw new InvalidOperationException($"Service of type {typeof(T)} not found.");

        return service;
    }
}
