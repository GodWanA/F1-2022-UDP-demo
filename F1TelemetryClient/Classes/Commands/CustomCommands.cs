using System.Windows.Input;

namespace F1TelemetryApp.Classes.Commands
{
    internal static class CustomCommands
    {
        public static readonly RoutedUICommand Exit = new RoutedUICommand(
            "Exit",
            "Exit",
            typeof(CustomCommands),
            new InputGestureCollection() { new KeyGesture(Key.F4, ModifierKeys.Alt) }
        );

        public static readonly RoutedUICommand OpenPreferences = new RoutedUICommand(
            "OpenPreferences",
            "OpenPreferences",
            typeof(CustomCommands),
            new InputGestureCollection() { new KeyGesture(Key.K, ModifierKeys.Control) }
        );

        public static readonly RoutedUICommand OpenMapTool = new RoutedUICommand(
            "OpenMapTool",
            "OpenMapTool",
            typeof(CustomCommands),
            new InputGestureCollection() { new KeyGesture(Key.M, ModifierKeys.Control) }
        );

        public static readonly RoutedUICommand OpenAbout = new RoutedUICommand(
            "OpenAbout",
            "OpenAbout",
            typeof(CustomCommands),
            new InputGestureCollection() { new KeyGesture(Key.I, ModifierKeys.Control) }
        );
    }
}
