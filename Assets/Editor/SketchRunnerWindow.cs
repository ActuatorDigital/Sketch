using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AIR.Sketch
{
    public class SketchRunnerWindow : EditorWindow
    {
        private const int BUTTON_WIDTH = 25;
        private const string RUNNING_SKETCH_NAME = "RUNNING_SKETCH_NAME";

        private List<SketchAssembly> _sketches = new List<SketchAssembly>();
        private Vector2 _scrollPosition = Vector2.zero;
        private Type _selectedSketch;

        [MenuItem("Window/General/Sketch Runner", priority = 202)]
        private static void Init()
        {
            var window = GetWindow<SketchRunnerWindow>();
            window.titleContent = new GUIContent("Sketch Runner", "View and run project sketches.");
            window.Show();
            window.RefreshSketchList();
        }

        private void OnGUI()
        {
            if (Application.isPlaying) {
                var sketchName = EditorPrefs.GetString(RUNNING_SKETCH_NAME);
                var cancel = GUILayout.Button(
                    "Stop " + sketchName,
                    GUILayout.ExpandWidth(true),
                    GUILayout.ExpandHeight(true));
                if(cancel)
                    EditorApplication.ExitPlaymode();
            } else {
                _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
                OnDrawSketchesFixtures();
                GUILayout.EndScrollView();
            }
        }

        private void OnInspectorUpdate()
        {
            if (_selectedSketch != null) {
                SketchRunner.RunSketch(_selectedSketch);
                EditorPrefs.SetString(RUNNING_SKETCH_NAME, _selectedSketch.Name);
                _selectedSketch = null;
            }
        }

        private void OnDrawSketchesFixtures()
        {
            foreach (var sketchAssembly in _sketches) {
                GUILayout.Label(sketchAssembly.AssemblyName);
                foreach (var sketchFixture in sketchAssembly.Fixtures) {
                    DrawSketchRunnerGUIItem(sketchFixture);
                }
            }
        }

        private void DrawSketchRunnerGUIItem(SketchFixture sketchFixture)
        {
            GUILayout.BeginHorizontal();

            var nameGUISkin = EditorStyles.boldLabel;
            var nameGUICon = new GUIContent(sketchFixture.FullName);

            var descGUISkin = EditorStyles.miniLabel;
            var descGUICon = new GUIContent(sketchFixture.Description);

            var nameTextHeight = nameGUISkin.CalcHeight(nameGUICon, Screen.width - BUTTON_WIDTH);
            var descriptionTextHeight = descGUISkin.CalcHeight(descGUICon, Screen.width - BUTTON_WIDTH);

            var textHeight = nameTextHeight + descriptionTextHeight;
            var buttonHeight = GUILayout.Height(textHeight);
            var buttonWidth = GUILayout.Width(textHeight);
            var runSketch = GUILayout.Button(">", buttonHeight, buttonWidth);
            if (runSketch) {
                _selectedSketch = sketchFixture.TypeInfo;
            }

            GUILayout.BeginVertical();

            GUILayout.Label(nameGUICon, nameGUISkin);
            GUILayout.Label(descGUICon, descGUISkin);

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void Awake() => RefreshSketchList();

        private void OnFocus() => RefreshSketchList();

        private void OnProjectChange() => RefreshSketchList();

        private void OnDestroy() => _sketches.Clear();

        private void RefreshSketchList()
        {
            _sketches.Clear();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                var assemblyName = assembly.GetName().Name;
                if (!assemblyName.EndsWith(".Sketches")) continue;
                var fixtures = new List<SketchFixture>();
                foreach (var type in assembly.GetTypes()) {
                    var sketchFixtureAttribute = (SketchFixtureAttribute) Attribute
                        .GetCustomAttribute(type, typeof(SketchFixtureAttribute));
                    if (sketchFixtureAttribute == null) continue;

                    var descriptionAttribute = (SketchDescriptionAttribute) Attribute
                        .GetCustomAttribute(type, typeof(SketchDescriptionAttribute));
                    var description = descriptionAttribute.Description;

                    var name = type.FullName;
                    var sketchFixture = new SketchFixture(name, type, description);
                    fixtures.Add(sketchFixture);
                }

                _sketches.Add(new SketchAssembly(assembly.GetName().Name, fixtures.ToArray()));
            }
        }

        private readonly struct SketchAssembly
        {
            public SketchAssembly(string assemblyName, SketchFixture[] fixtures)
            {
                AssemblyName = assemblyName;
                Fixtures = fixtures;
            }

            public string AssemblyName { get; }
            public SketchFixture[] Fixtures { get; }
        }

        private readonly struct SketchFixture
        {
            public SketchFixture(string fullName, Type type, string description)
            {
                FullName = fullName;
                TypeInfo = type;
                Description = description;
            }

            public string FullName { get; }
            public Type TypeInfo { get; }
            public string Description { get; }
        }
    }
}