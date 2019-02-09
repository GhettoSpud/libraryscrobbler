using LibraryScrobbler.Lib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

namespace LibraryScrobbler
{
    public partial class ScrobblingPage : Page, INotifyPropertyChanged
    {
        #region Properties

        private Dictionary<TreeViewItem, DataRow> _treeViewItemRowMap = new Dictionary<TreeViewItem, DataRow>();

        private DataSet DataSet { get; set; }

        public DataView Artists {
            get
            {
                return DataSet?.Tables["Artist"]?.AsDataView();;
            }
        }

        public DataView Albums {
            get
            {
                return DataSet?.Tables["Album"]?.AsDataView();
            }
        }

        public DataView Tracks {
            get
            {
                return DataSet?.Tables["Track"]?.AsDataView();
            }
        }

        private string _metadataRootDirectoryPath;
        public string MetadataRootDirectoryPath
        {
            get { return _metadataRootDirectoryPath; }
            set
            {
                _metadataRootDirectoryPath = value;
                RaisePropertyChanged("MetadataRootDirectoryPath");
            }
        }

        private string _refreshMessage;
        public string RefreshMessage
        {
            get { return _refreshMessage; }
            set
            {
                _refreshMessage = value;
                RaisePropertyChanged("RefreshMessage");
            }
        }

        private Brush _refreshMessageColor;
        public Brush RefreshMessageColor
        {
            get { return _refreshMessageColor; }
            set
            {
                _refreshMessageColor = value;
                RaisePropertyChanged("RefreshMessageColor");
            }
        }

        private void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public ScrobblingPage()
        {
            InitializeComponent();

            MetadataRootDirectoryPath = Properties.Settings.Default.OutputRootDirectoryPath;

            DataSet = BuildDataSet(MetadataRootDirectoryPath);

            DataContext = this;
        }

        private void MetadataRootDirectoryRefreshButton_Click(object sender, RoutedEventArgs args)
        {
            var dataSet = BuildDataSet(MetadataRootDirectoryPath);

            if (dataSet == null)
                return;

            DataSet = dataSet;

            Properties.Settings.Default.OutputRootDirectoryPath = MetadataRootDirectoryPath;
            Properties.Settings.Default.Save();
        }

        private void TrackScrobbleButton_Click(object sender, RoutedEventArgs e)
        {
            var context = (e.Source as FrameworkElement).DataContext as DataRowView;
            
            if (context.Row.Table == Artists.Table)
            {
            }
            else if (context.Row.Table == Albums.Table)
            {
                LibraryScrobbling.ScrobbleAlbum(context.Row.Table);
            }
            else if (context.Row.Table == Tracks.Table)
            {
                LibraryScrobbling.ScrobbleTrack(context.Row.Table);

            }
        }

        private DataSet BuildDataSet(string metadataRootDirectoryPath)
        {
            var sqliteFilepath = $"{metadataRootDirectoryPath}\\Sqlite\\music_metadata.sqlite";
            string connectionString = $"DataSource={sqliteFilepath};";

            var dataSet = new DataSet("Data");

            var artistTable = new DataTable("Artist");
            var albumTable = new DataTable("Album");
            var trackTable = new DataTable("Track");

            dataSet.Tables.Add(artistTable);
            dataSet.Tables.Add(albumTable);
            dataSet.Tables.Add(trackTable);

            dataSet.EnforceConstraints = false;

            try
            {
                RefreshMessage = "Loading Data...";
                RefreshMessageColor = new SolidColorBrush(Colors.Gold);

                using (var connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        string artistQuery = "SELECT * FROM Artist ORDER BY Name";
                        using (var artistCommand = new SQLiteCommand(artistQuery, connection, transaction))
                        {
                            using (var reader = artistCommand.ExecuteReader())
                            {
                                artistTable.Load(reader);
                            }
                        }

                        string albumQuery = "SELECT * FROM Album ORDER BY DateReleased";
                        using (var albumCommand = new SQLiteCommand(albumQuery, connection, transaction))
                        {
                            using (var reader = albumCommand.ExecuteReader())
                            {
                                albumTable.Load(reader);
                            }
                        }

                        string trackQuery = "SELECT * FROM Track ORDER BY DiscNumber, TrackNumber";
                        using (var trackCommand = new SQLiteCommand(trackQuery, connection, transaction))
                        {
                            using (var reader = trackCommand.ExecuteReader())
                            {
                                trackTable.Load(reader);
                            }
                        }
                        transaction.Commit();
                    }
                }

                artistTable.ChildRelations.Add(
                    "Albums",
                    artistTable.Columns["Name"],
                    albumTable.Columns["AlbumArtist"]);

                albumTable.ChildRelations.Add(
                    "Tracks",
                    albumTable.Columns["Title"],
                    trackTable.Columns["Album"]);
            }
            catch (SQLiteException e)
            {
                RefreshMessage = $"ERROR: {e.Message}";
                RefreshMessageColor = new SolidColorBrush(Colors.Red);

                Debug.WriteLine(e);
                return null;
            }
            catch (Exception e)
            {
                RefreshMessage = $"ERROR: Encountered a problem when loading data. Please try re-parsing the music library's data.";
                RefreshMessageColor = new SolidColorBrush(Colors.Red);

                Debug.WriteLine(e);
                return null;
            }

            RefreshMessage = $"Successfully loaded {artistTable.Rows.Count} Artists {albumTable.Rows.Count} Albums and {trackTable.Rows.Count} Tracks";
            RefreshMessageColor = new SolidColorBrush(Colors.LawnGreen);



            return dataSet;
        }
    }
}
