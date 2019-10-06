using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                foreach (FileToUnzip file in fileToUnzipCollection)
                {
                    ZipFile.ExtractToDirectory(file.FilePath, unzipDirectorySelectionDialog.SelectedPath);
                }

                MessageBox.Show("Unzipping complete!");
            }
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
