using UnityEngine;

public class ElevatorController : MonoBehaviour, IGimmickObserver
{
    private ElevatorPlatformMover mover;

    void Awake()
    {
        mover = GetComponent<ElevatorPlatformMover>();
        Debug.Log("controller awake start");
    }

    public void OnGimmickTriggered()
    {
        if (mover == null)
        {
            Debug.Log("mover is null");
            return;
        }

        if (mover.IsMoving())
        {
            Debug.Log("Elevator already moving, skipping trigger");
            return;
        }

        mover.StartElevator();
        Debug.Log("startElevator start");
    }
}