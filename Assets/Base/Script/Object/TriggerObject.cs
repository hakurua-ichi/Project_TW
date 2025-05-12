using UnityEngine;

[RequireComponent(typeof(GimmickSubject))]

public class TriggerObject : MonoBehaviour
{
    private GimmickSubject subject;

    void Awake()
    {
        subject = GetComponent<GimmickSubject>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("트리거 실행.");
            subject.Notify();  // 들어왔을 때 알림
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("트리거 종료.");
            subject.NotifyExit();  // 나갔을 때 알림
        }
    }
}
