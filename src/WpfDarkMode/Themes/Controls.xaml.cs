using System.Windows;

namespace WpfDarkMode.Themes;

public partial class Controls
{
    private void CloseWindow_Event(object sender, RoutedEventArgs e)
    {
        CloseWind(Window.GetWindow((FrameworkElement)e.Source));
    }

    private void AutoMinimize_Event(object sender, RoutedEventArgs e)
    {
        MaximizeRestore(Window.GetWindow((FrameworkElement)e.Source));
    }

    private void Minimize_Event(object sender, RoutedEventArgs e)
    {
        MinimizeWind(Window.GetWindow((FrameworkElement)e.Source));
    }

    private static void CloseWind(Window? window)
    {
        window?.Close();
    }

    private static void MaximizeRestore(Window? window)
    {
        if (window == null)
            return;
        window.WindowState = window.WindowState switch
        {
            WindowState.Normal => WindowState.Maximized,
            WindowState.Minimized or WindowState.Maximized => WindowState.Normal,
            _ => window.WindowState
        };
    }

    private static void MinimizeWind(Window? window)
    {
        if (window != null)
        {
            window.WindowState = WindowState.Minimized;
        }
    }
}
