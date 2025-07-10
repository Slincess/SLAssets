#pragma warning disable
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
using System.Threading.Tasks;
using Avalonia.Layout;
using System.Reflection;
using Avalonia.Media.Imaging;

///Todo:
///Add a stable save for Game asset library path [done]
///Add adding new Game assets to that library (Add asset button fuc) [done]
///ask if new game asset should be moved to game asset library (new windows for it)
///search options like search only for .mp3, 3d obj or only asset store recommandation [done]
///import to your game project
///support for finding and adding Freesound and OpenGameArt assets
///better UI
///publish first version WHOOOHOOO
///send email to asset store owners for allowance to search their asset store

namespace AssetFinder
{
    public partial class MainWindow : Window
    {
        //private List<Asset> Assets = new List<Asset>();
        public List<Asset> SearchResult = new List<Asset>();
        Settings settings;

        public Bitmap? sfxImage;

        Dictionary<int, SearchOptions> icons = new();

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

            LoadSettings();

            Panel.LayoutUpdated += (_, __) =>
            {
                var width = Panel.Bounds.Width;
                int count = Math.Max(1, (int)(width / 220));
                double size = width / count - 10;
                ItemSize = Math.Clamp(size, 77, 200);
            };

            
            
        }
        #region buttons

        private async void AddAsset_Clicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)// Add Asset button clicked
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select your Assets",
                AllowMultiple = true,
            };

            string[]? result = await dialog.ShowAsync(this);
            
            if (result != null && result.Length > 0)
            {
                foreach (var item in result)
                {
                    string des = Path.Combine(settings.AssetLib_Path, Path.GetFileName(item));
                    File.Move(item, des);
                }
            }
        }

        private async void seeAsset_Clicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e) // Se button clicked
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Select your Assets",
            };
            string? resultString = await dialog.ShowAsync(this);
        }


        private async void Addlibrary(object? sender, Avalonia.Interactivity.RoutedEventArgs e) //for add library button
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Select your Assets",
            };
            string? resultString = await dialog.ShowAsync(this);

            if (resultString != null && resultString.Length > 0)
            {
                settings.AssetLib_Path = resultString;
                SaveSettings();
                WriteAsset(resultString);
            }
        }


        private void RefreshClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            WriteAsset(settings.AssetLib_Path);
        }


        private void UserSearching(object? sender, Avalonia.Input.KeyEventArgs e) //search bar
        {
            if (e.Key == Key.Enter && sender != null)
            {
                var textBox = sender as TextBox;
                string Search = textBox.Text;
                Search.ToLower();

                SearchOptions searchOption = GetSearchOptions();

                if (!string.IsNullOrWhiteSpace(Search))
                {
                    var searchWords = Search.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    List<Asset> MatchingName = new List<Asset>();
                    SearchResult.Clear();

                    //first find matching names to the search
                    var found = settings.AlreadyKnowAssets.FindAll(asset =>
                    {
                        string fileName = asset.File_Name.ToLower();

                        return searchWords.All(word => fileName.Contains(word));
                    });


                    //get the assets and filter it with searched option
                    if (searchOption != SearchOptions.all)
                    {
                        var FoundType = found.FindAll(asset =>
                        {
                            var type = asset.File_Type;
                            return searchOption.Equals(type);
                        });
                        SearchResult.AddRange(FoundType);
                        //if search option isnt for all then filter the searchresult
                    }
                    else SearchResult.AddRange(found); //if not then just show the searchresult 


                    if (MainPanel.IsVisible == true) ToggleSearchPanel();

                    Panel.Children.Clear();
                    foreach (var item in SearchResult) //adding buttons for import and open for every found asset
                    {
                        var title = new TextBlock
                        {
                            Text = $"{item.File_Name}",
                            TextWrapping = TextWrapping.Wrap,
                            TextAlignment = TextAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Top,
                            HorizontalAlignment = HorizontalAlignment.Center
                        };

                        var icon = new Image
                        {
                            Source = GetIcon(item.File_Type.Value)
                        };

                        Grid.SetRow(title, 0);
                        Grid.SetRow(icon, 0);
                        var buttonGrid = new Grid
                        {
                            ColumnDefinitions = new ColumnDefinitions("*,*"),
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            VerticalAlignment = VerticalAlignment.Bottom
                        };
                        Grid.SetRow(buttonGrid, 1);

                        var openButton = new Button
                        {
                            Content = new TextBlock
                            {
                                Text = "Open in File explorer",
                                TextWrapping = TextWrapping.Wrap
                            },
                            Margin = new Thickness(5),
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            VerticalAlignment = VerticalAlignment.Bottom
                        };
                        Grid.SetColumn(openButton, 0);

                        openButton.Click += async (sender, e) =>
                        {
                            string fullypath = Path.GetFullPath(item.File_Path);
                            System.Diagnostics.Process.Start("explorer.exe", string.Format("/select,\"{0}\"", fullypath));
                        };

                        var importButton = new Button
                        {
                            Content = new TextBlock
                            {
                                Text = "Import into your project",
                                TextWrapping = TextWrapping.Wrap
                            },
                            Margin = new Thickness(5),
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            VerticalAlignment = VerticalAlignment.Bottom
                        };
                        Grid.SetColumn(importButton, 1);

                        importButton.Click += (sender, e) =>
                        {
                            Console.WriteLine($"Importing {item.File_Name}");
                            ImportAsset(item);
                        };

                        buttonGrid.Children.Add(openButton);
                        buttonGrid.Children.Add(importButton);

                        var container = new Border
                        {
                            Background = new SolidColorBrush(Avalonia.Media.Color.Parse("#3f3f3f")),
                            Width = ItemSize,
                            Height = ItemSize,
                            Margin = new Thickness(10),
                            Child = new Grid
                            {
                                RowDefinitions = new RowDefinitions("Auto,*"),
                                Children =
                                {
                                    title,
                                    buttonGrid
                                }
                            }
                        };

                        Panel.Children.Add(container);
                    }
                }
            }
        }

        private async void ImportAsset(Asset item)
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Select your asset folder",
            };
            string? resultString = await dialog.ShowAsync(this);
            if (resultString == null) return;

            string fullyresult = Path.GetFullPath(resultString);

            string des = Path.Combine(fullyresult, Path.GetFileName(item.File_Path));
            File.Copy(item.File_Path, des, overwrite: true);

        }

        #endregion



        //Get Search Option and set the otherone to the this one
        private SearchOptions GetSearchOptions()
        {
            if (MainPanel.IsVisible == true) { TypSelectSearch.SelectedIndex = TypSelect.SelectedIndex; return (SearchOptions)TypSelect.SelectedIndex; }
            else if (SearchPanel.IsVisible == true) { TypSelect.SelectedIndex = TypSelectSearch.SelectedIndex; return (SearchOptions)TypSelectSearch.SelectedIndex; }
            return SearchOptions.all;
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

        static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SlAsset",
            "settings.json"
        );

        private void WriteAsset(string resultString) // get assets from asset path
        {
            string filePath = resultString;
            settings.AssetLib_Path = filePath;
            string[] result = Directory.GetFiles(filePath, "*", SearchOption.AllDirectories);
            foreach (var item in result)
            {
                Asset asset = new Asset();
                asset.File_Name = Path.GetFileName(item.ToString());
                asset.File_Path = Path.GetFullPath(item.ToString());
                string type = Path.GetExtension(item.ToString().ToLower());
                switch (type)
                {
                    case ".mp4":
                        asset.File_Type = SearchOptions.only_sfx;
                    break;
                    case ".wav":
                        asset.File_Type = SearchOptions.only_sfx;
                        break;
                    case ".fbx":
                        asset.File_Type = SearchOptions.only_3d;
                        break;
                    case ".png":
                        asset.File_Type = SearchOptions.only_images;
                        break;
                    case ".jpeg":
                        asset.File_Type = SearchOptions.only_images;
                        break;
                    default:
                        asset.File_Type = SearchOptions.all;
                        break;
                }
                settings.AlreadyKnowAssets.Add(asset);
            }
            SaveSettings();
        }

        private void GetAppAsset()//gets the needed assets for the app
        {
            string assetsfolder = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\Assets";
            string sfxiconPath = Path.Combine(assetsfolder, "sfxicon.png");
            sfxImage = new Bitmap(sfxiconPath);
        }

        private Bitmap GetIcon(SearchOptions searchId)
        {
            switch (searchId)
            {
                case SearchOptions.all:
                    break;
                case SearchOptions.only_sfx:
                    return sfxImage;
                    break;
                case SearchOptions.only_3d:
                    return null;
                    break;
                case SearchOptions.only_gameEngineAsset:
                    return null;
                    break;
                case SearchOptions.only_images:
                    return null;
                    break;
                default:
                    break;
            }
            return null;
        }

        private void LoadSettings()
        {
            if (File.Exists(SettingsPath))
            {
                string json = File.ReadAllText(SettingsPath);
                settings = JsonConvert.DeserializeObject<Settings>(json) ?? new Settings();

                if (settings.AlreadyKnowAssets == null)
                    settings.AlreadyKnowAssets = new List<Asset>();
            }
            else
            {
                settings = new Settings();
                SaveSettings();
            }
            GetAppAsset();
        }

        private void SaveSettings()
        {
            if (!Directory.Exists(Path.GetDirectoryName(SettingsPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
            }

            string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(SettingsPath, json);
        }
    }
}