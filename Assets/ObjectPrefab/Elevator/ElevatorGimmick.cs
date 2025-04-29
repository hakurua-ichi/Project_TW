using UnityEngine;

public class ElevatorGimmick : MonoBehaviour, IGimmickObserver
{
    [SerializeField] private GimmickSubject TriggerObject;
    [SerializeField] private float FloorHeight = 2f;

    public GameObject actionObject;
    private GimmickContext gimmickContext;

    private bool isAction = false;

    void Start()
    {
        //전략 세팅
        gimmickContext = new GimmickContext();
        gimmickContext.SetAction(new ElevatorAction(actionObject, FloorHeight));

        // 옵저버 등록
        if (TriggerObject != null)
        {
            TriggerObject.AddObserverEnter(this); // 불 켜기
            TriggerObject.AddObserverExit(new ExitObserver(gimmickContext)); // 불 끄기
            Debug.Log("Elevator 옵저버 등록 성공");
        }
        else
        {
            Debug.LogWarning("GimmickSubject가 Elevator 오브젝트에 없습니다.");
        }
    }

    public void OnGimmickTriggered()
    {
        if (!isAction)
        {
            gimmickContext.ExecuteAction();
            isAction = true;
            Debug.Log("트리거 비활성화 완료");
        }
        else Debug.Log("isAction is true");
    }

    public void ResetisAction()
    {
        isAction = false;
    }

    // 내부 클래스: Elevator 트리거존 벗어났을 경우 전용 옵저버
    private class ExitObserver : IGimmickObserver
    {
        private GimmickContext context;

        public ExitObserver(GimmickContext ctx)
        {
            context = ctx;
        }
        public void OnGimmickTriggered()
        {
            context.CancelAction();
        }
    }
}
