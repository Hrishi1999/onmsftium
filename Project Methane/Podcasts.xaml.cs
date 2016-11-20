using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Project_Methane
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Podcasts : Page
    {
        bool isPlaying = false;
        public Podcasts()
        {
            this.InitializeComponent();

            Start();
        }
        async void Start()
        {
            bool success = await HarmonicKernel.Core.RetrivePodcast();

            this.DataContext = new ViewModel() { P_result = HarmonicKernel.Core.podcasts };
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Player.Source = new Uri(HarmonicKernel.Core.podcasts[FeedListView.SelectedIndex].stream_url + "?client_id=" + "c6838ff2dfb87782907b83c01067eac8");
            Position.Text = (HarmonicKernel.Core.podcasts[FeedListView.SelectedIndex].duration / -500 / 60).ToString();
            Slider.Maximum = (HarmonicKernel.Core.podcasts[FeedListView.SelectedIndex].duration / -500) / 60;
            Title.Text = HarmonicKernel.Core.podcasts[FeedListView.SelectedIndex].title;
        }

        private void Seek(object sender, RangeBaseValueChangedEventArgs e)
        {
            Player.Position = new TimeSpan(0, (int)Math.Truncate(Slider.Value), (int)(Math.Truncate(Slider.Value) - (int)Math.Floor((Math.Round(Slider.Value, 1)) * 60)));
            int x = (int)((Math.Round(Slider.Value, 1) - Math.Truncate(Slider.Value)) * 60);
            Position.Text = Math.Truncate(Slider.Value).ToString() + " minutes " + (x).ToString() + " seconds";
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (isPlaying == false)
            {
                Player.Pause();
                isPlaying = true;
            }
            else
            {
                Player.Play();
                isPlaying = false;
            }
        }
    }
}
