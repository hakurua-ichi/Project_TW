public interface ICameraSystem
{
    bool IsRotating { get; }
    event System.Action<float> RotationCompleted;
}