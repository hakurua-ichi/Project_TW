using UnityEngine;

public class OpenDoorAction : IGimmickAction
{
    private DoorAnimator doorAnimator;
    private DoorState doorState;

    public OpenDoorAction(GameObject doorObject, Transform player)
    {
        doorAnimator = doorObject.GetComponent<DoorAnimator>();
        doorState = new DoorState(player, doorObject.transform);
    }

    public void Action()
    {
        if (!doorState.IsOpen)
        {
            Debug.Log("문을 엽니다.");
            doorState.IsOpen = true;
            doorAnimator.SetState(doorState);
        }
        else
        {
            Debug.Log("문이 이미 열려있습니다.");
        }
    }

    public void Exit()
    {
        if (doorState.IsOpen)
        {
            Debug.Log("문을 닫습니다.");
            doorState.IsOpen = false;
            doorAnimator.SetState(doorState);
        }
        else
        {
            Debug.Log("문이 이미 닫혀있습니다.");
        }
    }

    public void Setup()
    {
        doorState.IsOpen = false;
    }
}