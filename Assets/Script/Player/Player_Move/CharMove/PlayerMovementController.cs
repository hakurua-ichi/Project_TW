using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float rotationSpeed = 10f; // 회전 속도 추가
    
    private Rigidbody rb;
    private bool isGrounded;
    private float airControl = 0.3f;

    void Start()
    {
        // 카메라 참조 가져오기 (지정되지 않은 경우)
        if (cameraTransform == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                cameraTransform = mainCamera.transform;
            }
            else
            {
                Debug.LogWarning("메인 카메라를 찾을 수 없습니다. 카메라 기준 이동이 작동하지 않을 수 있습니다.");
            }
        }
        
        // Rigidbody 설정
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
    }

    void Update()
    {
        // 바닥 체크
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.1f);
    }

    public void Move(float horizontalInput, float verticalInput)
    {
        if (rb == null) return;
        
        // 수직 입력(W/S)이 있으면 회전만 처리
        if (verticalInput != 0)
        {
            RotateTowardCamera(verticalInput);
        }
        
        // 수평 입력(A/D)으로만 이동 계산
        Vector3 moveVector = CalculateMoveVector(horizontalInput, 0); // 수직 입력은 0으로 전달
        
        // 현재 속도 가져오기
        Vector3 velocity = rb.linearVelocity;
        
        // 계산된 방향으로 이동 (Y속도는 유지하여 중력 적용)
        if(!isGrounded && !rb.isKinematic)
        {
            velocity.x = velocity.x * (1 - airControl) + moveVector.x * airControl;
            velocity.z = velocity.z * (1 - airControl) + moveVector.z * airControl;
            rb.linearVelocity = velocity;
        }
        if(!rb.isKinematic && isGrounded)
        {
            velocity.x = moveVector.x;
            velocity.z = moveVector.z;
            rb.linearVelocity = velocity;
        }
    }
    
    // 카메라 방향으로 회전하는 메서드 추가
    private void RotateTowardCamera(float verticalInput)
    {
        if (cameraTransform == null) return;
        
        // 카메라의 방향 벡터
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0; // Y축 영향 제거
        cameraForward.Normalize();
        
        // W키는 카메라 방향으로, S키는 카메라 반대 방향으로 회전
        Vector3 targetDirection = (verticalInput > 0) ? cameraForward : -cameraForward;
        
        if (targetDirection.sqrMagnitude > 0.001f)
        {
            // 목표 회전값 계산
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            
            // 부드럽게 회전
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private Vector3 CalculateMoveVector(float horizontalInput, float verticalInput)
    {
        if ((horizontalInput == 0 && verticalInput == 0) || cameraTransform == null)
            return Vector3.zero;
        
        // 카메라의 방향 벡터 가져오기
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        
        // Y축 영향 제거 (수평 이동만)
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();
        
        // 이동 벡터 계산
        return (cameraRight * horizontalInput + cameraForward * verticalInput) * moveSpeed;
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }
}