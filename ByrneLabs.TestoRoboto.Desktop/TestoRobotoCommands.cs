using System.Windows.Input;

namespace ByrneLabs.TestoRoboto.Desktop
{
    public static class TestoRobotoCommands
    {
        public static RoutedUICommand NewRequestMessage { get; } = new RoutedUICommand("New Request Message", nameof(NewRequestMessage), typeof(TestoRobotoCommands));

        public static RoutedUICommand NewRequestMessageCollection { get; } = new RoutedUICommand("New Request Message Collection", nameof(NewRequestMessageCollection), typeof(TestoRobotoCommands));
    }
}
