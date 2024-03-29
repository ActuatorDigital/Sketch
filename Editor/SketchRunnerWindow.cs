﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Actuator.Sketch
{
    public class SketchRunnerWindow : EditorWindow
    {
        private Texture2D _playIcon = null;
        private Texture2D _editIcon = null;
        private Texture2D _unpinnedIcon = null;
        private Texture2D _pinnedIcon = null;

        private const int BUTTON_WIDTH = 25;
        public const string RUNNING_SKETCH_NAME = "RUNNING_SKETCH_NAME";

        private readonly List<SketchAssembly> _sketches = new List<SketchAssembly>();
        private Vector2 _scrollPosition = Vector2.zero;
        private Type _selectedSketch;
        private string _searchString;

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
            if (Application.isPlaying)
            {
                var sketchName = EditorPrefs.GetString(RUNNING_SKETCH_NAME);
                var cancel = GUILayout.Button(
                    "Stop " + sketchName,
                    GUILayout.ExpandWidth(true),
                    GUILayout.ExpandHeight(true));
                if (cancel)
                    EditorApplication.ExitPlaymode();
            }
            else
            {
                OnDrawSketchesFixtures();
            }
        }

        private void OnInspectorUpdate()
        {
            if (_selectedSketch != null)
            {
                EditorPrefs.SetString(RUNNING_SKETCH_NAME, _selectedSketch.Name);
                SketchRunner.RunSketch(_selectedSketch);
                _selectedSketch = null;
            }
        }

        private void OnDrawSketchesFixtures()
        {
            DrawSketchSearch();
            if (!string.IsNullOrEmpty(_searchString))
                DrawFilteredSketches();
            else
                DrawSketchAssemblies();

            DrawPinnedFixtures();
        }

        private void DrawSketchSearch()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("Filter:", GUILayout.ExpandWidth(false));
            const string FilterSearchTextControlName = "SketchFilterSearchBoxName";
            GUI.SetNextControlName(FilterSearchTextControlName);
            _searchString = GUILayout.TextField(_searchString, EditorStyles.toolbarSearchField);
#if UNITY_2022_3_OR_NEWER
            if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSearchCancelButton")))
#else
            if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
#endif
            {
                // Remove focus if cleared
                _searchString = string.Empty;
                GUI.FocusControl(null);
            }

            GUILayout.EndHorizontal();
        }

        private void DrawFilteredSketches()
        {
            var actualSearchString = _searchString.Trim();

            var filteredSketches = _sketches
                .SelectMany(x => x.Fixtures)
                .Where(x => x.FullName?.IndexOf(actualSearchString, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                            x.Description?.IndexOf(actualSearchString, StringComparison.InvariantCultureIgnoreCase) >=
                            0);

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            foreach (var sketchFixture in filteredSketches)
                DrawSketchRunnerGUIItem(sketchFixture);
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }

        private void DrawSketchAssemblies()
        {
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            foreach (var sketchAssembly in _sketches)
                DrawSketchAssembly(sketchAssembly);
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }

        private void DrawPinnedFixtures()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.BeginHorizontal();
            Heading("Pinned");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Clear"))
                PinnedSketchTracker.ClearPinned();
            GUILayout.EndHorizontal();
            HorizontalLine();
            foreach (var sketchAssembly in _sketches)
            {
                foreach (var sketchFixure in sketchAssembly.Fixtures)
                {
                    if (PinnedSketchTracker.IsPinned(sketchFixure.FullName))
                    {
                        DrawSketchRunnerGUIItem(sketchFixure);
                    }
                }
            }

            GUILayout.EndVertical();
        }

        private void DrawSketchAssembly(SketchAssembly sketchAssembly)
        {
            if (!sketchAssembly.Fixtures.Any())
                return;

            HorizontalLine();

            Heading(sketchAssembly.AssemblyName);

            foreach (var sketchFixture in sketchAssembly.Fixtures)
                DrawSketchRunnerGUIItem(sketchFixture);
        }

        private static void Heading(string text)
        {
            var currentStyle = GUI.skin.label;
            currentStyle.fontSize = 13;
            GUILayout.Label(text, currentStyle);
        }

        private static void HorizontalLine()
        {
            GUILayout.Space(3f);
            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, .3f));
            GUILayout.Space(3f);
        }

        private void DrawSketchRunnerGUIItem(SketchFixture sketchFixture)
        {
            GUILayout.BeginHorizontal();

            var nameGUISkin = EditorStyles.boldLabel;
            var nameGUICon = new GUIContent(sketchFixture.ShortName);

            var descGUISkin = EditorStyles.miniLabel;
            var descGUICon = new GUIContent(sketchFixture.Description);

            var nameTextHeight = nameGUISkin.CalcHeight(nameGUICon, Screen.width - BUTTON_WIDTH);
            var descriptionTextHeight = descGUISkin.CalcHeight(descGUICon, Screen.width - BUTTON_WIDTH);

            var textHeight = nameTextHeight + descriptionTextHeight;
            var buttonHeight = GUILayout.Height(textHeight);
            var runButtonWidth = GUILayout.Width(textHeight * 1.4f);

            var playIconContent = new GUIContent(_playIcon);
            var runSketch = GUILayout.Button(playIconContent, buttonHeight, runButtonWidth);
            if (runSketch)
                _selectedSketch = sketchFixture.TypeInfo;

            var buttonWidth = GUILayout.Width(textHeight);
            var editIconContent = new GUIContent(_editIcon);
            var openCode = GUILayout.Button(editIconContent, buttonHeight, buttonWidth);
            if (openCode)
                AssetDatabase.OpenAsset(sketchFixture.Asset);

            var scriptIconContent = new GUIContent(EditorGUIUtility.IconContent("cs Script Icon").image);
            var selectAsset = GUILayout.Button(scriptIconContent, buttonHeight, buttonWidth);
            if (selectAsset)
                Selection.activeObject = sketchFixture.Asset;

            GUILayout.BeginVertical();

            GUILayout.Label(nameGUICon, nameGUISkin);
            GUILayout.Label(descGUICon, descGUISkin);

            GUILayout.EndVertical();

            var pinButtonSkin = new GUIStyle(GUI.skin.label);
            if (PinnedSketchTracker.IsPinned(sketchFixture.FullName))
            {
                var unpinnedIconContent = new GUIContent(_pinnedIcon);
                var unpinClicked = GUILayout.Button(unpinnedIconContent, pinButtonSkin, buttonHeight, buttonWidth);
                if (unpinClicked)
                    PinnedSketchTracker.UnpinSketch(sketchFixture.FullName);
            }
            else
            {
                var pinnedIconContent = new GUIContent(_unpinnedIcon);
                var pinClicked = GUILayout.Button(pinnedIconContent, pinButtonSkin, buttonHeight, buttonWidth);
                if (pinClicked)
                    PinnedSketchTracker.PinSketch(sketchFixture.FullName);
            }

            GUILayout.EndHorizontal();
        }

        private void Awake()
        {
            var playIconPath = "Packages/com.actuator.sketch/Editor/PlayIcon.png";
            _playIcon = (Texture2D) AssetDatabase.LoadAssetAtPath(playIconPath, typeof(Texture2D));

            var editIconPath = "Packages/com.actuator.sketch/Editor/EditIcon.png";
            _editIcon = (Texture2D) AssetDatabase.LoadAssetAtPath(editIconPath, typeof(Texture2D));

            var unpinnedIconPath = "Packages/com.actuator.sketch/Editor/UnpinnedIcon.png";
            _unpinnedIcon = (Texture2D) AssetDatabase.LoadAssetAtPath(unpinnedIconPath, typeof(Texture2D));

            var pinnedIconPath = "Packages/com.actuator.sketch/Editor/PinnedIcon.png";
            _pinnedIcon = (Texture2D) AssetDatabase.LoadAssetAtPath(pinnedIconPath, typeof(Texture2D));

            RefreshSketchList();
        }

        private void OnFocus() => RefreshSketchList();

        private void OnProjectChange() => RefreshSketchList();

        private void OnDestroy() => _sketches.Clear();

        private void RefreshSketchList()
        {
            _sketches.Clear();
            var locator = new SketchAssetLocator();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var fixtures = new List<SketchFixture>();
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsAbstract) continue;
                    var sketchFixtureAttribute = (SketchFixtureAttribute) Attribute
                        .GetCustomAttribute(type, typeof(SketchFixtureAttribute));
                    if (sketchFixtureAttribute == null) continue;

                    var descriptionAttribute = (SketchDescriptionAttribute) Attribute
                        .GetCustomAttribute(type, typeof(SketchDescriptionAttribute));
                    var description = descriptionAttribute?.Description;

                    var sketchFixture = new SketchFixture()
                    {
                        ShortName = type.Name,
                        FullName = type.FullName,
                        TypeInfo = type,
                        Description = description,
                        Asset = locator.FindAssetForType(type),
                    };
                    fixtures.Add(sketchFixture);
                }

                if (fixtures.Any())
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

        private struct SketchFixture
        {
            public string ShortName;
            public string FullName;
            public Type TypeInfo;
            public string Description;
            public MonoScript Asset;
        }
    }
}