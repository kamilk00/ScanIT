using MySqlConnector;
using System;
using Xamarin.Forms;

namespace ScanIT.ViewModels
{

    public class LoginViewModel : BaseViewModel
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

        public Command LoginCommand { get; }

        public LoginViewModel(string connectionString)
        {

            _connectionString = connectionString;
            LoginCommand = new Command(Login);

        }


        private async void Login(object obj)
        {

            try
            {

                DbConnectionManager connectionManager = new DbConnectionManager(_connectionString);

                bool isValidLogin = connectionManager.ValidateUserCredentials(username, password);

                if (isValidLogin)
                {

                    bool isUpdateSuccessful = connectionManager.UpdateIsLoggedStatus(username, true);

                    if (isUpdateSuccessful) {
                        await Application.Current.MainPage.Navigation.PopAsync();
                        MessagingCenter.Send(this, "LoggedUser", username);
                    }

                    else
                        await Application.Current.MainPage.DisplayAlert("Error", "Failed to update user status.", "OK");

                }

                else
                    await Application.Current.MainPage.DisplayAlert("Error", "Invalid username or password.", "OK");

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