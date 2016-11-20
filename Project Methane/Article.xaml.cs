using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
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
    public sealed partial class Article : Page
    {
        public Article()
        {
            this.InitializeComponent(); var view = ApplicationView.GetForCurrentView();
            var titleBar = view.TitleBar;

            titleBar.ButtonBackgroundColor = ((SolidColorBrush)ActionBar.BorderBrush).Color;
            Start();
            //      Window.Current.SetTitleBar(ActionBar);
        }
        void Start()
        {
            WebView.NavigateToString(HarmonicKernel.Core.LoadArticle(HarmonicKernel.Core.currentAticleId, WebView.Width, WebView.ActualHeight));

        }
        private void ContainsFullScreenElementChanged(WebView sender, object args)
        {
            if (WebView.ContainsFullScreenElement == true)
            {
                ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            }
        }
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void LoadComments(object sender, RoutedEventArgs e)
        {
            string a = HarmonicKernel.Core.articles[HarmonicKernel.Core.currentAticleId].title.Replace(' ', '-');
            string b = HarmonicKernel.Core.articles[HarmonicKernel.Core.currentAticleId].id.ToString();
            WebView.Navigate(new Uri("https://disqus.com/embed/comments/?base=default&amp;version=0e711e2730e4c6cf1685e5e4dfedf9c9&amp;f=winbeta&amp;t_i=" + b + "%20http%3A%2F%2Fwww.winbeta.org%2F%3Fp%3D" + b + "&amp;t_u=http%3A%2F%2Fwww.winbeta.org%2F" + a));
        }

        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();

        }

        private void textBlock2_Tapped(object sender, TappedRoutedEventArgs e)
        {
            string a = HarmonicKernel.Core.articles[HarmonicKernel.Core.currentAticleId].title.Replace(' ', '-');
            string b = HarmonicKernel.Core.articles[HarmonicKernel.Core.currentAticleId].id.ToString();
            WebView.Navigate(new Uri("https://disqus.com/embed/comments/?base=default&amp;version=0e711e2730e4c6cf1685e5e4dfedf9c9&amp;f=winbeta&amp;t_i=" + b + "%20http%3A%2F%2Fwww.winbeta.org%2F%3Fp%3D" + b + "&amp;t_u=http%3A%2F%2Fwww.winbeta.org%2F" + a));
        }

        private void AppBarButton_Click_2(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
    }

}

