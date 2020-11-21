using System.Collections;
using System.Collections.Generic;
using AIR.Sketch;
using UnityEngine;

[SketchFixture]
[SketchDescription("This is the other desc.")]
public class NewBehaviourScript : MonoBehaviour
{
    IEnumerator Start()
    {
        var c = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        while (true) {
            c.transform.position = Vector3.right * Mathf.Sin(Time.time);
            yield return new WaitForEndOfFrame();
        }
    }

}
