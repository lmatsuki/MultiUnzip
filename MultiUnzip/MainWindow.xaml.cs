using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Windows;
using WinForms = System.Windows.Forms;

namespace MultiUnzip
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<FileToUnzip> fileToUnzipCollection;
        ProgressBarWindow progressBarWindow = null;

        public MainWindow()
        {
            InitializeComponent();
            ClearItemsSource();
        }

        private void ClearItemsSource()
        {
            fileToUnzipCollection = new ObservableCollection<FileToUnzip>();
            filesToUnzip.ItemsSource = fileToUnzipCollection;
        }

        private void UnzipFiles_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];

                if (files == null || files.Length == 0)
                {
                    return;
                }

                AddFilesToUnzip(files);
            }
        }

        private void AddFilesToUnzip(string[] files)
        {
            foreach (string file in files)
            {
                FileToUnzip zipFile = new FileToUnzip(file);

                if (zipFile.IsValidExtension)
                {
                    fileToUnzipCollection.Add(zipFile);
                }
            }
        }

        private void ClearAllButton_Click(object sender, RoutedEventArgs e)
        {
            ClearItemsSource();
        }

        private void UnzipButton_Click(object sender, RoutedEventArgs e)
        {
            if (fileToUnzipCollection.Count == 0)
            {
                MessageBox.Show("There are no files to unzip.");
                return;
            }

            UnzipAllFiles();
        }

        private void UnzipAllFiles()
        {
            WinForms.FolderBrowserDialog unzipDirectorySelectionDialog = new WinForms.FolderBrowserDialog();
            unzipDirectorySelectionDialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            WinForms.DialogResult result = unzipDirectorySelectionDialog.ShowDialog();

            if (result == WinForms.DialogResult.OK)
            {
                try
                {
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.WorkerReportsProgress = true;
                    worker.DoWork += Worker_DoWork;
                    worker.ProgressChanged += Worker_ProgressChanged;

                    worker.RunWorkerAsync(argument: unzipDirectorySelectionDialog.SelectedPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                }
            }
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            string selectedPath = e.Argument as string;

            if (string.IsNullOrEmpty(selectedPath))
            {
                MessageBox.Show("Unexpected error with selected directory.");

                return;
            }

            Dispatcher.Invoke(() =>
            {
                unzipButton.IsEnabled = false;
                clearAllButton.IsEnabled = false;

                progressBarWindow = new ProgressBarWindow();
                progressBarWindow.Show();
            });

            float progress = 0f;
            foreach (FileToUnzip file in fileToUnzipCollection)
            {
                ZipFile.ExtractToDirectory(file.FilePath, selectedPath);
                progress++;

                int progressPercentage = (int)(progress / fileToUnzipCollection.Count * 100);
                (sender as BackgroundWorker).ReportProgress(progressPercentage);
            }

            Dispatcher.Invoke(() =>
            {
                unzipButton.IsEnabled = true;
                clearAllButton.IsEnabled = true;
            });

            MessageBox.Show("Unzipping complete!");
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBarWindow.UpdateProgress(e.ProgressPercentage);
        }
    }

    public class FileToUnzip
    {
        public FileToUnzip(string filePath)
        {
            FilePath = filePath;

            FileInfo fileInfo = new FileInfo(filePath);
            IsValidExtension = ValidExtensions.Contains(fileInfo.Extension);
            FileName = fileInfo.Name;
        }

        private IList<string> ValidExtensions => new List<string> { ".zip", ".rar" };

        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Extension { get; set; }
        public bool IsValidExtension { get; set; }
    }
}
