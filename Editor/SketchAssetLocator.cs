using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace AIR.Sketch
{
    public sealed class SketchAssetLocator
    {
        private const string CSHARP_EXT = ".cs";
        private List<string> _allScriptAssets;

        public SketchAssetLocator()
        {
            _allScriptAssets = AssetDatabase.GetAllAssetPaths()
                .Where(asset => asset.EndsWith(CSHARP_EXT))
                .ToList();
        }

        public MonoScript FindAssetForType(Type sketchFixture)
        {
            var asset = _allScriptAssets.FirstOrDefault(asset => asset.EndsWith(sketchFixture.Name + CSHARP_EXT));
            if (asset == null)
                return null;

            var csharpAsset = (MonoScript)AssetDatabase.LoadAssetAtPath(asset, typeof(MonoScript));
            return csharpAsset;
        }
    }
}