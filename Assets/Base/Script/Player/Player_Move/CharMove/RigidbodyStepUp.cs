using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Character Controller 스타일의 StepUp 기능을 Rigidbody에 제공합니다.
/// 계단이나 작은 턱을 자연스럽게 오를 수 있게 해줍니다.
/// </summary>
public class RigidbodyStepUp : MonoBehaviour
{
    [Header("스텝업 설정")]
    [Tooltip("넘을 수 있는 최대 계단 높이 (Character Controller의 stepOffset과 유사)")]
    [SerializeField] private float maxStepHeight = 0.5f;
    
    [Tooltip("스텝업 레이캐스트 거리 (앞쪽 감지 거리)")]
    [SerializeField] private float stepDetectionDistance = 0.5f;
    
    [Tooltip("스텝업 시 수직 이동 속도")]
    [SerializeField] private float stepUpSpeed = 0.3f;
    
    [Tooltip("전방 확인용 레이캐스트 갯수")]
    [SerializeField] private int forwardRayCount = 8;[Header("레이어 마스크 설정")]
    [Tooltip("스텝업을 검사할 레이어")]
    [SerializeField] private LayerMask stepUpMask = -1; // 기본적으로 모든 레이어 검사
    
    [Header("고급 설정")]
    [Tooltip("다중 감지 모드 - Character Controller 같은 느낌을 위해 다양한 각도에서 감지")]
    [SerializeField] private bool useMultipleRays = true;
    
    [Tooltip("걸림 감지 시 수직 보정력")]
    [SerializeField] private float hangPreventForce = 25f;
    
    [Header("디버그")]
    [Tooltip("디버그 레이 표시")]
    [SerializeField] private bool showDebugRays = true;

    private Rigidbody rb;
    private Collider col;
    private Vector3 lastVelocity;
    private float originalDrag;
    private float capsuleRadius;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        
        if (rb == null)
        {
            Debug.LogError("RigidbodyStepUp 컴포넌트에 Rigidbody가 없습니다!");
            enabled = false;
            return;
        }
        
        if (col == null)
        {
            Debug.LogError("RigidbodyStepUp 컴포넌트에 Collider가 없습니다!");
            enabled = false;
            return;
        }
        
        originalDrag = rb.linearDamping;
        
        // 콜라이더 타입에 따라 데이터 초기화
        if (col is CapsuleCollider capsule)
        {
            capsuleRadius = capsule.radius;
        }
        else if (col is SphereCollider sphere)
        {
            capsuleRadius = sphere.radius;
        }
        else
        {
            // 박스 콜라이더나 기타 타입일 경우 대략적인 반지름 계산
            BoxCollider box = col as BoxCollider;
            if (box != null)
            {
                capsuleRadius = Mathf.Min(box.size.x, box.size.z) * 0.5f;
            }
            else
            {
                capsuleRadius = 0.5f; // 기본값
            }
        }
    }

    private void FixedUpdate()
    {
        // 리지드바디 속도가 매우 느리면 체크하지 않음
        if (rb.linearVelocity.sqrMagnitude < 0.1f) return;

        // 이동 방향
        Vector3 moveDirection = rb.linearVelocity.normalized;
        moveDirection.y = 0f; // 수평 이동 방향만 고려
        
        // 스텝업 체크 및 적용
        CheckStepUp(moveDirection);
    }    
    private void CheckStepUp(Vector3 moveDirection)
    {
        if (moveDirection.sqrMagnitude < 0.01f) return;
        
        // 지면 체크 - 지면에 있을 때만 StepUp 수행
        bool isGrounded = CheckGrounded();
        if (!isGrounded) return;
        
        // 스텝 감지 및 처리
        CheckForStep(moveDirection);
    }
    
    private bool CheckForStep(Vector3 moveDirection)
    {
        // 콜라이더 바닥에서 시작하는 전방 레이캐스트
        Vector3 bottomPoint = transform.position + Vector3.down * (GetColliderHeight() * 0.45f);
        
        // 콜라이더 앞쪽으로 살짝 이동하여 더 정확한 감지
        Vector3 forwardPoint = bottomPoint + moveDirection * capsuleRadius * 0.8f;
        
        if (Physics.Raycast(forwardPoint, moveDirection, out RaycastHit hit, stepDetectionDistance, stepUpMask))
        {
            if (showDebugRays) Debug.DrawRay(forwardPoint, moveDirection * hit.distance, Color.red, 0.1f);
            
            // 감지된 장애물 높이에서 레이를 위로 쏘아 최대 높이 확인
            Vector3 stepCheckOrigin = hit.point + Vector3.up * 0.05f; // 살짝 위에서 시작
            
            if (Physics.Raycast(stepCheckOrigin, Vector3.up, out RaycastHit upHit, maxStepHeight, stepUpMask))
            {
                float stepHeight = upHit.point.y - bottomPoint.y;
                if (stepHeight <= maxStepHeight && stepHeight > 0.05f)
                {
                    // 머리 위치에서 장애물 앞쪽으로 레이캐스트하여 통과 가능 확인
                    Vector3 headPosition = bottomPoint + Vector3.up * (maxStepHeight + 0.1f);
                    if (!Physics.Raycast(headPosition, moveDirection, stepDetectionDistance, stepUpMask))
                    {
                        StepUpMovement(stepHeight);
                        return true;
                    }
                }
            }
        }
        return false;
    }
    
    private float GetColliderHeight()
    {
        float colliderHeight = 2.0f; // 기본값
        
        if (col is CapsuleCollider capsule)
        {
            colliderHeight = capsule.height;
        }
        else if (col is BoxCollider box)
        {
            colliderHeight = box.size.y;
        }
        else if (col is SphereCollider sphere)
        {
            colliderHeight = sphere.radius * 2.0f;
        }
        
        return colliderHeight;
    }
    
    private bool CheckGrounded()
    {
        // 다중 레이캐스트로 더 정확한 지면 감지
        Vector3[] checkPoints = new Vector3[5];
        checkPoints[0] = transform.position; // 중심
        
        // 콜라이더 중앙 아래에서 4방향으로 추가 체크
        float offset = capsuleRadius * 0.75f;
        checkPoints[1] = transform.position + transform.forward * offset;
        checkPoints[2] = transform.position - transform.forward * offset;
        checkPoints[3] = transform.position + transform.right * offset;
        checkPoints[4] = transform.position - transform.right * offset;
        
        foreach (Vector3 point in checkPoints)
        {
            if (Physics.Raycast(point, Vector3.down, 0.3f, stepUpMask))
            {
                return true;
            }
        }
        return false;
    }
    
    private void StepUpMovement(float stepHeight)
    {
        // Character Controller 스타일의 스텝업 구현
        
        // 현재 수평 속도 저장
        lastVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        
        // 단계 1: 높이 계산
        float verticalOffset = Mathf.Min(stepHeight * 1.2f, maxStepHeight); // 약간 여유 추가
        
        // 단계 2: 현재 위치에서 수직 이동 (Character Controller의 stepOffset 효과)
        Vector3 stepUpPosition = transform.position + new Vector3(0, verticalOffset, 0);
        
        // 단계 3: 계단에서 걸리지 않도록 속도 조정
        // - 수직 속도는 상승 속도로 설정
        // - 수평 속도는 유지하면서 걸림 방지를 위해 약간 증가
        Vector3 stepUpVelocity = new Vector3(
            rb.linearVelocity.x * 1.2f, 
            Mathf.Max(rb.linearVelocity.y, stepUpSpeed), 
            rb.linearVelocity.z * 1.2f
        );
        
        // 단계 4: 속도와 위치 적용 (부드러운 이동을 위해)
        rb.MovePosition(stepUpPosition); // 직접 위치 이동 (순간 이동 효과)
        rb.linearVelocity = stepUpVelocity; // 속도 조정 (부드러운 움직임 유지)
        
        // 추가: 약간의 하향력을 주어 계단 위에 안착하도록 함
        StartCoroutine(ApplyDownwardForce());
    }
    
    // 계단을 올라간 후 약간의 하향력을 주어 안착시키기 위한 코루틴
    private System.Collections.IEnumerator ApplyDownwardForce()
    {
        yield return new WaitForFixedUpdate();
        
        // 계단을 올라간 후 약간의 하향력 적용
        rb.AddForce(Vector3.down * rb.mass * 9.81f * 0.8f, ForceMode.Force);
        
        // 지면에 닿을 때까지 반복적으로 하향력 적용
        int attempts = 0;
        while (!CheckGrounded() && attempts < 5)
        {
            rb.AddForce(Vector3.down * rb.mass * 9.81f * 0.4f, ForceMode.Force);
            attempts++;
            yield return new WaitForFixedUpdate();
        }
    }

    // PlayerMovementController.cs에 추가
    
}