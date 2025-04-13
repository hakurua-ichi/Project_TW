using UnityEngine;

public class OpenDoorAction : IGimmickAction
{
    private GameObject doorAnimator;

    public OpenDoorAction(GameObject animator)
    {
        doorAnimator = animator;
    }

    public void Action()
    {
        Debug.Log("¹® ¿­¸²");
        doorAnimator.transform.localRotation = Quaternion.Euler(0, -90, 0);
    }

    public void Execute()
    {
        Debug.Log("¹® ´ÝÈû");
        doorAnimator.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }
}
