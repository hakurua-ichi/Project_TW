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
        observer = actionTarget?.GetComponent<IGimmickObserver>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || observer == null) return;

        // 트리거 존에 들어오면 옵저버 등록
        subject.AddExitObserver(observer);              // 변경
        subject.NotifyEnter();                          // 옵셔널: On-enter 이벤트
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") || observer == null) return;

        // 나가면 해제 + 종료 알림
        subject.NotifyExit();                           // ButtonClick 브로드캐스트
        subject.RemoveExitObserver(observer);           // 변경
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