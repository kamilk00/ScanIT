using Xamarin.Forms;
using ScanIT.ViewModels;

namespace ScanIT
{

    public partial class HistoryPage : ContentPage
    {

        public HistoryPage(string connectionString)
        {
            
            BindingContext = new HistoryViewModel(connectionString);
            InitializeComponent();

        }

    }

}