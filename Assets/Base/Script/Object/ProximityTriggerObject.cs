using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GimmickSubject), typeof(SphereCollider))]
public class ProximityTriggerObject : MonoBehaviour
{
    [Header("동작 할 오브젝트")]
    [SerializeField] private GameObject actionTarget;   // 실제 기믹 오브젝트
    [SerializeField] private float radius;
    public GameObject ActionTarget => actionTarget;

    private Transform player;
    private IGimmickObserver observer;
    private GimmickSubject subject;
    private InteractionsButtonAction ui;

    // ▶ 플레이어와 겹쳐 있는 트리거들을 관리
    private static readonly List<ProximityTriggerObject> inRange = new();

    void Awake()
    {
        subject = GetComponent<GimmickSubject>();

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
        col.radius = radius;
    }

    private void Start()
    {
        ui = InteractionsButtonAction.Instance;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    // ▶ 플레이어가 범위 안으로 들어올 때
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || observer == null) return;

        Debug.Log("캐릭 진입");
        Debug.Log($"[ProximityTriggerObject] {gameObject.name} Trigger Enter by: {other.gameObject.name}");

        // 리스트에 추가
        if (!inRange.Contains(this))
            inRange.Add(this);

        if (ui == null)
        {
            Debug.Log("[ProximityTriggerObject] UI 매니저가 없습니다.");
            return;
        }

        subject.AddButtonObserver(observer);
        ui.RequestSelection(this, actionTarget);
        Debug.Log("리퀘스트 완료");
    }

    // ▶ 플레이어가 범위를 벗어날 때
    void OnTriggerExit(Collider other)
    {
        Debug.Log("캐릭 이탈");
        if (!other.CompareTag("Player") || observer == null) return;

        // 리스트에서 제거
        inRange.Remove(this);

        if (ui == null) return;
        Debug.Log("옵저버 제거");
        subject.RemoveButtonObserver(observer);
        ui.NotifyExit(this);
    }

    /// <summary>
    /// 씬 언로드, 오브젝트 파괴, Disable 시에도 확실히 리스트에서 제거
    /// </summary>
    void OnDisable()
    {
        // Static 리스트에서 자신 제거
        inRange.Remove(this);

        // 버튼 이벤트 옵저버도 제거
        if (observer != null && subject != null)
        {
            subject.RemoveButtonObserver(observer);
        }
    }

    // OnDisable이 호출되면 충분하지만, 혹시를 위해 Destroy에서도 보장
    void OnDestroy() => OnDisable();

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
