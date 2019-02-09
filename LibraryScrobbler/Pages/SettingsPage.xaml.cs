using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security;
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

namespace LibraryScrobbler.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page, INotifyPropertyChanged
    {
        #region Properties

        private bool passwordChanged = false;

        private string _lastFmApiKey;
        public string LastFmApiKey
        {
            get { return _lastFmApiKey; }
            set
            {
                _lastFmApiKey = value;
                RaisePropertyChanged("LastFmApiKey");
            }
        }

        private string _lastFmUsername;
        public string LastFmUsername
        {
            get { return _lastFmUsername; }
            set
            {
                _lastFmUsername = value;
                RaisePropertyChanged("LastFmUsername");
            }
        }

        private string _lastFmApiSecret;
        public string LastFmApiSecret
        {
            get { return _lastFmApiSecret; }
            set
            {
                _lastFmApiSecret = value;
                RaisePropertyChanged("LastFmApiSecret");
            }
        }

        private string _saveStatusMessage;
        public string SaveStatusMessage
        {
            get { return _saveStatusMessage; }
            set
            {
                _saveStatusMessage = value;
                RaisePropertyChanged("SaveStatusMessage");
            }
        }

        private void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public SettingsPage()
        {
            InitializeComponent();

            LastFmApiKey = Properties.Settings.Default.LastFmApiKey;
            LastFmApiSecret = Properties.Settings.Default.LastFmApiSecret;
            LastFmUsername = Properties.Settings.Default.LastFmUsername;

            DataContext = this;
        }

        private void Web_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs args)
        {
            SaveStatusMessage = "";

            bool apiKeyChanged = Properties.Settings.Default.LastFmApiKey != LastFmApiKey;
            if (apiKeyChanged && !string.IsNullOrWhiteSpace(LastFmApiKey))
            {
                Properties.Settings.Default.LastFmApiKey = LastFmApiKey;
                SaveStatusMessage += "Updated API Key! ";
            }

            bool apiSecretChanged = Properties.Settings.Default.LastFmApiSecret != LastFmApiSecret;
            if (apiSecretChanged && !string.IsNullOrWhiteSpace(LastFmApiSecret))
            {
                Properties.Settings.Default.LastFmApiSecret = LastFmApiSecret;
                SaveStatusMessage += "Updated API Secret! ";
            }

            bool usernameChanged = Properties.Settings.Default.LastFmUsername != LastFmUsername;
            if (usernameChanged && !string.IsNullOrWhiteSpace(LastFmUsername))
            {
                Properties.Settings.Default.LastFmUsername = LastFmUsername;
                SaveStatusMessage += "Updated Username! ";
            }

            if (passwordChanged)
            {
                SaveStatusMessage += "Updated Password! ";
            }

            if (string.IsNullOrEmpty(SaveStatusMessage))
            {
                SaveStatusMessage = "No Settings Updated!";
            }

            Properties.Settings.Default.Save();

            passwordChanged = false;
        }

        private void LastFmPasswordChanged(object sender, RoutedEventArgs e)
        {
            passwordChanged = true;
            Properties.Settings.Default.LastFmPassword = (e.Source as PasswordBox).Password;
        }
    }
}
