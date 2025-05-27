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
        if (!doorState)
        {
            context.StartAction();    // 문 열기 애니메이션 트리거
            doorState = true;
        }
    }

    public void OnGimmickLeave()
    {
        if (doorState)
        {
            context.CancelAction();   // 문 닫기 애니메이션 트리거
            doorState = false;
        }
    }

    public void ButtonClick()
    {
        Debug.Log("DoorGimmick 실행");
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
