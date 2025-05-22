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

    public void OnGimmickEnter()
    {
        //gimmickContext.StartAction();
    }

    public void OnGimmickLeave()
    {

    }

    public void ButtonClick()
    {
        Debug.Log("DoorGimmick ½ÇÇà");
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
