using ScanIT.Models;
using System.Collections.Generic;
using Xamarin.Forms;

namespace ScanIT.ViewModels
{

    public class HistoryViewModel : BaseViewModel
    {

        private string _connectionString;
        private List<ScanHistoryItem> _historyItems;

        public List<ScanHistoryItem> historyItems
        {
            get => _historyItems;
            set
            {
                _historyItems = value;
                OnPropertyChanged();
            }
        }

        public HistoryViewModel(string connectionString)
        {

            _connectionString = connectionString;
            LoadHistory();

        }


        //load user's scanning history
        private void LoadHistory()
        {

            string loggedInUser = Application.Current.Properties.ContainsKey("loggedUser") ? Application.Current.Properties["loggedUser"] as string : null;
            if (!string.IsNullOrEmpty(loggedInUser))
            {

                DbConnectionManager connectionManager = new DbConnectionManager(_connectionString);
                historyItems = connectionManager.GetScanHistory(loggedInUser);

            }

        }

    }

}