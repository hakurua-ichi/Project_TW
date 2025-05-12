using UnityEngine;

public class RotationPhysicsHandler
{
    private Rigidbody targetRigidbody;
    private bool wasRotatingLastFrame;
    
    public RotationPhysicsHandler(Rigidbody rigidbody)
    {
        targetRigidbody = rigidbody;
        wasRotatingLastFrame = false;
    }
    
    public void HandlePhysicsForRotation(bool isRotating)
    {
        if (targetRigidbody == null) return;
        
        // 회전 시작 시
        if (isRotating && !wasRotatingLastFrame)
        {
            targetRigidbody.isKinematic = true;
        }
        
        // 회전 종료 시
        if (!isRotating && wasRotatingLastFrame)
        {
            targetRigidbody.isKinematic = false;
        }
        
        wasRotatingLastFrame = isRotating;
    }
    
    public bool GetLastRotationState()
    {
        return wasRotatingLastFrame;
    }
}