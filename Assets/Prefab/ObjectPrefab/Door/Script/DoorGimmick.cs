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

        // 옵저버 등록
        if (TriggerObject != null)
        {
            Debug.Log("Door 옵저버 등록 성공");
            TriggerObject.AddObserverEnter(this); // 문 옵저버 등록
        }
        else
        {
            Debug.LogWarning("GimmickSubject가 Door 오브젝트에 없습니다.");
        }
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
