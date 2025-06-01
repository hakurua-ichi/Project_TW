using UnityEngine;

public class DoorGimmick : MonoBehaviour, IGimmickObserver
{
    [SerializeField] private bool isKeyMode = false; // 열쇠 모드 여부
    [SerializeField] private string requiredItemId = "Key";   // 필요 아이템 ID
    [SerializeField] private GimmickSubject TriggerObject;
    private GameObject doorObject;
    private GimmickContext context;
    private GameObject player;
    private DoorState doorState; // 문 상태 관리

    [Header("상태 표시 텍스트 설정")]
    [SerializeField] private StateText stateText; // 상태 표시용 스크립트
    [SerializeField] private string stateTextMessage; // 상태 표시 메시지

    void Start()
    {
        doorObject = gameObject; // 현재 스크립트가 붙은 게임 오브젝트를 문 오브젝트로 설정
        player = GameObject.FindGameObjectWithTag("Player");
        doorState = new DoorState(player.transform, doorObject.transform);
        context = new GimmickContext();
        context.SetAction(new OpenDoorAction(doorObject, doorState, player.transform));
    }

    public void OnGimmickEnter()
    {
        if (!doorState.IsOpen)
        {
            context.StartAction();    // 문 열기 애니메이션 트리거
            doorState.IsOpen = true;
        }
    }

    public void OnGimmickLeave()
    {
        if (doorState.IsOpen)
        {
            context.CancelAction();   // 문 닫기 애니메이션 트리거
            doorState.IsOpen = false;
        }
    }

    // 열쇠가 있는지 확인하고 문을 여는 버튼 클릭 이벤트
    public void ButtonClick()
    {
        if(isKeyMode)
        {
            if (!InventoryManager.Instance.HasItem(requiredItemId))
            {
                stateText.SetText(true, stateTextMessage); // 상태 표시 메시지 출력
                return;
            }

            // 열쇠가 있을 때만 기존 로직 실행
            if (!doorState.IsOpen)
            {
                context.StartAction();   // 문 열기

                // 열쇠를 1회용으로 사용할 경우:
                //InventoryManager.Instance.RemoveCurrentItem();
            }
            else
            {
                context.CancelAction();  // 문 닫기
            }
        }

        else
        {
            if(doorState.IsOpen)
            {
                context.CancelAction();  // 문 닫기
            }
            else
            {
                context.StartAction();    // 문 열기
            }
        }
    }
}
