using UnityEngine;

public class TriggerObject : MonoBehaviour
{
    [SerializeField] private GimmickSubject subject;
    [SerializeField] private GimmickSubject exitSubject; // 나갔을 때 알릴 주체 (같은 걸 써도 됨)

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
            exitSubject.Notify();  // 나갔을 때 알림
        }
    }
}
