using UnityEngine;

public class DoorGimmick : MonoBehaviour, IGimmickObserver
{
    [SerializeField] private string requiredItemId = "Key";   // ★ 필요 아이템 ID
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

    //public void ButtonClick()
    //{
    //    Debug.Log("DoorGimmick 실행");
    //    if (!doorState)
    //    {
    //        context.StartAction();
    //        doorState = true;
    //    }
    //    else
    //    {
    //        context.CancelAction();
    //        doorState = false;
    //    }
    //}

    public void ButtonClick()
    {
        if (!InventoryManager.Instance.HasItem(requiredItemId))
        {
            Debug.Log("열쇠가 없어 문이 열리지 않습니다.");
            return;
        }

        // 열쇠가 있을 때만 기존 로직 실행
        if (!doorState)
        {
            context.StartAction();   // 문 열기
            doorState = true;

            // 열쇠를 1회용으로 사용할 경우:
            // InventoryManager.Instance.ClearSlot();
        }
        else
        {
            context.CancelAction();  // 문 닫기
            doorState = false;
        }
    }
}
