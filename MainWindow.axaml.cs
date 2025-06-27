using Avalonia.Controls;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using System.IO;
using Avalonia.Input;
namespace AssetFinder
{
    public partial class MainWindow : Window
    {
        private List<Asset> Assets = new List<Asset>();
        public List<Asset> SearchResult = new List<Asset>();

        public MainWindow()
        {
            InitializeComponent();
            
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
                string[] result = Directory.GetFiles(filePath,"*",SearchOption.AllDirectories);
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
            if(e.Key == Key.Enter && sender != null)
            {

                var textBox = sender as TextBox;
                string Search = textBox.Text;
                if (!string.IsNullOrWhiteSpace(Search))
                {
                    SearchResult.Clear();
                    var found = Assets.FindAll(asset => asset.File_Name.Contains(Search, StringComparison.OrdinalIgnoreCase));
                    SearchResult.AddRange(found);

                    MainPanel.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top;

                    var List = this.FindControl<StackPanel>("AssetFiles");
                    List.Children.Clear();
                    foreach (var item in SearchResult)
                    {
                        List.Children.Add(new TextBlock { Text = $"{item.File_Name}" });
                    }
                }

            }
        }
    }
}