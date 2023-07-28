using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Actuator.Sketch
{
    public class PinnedSketchTracker
    {
        private const string PINNED_SKETCHES = "PinnedSketches";

        private static List<string> GetCurrentPinnedSketches() =>
            EditorPrefs.GetString(PINNED_SKETCHES)
                .Split('|')
                .Distinct()
                .ToList();

        public static void PinSketch(string sketchName)
        {
            var currentSketches = GetCurrentPinnedSketches();
            if (!currentSketches.Contains(sketchName))
                currentSketches.Add(sketchName);

            UpdateSavedSketches(currentSketches);
        }

        public static void UnpinSketch(string sketchName)
        {
            var currentSketches = GetCurrentPinnedSketches();
            if (currentSketches.Contains(sketchName))
                currentSketches.Remove(sketchName);

            UpdateSavedSketches(currentSketches);
        }

        private static void UpdateSavedSketches(List<string> currentSketches)
        {
            var pinnedSketchesStr = string.Join("|", currentSketches);
            EditorPrefs.SetString(PINNED_SKETCHES, pinnedSketchesStr);
        }

        public static bool IsPinned(string sketchName)
            => GetCurrentPinnedSketches().Contains(sketchName);

        public static void ClearPinned()
        {
            EditorPrefs.DeleteKey(PINNED_SKETCHES);
        }
    }
}