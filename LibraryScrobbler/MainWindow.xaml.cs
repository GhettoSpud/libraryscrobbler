using GraphicScrobbler.Lib;
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

namespace GraphicScrobbler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Properties

        private string _rootDirectory;
        public string RootDirectory
        {
            get { return _rootDirectory; }
            set
            {
                _rootDirectory = value;
                RaisePropertyChanged("RootDirectory");
            }
        }

        private string _currentDirectory;
        public string CurrentDirectory
        {
            get { return _currentDirectory; }
            set
            {
                _currentDirectory = value;
                RaisePropertyChanged("CurrentDirectory");
            }
        }

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

        public void DirectoryButtonClicked(object sender, RoutedEventArgs args)
        {
            var folderPicker = new System.Windows.Forms.FolderBrowserDialog();
            var result = folderPicker.ShowDialog();
            RootDirectory = folderPicker.SelectedPath;
        }

        public void ParseButtonClicked(object sender, RoutedEventArgs args)
        {
            var directory = new DirectoryInfo(RootDirectory);
            var outputDirectory = new DirectoryInfo(directory.FullName + "\\Metadata");
            var shouldOverwrite = ShouldOverwrite.IsChecked ?? false;

            Task.Factory.StartNew(() =>
            {
                ParseMetadataRecursive(directory, outputDirectory, shouldOverwrite);
                CurrentDirectory = "Finished!";
            });
        }

        public void ParseMetadataRecursive(
            DirectoryInfo rootDirectory,
            DirectoryInfo rootOutputDirectory,
            bool shouldOverwrite)
        {
            //var currentOutputDirectory = new DirectoryInfo($"{rootOutputDirectory.FullName}\\{rootDirectory.Name}");
            CurrentDirectory = rootDirectory.FullName;
            var currentOutputDirectory = new DirectoryInfo($"{rootOutputDirectory}\\{rootDirectory.Name}");
            
            LibraryParsing.ParseMetadata(rootDirectory, currentOutputDirectory, shouldOverwrite);

            var subDirectories = rootDirectory.EnumerateDirectories();
            foreach (var directory in subDirectories)
            {
                ParseMetadataRecursive(directory, currentOutputDirectory, shouldOverwrite);
            }
        }
    }
}
