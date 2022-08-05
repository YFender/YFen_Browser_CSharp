using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace YFen_Browser_CSharp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            webView.GoBack();
            if (webView.CanGoBack == false)
            {
                BackButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                BackButton.Visibility = Visibility.Visible;
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            Regex regex = new Regex(@"(https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|www\.[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\.[^\s]{2,}|https?:\/\/(?:www\.|(?!www))[a-zA-Z0-9]+\.[^\s]{2,}|www\.[a-zA-Z0-9]+\.[^\s]{2,})");
            if (regex.IsMatch(SearchLine.Text))
            {
                webView.Source = new Uri(SearchLine.Text);
            }
            else
            {
                SearchLine.Text = $"https://duckduckgo.com/?q={SearchLine.Text}";
                webView.Source = new Uri(SearchLine.Text);
            }
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            SearchLine.Text = $"https://duckduckgo.com/";
            webView.Source = new Uri(SearchLine.Text);
        }

        private void webView_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            SearchLine.Text = webView.Source.ToString();
            if (webView.CanGoBack == false)
            {
                BackButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                BackButton.Visibility = Visibility.Visible;
            }
        }
    }
}
