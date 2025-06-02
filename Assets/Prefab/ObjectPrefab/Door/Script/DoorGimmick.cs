using UnityEngine;

public class DoorGimmick : MonoBehaviour, IGimmickObserver
{
    [Header("모드 선택")]
    [SerializeField] private bool KeyMode = false; // 열쇠 모드 여부
    [SerializeField] private bool PasswordMode = false; //비밀번호 모드 여부

    [Header("키 설정")]
    [SerializeField] private string requiredItemId = "Key";   // 필요 아이템 ID
    [Tooltip("열쇠를 한번 사용하면 사라지게 할 것인가? true => 열쇠 사용 시 사라짐")]
    [SerializeField] private bool KeyDelete = false;

    [Header("비밀번호 모드 관련 설정")]
    [Tooltip("씬에 1개만 존재해야 하는 PasswordData 컴포넌트를 지정하세요.")]
    [SerializeField] private PasswordData passwordData;      // Inspector에서 연결
    [SerializeField] private StateText passwordPromptText;   // 비밀번호 모드일 때 상태 메시지(예: '비밀번호를 입력하세요')
    [SerializeField] private string correctPasswordMessage;
    [SerializeField] private string incorrectPasswordMessage;

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


        // ▶ 비밀번호 모드용 이벤트 구독
        if (PasswordMode)
        {
            if (passwordData == null)
            {
                Debug.LogError("DoorGimmick: Inspector에서 PasswordData를 반드시 연결해야 합니다. (비밀번호 모드)");
                enabled = false;
                return;
            }
            // PasswordData에서 “정답 일치” 시 호출될 콜백 등록
            passwordData.OnPasswordMatched += OnPasswordCorrect;
            // 비밀번호가 틀렸을 때 (맞았다가 다시 틀린 경우)
            passwordData.OnPasswordUnmatched += OnPasswordIncorrect;
        }
    }

    private void OnDestroy()
    {
        // ▶ 이벤트 해제 (메모리 누수 방지)
        if (PasswordMode && passwordData != null)
        {
            passwordData.OnPasswordMatched -= OnPasswordCorrect;
            passwordData.OnPasswordUnmatched -= OnPasswordIncorrect;
        }
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
        if (KeyMode && PasswordMode)
        {
            Debug.Log("하나의 모드를 선택해주세요");
            return;
        }

        //열쇠 모드
        if(KeyMode)
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
                if(KeyDelete) InventoryManager.Instance.RemoveCurrentItem();
            }
            else
            {
                context.CancelAction();  // 문 닫기
            }
        }

        //열쇠 없는 버튼 모드
        else
        {
            if (doorState.IsOpen)
            {
                context.CancelAction();  // 문 닫기
            }
            else
            {
                context.StartAction();    // 문 열기
            }
        }
    }

    /// <summary>
    /// PasswordData에서 “정답 일치”를 알릴 때 호출되는 콜백
    /// → 실제로 문을 여는 로직 실행
    /// </summary>
    private void OnPasswordCorrect()
    {
        // 비밀번호 모드일 때만 동작
        if (!PasswordMode) return;

        // (Optional) “정답을 맞췄습니다” 메시지 띄우기
        if (stateText != null)
        {
            stateText.SetText(true, correctPasswordMessage);
        }

        // OpenDoorAction.Action()으로 실제 애니메이션을 통해 문 열기
        if (!doorState.IsOpen)
        {
            context.StartAction();
            doorState.IsOpen = true;
        }
    }

    /// <summary>
    /// PasswordData에서 “맞았다가 다시 틀린 경우”를 알릴 때 호출되는 콜백
    /// → 문이 열려 있으면 닫는 로직 실행
    /// </summary>
    private void OnPasswordIncorrect()
    {
        if (!PasswordMode) return;

        // (Optional) “비밀번호가 틀렸습니다. 문이 닫힙니다.” 메시지 띄우기
        if (stateText != null)
        {
            stateText.SetText(true, incorrectPasswordMessage);
        }

        // 문이 열려 있다면 닫고 상태 갱신
        if (doorState.IsOpen)
        {
            context.CancelAction();
            doorState.IsOpen = false;
        }
    }
}
