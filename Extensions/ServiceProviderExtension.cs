namespace TreniniApp.Extensions;

public static class ServiceProvider
{
    public static TService? GetService<TService>()
    {
        if (Current is null)
        {
            throw new InvalidOperationException(
                "ServiceProvider is not available on a valid IPlatformApplication instance."
            );
        }
        return Current.GetService<TService>();
    }

    public static IServiceProvider? Current =>
#if WINDOWS10_0_17763_0_OR_GREATER
        MauiWinUIApplication.Current?.Services;
#elif ANDROID
        IPlatformApplication.Current?.Services;
#elif IOS || MACCATALYST
        IPlatformApplication.Current?.Services;
#else
        null;
#endif
}
