using UnityEngine;

public class DoorGimmick : MonoBehaviour, IGimmickObserver
{
    [SerializeField] private GimmickSubject TriggerObject;

    public GameObject doorObject;
    private GimmickContext context;
    private bool doorState = false;

    void Start()
    {
        context = new GimmickContext();
        context.SetAction(new OpenDoorAction(doorObject, GameObject.FindGameObjectWithTag("Player").transform));
    }

    public void OnGimmickTriggered()
    {
        //gimmickContext.StartAction();
    }

    public void ButtonClick()
    {
        if (!doorState)
        {
            context.StartAction();
            doorState = true;
        }
        else
        {
            context.CancelAction();
            doorState = false;
        }
    }
}
