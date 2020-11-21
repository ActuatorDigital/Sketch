using System;
using System.Collections;
using System.Collections.Generic;
using AIR.Sketch;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    [AIR.Sketch.SketchFixtureAttribute]
    [SketchDescription("The following is the first desc.")]
    public class NewTestSketch : MonoBehaviour
    {
        private IEnumerator Start()
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            while (true) {
                yield return new WaitForEndOfFrame();
                cube.transform.position = Mathf.Sin(Time.time) * Vector3.up;
            }
        }
    }
}
