using Xamarin.Forms;
using ScanIT.ViewModels;

namespace ScanIT
{

    public partial class LoginPage : ContentPage
    {

        public LoginPage(string connectionString)
        {

            BindingContext = new LoginViewModel(connectionString);
            InitializeComponent();

        }

    }

}