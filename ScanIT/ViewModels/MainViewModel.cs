using MySqlConnector;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace ScanIT.ViewModels
{

    public class MainViewModel : BaseViewModel
    {

        private string _loggedUser;
        private string _connectionString;
        private DbConnectionManager _connectionManager;

        public Command CopyCommand { get; }
        public Command GoToRegistrationPageCommand { get; }
        public Command GoToFavouritesPageCommand { get; }
        public Command GoToLoginPageCommand { get; }
        public Command GoToHistoryPageCommand { get; }
        public Command GoToAboutPageCommand { get; }
        public Command LogoutCommand { get; }
        public Command AddToFavouritesCommand { get; }
        public Command EditDescriptionCommand { get; }

        private bool _codeTypeLabelVisibility;
        public bool codeTypeLabelVisibility
        {
            get { return _codeTypeLabelVisibility; }
            set { SetProperty(ref _codeTypeLabelVisibility, value); }
        }

        private bool _copyButtonVisibility;
        public bool copyButtonVisibility
        {
            get { return _copyButtonVisibility; }
            set { SetProperty(ref _copyButtonVisibility, value); }
        }

        private bool _addToFavouritesButtonVisibility;
        public bool addToFavouritesButtonVisibility
        {
            get { return _addToFavouritesButtonVisibility; }
            set { SetProperty(ref _addToFavouritesButtonVisibility, value); }
        }

        private bool _descriptionLabelVisibility;
        public bool descriptionLabelVisibility
        {
            get { return _descriptionLabelVisibility; }
            set { SetProperty(ref _descriptionLabelVisibility, value); }
        }
        
        private bool _scanResultLabelVisibility;
        public bool scanResultLabelVisibility
        {
            get { return _scanResultLabelVisibility; }
            set { SetProperty(ref _scanResultLabelVisibility, value); }
        }

        private bool _editDescriptionButtonVisibility;
        public bool editDescriptionButtonVisibility
        {
            get { return _editDescriptionButtonVisibility; }
            set { SetProperty(ref _editDescriptionButtonVisibility, value); }
        }

        private string _codeTypeLabel;
        public string codeTypeLabel
        {
            get { return _codeTypeLabel; }
            set { SetProperty(ref _codeTypeLabel, value); }
        }

        private string _descriptionLabel;
        public string descriptionLabel
        {
            get { return _descriptionLabel; }
            set { SetProperty(ref _descriptionLabel, value); }
        }

        private string _scanResultLabel;
        public string scanResultLabel
        {
            get { return _scanResultLabel; }
            set { SetProperty(ref _scanResultLabel, value); }
        }

        private bool _registerButtonVisibility;
        public bool registerButtonVisibility
        {
            get { return _registerButtonVisibility; }
            set { SetProperty(ref _registerButtonVisibility, value); }
        }

        private bool _loginButtonVisibility;
        public bool loginButtonVisibility
        {
            get { return _loginButtonVisibility; }
            set { SetProperty(ref _loginButtonVisibility, value); }
        }

        private bool _logoutButtonVisibility;
        public bool logoutButtonVisibility
        {
            get { return _logoutButtonVisibility; }
            set { SetProperty(ref _logoutButtonVisibility, value); }
        }

        private bool _historyButtonVisibility;
        public bool historyButtonVisibility
        {
            get { return _historyButtonVisibility; }
            set { SetProperty(ref _historyButtonVisibility, value); }
        }

        private bool _favouritesButtonVisibility;
        public bool favouritesButtonVisibility
        {
            get { return _favouritesButtonVisibility; }
            set { SetProperty(ref _favouritesButtonVisibility, value); }
        }

        public MainViewModel(string connectionString, string loggedUser)
        {

            _loggedUser = loggedUser;
            _connectionString = connectionString;
            _connectionManager = new DbConnectionManager(_connectionString);

            CopyCommand = new Command(async () => await CopyToClipboardAsync(scanResultLabel));
            GoToRegistrationPageCommand = new Command(async () => await GoToRegistrationPage());
            GoToFavouritesPageCommand = new Command(async () => await GoToFavouritesPage());
            GoToLoginPageCommand = new Command(async () => await GoToLoginPage());
            GoToHistoryPageCommand = new Command(async () => await GoToHistoryPage());
            GoToAboutPageCommand = new Command(async () => await GoToAboutPage());
            LogoutCommand = new Command(async () => await Logout());
            AddToFavouritesCommand = new Command(AddToFavourites);
            EditDescriptionCommand = new Command(async () => await EditDescription());

            UpdateButtonVisibility(_loggedUser);

            MessagingCenter.Subscribe<RegistrationViewModel, string>(this, "LoggedUser", (sender, username) =>
            {

                _loggedUser = username;
                UpdateLoggedUser();
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("Registered and Logged In", $"User '{username}' has been logged in!", "OK");
                });
                UpdateButtonVisibility(username);

            });

            MessagingCenter.Subscribe<LoginViewModel, string>(this, "LoggedUser", (sender, username) =>
            {

                _loggedUser = username;
                UpdateLoggedUser();
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("Logged In", $"User '{username}' has been logged in!", "OK");
                });
                UpdateButtonVisibility(username);

            });

        }


        private async Task CopyToClipboardAsync(string text)
        {

            if (!string.IsNullOrWhiteSpace(text))
                await Clipboard.SetTextAsync(text);

        }


        private async Task GoToRegistrationPage()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new RegistrationPage(_connectionString));
        }


        private async Task GoToFavouritesPage()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new FavouritesPage(_connectionString));
        }


        private async Task GoToLoginPage()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new LoginPage(_connectionString));
        }


        private async Task GoToHistoryPage()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new HistoryPage(_connectionString));
        }


        private async Task GoToAboutPage()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new AboutPage());
        }


        private async Task Logout()
        {

            bool answer = await Application.Current.MainPage.DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No");

            if (answer)
            {

                try
                {

                    bool updated = _connectionManager.UpdateIsLoggedStatus(_loggedUser, false);

                    if (updated)
                    {
                        _loggedUser = null;
                        UpdateButtonVisibility(_loggedUser);
                        UpdateLoggedUser();
                        await Application.Current.MainPage.DisplayAlert("Logged out", "User has been logged out successfully.", "OK");
                    }

                    else
                        await Application.Current.MainPage.DisplayAlert("Error", "Logout failed. Please try again.", "OK");

                }

                catch (MySqlException ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Database error. Please try again later.", "OK");
                }

                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "An unexpected error occurred.", "OK");
                }

            }

        }


        private void AddToFavourites()
        {

            if (!string.IsNullOrEmpty(_loggedUser))
            {

                if (!_connectionManager.IsAddedToFavourites(scanResultLabel, _loggedUser))
                {

                    bool inserted = _connectionManager.InsertFavourites(scanResultLabel, codeTypeLabel.Substring(14), _loggedUser);
                    if (!inserted)
                        Application.Current.MainPage.DisplayAlert("Save To Favourites Error", "Problems with database connection!", "OK");

                }

            }

        }


        private async Task EditDescription()
        {

            string existingDescription = _connectionManager.GetDescriptionFromDatabase(scanResultLabel);
            string newDescription = await Application.Current.MainPage.DisplayPromptAsync("Edit Description", "Enter the new description:", "OK", "Cancel", null, -1, null, existingDescription);

            if (newDescription != null)
            {
                _connectionManager.UpdateDescription(scanResultLabel, codeTypeLabel.Substring(14), newDescription, _loggedUser);
                descriptionLabel = "Description: " + newDescription;

            }

        }


        public void HandleScanResult(ZXing.Result result)
        {

            if (result.BarcodeFormat == ZXing.BarcodeFormat.EAN_8 || result.BarcodeFormat == ZXing.BarcodeFormat.EAN_13 ||
                result.BarcodeFormat == ZXing.BarcodeFormat.QR_CODE)
            {

                var descr = _connectionManager.GetDescriptionFromDatabase(result.Text);
                codeTypeLabelVisibility = true;
                scanResultLabelVisibility = true;
                descriptionLabelVisibility = result.BarcodeFormat != ZXing.BarcodeFormat.QR_CODE;
                
                codeTypeLabel = "Type of code: " + result.BarcodeFormat.ToString();
                scanResultLabel = result.Text;
                copyButtonVisibility = true;
                addToFavouritesButtonVisibility = !string.IsNullOrEmpty(_loggedUser);
                editDescriptionButtonVisibility = !string.IsNullOrEmpty(_loggedUser);
                descriptionLabel = "Description: " + descr;

                if (!string.IsNullOrEmpty(_loggedUser))
                {

                    if (!_connectionManager.IsDuplicateScan(scanResultLabel, _loggedUser))
                    {

                        bool inserted = _connectionManager.InsertScanningHistory(scanResultLabel, result.BarcodeFormat.ToString(), _loggedUser);
                        if (!inserted)
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                await Application.Current.MainPage.DisplayAlert("Save Scan Error", "Problems with database connection!", "OK");
                            });

                    }

                }

                descriptionLabel = "Description: " + _connectionManager.GetDescriptionFromDatabase(scanResultLabel);

            }

        }


        private void UpdateButtonVisibility(string username)
        {

            if (string.IsNullOrEmpty(username))
            {

                registerButtonVisibility = true;
                loginButtonVisibility = true;
                logoutButtonVisibility = false;
                historyButtonVisibility = false;
                favouritesButtonVisibility = false;
                addToFavouritesButtonVisibility = false;
                editDescriptionButtonVisibility = false;

            }

            else
            {

                registerButtonVisibility = false;
                loginButtonVisibility = false;
                logoutButtonVisibility = true;
                historyButtonVisibility = true;
                favouritesButtonVisibility = true;

            }

        }


        void UpdateLoggedUser()
        {

            Application.Current.Properties["loggedUser"] = _loggedUser;
            Application.Current.SavePropertiesAsync();

        }

    }

}