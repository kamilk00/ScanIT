using Xamarin.Forms;
using ScanIT.ViewModels;

namespace ScanIT
{

    public partial class MainPage : ContentPage
    {

        private MainViewModel viewModel;

        public MainPage(string connectionString, string loggedUser)
        {

            viewModel = new MainViewModel(connectionString, loggedUser);
            BindingContext = viewModel;
            InitializeComponent();

        }


        void ZXingScannerView_OnScanResult(ZXing.Result result)
        {
            viewModel.HandleScanResult(result);
        }

    }

}