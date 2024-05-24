using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace RWSS_WMiI
{
    public class LoadingPage : ContentPage
    {
        private ActivityIndicator _indicator;

        public LoadingPage()
        {
            BackgroundColor = Color.FromRgba(0, 0, 0, 0.5);

            _indicator = new ActivityIndicator
            {
                IsRunning = true,
                Color = Colors.Black,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };

            var loadingLabel = new Label
            {
                Text = "Ładowanie...",
                TextColor = Colors.Black,
                HorizontalOptions = LayoutOptions.Center
            };

            var stackLayout = new StackLayout
            {
                Padding = new Thickness(20),
                BackgroundColor = Colors.Gray,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    _indicator,
                    loadingLabel
                }
            };

            Content = stackLayout;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
        }

        public async Task Dismiss()
        {
            try
            {
                // Ensure the navigation context is valid
                if (Navigation.ModalStack.Contains(this))
                {
                    await Navigation.PopModalAsync();
                    _indicator.IsRunning = false;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur
                Console.WriteLine($"Exception while dismissing LoadingPage: {ex.Message}");
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // Ensure the indicator stops running
            _indicator.IsRunning = false;
        }

        protected override bool OnBackButtonPressed()
        {
            // Return true to prevent the back button from closing the page
            return true;
        }
    }
}
