using UnityEngine;

public class ElevatorState
{
    public bool IsMoving { get; set; }
    public ElevatorState()
    {
        this.IsMoving = false;
    }
}