using ScanIT.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ScanIT.ViewModels
{

    public class FavouritesViewModel : BaseViewModel
    {

        private List<FavouriteItem> _favouriteItems;
        DbConnectionManager _connectionManager;
        public Command RemoveFromFavouritesCommand { get; }

        public List<FavouriteItem> favouriteItems
        {
            get { return _favouriteItems; }
            set { SetProperty(ref _favouriteItems, value); }
        }

        public FavouritesViewModel(string connectionString)
        {

            _connectionManager = new DbConnectionManager(connectionString);
            LoadFavourites();
            RemoveFromFavouritesCommand = new Command<FavouriteItem>(async (selectedFavourite) => await RemoveFromFavourites(selectedFavourite));


        }


        private void LoadFavourites()
        {

            string _loggedUser = Application.Current.Properties.ContainsKey("loggedUser") ? Application.Current.Properties["loggedUser"] as string : null;
            if (!string.IsNullOrEmpty(_loggedUser))
                favouriteItems = _connectionManager.GetFavouriteItems(_loggedUser);

        }


        private async Task RemoveFromFavourites(FavouriteItem selectedFavourite)
        {

            string _loggedUser = Application.Current.Properties.ContainsKey("loggedUser") ? Application.Current.Properties["loggedUser"] as string : null;

            bool removeConfirmed = await Application.Current.MainPage.DisplayAlert("Remove Item",
                                                                                  $"Do you want to remove {selectedFavourite.Code} from favourites?",
                                                                                   "Yes", "No");

            if (removeConfirmed)
            {

                try
                {

                    _connectionManager.RemoveFavouriteItem(selectedFavourite.Code, _loggedUser);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        favouriteItems.Remove(selectedFavourite);
                        MessagingCenter.Send(this, "UpdateFavouriteListView", favouriteItems);
                    });

                }

                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to remove item from favourites.", "OK");
                }

            }

        
        }

    }

}