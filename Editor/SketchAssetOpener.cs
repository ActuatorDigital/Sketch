using System;
using UnityEditor;

namespace AIR.Sketch
{
    public static class SketchAssetOpener
    {
        private const string CSHARP_EXT = ".cs";

        public static void OpenSketch(Type sketchFixture)
        {
            foreach (var asset in AssetDatabase.GetAllAssetPaths())
            {
                if (!asset.EndsWith(sketchFixture.Name + CSHARP_EXT)) continue;

                var csharpAsset = (MonoScript)AssetDatabase.LoadAssetAtPath(asset, typeof(MonoScript));
                if (csharpAsset == null) continue;

                AssetDatabase.OpenAsset(csharpAsset);
                break;
            }
        }
    }
}