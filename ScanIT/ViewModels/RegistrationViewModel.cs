using MySqlConnector;
using System;
using Xamarin.Forms;

namespace ScanIT.ViewModels
{
    public class RegistrationViewModel : BaseViewModel
    {

        private string _connectionString;

        private string _username;
        public string username
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }

        private string _password;
        public string password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        public Command RegisterCommand { get; }

        public RegistrationViewModel(string connectionString)
        {

            _connectionString = connectionString;
            RegisterCommand = new Command(Register);

        }


        private async void Register()
        {

            try
            {

                DbConnectionManager connectionManager = new DbConnectionManager(_connectionString);

                bool inserted = connectionManager.RegisterUser(username, password);

                if (inserted)
                {

                    await Application.Current.MainPage.Navigation.PopAsync();
                    MessagingCenter.Send(this, "LoggedUser", username);

                }

            }

            catch (ArgumentException ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
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

}