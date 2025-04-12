using UnityEngine;

public class DoorGimmick : MonoBehaviour, IGimmickObserver
{
    public Animator doorAnimator;
    private GimmickContext gimmickContext;

    void Start()
    {
        gimmickContext = new GimmickContext();
        gimmickContext.SetAction(new OpenDoorAction(doorAnimator));
    }

    public void OnGimmickTriggered()
    {
        gimmickContext.ExecuteAction();
    }
}
