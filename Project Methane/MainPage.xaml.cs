using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Project_Methane
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            InitializeComponent();
        
            CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true; var view = ApplicationView.GetForCurrentView();
            var titleBar = view.TitleBar;

            titleBar.ButtonBackgroundColor = Colors.White;
            titleBar.ButtonForegroundColor = Color.FromArgb(255, 33, 134, 255);

            HarmonicKernel.Settings.accentColor = "#FF4C75";
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await HarmonicKernel.Core.GetArticleFeed();



            this.DataContext = new ViewModel() { Result = HarmonicKernel.Core.articles };
            BitmapImage bi = new BitmapImage();
            bi.UriSource = new Uri(HarmonicKernel.Core.articles[1].thumbnail);
            LimelightImage1.Source = bi;
            LimelightTitle1.Text = HarmonicKernel.Core.articles[1].title;
            BitmapImage bi2 = new BitmapImage();
            bi2.UriSource = new Uri(HarmonicKernel.Core.articles[2].thumbnail);
            Image1.Source = bi2;
            Description1.Text = HarmonicKernel.Core.articles[1].title;


            //   ProgressRing.IsActive = false;
            //    var x = MainGrid.ActualWidth;
        }



        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FeedListView.SelectedIndex > -1)
            {
                HarmonicKernel.Core.currentAticleId = (sender as GridView).SelectedIndex;
                Frame.Navigate(typeof(Article));
            }
        }




        private void Videos(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
           // Frame.Navigate(typeof(Videos));
        }

        private void Podcasts(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(Podcasts));
        }
    }
}
