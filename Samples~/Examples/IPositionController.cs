using UnityEngine;

public interface IPositionController
{
    void Tick(float deltaTime);
    Vector3 GetPosition();
}

public class SinePositionController : IPositionController
{
    private float _elapsedTime = 0;
    
    public void Tick(float deltaTime)
    {
        _elapsedTime += deltaTime;
    }

    public Vector3 GetPosition()
    {
        var sineValue = Mathf.Sin(_elapsedTime);
        return Vector3.up * sineValue * 2; 
    }
}