using UnityEngine;

public class DoorGimmick : MonoBehaviour, IGimmickObserver
{
    [SerializeField] private GimmickSubject TriggerObject;

    public GameObject doorObject;
    private GimmickContext gimmickContext;

    void Start()
    {
        gimmickContext = new GimmickContext();
        gimmickContext.SetAction(new OpenDoorAction(doorObject, GameObject.FindGameObjectWithTag("Player").transform));

        // 옵저버 등록
        if (TriggerObject != null)
        {
            Debug.Log("Door 옵저버 등록 성공");
            TriggerObject.AddObserverEnter(this); // 불 켜기
            TriggerObject.AddObserverExit(new ExitObserver(gimmickContext)); // 불 끄기
        }
        else
        {
            Debug.LogWarning("GimmickSubject가 Door 오브젝트에 없습니다.");
        }
    }

    public void OnGimmickTriggered()
    {
        gimmickContext.StartAction();
    }

    public void ButtonClick()
    {

    }

    // 내부 클래스: Light 끄기 전용 옵저버
    private class ExitObserver : IGimmickObserver
    {
        private GimmickContext context;

        public ExitObserver(GimmickContext ctx)
        {
            context = ctx;
        }

        public void OnGimmickTriggered()
        {
            context.CancelAction(); // 문 닫기
        }

        public void ButtonClick()
        {

        }
    }
}
