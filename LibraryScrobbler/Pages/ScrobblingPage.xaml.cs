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

        private DataView _artists;
        public DataView Artists {
            get { return _artists; }
            private set
            {
                _artists = value;
                RaisePropertyChanged("Artists");
            }
        }

        private int _trackCount = 0;
        public int TrackCount
        {
            get { return _trackCount; }
            private set
            {
                _trackCount = value;
                RaisePropertyChanged("TrackCount");
            }
        }

        private int _albumCount = 0;
        public int AlbumCount
        {
            get { return _albumCount; }
            private set
            {
                _albumCount = value;
                RaisePropertyChanged("AlbumCount");
            }
        }

        private int _artistCount = 0;
        public int ArtistCount
        {
            get { return _artistCount; }
            private set
            {
                _artistCount = value;
                RaisePropertyChanged("ArtistCount");
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

            var dataSet = BuildDataSet(MetadataRootDirectoryPath);

            Artists = dataSet?.Tables["Artist"]?.DefaultView;

            DataContext = this;
        }

        private void MetadataRootDirectoryRefreshButtonClicked(object sender, RoutedEventArgs args)
        {
            var dataSet = BuildDataSet(MetadataRootDirectoryPath);

            if (dataSet == null)
                return;

            Artists = dataSet.Tables["Artist"].DefaultView;

            Properties.Settings.Default.OutputRootDirectoryPath = MetadataRootDirectoryPath;
            Properties.Settings.Default.Save();
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
                ArtistCount = 0;
                AlbumCount = 0;
                TrackCount = 0;

                Debug.WriteLine(e);
                return null;
            }

            RefreshMessage = "Data loaded successfully!";
            RefreshMessageColor = new SolidColorBrush(Colors.LawnGreen);
            ArtistCount = artistTable.Rows.Count;
            AlbumCount = albumTable.Rows.Count;
            TrackCount = trackTable.Rows.Count;

            return dataSet;
        }
    }
}
