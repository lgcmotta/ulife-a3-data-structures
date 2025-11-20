using System.Globalization;

namespace A3.RouteSearchGraphs.Extensions;

internal static class ThreadExtensions
{
    extension(Thread thread)
    {
        internal (CultureInfo UI, CultureInfo Current) GetCurrentCultures()
        {
            return (thread.CurrentUICulture, thread.CurrentCulture);
        }

        internal void SetCurrentCultures(CultureInfo ui, CultureInfo current)
        {
            thread.CurrentUICulture = ui;
            thread.CurrentCulture = current;
        }

        internal void SetCurrentCultures(CultureInfo culture)
        {
            thread.CurrentUICulture = culture;
            thread.CurrentCulture = culture;
        }
    }
}