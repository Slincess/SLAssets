#pragma warning disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetFinder
{
    public class Settings
    {
        public string? AssetLib_Path;
        public List<Asset> AlreadyKnowAssets = new List<Asset>();
    } 
}
