using Xamarin.Forms;

namespace ScanIT
{
    public partial class App : Application
    {

        string connectionString = "";
        public App()
        {

            InitializeComponent();
            MainPage = new NavigationPage(new MainPage(connectionString, GetLoggedUser()));

        }

        public string loggedUser
        {

            get => Application.Current.Properties.ContainsKey("loggedUser") ? (string)Application.Current.Properties["loggedUser"] : string.Empty;
            set
            {
                Application.Current.Properties["loggedUser"] = value;
                Application.Current.SavePropertiesAsync();
            }

        }

        private string GetLoggedUser()
        {
            return loggedUser;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }

}