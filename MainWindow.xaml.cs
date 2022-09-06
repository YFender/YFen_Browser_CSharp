using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace YFen_Browser_CSharp
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            Check_version();


            if (File.Exists("Setup.msi"))
            {
                File.Delete("Setup.msi");
            }
        }



        private async Task Check_version()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Equals("application/vnd.github+json");
                httpClient.DefaultRequestHeaders.Add("User-Agent", "Other");
                string request = "https://api.github.com/repos/YFender/YFen_Browser_CSharp/releases/latest";
                HttpResponseMessage response = (await httpClient.GetAsync(request));
                string responseBody = await response.Content.ReadAsStringAsync();

                JObject jObject = JObject.Parse(responseBody);
                JToken list = jObject["name"];

                if (list.ToString() != "v1.0.3")
                {
                    var update = MessageBox.Show("Обнаружена новая версия браузера.\nОбновить?", "Доступно обновление", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (update == MessageBoxResult.Yes)
                    {
                        try
                        {
                            webView.Visibility = Visibility.Collapsed;
                            BackButton.Visibility = Visibility.Collapsed;
                            SearchButton.Visibility = Visibility.Collapsed;
                            SearchLine.Visibility = Visibility.Collapsed;
                            HomeButton.Visibility = Visibility.Collapsed;

                            DownloadProgressBar.Visibility = Visibility.Visible;

                            string downloadFileUrl = jObject["assets"][0]["browser_download_url"].ToString();
                            var destinationFilePath = Path.GetFullPath("Setup.msi");

                            using (var client = new HttpClientDownloadWithProgress(downloadFileUrl, destinationFilePath))
                            {
                                client.ProgressChanged += (totalFileSize, totalBytesDownloaded, progressPercentage) =>
                                {
                                    DownloadProgressBar.Value = (double)progressPercentage;
                                };

                                await client.StartDownload();
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Ошибка загрузки");
                            this.Close();
                        }
                        finally
                        {
                            Process.Start("Setup.msi");
                            this.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
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

        private void VKButton_Click(object sender, RoutedEventArgs e)
        {
            SearchLine.Text = "https://vk.com/";
            webView.Source = new Uri(SearchLine.Text);
        }
    }
}
