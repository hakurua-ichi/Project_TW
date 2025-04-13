using UnityEngine;

[RequireComponent(typeof(GimmickSubject))]

public class TriggerObject : MonoBehaviour
{
    private GimmickSubject subject;

    // RotateMap 인스펙터에서 연결된 변수
    [SerializeField] private RotateMap rotateMap;

    void Awake()
    {
        subject = GetComponent<GimmickSubject>();
    }

    void Start()
    {
        // 스위치가 트리거될 때 RotateMap을 옵저버로 등록
        if (rotateMap != null)
        {
            Debug.Log("회전 맵 옵저버 등록 성공");
            subject.AddObserverEnter(rotateMap);  // 트리거에 들어오면 회전 시작
        }
        else
        {
            Debug.LogWarning("회전 맵이 연결되지 않았습니다. 옵저버 등록 실패.");
        }
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
