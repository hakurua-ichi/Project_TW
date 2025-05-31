using UnityEngine;
using TMPro;

[RequireComponent(typeof(GimmickSubject), typeof(BoxCollider), typeof(Rigidbody))]
[RequireComponent(typeof(ProximityTriggerObject))]
public class LaddersGimmick : MonoBehaviour, IGimmickObserver
{
    [SerializeField] private GimmickSubject TriggerObject; // 트리거 오브젝트
    [SerializeField] private TextMeshProUGUI StatusText; // 상태 표시용 UI 텍스트
    private GimmickContext context; // 현재 사다리 상태를 관리하는 컨텍스트
    private GameObject player; // 플레이어 오브젝트
    private GameObject thisPoint; // 현재 사다리 포인트
    private GameObject otherPoint; // 다른 사다리 포인트
    private LaddersState laddersState; // 사다리 상태 관리
    private bool textVisible = false; // 상태 표시 텍스트 표시 여부
    private float textTimer = 0f; // 상태 표시 타이머

    void Start()
    {
        laddersState = new LaddersState(); // 현재 사다리 상태 초기화
        context = new GimmickContext();
        context.SetAction(new LaddersAction(laddersState));
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true; // 트리거로 설정
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true; // 물리 엔진의 영향을 받지 않도록 설정
        rigidbody.useGravity = false; // 중력 적용 안 함
    }

    public void OnGimmickEnter() { }

    public void OnGimmickLeave() { }

    public void ButtonClick()
    {
        if(!laddersState.useAble)
        {
            StatusText.text = "사다리가 막힌 듯 하다......"; // 사다리 사용 불가 메시지 표시
            textVisible = true; // 상태 표시 텍스트 표시
        }
        else context.StartAction(); // 사다리 타기 애니메이션 시작
    }

    void Update()
    {
        if(!textVisible) return;

        textTimer += Time.deltaTime; // 타이머 업데이트
        if (textTimer >= 2f) // 2초 후에 텍스트 숨김
        {
            textVisible = false;
            StatusText.text = ""; // 상태 표시 텍스트 초기화
            textTimer = 0f; // 타이머 초기화
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(otherPoint.CompareTag("LadderPoint"))
        {
            otherPoint = other.gameObject;
            laddersState.useAble = true; // 사다리 사용 가능 상태로 설정
            laddersState.Set(thisPoint, otherPoint, player);
            textVisible = false; // 상태 표시 텍스트 표시
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (otherPoint.CompareTag("LadderPoint"))
        {
            laddersState.useAble = false; // 사다리 사용 불가 상태로 설정
        }
    }
}