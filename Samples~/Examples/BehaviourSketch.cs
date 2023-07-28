using Actuator.Sketch;
using UnityEngine;

[SketchFixture]
[SketchDescription("Example for viewing created behaviour")]
public class BehaviourSketch : ScriptableObject
{
    private Transform _transform;
    private IPositionController _positionController;

    public void Start()
    {
        _transform = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
        _positionController = new SinePositionController();
    }

    public void Update()
    {
        _positionController.Tick(Time.deltaTime);
        _transform.position = _positionController.GetPosition();
    }
}
