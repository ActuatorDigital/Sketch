using Actuator.Sketch;
using UnityEngine;

[SketchFixture]
[SketchDescription("Method Invokes such as Update for Editor-only sketch")]
public class ScriptableObjectInvokeSketch : ScriptableObject
{
    private Transform _transform;
    
    public void Start()
    {
        _transform = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
    }

    public void Update()
    {
        _transform.Rotate(Vector3.up, 90 * Time.deltaTime);
    }

    public void OnDrawGizmos()
    {
        var position = _transform.position;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(position, position + _transform.forward * 5);
    }

    public void OnDestroy()
    {
        if (_transform != null)
            Destroy(_transform.gameObject);
    }
}
