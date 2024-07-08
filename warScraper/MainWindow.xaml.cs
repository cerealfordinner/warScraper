using System.IO;
using System.Net.Http;
using System.Windows;
using Microsoft.Win32;

namespace warScraper;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private string _saveFolderPath;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void SelectSaveLocation_Click(object sender, RoutedEventArgs e)
    {
        var folderDialog = new OpenFolderDialog();
        if (folderDialog.ShowDialog() == true)
        {
            _saveFolderPath = folderDialog.FolderName;
        }
    }

    private async void DownloadImages_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(BaseUrlTextBox.Text) || string.IsNullOrWhiteSpace(_saveFolderPath))
        {
            var messageBox = new Wpf.Ui.Controls.MessageBox();
            messageBox.Title = "Invalid Input";
            messageBox.Content = "Please enter a valid URL and select a save location.";
            messageBox.ShowDialogAsync();
            return;
        }

        await DownloadImagesAsync(BaseUrlTextBox.Text, _saveFolderPath);
    }

    private async Task DownloadImagesAsync(string baseUrl, string saveFolderPath)
    {
        using (HttpClient client = new HttpClient())
        {
            int filesDownloaded = 0;
            
            for (int i = 1; i <= 4; i++)
            {
                for (int j = 1; j <= 12; j++)
                {
                    string imageUrl = $"{baseUrl}/{i:D2}-{j:D2}.jpg";
                    try
                    {
                        byte[] imageData = await client.GetByteArrayAsync(imageUrl);
                        string fileName = $"{i:D2}-{j:D2}.jpg";
                        string filePath = Path.Combine(saveFolderPath, fileName);
                        await File.WriteAllBytesAsync(filePath, imageData);

                        filesDownloaded++;
                        DownloadProgressBar.Value = filesDownloaded;
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions (e.g., log the error, notify the user, etc.)
                    }
                }
            }
        }

        var messageBox = new Wpf.Ui.Controls.MessageBox();
        messageBox.Title = "Download Complete";
        messageBox.Content = "All images have been downloaded successfully.";
        await messageBox.ShowDialogAsync();
    }
}