using TMPro;
using UnityEngine;

public class InteractionsButtonAction : MonoBehaviour
{
    public static InteractionsButtonAction Instance { get; private set; }

    [SerializeField] private GameObject uiRoot;   // 실제 버튼 오브젝트
    private ProximityTriggerObject currentTrigger;
    private TextMeshProUGUI buttonText;

    void Awake()
    {
        buttonText = uiRoot.GetComponentInChildren<TextMeshProUGUI>();
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        uiRoot.SetActive(false);

    }

    // 트리거가 ‘나를 선택해 달라’고 호출
    public void RequestSelection(ProximityTriggerObject trigger, GameObject objectName)
    {
        // 더 가까우면 교체
        if (currentTrigger == null ||
            trigger.DistanceToPlayer < currentTrigger.DistanceToPlayer)
        {
            currentTrigger = trigger;
            buttonText.text = objectName.name; // 버튼 텍스트 변경
            uiRoot.SetActive(true);
        }
    }

    // 트리거가 범위를 벗어났다고 알림
    public void NotifyExit(ProximityTriggerObject trigger)
    {
        if (trigger == currentTrigger)
        {
            currentTrigger = ProximityTriggerObject.GetClosestInRange();
            uiRoot.SetActive(currentTrigger != null);
        }
    }

    // 버튼 클릭 시 호출
    public void OnButtonClicked()
    {
        Debug.Log("Button Clicked");
        currentTrigger.InvokeButton();   // Subject.NotifyExit() 내부에서 실행
    }
}
