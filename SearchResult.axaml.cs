using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AssetFinder;
using System.Collections.Generic;

namespace AssetFinder;

public partial class SearchResult : UserControl
{
    public SearchResult()
    {
        InitializeComponent();
    }

    public void SetResult(List<Asset> assets)
    {
        var List = this.FindControl<StackPanel>("AssetFiles");
        List.Children.Clear();
        foreach (var item in assets)
        {
            List.Children.Add(new TextBlock { Text = $"{item.File_Name}" });
        }
    }
}