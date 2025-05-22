using UnityEngine;

[RequireComponent(typeof(GimmickSubject))]
public class TriggerObject : MonoBehaviour
{
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
        if (!other.CompareTag("Player") || observer == null) return;

        // 1 일단 옵저버를 두 리스트에 등록
        subject.AddEnterObserver(observer);
        subject.AddLeaveObserver(observer);

        // 2 '존 진입' 이벤트 브로드캐스트
        subject.NotifyEnter();
    }

    /* ────────── 트리거 존 이탈 ────────── */
    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") || observer == null) return;

        // 1 '존 이탈' 이벤트 브로드캐스트
        subject.NotifyLeave();

        // 2 더 이상 필요 없으니 등록 해제
        subject.RemoveEnterObserver(observer);
        subject.RemoveLeaveObserver(observer);
    }
}

#region
/*
구형 코드
using UnityEngine;

[RequireComponent(typeof(GimmickSubject))]

public class TriggerObject : MonoBehaviour
{
    [SerializeField] private GameObject actionTarget;
    private IGimmickObserver observer;
    private GimmickSubject subject;

    void Awake()
    {
        subject = GetComponent<GimmickSubject>();
        observer = actionTarget.GetComponent<IGimmickObserver>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && observer != null)
        {
            Debug.Log("트리거 실행.");
            subject.Notify(observer);  // 들어왔을 때 알림
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && observer != null)
        {
            Debug.Log("트리거 종료.");
            subject.NotifyExit(observer);  // 나갔을 때 알림
        }
    }
}
*/
#endregion