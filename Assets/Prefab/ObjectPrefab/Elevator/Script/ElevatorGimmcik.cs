using UnityEngine;

[RequireComponent(typeof(GimmickSubject))]
public class ElevatorGimmcik : MonoBehaviour, IGimmickObserver
{
    [SerializeField] private GimmickSubject triggerObject; // 트리거 오브젝트

    public GameObject elevatorObject;
    private GimmickContext gimmickContext;
    private bool isPlayerOnElevator = false;

    private void Start()
    {
        gimmickContext = new GimmickContext();
        gimmickContext.SetAction(new ElevatorAction(elevatorObject));

        if (elevatorObject != null)
        {
            //Debug.Log("Elevator 옵저버 등록 성공");
            //triggerObject.AddObserverEnter(this); // 엘리베이터 작동
        }
        else
        {
            Debug.LogWarning("Elevator 오브젝트가 없습니다.");
        }
    }

    public void OnGimmickEnter()
    {
        if (!isPlayerOnElevator)
        {
            Debug.Log("엘리베이터에 탑승");
            isPlayerOnElevator = true;
            gimmickContext.StartAction();
        }
        else
        {
            Debug.Log("엘리베이터에서 하차");
            isPlayerOnElevator = false;
            gimmickContext.CancelAction();
        }
    }

    public void OnGimmickLeave()
    {

    }

    public void ButtonClick()
    {

    }
}