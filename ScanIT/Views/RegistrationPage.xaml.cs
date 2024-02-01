using Xamarin.Forms;
using ScanIT.ViewModels;

namespace ScanIT
{

    public partial class RegistrationPage : ContentPage
    {
        public RegistrationPage(string connectionString)
        {

            BindingContext = new RegistrationViewModel(connectionString);
            InitializeComponent();

        }

    }

}