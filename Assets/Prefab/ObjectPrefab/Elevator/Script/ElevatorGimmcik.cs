using UnityEngine;

[RequireComponent(typeof(GimmickSubject))]
public class ElevatorGimmcik : MonoBehaviour, IGimmickObserver
{
    [SerializeField] private GimmickSubject triggerObject; // 트리거 오브젝트

    public GameObject elevatorObject;
    private GimmickContext gimmickContext;

    private void Start()
    {
        gimmickContext = new GimmickContext();
        gimmickContext.SetAction(new ElevatorAction(elevatorObject));

        if (elevatorObject != null)
        {
            Debug.Log("Elevator 옵저버 등록 성공");
            triggerObject.AddObserverEnter(this); // 엘리베이터 작동
            triggerObject.AddObserverExit(new ExitObserver(gimmickContext)); // 엘리베이터 정지
        }
        else
        {
            Debug.LogWarning("Elevator 오브젝트가 없습니다.");
        }
    }

    public void OnGimmickTriggered()
    {
        gimmickContext.StartAction();
    }

    public void ButtonClick()
    {

    }

    // 내부 클래스: 엘리베이터 정지 전용 옵저버
    private class ExitObserver : IGimmickObserver
    {
        private GimmickContext context;
        public ExitObserver(GimmickContext ctx)
        {
            context = ctx;
        }
        public void OnGimmickTriggered()
        {
            context.CancelAction(); // 엘리베이터 정지
        }

        public void ButtonClick()
        {

        }
    }
}