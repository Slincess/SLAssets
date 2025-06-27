using Avalonia.Controls;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using System.Formats.Asn1;
using System.IO;
namespace AssetFinder
{
    public partial class MainWindow : Window
    {
        private List<string> Assets = new List<string>();

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

            string? result = await dialog.ShowAsync(this);

            if (result != null && result.Length > 0)
            {
                string filePath = result;
                Assets.AddRange(Directory.GetFiles(result, "*", SearchOption.AllDirectories));
            }
        }

        private void UserSearching(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            //List
        }
    }
}