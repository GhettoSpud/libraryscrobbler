using LibraryScrobbler.Lib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace LibraryScrobbler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Properties

        private string _rootDirectoryPath;
        public string InputRootDirectoryPath
        {
            get { return _rootDirectoryPath; }
            set
            {
                _rootDirectoryPath = value;
                RaisePropertyChanged("InputRootDirectoryPath");
            }
        }

        private string _currentDirectoryPath;
        public string CurrentDirectoryPath
        {
            get { return _currentDirectoryPath; }
            set
            {
                _currentDirectoryPath = value;
                RaisePropertyChanged("CurrentDirectoryPath");
            }
        }

        private string _outputDirectoryPath;
        public string OutputRootDirectoryPath
        {
            get { return _outputDirectoryPath; }
            set
            {
                _outputDirectoryPath = value;
                RaisePropertyChanged("OutputRootDirectoryPath");
            }
        }

        private string SqliteFilepath { get { return $"{OutputRootDirectoryPath}\\Sqlite\\music_metadata.sqlite"; } }
        private string SqliteConnectionString { get { return $"DataSource={SqliteFilepath};"; } }

        private void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void InputDirectoryButtonClicked(object sender, RoutedEventArgs args)
        {
            var folderPicker = new System.Windows.Forms.FolderBrowserDialog();
            var result = folderPicker.ShowDialog();
            InputRootDirectoryPath = folderPicker.SelectedPath;
        }

        public void ParseButtonClicked(object sender, RoutedEventArgs args)
        {
            var directory = new DirectoryInfo(InputRootDirectoryPath);
            var outputDirectory = new DirectoryInfo(OutputRootDirectoryPath);
            var shouldOverwrite = ShouldOverwrite.IsChecked ?? false;

            Task.Factory.StartNew(() =>
            {
                if (shouldOverwrite)
                {
                    LibraryParsing.CreateDatabase(SqliteFilepath);
                }

                ParseMetadataRecursive(directory, outputDirectory, "", shouldOverwrite);
                CurrentDirectoryPath = "Finished!";
            });
        }

        public void ParseMetadataRecursive(
            DirectoryInfo rootDirectory,
            DirectoryInfo rootOutputDirectory,
            string subDirectorySuffix,
            bool shouldOverwrite)
        {
            var currentInputDirectory = new DirectoryInfo($"{rootDirectory.FullName}\\{subDirectorySuffix}");
            var currentOutputDirectory = new DirectoryInfo($"{rootOutputDirectory.FullName}\\{subDirectorySuffix}");
            CurrentDirectoryPath = currentInputDirectory.FullName;

            var jsonOutputDirectory = new DirectoryInfo($"{currentOutputDirectory.FullName}\\Json\\");
            string sqliteFilepath = $"{rootOutputDirectory.FullName}\\Sqlite\\music_metadata.sqlite";

            LibraryParsing.ParseMetadata(currentInputDirectory, jsonOutputDirectory, sqliteFilepath, shouldOverwrite);

            var subDirectories = currentInputDirectory.EnumerateDirectories();
            foreach (var subDirectory in subDirectories)
            {
                var suffix = $"{subDirectorySuffix}\\{subDirectory.Name}\\";
                ParseMetadataRecursive(rootDirectory, rootOutputDirectory, suffix, shouldOverwrite);
            }
        }
    }
}
