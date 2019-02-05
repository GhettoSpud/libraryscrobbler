using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LibraryScrobbler.Lib;

namespace LibraryScrobbler.Pages
{
    /// <summary>
    /// Interaction logic for ParsingPage.xaml
    /// </summary>
    public partial class ParsingPage : Page, INotifyPropertyChanged
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

        public ParsingPage()
        {
            InitializeComponent();

            InputRootDirectoryPath = null;
            OutputRootDirectoryPath = null;
            CurrentDirectoryPath = null;
        }

        public void InputDirectoryButtonClicked(object sender, RoutedEventArgs args)
        {
            var folderPicker = new System.Windows.Forms.FolderBrowserDialog();
            var result = folderPicker.ShowDialog();
            InputRootDirectoryPath = folderPicker.SelectedPath;
        }

        public void OutputDirectoryButtonClicked(object sender, RoutedEventArgs args)
        {
            var folderPicker = new System.Windows.Forms.FolderBrowserDialog();
            var result = folderPicker.ShowDialog();
            OutputRootDirectoryPath = folderPicker.SelectedPath;
        }

        public void ParseButtonClicked(object sender, RoutedEventArgs args)
        {
            var directory = new DirectoryInfo(InputRootDirectoryPath);
            var outputDirectory = new DirectoryInfo(OutputRootDirectoryPath);
            bool shouldOverwrite = ShouldOverwrite.IsChecked ?? false;
            bool exportSqlite = ExportSqlite.IsChecked ?? false;
            bool exportJson = ExportJson.IsChecked ?? false;

            Task.Factory.StartNew(() =>
            {
                if (shouldOverwrite)
                {
                    LibraryParsing.CreateDatabase(SqliteFilepath);
                }

                ParseMetadataRecursive(directory, outputDirectory, "", exportSqlite, exportJson, shouldOverwrite);
                CurrentDirectoryPath = "Finished!";
            });
        }

        public void ParseMetadataRecursive(
            DirectoryInfo rootDirectory,
            DirectoryInfo rootOutputDirectory,
            string subDirectorySuffix,
            bool exportSqlite,
            bool exportJson,
            bool shouldOverwrite)
        {
            var currentInputDirectory = new DirectoryInfo($"{rootDirectory.FullName}\\{subDirectorySuffix}");
            var currentOutputDirectory = new DirectoryInfo($"{rootOutputDirectory.FullName}\\{subDirectorySuffix}");
            CurrentDirectoryPath = $"$\\{subDirectorySuffix}";

            var jsonOutputDirectory = new DirectoryInfo($"{rootOutputDirectory.FullName}\\Json\\{subDirectorySuffix}");
            string sqliteFilepath = $"{rootOutputDirectory.FullName}\\Sqlite\\music_metadata.sqlite";

            LibraryParsing.ParseMetadata(currentInputDirectory, jsonOutputDirectory, sqliteFilepath, exportSqlite, exportJson, shouldOverwrite);

            var subDirectories = currentInputDirectory.EnumerateDirectories();
            foreach (var subDirectory in subDirectories)
            {
                var suffix = $"{subDirectorySuffix}{subDirectory.Name}\\";
                ParseMetadataRecursive(rootDirectory, rootOutputDirectory, suffix, exportSqlite, exportJson, shouldOverwrite);
            }
        }
    }
}
