using Actuator.Sketch;
using UnityEngine;

[SketchFixture]
[SketchDescription("Sketch using Serialized reference types")]
public class SerializedValueSketch : ScriptableObject
{
    // Sketch is intended to be code-first, so serialized fields should only be used
    // for reference types like prefabs that cannot be set up in code.
    
    // Serialized reference types will show and be settable from inspector in edit mode
    [SerializeField] private GameObject _objectPrefab;
    
    // Serialized value types will not show in inspector.
    [SerializeField] private int _rows = 5;
    
    private const int COLUMNS = 5;
    private const float SPACING = 3f;

    public void Start()
    {
        var camera = Camera.main;
        if (camera)
        {
            camera.transform.position = new Vector3((_rows - 1) * SPACING / 2, 10, -10);
            camera.transform.Rotate(Vector3.right, 45f);
        }
        
        for (var x = 0; x < _rows; x++)
        {
            for (var y = 0; y < COLUMNS; y++)
            {
                var model = Instantiate(_objectPrefab).transform;
                model.position = new Vector3(x, 0, y) * SPACING;
            }
        }
    }
}
