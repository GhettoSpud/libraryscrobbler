using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
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
    public partial class ScrobblingPage : Page
    {
        private Dictionary<TreeViewItem, DataRow> _treeViewItemRowMap = new Dictionary<TreeViewItem, DataRow>();

        public DataView Artists { get; private set; }

        public ScrobblingPage()
        {
            InitializeComponent();

            var dataSet = BuildDataSet();

            DataContext = new
            {
                Artists = dataSet.Tables["Artist"].DefaultView,
            };
        }

        private DataSet BuildDataSet()
        {
            var sqliteFilepath = "D:\\files\\projects\\libraryscrobbler-data\\metadata\\Sqlite\\music_metadata.sqlite";
            string connectionString = $"DataSource={sqliteFilepath};";

            var dataSet = new DataSet("Data");

            var artistTable = new DataTable("Artist");
            var albumTable = new DataTable("Album");
            var trackTable = new DataTable("Track");

            dataSet.Tables.Add(artistTable);
            dataSet.Tables.Add(albumTable);
            dataSet.Tables.Add(trackTable);

            dataSet.EnforceConstraints = false;

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    string artistQuery = "SELECT * FROM Artist";
                    using (var artistCommand = new SQLiteCommand(artistQuery, connection, transaction))
                    {
                        using (var reader = artistCommand.ExecuteReader())
                        {
                            artistTable.Load(reader);
                        }
                    }

                    string albumQuery = "SELECT * FROM Album";
                    using (var albumCommand = new SQLiteCommand(albumQuery, connection, transaction))
                    {
                        using (var reader = albumCommand.ExecuteReader())
                        {
                            albumTable.Load(reader);
                        }
                    }

                    string trackQuery = "SELECT * FROM Track";
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
                albumTable.Columns["Artist"]);

            albumTable.ChildRelations.Add(
                "Tracks",
                albumTable.Columns["Title"],
                trackTable.Columns["Album"]);

            return dataSet;
        }
    }
}
