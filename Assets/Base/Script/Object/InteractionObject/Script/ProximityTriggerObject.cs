using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GimmickSubject))]
public class ProximityTriggerObject : MonoBehaviour
{
    [SerializeField] private float radius = 2f;
    [SerializeField] private Transform player;

    [Header("동작 할 오브젝트")]
    [SerializeField] private GameObject actionTarget;       // 실제 기믹 오브젝트
    private MonoBehaviour observerComponent;                // 드래그 전용 슬롯

    private IGimmickObserver observer;
    private GimmickSubject subject;

    /* 플레이어가 반경 안에 들어온 트리거를 관리 */
    private static readonly List<ProximityTriggerObject> inRange = new();

    public float DistanceToPlayer =>
        player ? Vector3.Distance(transform.position, player.position) : float.MaxValue;

    /* -------------------- 초기화 -------------------- */
    void Awake()
    {
        observerComponent = actionTarget.GetComponent<MonoBehaviour>();
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

    /* -------------------- 거리 체크 -------------------- */
    void Update()
    {
        if (observer == null || player == null) return;

        bool isInside = DistanceToPlayer <= radius;

        /* -------------------- 버튼/UI 매니저 존재 여부 검사 -------------------- */
        var ui = InteractionsButtonAction.Instance;
        if (ui == null)
        {
            // 다중 스팸 방지용 static 플래그
            LogMissingUIMgrOnce();
            return;                 // UI 없으면 이후 로직 스킵
        }

        /* 플레이어 진입 */
        if (isInside && !inRange.Contains(this))
        {
            inRange.Add(this);
            subject.AddButtonObserver(observer);                       // 버튼 전용 옵저버 등록
            ui.RequestSelection(this, actionTarget);
        }
        /* 플레이어 이탈 */
        else if (!isInside && inRange.Contains(this))
        {
            inRange.Remove(this);
            subject.RemoveButtonObserver(observer);                    // 해제
            ui.NotifyExit(this);
        }
    }

    /* -------------------- 경고를 ‘딱 한 번’만 출력 -------------------- */
    private static bool _warnedMissingUI;
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private static void LogMissingUIMgrOnce()
    {
        if (_warnedMissingUI) return;
        Debug.LogWarning(
            "[ProximityTriggerObject] InteractionsButtonAction.Instance is null — " +
            "UI 버튼 프리팹이 씬에 없거나 아직 초기화되지 않았습니다.");
        _warnedMissingUI = true;
    }

    /* UI 매니저에서 호출 ? 실제 버튼 클릭 시 */
    public void InvokeButton() => subject.NotifyButton();

    /* -------------------- 헬퍼: 가장 가까운 트리거 반환 -------------------- */
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

    /* -------------------- 디버그 Gizmo -------------------- */
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 1, 0.4f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
