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
