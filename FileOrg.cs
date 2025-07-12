#pragma warning disable
using AssetFinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetFinder
{
    public class FileOrg
    {
        public Settings settings;

        public void StartOrganizer()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            settings = Settings.LoadSettings();
        }

        

        private void FindModelFiles_Pare() // finds files with same name but diffrent type and puts them together but only for 3d files (only fbx and blend and so on)
        {
            List<Asset> models = new();
            var found = settings.AlreadyKnowAssets.FindAll(asset =>
            {
                var type = asset.File_Type;
                return asset.File_Type.Equals(SearchOptions.only_3d);
            });
            models.AddRange(found);

            List<SameFile> sameFiles = new();

            SameFile foundName = models.FindAll(asset =>
            {
                SameFile same = new();
                asset.File_Name.Equals(same.FileName);
                return same;
            });


        }

    }
}
