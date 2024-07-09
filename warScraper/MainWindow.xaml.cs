using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
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
        DownloadButton.IsEnabled = false;
        if (string.IsNullOrWhiteSpace(BaseUrlTextBox.Text) || string.IsNullOrWhiteSpace(_saveFolderPath))
        {
            var errorContent = new Card
            {
                Margin = new Thickness(15),
                Background = Brushes.Transparent,
                Content = new StackPanel
                {
                    Children =
                    {
                        new TextBlock
                        {
                            Text = "Please enter a valid URL and select a save location",
                            Margin = new Thickness(0, 0, 0, 16)
                        },
                        new Button { Content = "OK", Command = DialogHost.CloseDialogCommand }
                    }
                }
            };
            await DialogHost.Show(errorContent, "RootDialog");
            DownloadButton.IsEnabled = true;
            return;
        }

        await DownloadImagesAsync(BaseUrlTextBox.Text, _saveFolderPath);
        DownloadButton.IsEnabled = true;
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
                    string imageUrl = $"{baseUrl}/large/{i:D2}-{j:D2}.jpg";
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

        var successContent = new Card
        {
            Margin = new Thickness(15),
            Background = Brushes.Transparent,
            Content = new StackPanel
            {
                Children =
                {
                    new TextBlock
                    {
                        Text = "All images downloaded successfully!",
                        Margin = new Thickness(0, 0, 0, 16)
                    },
                    new Button { Content = "OK", Command = DialogHost.CloseDialogCommand }
                }
            }
        };
        await DialogHost.Show(successContent, "RootDialog");
    }

    private void ColorZone_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            DragMove();
        }
    }

    private void Minimize_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void Maximize_Click(object sender, RoutedEventArgs e)
    {
        if (WindowState == WindowState.Maximized)
        {
            WindowState = WindowState.Normal;
        }
        else
        {
            WindowState = WindowState.Maximized;
        }
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}