using UnityEngine;

[RequireComponent(typeof(GimmickSubject))]
public class TriggerObject : MonoBehaviour
{
    [Header("동작 할 오브젝트")]
    [SerializeField] private GameObject actionTarget;

    private IGimmickObserver observer;   // 실제 기믹 스크립트
    private GimmickSubject subject;    // 퍼블리셔

    void Awake()
    {
        subject = GetComponent<GimmickSubject>();
        observer = actionTarget ? actionTarget.GetComponent<IGimmickObserver>() : null;
    }

    void Start()
    {
        if (actionTarget == null || observer == null)
            Debug.LogError($"{name} : actionTarget 가 비어 있거나 IGimmickObserver 구현이 없습니다.", this);
    }

    /* ────────── 트리거 존 진입 ────────── */
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("즉시 발동 범위 캐릭 진입");
        if (!other.CompareTag("Player") || observer == null) return;
        Debug.Log("옵저버 등록");
        // 1 일단 옵저버를 두 리스트에 등록
        subject.AddEnterObserver(observer);
        subject.AddLeaveObserver(observer);

        // 2 '존 진입' 이벤트 브로드캐스트
        subject.NotifyEnter();
    }

    /* ────────── 트리거 존 이탈 ────────── */
    void OnTriggerExit(Collider other)
    {
        Debug.Log("즉시 발동 범위 캐릭 이탈");
        if (!other.CompareTag("Player") || observer == null) return;

        // 1 '존 이탈' 이벤트 브로드캐스트
        subject.NotifyLeave();

        // 2 더 이상 필요 없으니 등록 해제
        subject.RemoveEnterObserver(observer);
        subject.RemoveLeaveObserver(observer);
    }

    /// <summary>
    /// 트리거 오브젝트가 비활성화되거나 파괴될 때 옵저버 등록 해제 보장
    /// </summary>
    void OnDisable()
    {
        if (observer != null && subject != null)
        {
            subject.RemoveEnterObserver(observer);
            subject.RemoveLeaveObserver(observer);
        }
    }

    void OnDestroy() => OnDisable();
}
