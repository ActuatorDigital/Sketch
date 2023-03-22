using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AIR.Sketch
{
    public class SketchFixtureRunner : MonoBehaviour
    {
        private Component _monobeh;
        private ScriptableObject _so;

#if UNITY_EDITOR
        public void Awake()
        {
            UnityEditor.EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
        }

        private void EditorApplication_playModeStateChanged(UnityEditor.PlayModeStateChange obj)
        {
            if (obj == UnityEditor.PlayModeStateChange.ExitingPlayMode)
            {
                DestroyImmediate(gameObject);
                UnityEditor.EditorApplication.playModeStateChanged -= EditorApplication_playModeStateChanged;
            }
        }
#endif

        public void RunSketchFixture(Type type)
        {
            if (type.IsSubclassOf(typeof(MonoBehaviour)))
            {
                _monobeh = gameObject.AddComponent(type);
            }
            else if (type.IsSubclassOf(typeof(ScriptableObject)))
            {
                _so = ScriptableObject.CreateInstance(type);
            }
            else
            {
                Debug.LogError($"Unhandled sketch fixture type '{type}'.");
            }
        }

        public void OnDestroy()
        {
            AttemptToInvokeMethod(_so);
            
            if (_monobeh != null)
                Destroy(_monobeh);
            if (_so != null)
                Destroy(_so);
        }

        public void Start() => AttemptToInvokeMethod(_so);
        public void Update() => AttemptToInvokeMethod(_so);
        public void FixedUpdate() => AttemptToInvokeMethod(_so);
        public void LateUpdate() => AttemptToInvokeMethod(_so);
        public void OnGUI() => AttemptToInvokeMethod(_so);
        public void OnDrawGizmos() => AttemptToInvokeMethod(_so);
        public void OnDrawGizmosSelected() => AttemptToInvokeMethod(_so);

        private void AttemptToInvokeMethod(object obj, [CallerMemberName] string callerName = "")
        {
            // No, it cannot be simplified as UnityEngine.Object is a comparison overload.
            if (obj != null)
            {
                obj.GetType().GetMethod(callerName)?.Invoke(obj, null);
            }
        }
    }
}