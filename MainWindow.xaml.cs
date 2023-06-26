using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
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


namespace PhotoDownloader
{

    public partial class MainWindow : Window
    {
        private WebClient client = new();
        public MainWindow()
        {
            InitializeComponent();

            client.DownloadProgressChanged += Client_DownloadProgressChanged;
        }

        private async void DownloadBtnClick(object sender, RoutedEventArgs e)
        {
            string width = widthTextBox.Text;
            string height = heightTextBox.Text;
            string category = categoryTextBox.Text;


            if (client.IsBusy)
            {
                MessageBox.Show("Try again later!");
                return;
            }

            string url = urlTxtBox.Text;

            //if (string.IsNullOrWhiteSpace(url))
            //{
            //    MessageBox.Show("Invalid URL!");
            //    return;
            //}

            //await DownloadFileAsync(url);
            await DownloadImageAsync(width, height, category);
            // add new item to history
            //AddHistoryItem(url);
        }

        private async Task DownloadFileAsync(string url)
        {
            // get desktop path
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            // get file name
            string name = Path.GetFileName(url);

            // create destination file path: "desktop/name.ext"
            string dest = Path.Combine(desktopPath, name);

            await client.DownloadFileTaskAsync(url, dest);
        }

        private void AddHistoryItem(string fileName)
        {
            historyList.Items.Add($"{DateTime.Now.ToShortTimeString()}: {fileName}");
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }
        private async Task DownloadImageAsync(string width, string height, string category)
        {
            try
            {

                string url = $"https://source.unsplash.com/random/{width}x{height}/?{category}&1";


                using (WebClient client = new WebClient())
                {

                    byte[] imageData = await client.DownloadDataTaskAsync(url);


                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = $"{category}_{width}{height}.jpg";
                    saveFileDialog.DefaultExt = ".jpg";
                    saveFileDialog.Filter = "JPEG Image|*.jpg";


                    bool? result = saveFileDialog.ShowDialog();


                    if (result == true)
                    {
                        string filePath = saveFileDialog.FileName;
                        File.WriteAllBytes(filePath, imageData);
                        Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
                        MessageBox.Show("Image downloaded successfully and saved.");
                        string nameOfFile = Path.GetFileName(filePath);
                        if (nameOfFile != "")
                        {
                            AddHistoryItem(nameOfFile);
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error downloading image: {ex.Message}");
            }
        }

    }
}
