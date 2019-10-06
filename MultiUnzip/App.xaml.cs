using System.Windows;
using System.Windows.Threading;

namespace MultiUnzip
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void FallbackUnhandledExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, "Something went wrong", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
