using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GimmickSubject), typeof(SphereCollider))]
public class ProximityTriggerObject : MonoBehaviour
{
    [Header("동작 할 오브젝트")]
    [SerializeField] private GameObject actionTarget;   // 실제 기믹 오브젝트
    [SerializeField] private Transform player;

    private IGimmickObserver observer;
    private GimmickSubject subject;
    private InteractionsButtonAction ui;

    // ▶ 플레이어와 겹쳐 있는 트리거들을 관리
    private static readonly List<ProximityTriggerObject> inRange = new();

    void Awake()
    {
        subject = GetComponent<GimmickSubject>();
        ui = GetComponent<InteractionsButtonAction>();

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

        if (observer == null)
            Debug.LogError($"[{name}] IGimmickObserver 구현체를 찾지 못했습니다!", this);

        // SphereCollider 세팅
        var col = GetComponent<SphereCollider>();
        col.isTrigger = true;
    }

    // ▶ 플레이어가 범위 안으로 들어올 때
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("캐릭 진입");
        if (!other.CompareTag("Player") || observer == null) return;

        // 리스트에 추가
        if (!inRange.Contains(this))
            inRange.Add(this);
        Debug.Log("214124");

        if (ui == null)
        {
            Debug.Log("[ProximityTriggerObject] UI 매니저가 없습니다.");
            return;
        }

        Debug.Log("캐릭 진입2");
        subject.AddButtonObserver(observer);
        ui.RequestSelection(this, actionTarget);
    }

    // ▶ 플레이어가 범위를 벗어날 때
    void OnTriggerExit(Collider other)
    {
        Debug.Log("캐릭 이탈");
        if (!other.CompareTag("Player") || observer == null) return;

        // 리스트에서 제거
        inRange.Remove(this);

        if (ui == null) return;

        subject.RemoveButtonObserver(observer);
        ui.NotifyExit(this);
    }

    // ▶ 버튼 클릭 시 호출
    public void InvokeButton()
    {
        subject.NotifyButton();
    }

    // ▶ 지금 inRange 안에 있는 트리거들 중 플레이어에 가장 가까운 것
    public static ProximityTriggerObject GetClosestInRange()
    {
        // 플레이어 Transform을 찾습니다
        var playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO == null) return null;
        var playerT = playerGO.transform;

        ProximityTriggerObject closest = null;
        float minSqr = float.MaxValue;

        // 리스트 순회하며 거리 비교
        for (int i = inRange.Count - 1; i >= 0; i--)
        {
            var t = inRange[i];
            if (t == null)
            {
                inRange.RemoveAt(i);
                continue;
            }

            float sqr = (t.transform.position - playerT.position).sqrMagnitude;
            if (sqr < minSqr)
            {
                minSqr = sqr;
                closest = t;
            }
        }

        return closest;
    }

    void OnDrawGizmosSelected()
    {
        var col = GetComponent<SphereCollider>();
        Gizmos.color = new Color(0, 1, 1, 0.4f);
        Gizmos.DrawWireSphere(transform.position, col.radius);
    }
}


//using System.Collections.Generic;
//using UnityEngine;

//[RequireComponent(typeof(GimmickSubject))]
//public class ProximityTriggerObject : MonoBehaviour
//{
//    [SerializeField] private float radius = 2f;
//    [SerializeField] private Transform player;

//    [Header("동작 할 오브젝트")]
//    [SerializeField] private GameObject actionTarget;       // 실제 기믹 오브젝트
//    private MonoBehaviour observerComponent;                // 드래그 전용 슬롯

//    private IGimmickObserver observer;
//    private GimmickSubject subject;

//    /* 플레이어가 반경 안에 들어온 트리거를 관리 */
//    private static readonly List<ProximityTriggerObject> inRange = new();

//    public float DistanceToPlayer =>
//        player ? Vector3.Distance(transform.position, player.position) : float.MaxValue;

//    /* -------------------- 초기화 -------------------- */
//    void Awake()
//    {
//        observerComponent = actionTarget.GetComponent<MonoBehaviour>();
//        subject = GetComponent<GimmickSubject>();
//        observer = observerComponent as IGimmickObserver;

//        /* 1) actionTarget 안에서 찾기 */
//        if (observer == null && actionTarget != null)
//            observer = actionTarget.GetComponent<IGimmickObserver>() ??
//                       actionTarget.GetComponentInChildren<IGimmickObserver>(true);

//        /* 2) 자기 자신 쪽에서 찾기 */
//        if (observer == null)
//            observer = GetComponent<IGimmickObserver>() ??
//                       GetComponentInChildren<IGimmickObserver>(true);

//        if (observer == null)
//            Debug.LogError($"[{name}] IGimmickObserver 구현체를 찾지 못했습니다!", this);

//        /* 플레이어 참조가 없으면 태그로 보강 */
//        if (player == null && GameObject.FindGameObjectWithTag("Player") is { } pObj)
//            player = pObj.transform;
//    }

//    /* -------------------- 거리 체크 -------------------- */
//    void Update()
//    {
//        if (observer == null) { Debug.Log($"{name}: observer NULL"); return; }
//        if (player == null) { Debug.Log($"{name}: player   NULL"); return; }

//        bool isInside = DistanceToPlayer <= radius;

//        /* -------------------- 버튼/UI 매니저 존재 여부 검사 -------------------- */
//        var ui = InteractionsButtonAction.Instance;
//        if (ui == null)
//        {
//            // 다중 스팸 방지용 static 플래그
//            LogMissingUIMgrOnce();
//            return;                 // UI 없으면 이후 로직 스킵
//        }

//        /* 플레이어 진입 */
//        if (isInside && !inRange.Contains(this))
//        {
//            Debug.Log("캐릭터 UI 범위진입");
//            inRange.Add(this);
//            subject.AddButtonObserver(observer);                       // 버튼 전용 옵저버 등록
//            ui.RequestSelection(this, actionTarget);
//        }
//        /* 플레이어 이탈 */
//        else if (!isInside && inRange.Contains(this))
//        {
//            Debug.Log("캐릭터 UI 범위 이탈");
//            inRange.Remove(this);
//            subject.RemoveButtonObserver(observer);                    // 해제
//            ui.NotifyExit(this);
//        }

//        Debug.Log("isInside: ");
//    }

//    /* -------------------- 경고를 ‘딱 한 번’만 출력 -------------------- */
//    private static bool _warnedMissingUI;
//    [System.Diagnostics.Conditional("UNITY_EDITOR")]
//    private static void LogMissingUIMgrOnce()
//    {
//        if (_warnedMissingUI) return;
//        Debug.LogWarning(
//            "[ProximityTriggerObject] InteractionsButtonAction.Instance is null — " +
//            "UI 버튼 프리팹이 씬에 없거나 아직 초기화되지 않았습니다.");
//        _warnedMissingUI = true;
//    }

//    /* UI 매니저에서 호출 ? 실제 버튼 클릭 시 */
//    public void InvokeButton() => subject.NotifyButton();

//    /* -------------------- 헬퍼: 가장 가까운 트리거 반환 -------------------- */
//    public static ProximityTriggerObject GetClosestInRange()
//    {
//        float min = float.MaxValue;
//        ProximityTriggerObject closest = null;

//        for (int i = inRange.Count - 1; i >= 0; i--)
//        {
//            var t = inRange[i];
//            if (t == null) { inRange.RemoveAt(i); continue; }   // 고스트 참조 청소

//            float d = t.DistanceToPlayer;
//            if (d < min) { min = d; closest = t; }
//        }
//        return closest;
//    }

//    /* 파괴,비활성 시 리스트 정리 */
//    void OnDisable() => inRange.Remove(this);

//    /* -------------------- 디버그 Gizmo -------------------- */
//    void OnDrawGizmosSelected()
//    {
//        Gizmos.color = new Color(0, 1, 1, 0.4f);
//        Gizmos.DrawWireSphere(transform.position, radius);
//    }
//}
