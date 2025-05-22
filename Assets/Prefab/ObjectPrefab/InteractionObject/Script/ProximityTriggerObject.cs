using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GimmickSubject))]
public class ProximityTriggerObject : MonoBehaviour
{
    [SerializeField] private float radius = 2f;
    [SerializeField] private Transform player;

    [Header("Observer (IGimmickObserver 구현 컴포넌트)")]
    [SerializeField] private GameObject actionTarget;      // 실제 기믹 오브젝트
    [SerializeField] private MonoBehaviour observerComponent; // 드래그 전용 슬롯

    private IGimmickObserver observer;
    private GimmickSubject subject;

    /* 플레이어가 반경 안에 들어온 트리거를 관리 */
    private static readonly List<ProximityTriggerObject> inRange = new();

    public float DistanceToPlayer =>
        player ? Vector3.Distance(transform.position, player.position) : float.MaxValue;

    /* ────────── 초기화 ────────── */
    void Awake()
    {
        subject = GetComponent<GimmickSubject>();
        observer = observerComponent as IGimmickObserver;

        /* 1) actionTarget 안에서 찾기 */
        if (observer == null && actionTarget != null)
            observer = actionTarget.GetComponent<IGimmickObserver>() ??
                       actionTarget.GetComponentInChildren<IGimmickObserver>(true);

        /* 2) 자기 자신 쪽에서 찾기 */
        if (observer == null)
            observer = GetComponent<IGimmickObserver>() ??
                       GetComponentInChildren<IGimmickObserver>(true);

        if (observer == null)
            Debug.LogError($"[{name}] IGimmickObserver 구현체를 찾지 못했습니다!", this);

        /* 플레이어 참조가 없으면 태그로 보강 */
        if (player == null && GameObject.FindGameObjectWithTag("Player") is { } pObj)
            player = pObj.transform;
    }

    /* ────────── 거리 체크 ────────── */
    void Update()
    {
        if (observer == null || player == null) return;

        bool isInside = DistanceToPlayer <= radius;

        /* 플레이어 진입 */
        if (isInside && !inRange.Contains(this))
        {
            inRange.Add(this);
            subject.AddButtonObserver(observer);                       // 버튼 전용 옵저버 등록

            if (InteractionsButtonAction.Instance != null)
                InteractionsButtonAction.Instance.RequestSelection(this, actionTarget);
        }
        /* 플레이어 이탈 */
        else if (!isInside && inRange.Contains(this))
        {
            inRange.Remove(this);
            subject.RemoveButtonObserver(observer);                    // 해제

            if (InteractionsButtonAction.Instance != null)
                InteractionsButtonAction.Instance.NotifyExit(this);
        }
    }

    /* UI 매니저에서 호출 ? 실제 버튼 클릭 시 */
    public void InvokeButton() => subject.NotifyButton();

    /* ────────── 헬퍼: 가장 가까운 트리거 반환 ────────── */
    public static ProximityTriggerObject GetClosestInRange()
    {
        float min = float.MaxValue;
        ProximityTriggerObject closest = null;

        for (int i = inRange.Count - 1; i >= 0; i--)
        {
            var t = inRange[i];
            if (t == null) { inRange.RemoveAt(i); continue; }   // 고스트 참조 청소

            float d = t.DistanceToPlayer;
            if (d < min) { min = d; closest = t; }
        }
        return closest;
    }

    /* 파괴,비활성 시 리스트 정리 */
    void OnDisable() => inRange.Remove(this);

    /* ────────── 디버그 Gizmo ────────── */
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 1, 0.4f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}


//using UnityEngine;
//using TMPro;

//[RequireComponent(typeof(GimmickSubject))]
//public class ProximityTriggerObject : MonoBehaviour
//{
//    [SerializeField] private float detectionRadius = 3f;
//    private Transform playerTransform;
//    [SerializeField] private GameObject button_UI;
//    [SerializeField] private GameObject actionObject; // 실제 동작할 기믹 오브젝트

//    private TextMeshProUGUI buttonText;
//    private string actionObjectName;
//    private bool isPlayerInRange = false;

//    private GimmickSubject subject;           // 옵저버 등록/해제용
//    private IGimmickObserver observer;        // actionObject의 옵저버 인터페이스

//    private void Awake()
//    {
//        // 플레이어 트랜스폼 캐싱
//        GameObject player = GameObject.FindGameObjectWithTag("Player");
//        if (player != null)
//            playerTransform = player.transform;

//        // GimmickSubject와 Observer 캐싱
//        subject = GetComponent<GimmickSubject>();
//        if (actionObject != null)
//            observer = actionObject.GetComponent<IGimmickObserver>();
//    }

//    private void Start()
//    {
//        if (button_UI != null)
//        {
//            button_UI.SetActive(false);
//            buttonText = button_UI.GetComponentInChildren<TextMeshProUGUI>();
//        }

//        if (actionObject != null)
//            actionObjectName = actionObject.name;

//        // 플레이어 재탐색 (혹시 Awake에서 못 찾았으면)
//        if (playerTransform == null)
//        {
//            GameObject player = GameObject.FindGameObjectWithTag("Player");
//            if (player != null)
//                playerTransform = player.transform;
//        }
//    }

//    private void Update()
//    {
//        if (playerTransform == null || subject == null || observer == null) return;

//        float distance = Vector3.Distance(transform.position, playerTransform.position);

//        if (distance <= detectionRadius && !isPlayerInRange)
//        {
//            isPlayerInRange = true;

//            // UI 노출 등
//            button_UI?.SetActive(true);
//            buttonText.text = actionObjectName;

//            // 버튼 액션 쪽에 현재 트리거 전달
//            Object.FindFirstObjectByType<InteractionsButtonAction>()?
//                  .SetCurrentTriggerObject(this);

//            // Button 클릭을 위한 옵저버 등록
//            subject.AddExitObserver(observer);              // 변경
//        }
//        else if (distance > detectionRadius && isPlayerInRange)
//        {
//            isPlayerInRange = false;

//            button_UI?.SetActive(false);

//            // 등록 해제
//            subject.RemoveExitObserver(observer);           // 변경
//        }
//    }

//    // 외부에서 바로 접근하도록 헬퍼 추가
//    public GimmickSubject GetSubject() => subject;       // (기존)
//    public IGimmickObserver GetObserver() => observer;      // (신규 이름 같음)

//    #region
//    /*
//    구형 코드
//    private void Update()
//    {
//        if (playerTransform == null || subject == null || observer == null) return;

//        float distance = Vector3.Distance(transform.position, playerTransform.position);

//        if (distance <= detectionRadius && !isPlayerInRange)
//        {
//            isPlayerInRange = true;

//            // UI 버튼 표시 및 텍스트 설정
//            if (button_UI != null && buttonText != null)
//            {
//                buttonText.text = actionObjectName;
//                button_UI.SetActive(true);
//            }

//            // 현재 트리거를 버튼 로직에 등록
//            var buttonLogic = Object.FindFirstObjectByType<InteractionsButtonAction>();
//            if (buttonLogic != null)
//                buttonLogic.SetCurrentTriggerObject(this);

//            // 옵저버 등록
//            subject.AddObserverEnter(observer);
//        }
//        else if (distance > detectionRadius && isPlayerInRange)
//        {
//            isPlayerInRange = false;

//            // UI 버튼 숨기기
//            if (button_UI != null)
//                button_UI.SetActive(false);

//            // 옵저버 해제
//            subject.RemoveObserverEnter(observer);
//        }
//    }
//    */
//    #endregion

//    public string GetActionObjectName()
//    {
//        return actionObjectName;
//    }

//    private void OnDrawGizmosSelected()
//    {
//        Gizmos.color = Color.cyan;
//        Gizmos.DrawWireSphere(transform.position, detectionRadius);
//    }
//}