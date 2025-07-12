#pragma warning disable
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetFinder
{
    public class Settings
    {
        public string? AssetLib_Path;
        public string? lastAssetFolder;
        public List<Asset> AlreadyKnowAssets = new List<Asset>();


        static readonly string SettingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SlAsset",
            "settings.json"
        );

        public static Settings LoadSettings()
        {
            if (File.Exists(SettingsPath))
            {
                string json = File.ReadAllText(SettingsPath);
                Settings settings = JsonConvert.DeserializeObject<Settings>(json) ?? new Settings();

                if (settings.AlreadyKnowAssets == null)
                    settings.AlreadyKnowAssets = new List<Asset>();
                return settings;
            }
            else
            {
                Settings settings = new Settings();
                SaveSettings(settings);
                return settings;
            }
            //GetAppAsset();
        }

        private static Settings SaveSettings(Settings settings)
        {
            if (!Directory.Exists(Path.GetDirectoryName(SettingsPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
            }
            string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(SettingsPath, json);
            return settings;
        }
    } 
}
