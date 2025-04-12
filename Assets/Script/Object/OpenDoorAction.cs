using UnityEngine;

public class OpenDoorAction : IGimmickAction
{
    private Animator doorAnimator;

    public OpenDoorAction(Animator animator)
    {
        doorAnimator = animator;
    }

    public void Action()
    {
        doorAnimator.SetTrigger("Open");
    }

    public void Execute()
    {
        // This method can be used to execute additional logic if needed
        // For now, it does nothing
    }
}
