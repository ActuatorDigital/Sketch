using System.Collections;
using AIR.Flume;
using AIR.Sketch;
using UnityEngine;

namespace Tests
{

    public class ExampleService : IExampleService
    {
        public float GetTime() => Time.time;
    }

    public interface IExampleService
    {
        float GetTime();
    }

    [SketchFixture]
    [SketchDescription("The following is the first desc.")]
    [SketchDependsOn(typeof(IExampleService), typeof(ExampleService))]
    public class NewTestSketch : DependentBehaviour
    {
        private IExampleService _exampleService;
        public void Inject(IExampleService exampleService)
        {
            UnityEngine.Debug.Log("NewTestSketch.Inject");
            _exampleService = exampleService;
        }

        private IEnumerator Start()
        {
            UnityEngine.Debug.Log("NewTestSketch.Start");
            
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            while (true) {
                yield return new WaitForEndOfFrame();
                cube.transform.position = Mathf.Sin(_exampleService.GetTime()) * Vector3.up;
            }
        }
    }
}
