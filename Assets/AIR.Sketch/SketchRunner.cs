using System;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AIR.Sketch
{
    [InitializeOnLoad]
    public class SketchRunner
    {
        private const string RUNNING_SKETCH_KEY = "SKETCHRUNNER_RUNNING_SKETCH";
        private const string SCENE_BEFORE_SKETCH_KEY = "SKETCHRUNNER_SCENE_BEFORE_SKETCH";

        static SketchRunner()
        {
            EditorApplication.playModeStateChanged += delegate(PlayModeStateChange change) {
                var runningSketch = EditorPrefs.GetBool(RUNNING_SKETCH_KEY);
                if (runningSketch && change == PlayModeStateChange.EnteredEditMode) {
                    EditorPrefs.SetBool(RUNNING_SKETCH_KEY, false);
                    var sceneNameBeforeSketch = EditorPrefs.GetString(SCENE_BEFORE_SKETCH_KEY);
                    if (!string.IsNullOrEmpty(sceneNameBeforeSketch))
                        EditorSceneManager.OpenScene(sceneNameBeforeSketch);
                }
            };
        }

        public static void RunSketch(Type sketchType)
        {
            // TODO: Check sketch is monobehaviour

            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            var currentScene = SceneManager.GetActiveScene();
            EditorPrefs.SetString(SCENE_BEFORE_SKETCH_KEY, currentScene.path);

            var sketchScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
            sketchScene.name = "Sketch";
            SceneManager.SetActiveScene(sketchScene);

            EditorApplication.EnterPlaymode();
            EditorPrefs.SetBool(RUNNING_SKETCH_KEY, true);

            var sketchGo = new GameObject(sketchType.Name);
            sketchGo.AddComponent<SketchServiceInstaller>();
            sketchGo.AddComponent(sketchType);
        }
    }
}