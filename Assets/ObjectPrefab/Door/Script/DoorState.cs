using UnityEngine;

public class DoorState
{
    public bool IsOpen { get; set; }
    public Transform Player { get; private set; }
    public Transform DoorTransform { get; private set; }

    public DoorState(Transform player, Transform doorTransform)
    {
        Player = player;
        DoorTransform = doorTransform;
        IsOpen = false;
    }
}