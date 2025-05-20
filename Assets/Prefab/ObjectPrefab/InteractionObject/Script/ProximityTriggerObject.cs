using UnityEngine;
using TMPro;

[RequireComponent(typeof(GimmickSubject))]
public class ProximityTriggerObject : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 3f;
    private Transform playerTransform;
    [SerializeField] private GameObject button_UI;
    [SerializeField] private GameObject actionObject; // 실제 동작할 기믹 오브젝트

    private TextMeshProUGUI buttonText;
    private string actionObjectName;
    private bool isPlayerInRange = false;

    private GimmickSubject subject;           // 옵저버 등록/해제용
    private IGimmickObserver observer;        // actionObject의 옵저버 인터페이스

    private void Awake()
    {
        // 플레이어 트랜스폼 캐싱
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;

        // GimmickSubject와 Observer 캐싱
        subject = GetComponent<GimmickSubject>();
        if (actionObject != null)
            observer = actionObject.GetComponent<IGimmickObserver>();
    }

    private void Start()
    {
        if (button_UI != null)
        {
            button_UI.SetActive(false);
            buttonText = button_UI.GetComponentInChildren<TextMeshProUGUI>();
        }

        if (actionObject != null)
            actionObjectName = actionObject.name;

        // 플레이어 재탐색 (혹시 Awake에서 못 찾았으면)
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }
    }

    private void Update()
    {
        if (playerTransform == null || subject == null || observer == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= detectionRadius && !isPlayerInRange)
        {
            isPlayerInRange = true;

            // UI 버튼 표시 및 텍스트 설정
            if (button_UI != null && buttonText != null)
            {
                buttonText.text = actionObjectName;
                button_UI.SetActive(true);
            }

            // 현재 트리거를 버튼 로직에 등록
            var buttonLogic = Object.FindFirstObjectByType<InteractionsButtonAction>();
            if (buttonLogic != null)
                buttonLogic.SetCurrentTriggerObject(this);

            // 옵저버 등록
            subject.AddObserverEnter(observer);
        }
        else if (distance > detectionRadius && isPlayerInRange)
        {
            isPlayerInRange = false;

            // UI 버튼 숨기기
            if (button_UI != null)
                button_UI.SetActive(false);

            // 옵저버 해제
            subject.RemoveObserverEnter(observer);
        }
    }

    public string GetActionObjectName()
    {
        return actionObjectName;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}