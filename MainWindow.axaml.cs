using Avalonia.Controls;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using System.IO;
using Avalonia.Input;
using System.Drawing;
using Avalonia.Media;
using Avalonia;
using System.Linq;
namespace AssetFinder
{
    public partial class MainWindow : Window
    {
        private List<Asset> Assets = new List<Asset>();
        public List<Asset> SearchResult = new List<Asset>();

        private double _itemSize = 150;
        public double ItemSize
        {
            get => _itemSize;
            set
            {
                if (_itemSize != value)
                {
                    _itemSize = value;
                    ItemSizeChanged?.Invoke(value);
                }
            }
        }
        public event Action<double>? ItemSizeChanged;

        public MainWindow()
        {
            InitializeComponent();

            Panel.LayoutUpdated += (_, __) =>
            {
                var width = Panel.Bounds.Width;
                int count = Math.Max(1, (int)(width / 220));
                double size = width / count - 10;
                ItemSize = Math.Clamp(size, 77, 200);
            };
        }

        private async void AddAsset_Clicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
#pragma warning disable CS0618 // Typ oder Element ist veraltet
            var dialog = new OpenFileDialog
            {
                Title = "Select your Assets",
                AllowMultiple = true,
                Filters = new List<FileDialogFilter>
                    {
                        new FileDialogFilter { Name = "Textdateien", Extensions = { "txt" } },
                        new FileDialogFilter { Name = "Alle Dateien", Extensions = { "*" } }
                    }
            };
#pragma warning restore CS0618 // Typ oder Element ist veraltet

            string[]? result = await dialog.ShowAsync(this);

            if (result != null && result.Length > 0)
            {
                string filePath = result[0];
                Console.WriteLine("Ausgewählte Datei: " + filePath);
            }
        }

        private async void seeAsset_Clicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
#pragma warning disable CS0618 // Typ oder Element ist veraltet
            var dialog = new OpenFolderDialog
            {
                Title = "Select your Assets",
            };
#pragma warning restore CS0618 // Typ oder Element ist veraltet

            string? resultString = await dialog.ShowAsync(this);

            if (resultString != null && resultString.Length > 0)
            {
                string filePath = resultString;
                string[] result = Directory.GetFiles(filePath, "*", SearchOption.AllDirectories);
                foreach (var item in result)
                {
                    Asset asset = new Asset();
                    asset.File_Name = Path.GetFileName(item.ToString());
                    asset.File_Path = Path.GetFullPath(item.ToString());

                    Assets.Add(asset);
                }
            }
        }

        private void UserSearching(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter && sender != null)
            {
                var textBox = sender as TextBox;
                string Search = textBox.Text;
                Search.ToLower();
                if (!string.IsNullOrWhiteSpace(Search))
                {
                    var searchWords = Search.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    SearchResult.Clear();
                    var found = Assets.FindAll(asset =>
                    {
                        string fileName = asset.File_Name.ToLower();

                        return searchWords.All(word => fileName.Contains(word));
                    });
                    SearchResult.AddRange(found);

                    if(MainPanel.IsVisible == true) ToggleSearchPanel();

                    Panel.Children.Clear();
                    foreach (var item in SearchResult)
                    {
                        Panel.Children.Add(new Button
                        {
                            Background = new SolidColorBrush(Avalonia.Media.Color.Parse("#3f3f3f")),
                            Foreground = Brushes.White,
                            Content = $"{item.File_Name}",
                            Width = ItemSize,
                            Height = ItemSize,
                            Margin = new Thickness(10)
                        });
                    }
                }
            }
        }

        private void UpdateButtonSizes()
        {
            foreach (var child in Panel.Children)
            {
                if (child is Button btn)
                {
                    btn.Width = ItemSize;
                    btn.Height = ItemSize;
                }
            }
        }

        private void ToggleSearchPanel()
        {
            MainPanel.IsVisible = !MainPanel.IsVisible;
            SearchPanel.IsVisible = !SearchPanel.IsVisible;
            SearchButtons.IsVisible = !SearchButtons.IsVisible;
        }
    }
}