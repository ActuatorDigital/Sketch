using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AIR.Sketch
{
    [InitializeOnLoad]
    public class SketchRunner
    {

        private const string RUNNING_SKETCH = "SKETCHRUNNER_RUNNING_SKETCH";
        private const string SCENE_BEFORE_SKETCH = "SKETCHRUNNER_SCENE_BEFORE_SKETCH";

        static SketchRunner()
        {
            EditorApplication.playModeStateChanged += delegate(PlayModeStateChange change) {
            
                var runningSketch = EditorPrefs.GetBool(nameof(RUNNING_SKETCH));
                if (runningSketch && change == PlayModeStateChange.EnteredEditMode) {
                    EditorPrefs.SetBool(nameof(RUNNING_SKETCH), false);
                    var sceneNameBeforeSketch = EditorPrefs.GetString(SCENE_BEFORE_SKETCH);
                    if(!string.IsNullOrEmpty(sceneNameBeforeSketch))
                        EditorSceneManager.OpenScene(sceneNameBeforeSketch);
                }
            };
        }
        
        public static void RunSketch(Type type)
        {
        
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            var currentScene = SceneManager.GetActiveScene();
            EditorPrefs.SetString(SCENE_BEFORE_SKETCH, currentScene.path);
        
            var sketchScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
            sketchScene.name = "Sketch";
            SceneManager.SetActiveScene(sketchScene);

            EditorApplication.EnterPlaymode();
            EditorPrefs.SetBool(nameof(RUNNING_SKETCH), true);

            new GameObject(type.Name).AddComponent(type);
        }
    
    }
}