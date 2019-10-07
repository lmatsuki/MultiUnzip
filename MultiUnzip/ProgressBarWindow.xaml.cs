using System.Windows;

namespace MultiUnzip
{
    /// <summary>
    /// Interaction logic for ProgressBarWindow.xaml
    /// </summary>
    public partial class ProgressBarWindow : Window
    {
        public ProgressBarWindow()
        {
            InitializeComponent();
        }

        public void UpdateProgress(int percentage)
        {
            progressBarStatus.Value = percentage;

            if (percentage >= 100)
            {
                Close();
            }
        }
    }
}
