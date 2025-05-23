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

    /// <summary>
    /// ProximityTriggerObject가 자신을 선택해 달라고 요청할 때 호출됩니다.
    /// 가장 가까운 트리거만 표시하도록 currentTrigger를 업데이트합니다.
    /// </summary>
    public void RequestSelection(ProximityTriggerObject trigger, GameObject objectName)
    {
        Debug.Log("리퀘스트 실행");
        if (trigger == null) return;

        // ① 플레이어 Transform 가져오기
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null) return;
        var playerT = playerObj.transform;

        // ② trigger와 player 간 거리 계산
        float dist = Vector3.Distance(trigger.transform.position, playerT.position);

        // ③ 기존 로직 대신 dist 비교
        float currentDist = currentTrigger != null
            ? Vector3.Distance(currentTrigger.transform.position, playerT.position)
            : float.MaxValue;

        if (dist < currentDist)
        {
            currentTrigger = trigger;
            buttonText.text = objectName != null ? objectName.name : "Interact";
            uiRoot.SetActive(true);
        }
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


//using TMPro;
//using UnityEngine;

//public class InteractionsButtonAction : MonoBehaviour
//{
//    public static InteractionsButtonAction Instance { get; private set; }

//    [SerializeField] private GameObject uiRoot;   // 실제 버튼 오브젝트
//    private ProximityTriggerObject currentTrigger;
//    private TextMeshProUGUI buttonText;

//    void Awake()
//    {
//        buttonText = uiRoot.GetComponentInChildren<TextMeshProUGUI>();
//        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
//        Instance = this;
//        uiRoot.SetActive(false);

//    }

//    // 트리거가 ‘나를 선택해 달라’고 호출
//    public void RequestSelection(ProximityTriggerObject trigger, GameObject objectName)
//    {
//        // 더 가까우면 교체
//        if (currentTrigger == null ||
//            trigger.DistanceToPlayer < currentTrigger.DistanceToPlayer)
//        {
//            currentTrigger = trigger;
//            buttonText.text = objectName.name; // 버튼 텍스트 변경
//            uiRoot.SetActive(true);
//        }
//    }

//    // 트리거가 범위를 벗어났다고 알림
//    public void NotifyExit(ProximityTriggerObject trigger)
//    {
//        if (trigger == currentTrigger)
//        {
//            currentTrigger = ProximityTriggerObject.GetClosestInRange();
//            uiRoot.SetActive(currentTrigger != null);
//        }
//    }

//    // 버튼 클릭 시 호출
//    public void OnButtonClicked()
//    {
//        Debug.Log("Button Clicked");
//        currentTrigger.InvokeButton();   // Subject.NotifyExit() 내부에서 실행
//    }
//}
