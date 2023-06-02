using System;
using System.Linq;
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
        private const string SKETCH_FULL_TYPE_NAME_KEY = "SKETCHRUNNER_FULL_TYPE_NAME";
        private const string SKETCH_ASSEMBLY_NAME_KEY = "SKETCHRUNNER_ASSEMBLY_NAME";

        static SketchRunner()
        {
            EditorApplication.playModeStateChanged += change =>
            {
                var runningSketch = EditorPrefs.GetBool(RUNNING_SKETCH_KEY);
                if (!runningSketch)
                    return;

                if (change == PlayModeStateChange.EnteredEditMode)
                {
                    EditorPrefs.SetBool(RUNNING_SKETCH_KEY, false);
                    var sceneNameBeforeSketch = EditorPrefs.GetString(SCENE_BEFORE_SKETCH_KEY);
                    if (!string.IsNullOrEmpty(sceneNameBeforeSketch))
                    {
                        EditorSceneManager.OpenScene(sceneNameBeforeSketch);
                        EditorPrefs.SetString(SCENE_BEFORE_SKETCH_KEY, string.Empty);
                    }
                    EditorPrefs.SetString(SCENE_BEFORE_SKETCH_KEY, string.Empty);
                    EditorPrefs.SetString(SketchRunnerWindow.RUNNING_SKETCH_NAME, string.Empty);
                }
                else if (change == PlayModeStateChange.EnteredPlayMode)
                {
                    var fullTypeName = EditorPrefs.GetString(SKETCH_FULL_TYPE_NAME_KEY);
                    var asmName = EditorPrefs.GetString(SKETCH_ASSEMBLY_NAME_KEY);
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    var asm = assemblies.First(a => a.FullName == asmName);
                    var sketchType = asm.GetType(fullTypeName);
                    var sketchGo = new GameObject(sketchType.Name);
#if USE_FLUME
                    sketchGo.AddComponent<SketchServiceInstaller>();
#endif
                    var fixtureRunner = sketchGo.AddComponent<SketchFixtureRunner>();
                    fixtureRunner.RunSketchFixture(sketchType);
                }
            };
        }

        public static void RunSketch(Type sketchType)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            var currentScene = SceneManager.GetActiveScene();
            EditorPrefs.SetString(SCENE_BEFORE_SKETCH_KEY, currentScene.path);
            EditorPrefs.SetString(SKETCH_FULL_TYPE_NAME_KEY, sketchType.FullName);
            EditorPrefs.SetString(SKETCH_ASSEMBLY_NAME_KEY, sketchType.Assembly.FullName);

            var sketchScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
            sketchScene.name = "Sketch";
            SceneManager.SetActiveScene(sketchScene);

            EditorApplication.EnterPlaymode();
            EditorPrefs.SetBool(RUNNING_SKETCH_KEY, true);
        }
    }
}