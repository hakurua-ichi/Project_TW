using UnityEngine;
using TMPro;

[RequireComponent(typeof(GimmickSubject), typeof(BoxCollider), typeof(Rigidbody))]
[RequireComponent(typeof(ProximityTriggerObject))]
public class LaddersGimmick : MonoBehaviour, IGimmickObserver
{
    private GimmickSubject TriggerObject; // 트리거 오브젝트
    private TextMeshProUGUI StatusText; // 상태 표시용 UI 텍스트
    private GimmickContext context; // 현재 사다리 상태를 관리하는 컨텍스트
    private GameObject player; // 플레이어 오브젝트
    private GameObject thisPoint; // 현재 사다리 포인트
    private GameObject otherPoint; // 다른 사다리 포인트
    private LaddersState laddersState; // 사다리 상태 관리
    private bool textVisible = false; // 상태 표시 텍스트 표시 여부
    private float textTimer = 0f; // 상태 표시 타이머

    void Start()
    {
        TriggerObject = GetComponent<GimmickSubject>(); // GimmickSubject 컴포넌트 가져오기
        laddersState = new LaddersState(); // 현재 사다리 상태 초기화
        context = new GimmickContext(); // 현재 사다리 상태를 관리하는 컨텍스트 생성
        context.SetAction(new LaddersAction(laddersState)); // 사다리 타기 애니메이션 설정
        BoxCollider boxCollider = GetComponent<BoxCollider>(); // 박스 콜라이더 컴포넌트 가져오기
        boxCollider.isTrigger = true; // 트리거로 설정
        Rigidbody rigidbody = GetComponent<Rigidbody>(); // Rigidbody 컴포넌트 가져오기
        rigidbody.isKinematic = true; // 물리 엔진의 영향을 받지 않도록 설정
        rigidbody.useGravity = false; // 중력 적용 안 함
        thisPoint = gameObject; // 현재 사다리 포인트 설정
        player = GameObject.FindGameObjectWithTag("Player"); // 플레이어 오브젝트 찾기
        StatusText = GameObject.FindGameObjectWithTag("StatusText")?.GetComponent<TextMeshProUGUI>(); // 상태 표시용 UI 텍스트 찾기
        StatusText.text = ""; // 상태 표시 텍스트 초기화
        textVisible = false; // 상태 표시 텍스트 숨김
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
        if(other != null && other.CompareTag("Ladders"))
        {
            otherPoint = other.gameObject;
            laddersState.useAble = true; // 사다리 사용 가능 상태로 설정
            laddersState.Set(thisPoint, otherPoint, player);
            textVisible = false; // 상태 표시 텍스트 표시
        }
        else return;
    }

    void OnTriggerExit(Collider other)
    {
        if (other != null && other.CompareTag("Ladders"))
        {
            laddersState.useAble = false; // 사다리 사용 불가 상태로 설정
            other = null; // 다른 포인트 초기화
        }
        else return;
    }
}