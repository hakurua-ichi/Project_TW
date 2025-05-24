using TMPro;
using UnityEngine;

[RequireComponent(typeof(Canvas))] // Ensure UI Canvas present if needed
public class InteractionsButtonAction : MonoBehaviour
{
    public static InteractionsButtonAction Instance { get; private set; }

    [SerializeField] private GameObject uiRoot;   // 실제 버튼 오브젝트 (Canvas 하위)
    private TextMeshProUGUI buttonText;
    private ProximityTriggerObject currentTrigger;

    void Awake()
    {
        // 싱글턴 설정
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // UI Root 확인 및 비활성화
        if (uiRoot == null)
        {
            Debug.LogError("[InteractionsButtonAction] uiRoot가 할당되지 않았습니다! UI 프리팹을 드래그하세요.", this);
            enabled = false;
            return;
        }

        buttonText = uiRoot.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText == null)
        {
            Debug.LogError("[InteractionsButtonAction] uiRoot 하위에 TextMeshProUGUI 컴포넌트가 없습니다.", this);
        }

        uiRoot.SetActive(false);
    }

    void Update()
    {
        // inRange 에 등록된 것들 중 가장 가까운 트리거를 꺼내 온다
        var closest = ProximityTriggerObject.GetClosestInRange();

        // 바뀐 게 없으면 아무 것도 하지 않는다
        if (closest == currentTrigger) return;

        currentTrigger = closest;

        if (currentTrigger != null)
        {
            // 버튼 텍스트 세팅
            buttonText.text = currentTrigger.ActionTarget.name;
            uiRoot.SetActive(true);
        }
        else
        {
            // 범위 밖으로 모두 벗어난 경우
            uiRoot.SetActive(false);
        }
    }

    /// <summary>
    /// ProximityTriggerObject가 자신을 선택해 달라고 요청할 때 호출됩니다.
    /// 가장 가까운 트리거만 표시하도록 currentTrigger를 업데이트합니다.
    /// </summary>
    public void RequestSelection(ProximityTriggerObject trigger, GameObject objectName)
    {
        Debug.Log("리퀘스트 실행");
        // 들어온 trigger는 무시하고, inRange 전체에서 closest만 추출
        var closest = ProximityTriggerObject.GetClosestInRange();
        if (closest == null)
        {
            uiRoot.SetActive(false);
            return;
        }

        // ▶ 새로 뽑은 closest가 달라졌을 때만 갱신
        if (closest != currentTrigger)
        {
            currentTrigger = closest;
            // 가장 가까운 트리거의 actionTarget 이름을 버튼에 표시
            buttonText.text = closest.ActionTarget != null
                ? closest.ActionTarget.name
                : "Interact";
        }

        uiRoot.SetActive(true);
        Debug.Log("리퀘스트 종료");
    }

    /// <summary>
    /// ProximityTriggerObject에서 범위를 벗어났다고 알릴 때 호출됩니다.
    /// 필요 시 자동으로 다음 가장 가까운 트리거로 갱신합니다.
    /// </summary>
    public void NotifyExit(ProximityTriggerObject trigger)
    {
        if (trigger == null || trigger != currentTrigger) return;
        currentTrigger = ProximityTriggerObject.GetClosestInRange();
        if (currentTrigger != null)
            buttonText.text = currentTrigger.ActionTarget?.name ?? "Interact";
        uiRoot.SetActive(currentTrigger != null);
    }

    /// <summary>
    /// 버튼 클릭 시 연결된 ProximityTriggerObject의 InvokeButton을 호출합니다.
    /// </summary>
    public void OnButtonClicked()
    {
        if (currentTrigger == null)
        {
            Debug.LogWarning("[InteractionsButtonAction] currentTrigger가 null입니다. 버튼 클릭을 처리할 대상이 없습니다.");
            return;
        }
        currentTrigger.InvokeButton();
    }
}
