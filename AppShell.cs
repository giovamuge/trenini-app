using TreniniApp.Pages;

namespace TreniniApp;

/* Not in use actually */
public class AppShell : Shell
{
    public AppShell(MainPage mainPage)
    {
        Items.Add(mainPage);

#if IOS || MACCATALYST
        // ShellAttachedProperties.SetPrefersLargeTitles(this, true);
#endif
    }
}
