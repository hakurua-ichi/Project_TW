using UnityEngine;

// LaddersGimmick과 유사한 RequireComponent 설정
[RequireComponent(typeof(GimmickSubject), typeof(BoxCollider))] // Rigidbody는 텍스트 표시에 필수 아님
[RequireComponent(typeof(ProximityTriggerObject))]
public class TextDisplayGimmick : MonoBehaviour, IGimmickObserver
{
    private GimmickContext context;
    private TextDisplayState textDisplayState;
    // private bool isBusy = false; // 페이드 없으므로 필요 없을 수 있음

    [Header("표시할 텍스트 설정")]
    [Tooltip("상호작용 시 화면에 표시될 메시지입니다.")]
    [SerializeField] private string messageToShow = "여기에 메시지를 입력하세요.";

    [Header("상태 표시 UI 설정")]
    [SerializeField] private StateText stateTextUI; // 기존 StateText 스크립트 참조

    // ScreenFader는 제거 (텍스트 표시에 페이드 불필요)
    // [Header("스크린 페이드 인 아웃 설정")]
    // [SerializeField] private ScreenFader screenFader;

    void Awake() // Start 대신 Awake 사용 권장 (ProximityTriggerObject 등과의 초기화 순서 고려)
    {
        if (stateTextUI == null)
        {
            Debug.LogError("StateText UI가 할당되지 않았습니다! Inspector에서 연결해주세요.", this);
            enabled = false; // 필수 컴포넌트 없으면 비활성화
            return;
        }

        textDisplayState = new TextDisplayState();
        textDisplayState.SetMessage(messageToShow); // Inspector에서 설정한 메시지로 초기화

        context = new GimmickContext();
        // TextDisplayAction 생성 시 TextDisplayState와 StateText UI를 전달
        context.SetAction(new TextDisplayAction(textDisplayState, stateTextUI));

        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            boxCollider.isTrigger = true; // 상호작용 범위 감지용
        }
        else
        {
            Debug.LogError("BoxCollider가 없습니다. ProximityTriggerObject가 제대로 작동하지 않을 수 있습니다.", this);
        }
    }

    public void OnGimmickEnter()
    {
        // 이 기믹에서는 사용하지 않음
        // Debug.Log("TextDisplayGimmick: OnGimmickEnter");
    }

    public void OnGimmickLeave()
    {
        // 만약 범위를 벗어나면 텍스트를 숨김김
        if (textDisplayState != null && textDisplayState.IsActive && stateTextUI != null)
        {
            stateTextUI.UnVisible();
            textDisplayState.IsActive = false;
            Debug.Log("범위 이탈, 텍스트 숨김.");
        }
    }

    public void ButtonClick() // ProximityTriggerObject의 버튼 클릭 시 호출
    {
        if (context == null)
        {
            Debug.LogError("GimmickContext가 초기화되지 않았습니다.", this);
            return;
        }

        // 페이드 없이 바로 액션 실행
        context.StartAction();
    }
}