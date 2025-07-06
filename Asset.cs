using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssetFinder;

namespace AssetFinder
{
    public class Asset
    {
        public string? File_Name;
        public string? File_Path;
        public SearchOptions? File_Type;
    }

    public enum SearchOptions
    {
        all,
        only_sfx,
        only_3d,
        only_gameEngineAsset,
        only_images
    }
}
