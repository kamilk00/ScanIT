using Xamarin.Forms;
using ScanIT.ViewModels;
using ScanIT.Models;
using System.Collections.Generic;

namespace ScanIT
{

    public partial class FavouritesPage : ContentPage
    {

        public FavouritesPage(string connectionString)
        {

            BindingContext = new FavouritesViewModel(connectionString);
            InitializeComponent();

            FavouriteListView.ItemSelected += async (sender, e) =>
            {

                if (e.SelectedItem is FavouriteItem selectedFavourite)
                {

                    ((FavouritesViewModel)BindingContext).RemoveFromFavouritesCommand.Execute(selectedFavourite);
                    FavouriteListView.SelectedItem = null;

                }

            };

            MessagingCenter.Subscribe<FavouritesViewModel, List<FavouriteItem>>(this, "UpdateFavouriteListView", (sender, favouriteItems) =>
            {
                FavouriteListView.ItemsSource = null;
                FavouriteListView.ItemsSource = favouriteItems;
            });

        }

    }

}