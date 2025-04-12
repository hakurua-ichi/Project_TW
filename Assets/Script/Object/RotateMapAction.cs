using UnityEngine;

public class RotateMapAction : IGimmickAction
{
    private Transform mapTransform;
    private Vector3 rotationAngle;

    public RotateMapAction(Transform mapTransform, Vector3 rotationAngle)
    {
        this.mapTransform = mapTransform;
        this.rotationAngle = rotationAngle;
    }

    public void Action()
    {
        mapTransform.Rotate(rotationAngle);
    }

    public void Execute()
    {
        // This method can be used to execute additional logic if needed
        // For now, it does nothing
    }
}