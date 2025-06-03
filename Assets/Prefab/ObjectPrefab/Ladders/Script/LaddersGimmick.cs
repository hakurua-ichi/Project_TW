using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(GimmickSubject), typeof(BoxCollider), typeof(Rigidbody))]
[RequireComponent(typeof(ProximityTriggerObject))]
public class LaddersGimmick : MonoBehaviour, IGimmickObserver
{
    //private GimmickSubject TriggerObject; // 트리거 오브젝트
    private GimmickContext context; // 현재 사다리 상태를 관리하는 컨텍스트
    private GameObject player; // 플레이어 오브젝트
    private GameObject thisPoint; // 현재 사다리 포인트
    private GameObject otherPoint; // 다른 사다리 포인트
    private LaddersState laddersState; // 사다리 상태 관리
    private bool isBusy = false; // 페이드 중복 방지용

    [Header("상태 표시 텍스트 설정")]
    [SerializeField] private StateText stateText; // 상태 표시용 스크립트
    [SerializeField] private string stateTextMessage; // 상태 표시 메시지

    [Header("스크린 페이드 인 아웃 설정")]
    [SerializeField] private ScreenFader screenFader; // ScreenFader 컴포넌트를 연결

    void Start()
    {
        //TriggerObject = GetComponent<GimmickSubject>(); // GimmickSubject 컴포넌트 가져오기
        laddersState = new LaddersState(); // 현재 사다리 상태 초기화
        context = new GimmickContext(); // 현재 사다리 상태를 관리하는 컨텍스트 생성
        context.SetAction(new LaddersAction(laddersState, this));  // this == MonoBehaviour
        BoxCollider boxCollider = GetComponent<BoxCollider>(); // 박스 콜라이더 컴포넌트 가져오기
        boxCollider.isTrigger = true; // 트리거로 설정
        Rigidbody rigidbody = GetComponent<Rigidbody>(); // Rigidbody 컴포넌트 가져오기
        rigidbody.isKinematic = true; // 물리 엔진의 영향을 받지 않도록 설정
        rigidbody.useGravity = false; // 중력 적용 안 함
        thisPoint = gameObject; // 현재 사다리 포인트 설정
        player = GameObject.FindGameObjectWithTag("Player"); // 플레이어 오브젝트 찾기
    }

    public void OnGimmickEnter() { }

    public void OnGimmickLeave() { }

    public void ButtonClick()
    {
        if (isBusy) return; // 페이드나 이동 중복 방지

        if (!laddersState.useAble)
        {
            stateText.SetText(true, stateTextMessage);
        }
        else
        {
            // 페이드 아웃 → 사다리 이동 → 페이드 인
            StartCoroutine(screenFader.FadeOut_Move_FadeIn(context.StartAction));
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other != null && other.CompareTag("Ladders"))
        {
            otherPoint = other.gameObject;
            laddersState.useAble = true; // 사다리 사용 가능 상태로 설정
            laddersState.Set(thisPoint, otherPoint, player);
            stateText.UnVisible(); // 상태 표시 텍스트 숨김
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