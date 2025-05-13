using UnityEngine;
using TMPro;

[RequireComponent(typeof(GimmickSubject))]
public class ProximityTriggerObject : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 3f;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject button_UI;
    [SerializeField] private GameObject actionObject; // 액션 오브젝트
    private TextMeshProUGUI buttonText; // 버튼 텍스트
    private string actionObjectName;
    private bool isPlayerInRange = false;

    private void Awake()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }
    }

    private void Start()
    {
        button_UI.SetActive(false);
        buttonText = button_UI.GetComponentInChildren<TextMeshProUGUI>();
        actionObjectName = actionObject.name;
    }

    private void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= detectionRadius && !isPlayerInRange)
        {
            isPlayerInRange = true;
            buttonText.text = actionObjectName; // 버튼 텍스트 설정
            button_UI.SetActive(isPlayerInRange);

            // 버튼 로직에 현재 트리거 등록
            var buttonLogic = Object.FindFirstObjectByType<InteractionsButtonAction>();

            if (buttonLogic != null)
                buttonLogic.SetCurrentTriggerObject(this);
        }
        else if (distance > detectionRadius && isPlayerInRange)
        {
            isPlayerInRange = false;
            button_UI.SetActive(isPlayerInRange);
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
